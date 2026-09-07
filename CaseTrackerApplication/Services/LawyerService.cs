using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class LawyerService : ILawyerService
    {
        private readonly ILawyerRepository _lawyerRepository;

        public LawyerService(
            ILawyerRepository lawyerRepository)
        {
            _lawyerRepository = lawyerRepository;
        }

        public async Task<IEnumerable<Lawyer>> GetAllAsync()
        {
            return await _lawyerRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Lawyer>> GetAllActiveAsync()
        {
            return await _lawyerRepository.GetAllActiveAsync();
        }

        public async Task<Lawyer?> GetByIdAsync(
            Guid lawyerId)
        {
            return await _lawyerRepository.GetByIdAsync(
                lawyerId);
        }

        public async Task<Lawyer?> GetByUserIdAsync(
            Guid userId)
        {
            return await _lawyerRepository.GetByUserIdAsync(
                userId);
        }

        public async Task<Lawyer?> GetByBarCouncilIdAsync(
            string barCouncilId)
        {
            return await _lawyerRepository
                .GetByBarCouncilIdAsync(barCouncilId);
        }

        public async Task<IEnumerable<Lawyer>>
            GetByLawFirmIdAsync(Guid lawFirmId)
        {
            return await _lawyerRepository
                .GetByLawFirmIdAsync(lawFirmId);
        }

        public async Task<IEnumerable<Lawyer>>
            SearchByNameAsync(string fullName)
        {
            return await _lawyerRepository
                .SearchByNameAsync(fullName);
        }
    }
}