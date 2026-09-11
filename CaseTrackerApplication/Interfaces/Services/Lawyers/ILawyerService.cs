using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface ILawyerService
    {
        Task<IEnumerable<Lawyer>> GetAllAsync();

        Task<IEnumerable<Lawyer>> GetAllActiveAsync();

        Task<Lawyer?> GetByIdAsync(Guid lawyerId);

        Task<Lawyer?> GetByUserIdAsync(Guid userId);

        Task<Lawyer?> GetByBarCouncilIdAsync(
            string barCouncilId);

        Task<IEnumerable<Lawyer>> GetByLawFirmIdAsync(
            Guid lawFirmId);

        Task<IEnumerable<Lawyer>> SearchByNameAsync(
            string fullName);
    }
}