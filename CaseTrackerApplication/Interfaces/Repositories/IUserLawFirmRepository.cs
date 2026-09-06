using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories
{
    public interface IUserLawFirmRepository : IRepository<UserLawFirm>
    {
        Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(Guid lawFirmId);

        Task<UserLawFirm?> GetByUserAndLawFirmAsync(
            Guid userId,
            Guid lawFirmId);

        Task<bool> IsUserInLawFirmAsync(
            Guid userId,
            Guid lawFirmId);

        Task<IEnumerable<UserLawFirm>> GetAllActiveAsync();
    }
}