using Domain.Models;

namespace Application.Interfaces
{
    /// <summary>
    /// Repository interface for Lawyer entity operations.
    /// Defines all data access contracts for lawyer management.
    /// </summary>
    public interface ILawyerRepository
    {
        // Query operations
        Task<Lawyer?> GetByMobileNumberAsync(string mobileNumber);
        Task<Lawyer?> GetByIdAsync(Guid lawyerId);
        Task<bool> MobileNumberExistsAsync(string mobileNumber);
        Task<IEnumerable<Lawyer>> GetAllAsync();

        // Command operations
        Task<Lawyer> AddAsync(Lawyer lawyer);
        Task<Lawyer> UpdateAsync(Lawyer lawyer);
        Task DeleteAsync(Guid lawyerId);
        Task SaveChangesAsync();
    }
}
