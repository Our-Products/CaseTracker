using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Lawyer entity operations.
    /// </summary>
    public interface ILawyerRepository : IRepository<Lawyer>
    {
        Task<Lawyer?> GetByUserIdAsync(Guid userId);

        Task<Lawyer?> GetByBarCouncilIdAsync(string barCouncilId);

        Task<IEnumerable<Lawyer>> GetByLawFirmIdAsync(Guid lawFirmId);

        Task<IEnumerable<Lawyer>> GetAllActiveAsync();

        Task<IEnumerable<Lawyer>> SearchByNameAsync(string fullName);
    }
}