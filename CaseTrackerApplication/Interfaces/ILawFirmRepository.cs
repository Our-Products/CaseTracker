using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for LawFirm entity operations.
    /// Defines all data access contracts for law firm management.
    /// </summary>
    public interface ILawFirmRepository
    {
        /// <summary>
        /// Get LawFirm by registration number asynchronously.
        /// </summary>
        Task<LawFirm?> GetByRegistrationNumberAsync(string registrationNumber);

        /// <summary>
        /// Get all active law firms asynchronously.
        /// </summary>
        Task<IEnumerable<LawFirm>> GetAllActiveAsync();

        /// <summary>
        /// Search law firms by name asynchronously.
        /// </summary>
        Task<IEnumerable<LawFirm>> SearchByNameAsync(string firmName);
    }
}
