using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;

namespace CaseTrackerApplication.Interfaces.Services.ECourts
{
    public interface IECourtClient
    {
        Task<List<ECourtsStateItem>> GetStatesAsync();
        Task<List<ECourtsDistrictItem>> GetDistrictsAsync(string stateCode);
        Task<List<ECourtsComplexItem>> GetComplexesAsync(string stateCode, string districtCode);
        Task<List<ECourtsCourtItem>> GetCourtsAsync(string stateCode, string districtCode, string complexCode);
        Task<ECourtsCaseDetailPayload?> GetCaseDetailAsync(string cnrNumber);
    }
}
