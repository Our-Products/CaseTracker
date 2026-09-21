using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Cases;

namespace CaseTrackerApplication.Interfaces.Services.Cases
{
    public interface ICaseService
    {
        Task<IEnumerable<CaseDto>> GetAllCasesAsync(Guid? lawFirmId);
        Task<CaseDetailDto> GetCaseByIdAsync(Guid caseId);
        Task<CaseDto> GetCaseByCnrAsync(string cnrNumber);
        Task<CaseDto> CreateCaseAsync(CreateCaseRequest request, Guid currentUserId, Guid? lawFirmId);
        Task<CaseDto> UpdateCaseAsync(Guid caseId, UpdateCaseRequest request, Guid currentUserId);
        Task DeleteCaseAsync(Guid caseId, Guid currentUserId);
        Task<CaseDto> SyncCaseNowAsync(Guid caseId, Guid currentUserId);
        Task<IEnumerable<CaseLawyerItemDto>> GetCaseLawyersAsync(Guid caseId);
        Task<CaseLawyerItemDto> AssignLawyerAsync(Guid caseId, AssignCaseLawyerRequest request, Guid currentUserId);
        Task RemoveLawyerAsync(Guid caseId, Guid lawyerId, Guid currentUserId);
    }
}
