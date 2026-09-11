using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface ILawFirmService
    {
        Task<IEnumerable<LawFirm>> GetAllAsync();

        Task<IEnumerable<LawFirm>> GetAllActiveAsync();

        Task<LawFirm?> GetByIdAsync(Guid lawFirmId);

        Task<LawFirm?> GetByRegistrationNumberAsync(
            string registrationNumber);

        Task<IEnumerable<LawFirm>> SearchByNameAsync(
            string firmName);
    }
}