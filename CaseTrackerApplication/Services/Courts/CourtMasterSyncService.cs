using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerDomain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CaseTrackerApplication.Services.Courts
{
    public class CourtMasterSyncService : ICourtMasterSyncService
    {
        private readonly ICourtRepository _courtRepository;
        private readonly IECourtClient _ecourtClient;
        private readonly ILogger<CourtMasterSyncService> _logger;
        private readonly CourtMasterSyncSettings _defaultSettings;

        public CourtMasterSyncService(
            ICourtRepository courtRepository,
            IECourtClient ecourtClient,
            ILogger<CourtMasterSyncService> logger,
            IOptions<CourtMasterSyncSettings>? defaultSettings = null)
        {
            _courtRepository = courtRepository;
            _ecourtClient = ecourtClient;
            _logger = logger;
            _defaultSettings = defaultSettings?.Value ?? new CourtMasterSyncSettings();
        }

        public async Task<CourtMasterSyncExecutionSummaryDto> SynchronizeAsync(
            CourtMasterSyncSettings? settings = null,
            CancellationToken cancellationToken = default)
        {
            var activeSettings = settings ?? _defaultSettings;
            var overallSw = Stopwatch.StartNew();
            var startedAt = DateTimeOffset.UtcNow;

            var summary = new CourtMasterSyncExecutionSummaryDto
            {
                StartedAt = startedAt,
                Status = "Completed"
            };

            if (!activeSettings.Enabled)
            {
                _logger.LogInformation("Court master synchronization is disabled in settings. Skipping execution.");
                summary.CompletedAt = DateTimeOffset.UtcNow;
                summary.Status = "Skipped";
                summary.ErrorMessage = "Synchronization is disabled in configuration.";
                return summary;
            }

            var statesToSync = activeSettings.TargetStates != null && activeSettings.TargetStates.Count > 0
                ? activeSettings.TargetStates
                : new List<string> { "TN", "PY" };

            _logger.LogInformation("Starting scheduled Court Master synchronization for states: {States}", string.Join(", ", statesToSync));

            int totalApiRequests = 0;

            foreach (var rawCode in statesToSync)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Court Master synchronization cancelled by request.");
                    summary.Status = "Cancelled";
                    break;
                }

                if (totalApiRequests >= activeSettings.MaxApiRequestsPerRun)
                {
                    _logger.LogWarning("Reached maximum API requests limit ({Max}). Halting synchronization safely.", activeSettings.MaxApiRequestsPerRun);
                    summary.Status = "Partial";
                    summary.ErrorMessage = $"Reached safety API request limit of {activeSettings.MaxApiRequestsPerRun}.";
                    break;
                }

                var stateSummary = await SynchronizeStateInternalAsync(
                    rawCode.Trim().ToUpperInvariant(),
                    activeSettings,
                    totalApiRequests,
                    cancellationToken);

                totalApiRequests += stateSummary.StateApiRequests;
                summary.States.Add(stateSummary.StateCode);

                // Aggregate stats
                summary.Districts.Received += stateSummary.Districts.Received;
                summary.Districts.Inserted += stateSummary.Districts.Inserted;
                summary.Districts.Updated += stateSummary.Districts.Updated;
                summary.Districts.Skipped += stateSummary.Districts.Skipped;
                summary.Districts.Failed += stateSummary.Districts.Failed;

                summary.CourtComplexes.Received += stateSummary.CourtComplexes.Received;
                summary.CourtComplexes.Inserted += stateSummary.CourtComplexes.Inserted;
                summary.CourtComplexes.Updated += stateSummary.CourtComplexes.Updated;
                summary.CourtComplexes.Skipped += stateSummary.CourtComplexes.Skipped;
                summary.CourtComplexes.Failed += stateSummary.CourtComplexes.Failed;

                summary.Courts.Received += stateSummary.Courts.Received;
                summary.Courts.Inserted += stateSummary.Courts.Inserted;
                summary.Courts.Updated += stateSummary.Courts.Updated;
                summary.Courts.Skipped += stateSummary.Courts.Skipped;
                summary.Courts.Failed += stateSummary.Courts.Failed;

                if (stateSummary.HasError && summary.Status == "Completed")
                {
                    summary.Status = "Partial";
                    summary.ErrorMessage = stateSummary.ErrorMessage;
                }
            }

            overallSw.Stop();
            summary.CompletedAt = DateTimeOffset.UtcNow;
            summary.DurationMs = overallSw.ElapsedMilliseconds;
            summary.ApiRequests = totalApiRequests;

            _logger.LogInformation(
                "Court Master synchronization finished with status {Status} in {Duration}ms. Total API requests: {Reqs}.\n{Summary}",
                summary.Status, summary.DurationMs, summary.ApiRequests, summary.ToFormattedSummary());

            return summary;
        }

        public async Task<CourtMasterSyncExecutionSummaryDto> SynchronizeStateAsync(
            string stateCode,
            CourtMasterSyncSettings? settings = null,
            CancellationToken cancellationToken = default)
        {
            var singleStateSettings = new CourtMasterSyncSettings
            {
                Enabled = settings?.Enabled ?? _defaultSettings.Enabled,
                TargetStates = new List<string> { stateCode.Trim().ToUpperInvariant() },
                MaxApiRequestsPerRun = settings?.MaxApiRequestsPerRun ?? _defaultSettings.MaxApiRequestsPerRun,
                RetryCount = settings?.RetryCount ?? _defaultSettings.RetryCount,
                RetryDelaySeconds = settings?.RetryDelaySeconds ?? _defaultSettings.RetryDelaySeconds,
                DeactivateMissingEntities = settings?.DeactivateMissingEntities ?? _defaultSettings.DeactivateMissingEntities
            };

            return await SynchronizeAsync(singleStateSettings, cancellationToken);
        }

        private async Task<StateSyncInternalResult> SynchronizeStateInternalAsync(
            string stateCode,
            CourtMasterSyncSettings settings,
            int currentTotalApiRequests,
            CancellationToken cancellationToken)
        {
            var stateSw = Stopwatch.StartNew();
            var startedAt = DateTimeOffset.UtcNow;
            var result = new StateSyncInternalResult { StateCode = stateCode };

            string stateName = stateCode switch
            {
                "TN" => "Tamil Nadu",
                "PY" => "Puducherry",
                _ => stateCode
            };

            _logger.LogInformation("Processing State: {StateName} ({StateCode})", stateName, stateCode);

            // 1. Upsert State entity via repository
            State stateEntity;
            try
            {
                stateEntity = await _courtRepository.UpsertStateAsync(stateCode, stateName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upsert state record for {StateCode}", stateCode);
                result.HasError = true;
                result.ErrorMessage = $"Database error upserting state {stateCode}: {ex.Message}";
                return result;
            }

            // 2. Fetch external districts
            List<ECourtsDistrictItem> externalDistricts;
            try
            {
                _logger.LogInformation("Requesting external districts for state {StateCode}...", stateCode);
                externalDistricts = await _ecourtClient.GetDistrictsAsync(stateCode);
                result.StateApiRequests++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve districts for state {StateCode} from eCourts API.", stateCode);
                result.HasError = true;
                result.ErrorMessage = $"Failed to fetch districts for {stateCode}: {ex.Message}";
                await RecordHistoryAsync(stateCode, startedAt, DateTimeOffset.UtcNow, "Failed", result, stateSw.ElapsedMilliseconds);
                return result;
            }

            result.Districts.Received = externalDistricts.Count;
            _logger.LogInformation("Received {Count} districts for {StateName}", externalDistricts.Count, stateName);

            // 3. Load existing districts for change detection
            var existingDistricts = (await _courtRepository.GetDistrictsByStateAsync(stateEntity.StateId)).ToList();
            var externalDistrictCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var extDist in externalDistricts)
            {
                if (cancellationToken.IsCancellationRequested) break;

                if ((currentTotalApiRequests + result.StateApiRequests) >= settings.MaxApiRequestsPerRun)
                {
                    _logger.LogWarning("API request limit reached during district processing for state {StateCode}.", stateCode);
                    result.HasError = true;
                    result.ErrorMessage = "Reached maximum API requests limit.";
                    break;
                }

                externalDistrictCodes.Add(extDist.DistrictCode);
                District? existingDist = existingDistricts.FirstOrDefault(d =>
                    string.Equals(d.DistrictCode, extDist.DistrictCode, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(d.DistrictName, extDist.DistrictName, StringComparison.OrdinalIgnoreCase));

                District districtEntity;
                if (existingDist == null)
                {
                    districtEntity = await _courtRepository.UpsertDistrictAsync(stateEntity.StateId, extDist.DistrictCode, extDist.DistrictName);
                    result.Districts.Inserted++;
                    _logger.LogDebug("Inserted district {DistrictCode} - {DistrictName}", extDist.DistrictCode, extDist.DistrictName);
                }
                else
                {
                    districtEntity = existingDist;
                    bool isChanged = !string.Equals(existingDist.DistrictName, extDist.DistrictName, StringComparison.Ordinal) ||
                                     !string.Equals(existingDist.DistrictCode, extDist.DistrictCode, StringComparison.OrdinalIgnoreCase) ||
                                     existingDist.Status != "Active";

                    if (isChanged)
                    {
                        districtEntity = await _courtRepository.UpsertDistrictAsync(stateEntity.StateId, extDist.DistrictCode, extDist.DistrictName);
                        result.Districts.Updated++;
                        _logger.LogDebug("Updated district {DistrictCode} - {DistrictName}", extDist.DistrictCode, extDist.DistrictName);
                    }
                    else
                    {
                        result.Districts.Skipped++;
                    }
                }

                // 4. Fetch and synchronize court complexes for this district
                try
                {
                    _logger.LogInformation("Requesting court complexes for district {DistrictName} ({DistrictCode})...",
                        extDist.DistrictName, extDist.DistrictCode);

                    var externalComplexes = await _ecourtClient.GetComplexesAsync(stateCode, extDist.DistrictCode);
                    result.StateApiRequests++;
                    result.CourtComplexes.Received += externalComplexes.Count;

                    var existingComplexes = (await _courtRepository.GetComplexesByDistrictAsync(districtEntity.DistrictId)).ToList();
                    var externalComplexCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var extComplex in externalComplexes)
                    {
                        externalComplexCodes.Add(extComplex.CourtComplexCode);
                        var existingComp = existingComplexes.FirstOrDefault(c =>
                            string.Equals(c.ComplexCode, extComplex.CourtComplexCode, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(c.ComplexName, extComplex.CourtComplexName, StringComparison.OrdinalIgnoreCase));

                        CourtComplex complexEntity;
                        if (existingComp == null)
                        {
                            complexEntity = await _courtRepository.UpsertComplexAsync(districtEntity.DistrictId, extComplex.CourtComplexCode, extComplex.CourtComplexName);
                            result.CourtComplexes.Inserted++;
                            _logger.LogDebug("Inserted complex {Code} - {Name}", extComplex.CourtComplexCode, extComplex.CourtComplexName);
                        }
                        else
                        {
                            bool compChanged = !string.Equals(existingComp.ComplexName, extComplex.CourtComplexName, StringComparison.Ordinal) ||
                                              !string.Equals(existingComp.ComplexCode, extComplex.CourtComplexCode, StringComparison.OrdinalIgnoreCase) ||
                                              existingComp.Status != "Active";

                            if (compChanged)
                            {
                                complexEntity = await _courtRepository.UpsertComplexAsync(districtEntity.DistrictId, extComplex.CourtComplexCode, extComplex.CourtComplexName);
                                result.CourtComplexes.Updated++;
                                _logger.LogDebug("Updated complex {Code} - {Name}", extComplex.CourtComplexCode, extComplex.CourtComplexName);
                            }
                            else
                            {
                                complexEntity = existingComp;
                                result.CourtComplexes.Skipped++;
                            }
                        }

                        // 5. Fetch and synchronize individual courts for this court complex
                        if ((currentTotalApiRequests + result.StateApiRequests) < settings.MaxApiRequestsPerRun)
                        {
                            try
                            {
                                _logger.LogInformation("Requesting courts for complex {ComplexName} ({ComplexCode})...",
                                    extComplex.CourtComplexName, extComplex.CourtComplexCode);

                                var externalCourts = (await _ecourtClient.GetCourtsAsync(stateCode, extDist.DistrictCode, extComplex.CourtComplexCode)) ?? new List<ECourtsCourtItem>();
                                result.StateApiRequests++;
                                result.Courts.Received += externalCourts.Count;

                                var existingCourts = (await _courtRepository.GetCourtsByComplexAsync(complexEntity.CourtComplexId) ?? Enumerable.Empty<Court>()).ToList();

                                foreach (var extCourt in externalCourts)
                                {
                                    var courtCode = !string.IsNullOrWhiteSpace(extCourt.Court) ? extCourt.Court : extCourt.CourtNo ?? Guid.NewGuid().ToString();
                                    var existingCourt = existingCourts.FirstOrDefault(c =>
                                        string.Equals(c.CourtCode, courtCode, StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(c.CourtName, extCourt.CourtName, StringComparison.OrdinalIgnoreCase));

                                    if (existingCourt == null)
                                    {
                                        await _courtRepository.UpsertCourtAsync(complexEntity.CourtComplexId, courtCode, extCourt.CourtName, extCourt.CourtNo, extCourt.JudgeName);
                                        result.Courts.Inserted++;
                                        _logger.LogDebug("Inserted court {Code} - {Name}", courtCode, extCourt.CourtName);
                                    }
                                    else
                                    {
                                        bool courtChanged = !string.Equals(existingCourt.CourtName, extCourt.CourtName, StringComparison.Ordinal) ||
                                                            !string.Equals(existingCourt.CourtCode, courtCode, StringComparison.OrdinalIgnoreCase) ||
                                                            existingCourt.Status != "Active";

                                        if (courtChanged)
                                        {
                                            await _courtRepository.UpsertCourtAsync(complexEntity.CourtComplexId, courtCode, extCourt.CourtName, extCourt.CourtNo, extCourt.JudgeName);
                                            result.Courts.Updated++;
                                            _logger.LogDebug("Updated court {Code} - {Name}", courtCode, extCourt.CourtName);
                                        }
                                        else
                                        {
                                            result.Courts.Skipped++;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error synchronizing courts for complex {ComplexCode}.", extComplex.CourtComplexCode);
                                result.Courts.Failed++;
                            }
                        }
                    }

                    // Deactivate missing complexes if configured
                    if (settings.DeactivateMissingEntities && existingDist != null)
                    {
                        foreach (var existingComp in existingComplexes)
                        {
                            if (!string.IsNullOrWhiteSpace(existingComp.ComplexCode) &&
                                !externalComplexCodes.Contains(existingComp.ComplexCode) &&
                                existingComp.Status == "Active")
                            {
                                await _courtRepository.UpdateComplexStatusAsync(existingComp.CourtComplexId, "Inactive");
                                result.CourtComplexes.Updated++;
                                _logger.LogInformation("Deactivated missing court complex: {Code}", existingComp.ComplexCode);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error synchronizing court complexes for district {DistrictCode}.", extDist.DistrictCode);
                    result.Districts.Failed++;
                    result.HasError = true;
                    result.ErrorMessage = $"Error on district {extDist.DistrictCode}: {ex.Message}";
                }
            }

            // Deactivate missing districts if configured
            if (settings.DeactivateMissingEntities)
            {
                foreach (var dist in existingDistricts)
                {
                    if (!string.IsNullOrWhiteSpace(dist.DistrictCode) &&
                        !externalDistrictCodes.Contains(dist.DistrictCode) &&
                        dist.Status == "Active")
                    {
                        await _courtRepository.UpdateDistrictStatusAsync(dist.DistrictId, "Inactive");
                        result.Districts.Updated++;
                        _logger.LogInformation("Deactivated missing district: {Code}", dist.DistrictCode);
                    }
                }
            }

            stateSw.Stop();
            string status = result.HasError ? "Partial" : "Completed";
            await RecordHistoryAsync(stateCode, startedAt, DateTimeOffset.UtcNow, status, result, stateSw.ElapsedMilliseconds);

            return result;
        }

        private async Task RecordHistoryAsync(
            string stateCode,
            DateTimeOffset startedAt,
            DateTimeOffset completedAt,
            string status,
            StateSyncInternalResult result,
            long durationMs)
        {
            try
            {
                var history = new CourtMasterSyncHistory
                {
                    Id = Guid.NewGuid(),
                    State = stateCode,
                    StartedAt = startedAt,
                    CompletedAt = completedAt,
                    Status = status,
                    RecordsRead = result.Districts.Received + result.CourtComplexes.Received,
                    RecordsInserted = result.Districts.Inserted + result.CourtComplexes.Inserted,
                    RecordsUpdated = result.Districts.Updated + result.CourtComplexes.Updated,
                    RecordsSkipped = result.Districts.Skipped + result.CourtComplexes.Skipped,
                    RecordsFailed = result.Districts.Failed + result.CourtComplexes.Failed,
                    ApiRequests = result.StateApiRequests,
                    ErrorMessage = result.ErrorMessage,
                    DurationMs = durationMs
                };

                await _courtRepository.AddSyncHistoryAsync(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist CourtMasterSyncHistory audit record.");
            }
        }

        private class StateSyncInternalResult
        {
            public string StateCode { get; set; } = null!;
            public SyncEntityStats Districts { get; set; } = new();
            public SyncEntityStats CourtComplexes { get; set; } = new();
            public SyncEntityStats Courts { get; set; } = new();
            public int StateApiRequests { get; set; }
            public bool HasError { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }
}
