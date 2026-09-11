using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID is required.");
            }

            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User?> GetByMobileNumberAsync(
            string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            return await _userRepository
                .GetByMobileNumberAsync(mobileNumber);
        }

        public async Task<bool> MobileNumberExistsAsync(
            string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            return await _userRepository
                .MobileNumberExistsAsync(mobileNumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            return await _userRepository
                .EmailExistsAsync(email);
        }
    }
}