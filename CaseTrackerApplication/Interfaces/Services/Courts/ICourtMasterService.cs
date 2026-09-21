using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.DTOs.ECourts;

namespace CaseTrackerApplication.Interfaces.Services.Courts
{
    public interface ICourtMasterService
    {
        Task<IEnumerable<StateDto>> GetStatesAsync();
        Task<IEnumerable<DistrictDto>> GetDistrictsAsync(Guid stateId);
        Task<IEnumerable<CourtComplexDto>> GetComplexesAsync(Guid districtId);
        Task<IEnumerable<CourtDto>> GetCourtsAsync(Guid complexId);
        Task<CourtMasterSyncResultDto> SynchronizeMasterDataAsync(string stateCode);
    }
}
