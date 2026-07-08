using CaseTracker.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaseTrackerApp.Services;

public interface ICaseApi
{
    Task<List<CaseDto>> GetCasesAsync();
    Task<CaseDto?> GetCaseAsync(int id);
    Task<CaseDto> CreateCaseAsync(CaseDto item);
    Task UpdateCaseAsync(CaseDto item);
    Task DeleteCaseAsync(int id);
}
