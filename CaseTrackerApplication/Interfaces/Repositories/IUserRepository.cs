using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for User-specific data access operations.
    /// Common CRUD operations are inherited from IRepository<User>.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByMobileNumberAsync(string mobileNumber);

        Task<bool> MobileNumberExistsAsync(string mobileNumber);

        Task<bool> EmailExistsAsync(string email);
    }
}