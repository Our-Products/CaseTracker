using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetByMobileNumberAsync(string mobileNumber);

        Task<bool> MobileNumberExistsAsync(string mobileNumber);
        Task<bool> EmailExistsAsync(string email);
    }
}