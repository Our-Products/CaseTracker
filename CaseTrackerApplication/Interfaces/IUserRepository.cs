using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for Lawyer entity operations.
    /// Defines all data access contracts for lawyer management.
    /// </summary>
    public interface IUserRepository
    {
        // Query operations
        Task<User?> GetByMobileNumberAsync(string mobileNumber);
        Task<User?> GetByIdAsync(Guid userId);
        Task<bool> MobileNumberExistsAsync(string mobileNumber);
        Task<IEnumerable<User>> GetAllAsync();

        // Command operations
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
