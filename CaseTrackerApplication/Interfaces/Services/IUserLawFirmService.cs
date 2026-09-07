using CaseTrackerApplication.DTOs.UserLawFirms;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IUserLawFirmService
    {
        Task<IEnumerable<UserLawFirm>> GetAllAsync();

        Task<IEnumerable<UserLawFirm>> GetAllActiveAsync();

        Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(
            Guid userId);

        Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(
            Guid lawFirmId);

        Task<UserLawFirm?> GetByUserAndLawFirmAsync(
            Guid userId,
            Guid lawFirmId);

        Task<bool> IsUserInLawFirmAsync(
            Guid userId,
            Guid lawFirmId);

        Task<UserLawFirm> AddUserToLawFirmAsync(
            AddUserLawFirmRequest request);

        Task RemoveUserFromLawFirmAsync(
            Guid userId,
            Guid lawFirmId);
    }
}