using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class LawFirmService : ILawFirmService
    {
        private readonly ILawFirmRepository _lawFirmRepository;

        public LawFirmService(
            ILawFirmRepository lawFirmRepository)
        {
            _lawFirmRepository = lawFirmRepository;
        }

        public async Task<IEnumerable<LawFirm>> GetAllAsync()
        {
            return await _lawFirmRepository.GetAllAsync();
        }

        public async Task<IEnumerable<LawFirm>> GetAllActiveAsync()
        {
            return await _lawFirmRepository.GetAllActiveAsync();
        }

        public async Task<LawFirm?> GetByIdAsync(
            Guid lawFirmId)
        {
            return await _lawFirmRepository.GetByIdAsync(
                lawFirmId);
        }

        public async Task<LawFirm?> GetByRegistrationNumberAsync(
            string registrationNumber)
        {
            return await _lawFirmRepository
                .GetByRegistrationNumberAsync(
                    registrationNumber);
        }

        public async Task<IEnumerable<LawFirm>>
            SearchByNameAsync(string firmName)
        {
            return await _lawFirmRepository
                .SearchByNameAsync(firmName);
        }
    }
}