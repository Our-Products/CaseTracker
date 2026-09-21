using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services.ECourts
{
    public class ECourtSyncService : IECourtSyncService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IHearingRepository _hearingRepository;
        private readonly IECourtClient _ecourtClient;
        private readonly IECourtApiLogRepository _apiLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        private const int DefaultBatchSize = 25;
        private const int DefaultSyncIntervalHours = 24;

        public ECourtSyncService(
            ICaseRepository caseRepository,
            IHearingRepository hearingRepository,
            IECourtClient ecourtClient,
            IECourtApiLogRepository apiLogRepository,
            IUnitOfWork unitOfWork)
        {
            _caseRepository = caseRepository;
            _hearingRepository = hearingRepository;
            _ecourtClient = ecourtClient;
            _apiLogRepository = apiLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ECourtSyncResultDto> RunScheduledCaseStatusSyncAsync(int? batchSizeOverride = null)
        {
            var sw = Stopwatch.StartNew();
            var startedAt = DateTimeOffset.UtcNow;
            int batchSize = batchSizeOverride ?? DefaultBatchSize;

            var eligibleCases = (await _caseRepository.GetEligibleForSyncAsync(batchSize, DefaultSyncIntervalHours)).ToList();

            var result = new ECourtSyncResultDto
            {
                JobName = "CaseStatusSync",
                StartedAt = startedAt,
                TotalEligible = eligibleCases.Count,
                Processed = 0,
                Succeeded = 0,
                Failed = 0,
                Skipped = 0,
                CreditsCharged = 0,
                Errors = new List<string>()
            };

            foreach (var caseEntity in eligibleCases)
            {
                if (string.IsNullOrWhiteSpace(caseEntity.CnrNumber))
                {
                    result.Skipped++;
                    continue;
                }

                result.Processed++;
                var attemptTime = DateTimeOffset.UtcNow;
                caseEntity.LastSyncAttemptAt = attemptTime;

                try
                {
                    var payload = await _ecourtClient.GetCaseDetailAsync(caseEntity.CnrNumber);
                    if (payload?.CourtCaseData != null)
                    {
                        var data = payload.CourtCaseData;

                        if (!string.IsNullOrWhiteSpace(data.CaseStatus))
                        {
                            caseEntity.CaseStatus = data.CaseStatus;
                        }
                        if (!string.IsNullOrWhiteSpace(data.CaseNumber))
                        {
                            caseEntity.CaseNumber = data.CaseNumber;
                        }
                        if (!string.IsNullOrWhiteSpace(data.RegistrationNumber))
                        {
                            caseEntity.RegistrationNumber = data.RegistrationNumber;
                        }

                        // Sync upcoming and historical hearings
                        if (data.HistoryOfCaseHearings != null)
                        {
                            var existingHearings = (await _hearingRepository.GetHearingsByCaseAsync(caseEntity.CaseId)).ToList();
                            foreach (var h in data.HistoryOfCaseHearings)
                            {
                                if (DateTime.TryParse(h.HearingDate, out var hDate))
                                {
                                    if (!existingHearings.Any(eh => eh.HearingDate.Date == hDate.Date))
                                    {
                                        await _hearingRepository.AddAsync(new CaseHearing
                                        {
                                            HearingId = Guid.NewGuid(),
                                            CaseId = caseEntity.CaseId,
                                            HearingDate = hDate,
                                            JudgeName = h.Judge,
                                            PurposeOfHearing = string.IsNullOrWhiteSpace(h.PurposeOfListing) ? "Hearing" : h.PurposeOfListing,
                                            BusinessOnDate = h.BusinessOnDate,
                                            HearingStatus = "Adjourned",
                                            CreatedAt = attemptTime,
                                            UpdatedAt = attemptTime
                                        });
                                    }
                                }
                            }
                        }

                        caseEntity.LastSyncedAt = attemptTime;
                        caseEntity.LastSuccessfulSyncAt = attemptTime;
                        caseEntity.SyncStatus = "Success";
                        caseEntity.SyncError = null;
                        caseEntity.UpdatedAt = attemptTime;

                        result.Succeeded++;
                        result.CreditsCharged += 1; // standard credit deduction per case detail
                    }
                    else
                    {
                        caseEntity.SyncStatus = "Failed";
                        caseEntity.SyncError = "No docket record returned from eCourts.";
                        caseEntity.UpdatedAt = attemptTime;
                        result.Failed++;
                        result.Errors.Add($"CNR {caseEntity.CnrNumber}: No data returned.");
                    }
                }
                catch (Exception ex)
                {
                    caseEntity.SyncStatus = "Failed";
                    caseEntity.SyncError = ex.Message;
                    caseEntity.UpdatedAt = attemptTime;
                    result.Failed++;
                    result.Errors.Add($"CNR {caseEntity.CnrNumber}: {ex.Message}");
                }

                await _caseRepository.UpdateAsync(caseEntity);
                await _unitOfWork.SaveChangesAsync();
            }

            sw.Stop();
            result.CompletedAt = DateTimeOffset.UtcNow;
            result.DurationMs = sw.ElapsedMilliseconds;
            result.Message = $"Completed batch sync. Processed: {result.Processed}, Succeeded: {result.Succeeded}, Failed: {result.Failed}, Skipped: {result.Skipped}.";

            return result;
        }

        public async Task<ECourtSyncResultDto> RunScheduledHearingSyncAsync(int? batchSizeOverride = null, int daysAhead = 14)
        {
            var sw = Stopwatch.StartNew();
            var startedAt = DateTimeOffset.UtcNow;
            int batchSize = batchSizeOverride ?? DefaultBatchSize;

            var eligibleCases = (await _caseRepository.GetEligibleForHearingSyncAsync(batchSize, daysAhead)).ToList();

            var result = new ECourtSyncResultDto
            {
                JobName = "HearingSync",
                StartedAt = startedAt,
                TotalEligible = eligibleCases.Count,
                Processed = 0,
                Succeeded = 0,
                Failed = 0,
                Skipped = 0,
                CreditsCharged = 0,
                Errors = new List<string>()
            };

            foreach (var caseEntity in eligibleCases)
            {
                if (string.IsNullOrWhiteSpace(caseEntity.CnrNumber))
                {
                    result.Skipped++;
                    continue;
                }

                result.Processed++;
                var attemptTime = DateTimeOffset.UtcNow;
                caseEntity.LastSyncAttemptAt = attemptTime;

                try
                {
                    var payload = await _ecourtClient.GetCaseDetailAsync(caseEntity.CnrNumber);
                    if (payload?.CourtCaseData != null)
                    {
                        var data = payload.CourtCaseData;

                        if (data.HistoryOfCaseHearings != null && data.HistoryOfCaseHearings.Any())
                        {
                            var existingHearings = (await _hearingRepository.GetHearingsByCaseAsync(caseEntity.CaseId)).ToList();
                            foreach (var h in data.HistoryOfCaseHearings)
                            {
                                if (DateTime.TryParse(h.HearingDate, out var hDate))
                                {
                                    var existing = existingHearings.FirstOrDefault(eh => eh.HearingDate.Date == hDate.Date);
                                    if (existing != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(h.Judge))
                                            existing.JudgeName = h.Judge.Trim();
                                        if (!string.IsNullOrWhiteSpace(h.PurposeOfListing))
                                            existing.PurposeOfHearing = h.PurposeOfListing.Trim();
                                        if (!string.IsNullOrWhiteSpace(h.BusinessOnDate))
                                            existing.BusinessOnDate = h.BusinessOnDate.Trim();

                                        existing.HearingStatus = hDate.Date >= DateTime.UtcNow.Date ? "Scheduled" : "Adjourned";
                                        existing.UpdatedAt = attemptTime;
                                        await _hearingRepository.UpdateAsync(existing);
                                    }
                                    else
                                    {
                                        await _hearingRepository.AddAsync(new CaseHearing
                                        {
                                            HearingId = Guid.NewGuid(),
                                            CaseId = caseEntity.CaseId,
                                            HearingDate = hDate,
                                            JudgeName = h.Judge?.Trim(),
                                            PurposeOfHearing = string.IsNullOrWhiteSpace(h.PurposeOfListing) ? "Hearing" : h.PurposeOfListing.Trim(),
                                            BusinessOnDate = h.BusinessOnDate?.Trim(),
                                            HearingStatus = hDate.Date >= DateTime.UtcNow.Date ? "Scheduled" : "Adjourned",
                                            CreatedAt = attemptTime,
                                            UpdatedAt = attemptTime
                                        });
                                    }
                                }
                            }
                        }

                        caseEntity.LastSyncedAt = attemptTime;
                        caseEntity.LastSuccessfulSyncAt = attemptTime;
                        caseEntity.SyncStatus = "Success";
                        caseEntity.SyncError = null;
                        caseEntity.UpdatedAt = attemptTime;

                        result.Succeeded++;
                        result.CreditsCharged += 1;
                    }
                    else
                    {
                        caseEntity.SyncStatus = "Failed";
                        caseEntity.SyncError = "No docket record returned from eCourts.";
                        caseEntity.UpdatedAt = attemptTime;
                        result.Failed++;
                        result.Errors.Add($"CNR {caseEntity.CnrNumber}: No data returned.");
                    }
                }
                catch (Exception ex)
                {
                    caseEntity.SyncStatus = "Failed";
                    caseEntity.SyncError = ex.Message;
                    caseEntity.UpdatedAt = attemptTime;
                    result.Failed++;
                    result.Errors.Add($"CNR {caseEntity.CnrNumber}: {ex.Message}");
                }

                await _caseRepository.UpdateAsync(caseEntity);
                await _unitOfWork.SaveChangesAsync();
            }

            sw.Stop();
            result.CompletedAt = DateTimeOffset.UtcNow;
            result.DurationMs = sw.ElapsedMilliseconds;
            result.Message = $"Completed hearing sync. Processed: {result.Processed}, Succeeded: {result.Succeeded}, Failed: {result.Failed}, Skipped: {result.Skipped}.";

            return result;
        }

        public async Task<ECourtUsageSummaryDto> GetApiUsageSummaryAsync(DateTimeOffset from, DateTimeOffset to)
        {
            return await _apiLogRepository.GetUsageSummaryAsync(from, to);
        }
    }
}
