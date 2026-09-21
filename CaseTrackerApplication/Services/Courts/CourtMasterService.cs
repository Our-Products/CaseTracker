using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;

namespace CaseTrackerApplication.Services.Courts
{
    public class CourtMasterService : ICourtMasterService
    {
        private readonly ICourtRepository _courtRepository;
        private readonly IECourtClient _ecourtClient;

        public CourtMasterService(
            ICourtRepository courtRepository,
            IECourtClient ecourtClient)
        {
            _courtRepository = courtRepository;
            _ecourtClient = ecourtClient;
        }

        public async Task<IEnumerable<StateDto>> GetStatesAsync()
        {
            var states = await _courtRepository.GetAllStatesAsync();
            return states.Select(s => new StateDto
            {
                StateId = s.StateId,
                StateName = s.StateName,
                StateCode = s.StateCode,
                Status = s.Status
            });
        }

        public async Task<IEnumerable<DistrictDto>> GetDistrictsAsync(Guid stateId)
        {
            var districts = await _courtRepository.GetDistrictsByStateAsync(stateId);
            return districts.Select(d => new DistrictDto
            {
                DistrictId = d.DistrictId,
                DistrictName = d.DistrictName,
                StateId = d.StateId,
                DistrictCode = d.DistrictCode,
                Status = d.Status
            });
        }

        public async Task<IEnumerable<CourtComplexDto>> GetComplexesAsync(Guid districtId)
        {
            var complexes = await _courtRepository.GetComplexesByDistrictAsync(districtId);
            return complexes.Select(c => new CourtComplexDto
            {
                CourtComplexId = c.CourtComplexId,
                ComplexName = c.ComplexName,
                DistrictId = c.DistrictId,
                ComplexCode = c.ComplexCode,
                Status = c.Status
            });
        }

        public async Task<IEnumerable<CourtDto>> GetCourtsAsync(Guid complexId)
        {
            var courts = await _courtRepository.GetCourtsByComplexAsync(complexId);
            return courts.Select(c => new CourtDto
            {
                CourtId = c.CourtId,
                CourtName = c.CourtName,
                CourtCode = c.CourtCode,
                CourtComplexId = c.CourtComplexId,
                CourtTypeId = c.CourtTypeId,
                ComplexName = c.CourtComplex?.ComplexName,
                Status = c.Status
            });
        }

        public async Task<CourtMasterSyncResultDto> SynchronizeMasterDataAsync(string stateCode)
        {
            var sw = Stopwatch.StartNew();
            var startedAt = DateTimeOffset.UtcNow;
            string targetCode = stateCode.Trim().ToUpperInvariant();

            string stateName = targetCode switch
            {
                "TN" => "Tamil Nadu",
                "PY" => "Puducherry",
                _ => targetCode
            };

            var stateEntity = await _courtRepository.UpsertStateAsync(targetCode, stateName);
            int districtsCount = 0;
            int complexesCount = 0;
            int courtsCount = 0;

            try
            {
                var externalDistricts = await _ecourtClient.GetDistrictsAsync(targetCode);
                foreach (var extDist in externalDistricts)
                {
                    var districtEntity = await _courtRepository.UpsertDistrictAsync(
                        stateEntity.StateId, extDist.DistrictCode, extDist.DistrictName);
                    districtsCount++;

                    var externalComplexes = await _ecourtClient.GetComplexesAsync(targetCode, extDist.DistrictCode);
                    foreach (var extComplex in externalComplexes)
                    {
                        var complexEntity = await _courtRepository.UpsertComplexAsync(
                            districtEntity.DistrictId, extComplex.CourtComplexCode, extComplex.CourtComplexName);
                        complexesCount++;

                        var externalCourts = await _ecourtClient.GetCourtsAsync(targetCode, extDist.DistrictCode, extComplex.CourtComplexCode);
                        foreach (var extCourt in externalCourts)
                        {
                            await _courtRepository.UpsertCourtAsync(
                                complexEntity.CourtComplexId,
                                extCourt.Court,
                                extCourt.CourtName,
                                extCourt.CourtNo,
                                extCourt.JudgeName);
                            courtsCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new CourtMasterSyncResultDto
                {
                    TargetState = targetCode,
                    StartedAt = startedAt,
                    CompletedAt = DateTimeOffset.UtcNow,
                    StatesProcessed = 1,
                    DistrictsProcessed = districtsCount,
                    ComplexesProcessed = complexesCount,
                    CourtsProcessed = courtsCount,
                    DurationMs = sw.ElapsedMilliseconds,
                    Message = $"Synchronization partially completed with notice: {ex.Message}"
                };
            }

            sw.Stop();
            return new CourtMasterSyncResultDto
            {
                TargetState = targetCode,
                StartedAt = startedAt,
                CompletedAt = DateTimeOffset.UtcNow,
                StatesProcessed = 1,
                DistrictsProcessed = districtsCount,
                ComplexesProcessed = complexesCount,
                CourtsProcessed = courtsCount,
                DurationMs = sw.ElapsedMilliseconds,
                Message = $"Successfully synchronized master court tree for {stateName}."
            };
        }
    }
}
