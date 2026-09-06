using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for Lawyer entity operations.
    /// Defines all data access contracts for lawyer management.
    /// </summary>
    public interface ILawyerRepository
    {
        /// <summary>
        /// Get Lawyer by UserId asynchronously.
        /// </summary>
        Task<Lawyer?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Get Lawyer by Bar Council ID asynchronously.
        /// </summary>
        Task<Lawyer?> GetByBarCouncilIdAsync(string barCouncilId);

        /// <summary>
        /// Get all lawyers associated with a specific law firm asynchronously.
        /// </summary>
        Task<IEnumerable<Lawyer>> GetByLawFirmIdAsync(Guid lawFirmId);

        /// <summary>
        /// Get all active lawyers asynchronously.
        /// </summary>
        Task<IEnumerable<Lawyer>> GetAllActiveAsync();

        /// <summary>
        /// Search lawyers by full name asynchronously.
        /// </summary>
        Task<IEnumerable<Lawyer>> SearchByNameAsync(string fullName);
    }
}
