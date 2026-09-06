using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for LawFirm entity operations.
    /// </summary>
    public interface ILawFirmRepository : IRepository<LawFirm>
    {
        Task<LawFirm?> GetByRegistrationNumberAsync(
            string registrationNumber);

        Task<IEnumerable<LawFirm>> GetAllActiveAsync();

        Task<IEnumerable<LawFirm>> SearchByNameAsync(
            string firmName);
    }
}