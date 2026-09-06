using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for UserLawFirm junction entity operations.
    /// Defines all data access contracts for user-law firm associations.
    /// </summary>
    public interface IUserLawFirmRepository
    {
        /// <summary>
        /// Get all law firms associated with a specific user asynchronously.
        /// </summary>
        Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Get all users associated with a specific law firm asynchronously.
        /// </summary>
        Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(Guid lawFirmId);

        /// <summary>
        /// Get a specific UserLawFirm association asynchronously.
        /// </summary>
        Task<UserLawFirm?> GetByUserAndLawFirmAsync(Guid userId, Guid lawFirmId);

        /// <summary>
        /// Check if a user is associated with a law firm asynchronously.
        /// </summary>
        Task<bool> IsUserInLawFirmAsync(Guid userId, Guid lawFirmId);

        /// <summary>
        /// Get all active UserLawFirm associations asynchronously.
        /// </summary>
        Task<IEnumerable<UserLawFirm>> GetAllActiveAsync();
    }
}
