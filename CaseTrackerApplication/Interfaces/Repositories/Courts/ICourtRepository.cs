using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories.Courts
{
    public interface ICourtRepository
    {
        Task<IEnumerable<State>> GetAllStatesAsync();
        Task<State?> GetStateByCodeAsync(string stateCode);
        Task<State> UpsertStateAsync(string stateCode, string stateName);

        Task<IEnumerable<District>> GetDistrictsByStateAsync(Guid stateId);
        Task<District?> GetDistrictByCodeAsync(Guid stateId, string districtCode);
        Task<District> UpsertDistrictAsync(Guid stateId, string districtCode, string districtName);

        Task<IEnumerable<CourtComplex>> GetComplexesByDistrictAsync(Guid districtId);
        Task<CourtComplex?> GetComplexByCodeAsync(Guid districtId, string complexCode);
        Task<CourtComplex> UpsertComplexAsync(Guid districtId, string complexCode, string complexName);

        Task<IEnumerable<Court>> GetCourtsByComplexAsync(Guid complexId);
        Task<Court?> GetCourtByCodeAsync(Guid complexId, string courtCode);
        Task<Court> UpsertCourtAsync(Guid complexId, string courtCode, string courtName, string? courtNo, string? judgeName);

        Task<Court?> GetCourtByIdAsync(Guid courtId);

        Task UpdateDistrictStatusAsync(Guid districtId, string status);
        Task UpdateComplexStatusAsync(Guid courtComplexId, string status);
        Task AddSyncHistoryAsync(CourtMasterSyncHistory history);
        Task<IEnumerable<CourtMasterSyncHistory>> GetSyncHistoriesAsync(int count = 10);
    }
}
