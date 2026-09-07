using CaseTrackerApplication.DTOs.UserLawFirms;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class UserLawFirmService : IUserLawFirmService
    {
        private readonly IUserLawFirmRepository _userLawFirmRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILawFirmRepository _lawFirmRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserLawFirmService(
            IUserLawFirmRepository userLawFirmRepository,
            IUserRepository userRepository,
            ILawFirmRepository lawFirmRepository,
            IUnitOfWork unitOfWork)
        {
            _userLawFirmRepository = userLawFirmRepository;
            _userRepository = userRepository;
            _lawFirmRepository = lawFirmRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserLawFirm>> GetAllAsync()
        {
            return await _userLawFirmRepository.GetAllAsync();
        }

        public async Task<IEnumerable<UserLawFirm>> GetAllActiveAsync()
        {
            return await _userLawFirmRepository.GetAllActiveAsync();
        }

        public async Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(
            Guid userId)
        {
            return await _userLawFirmRepository
                .GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(
            Guid lawFirmId)
        {
            return await _userLawFirmRepository
                .GetByLawFirmIdAsync(lawFirmId);
        }

        public async Task<UserLawFirm?> GetByUserAndLawFirmAsync(
            Guid userId,
            Guid lawFirmId)
        {
            return await _userLawFirmRepository
                .GetByUserAndLawFirmAsync(
                    userId,
                    lawFirmId);
        }

        public async Task<bool> IsUserInLawFirmAsync(
            Guid userId,
            Guid lawFirmId)
        {
            return await _userLawFirmRepository
                .IsUserInLawFirmAsync(
                    userId,
                    lawFirmId);
        }

        public async Task<UserLawFirm> AddUserToLawFirmAsync(
            AddUserLawFirmRequest request)
        {
            if (request == null)
                throw new ValidationException(
                    "Request is required.");

            if (request.UserId == Guid.Empty)
                throw new ValidationException(
                    "User ID is required.");

            if (request.LawFirmId == Guid.Empty)
                throw new ValidationException(
                    "Law firm ID is required.");

            var user = await _userRepository
                .GetByIdAsync(request.UserId);

            if (user == null)
                throw new NotFoundException(
                    "User not found.");

            if (user.Status != "Active")
                throw new ConflictException(
                    "User is inactive.");

            var lawFirm = await _lawFirmRepository
                .GetByIdAsync(request.LawFirmId);

            if (lawFirm == null)
                throw new NotFoundException(
                    "Law firm not found.");

            if (lawFirm.Status != "Active")
                throw new ConflictException(
                    "Law firm is inactive.");

            var exists = await _userLawFirmRepository
                .IsUserInLawFirmAsync(
                    request.UserId,
                    request.LawFirmId);

            if (exists)
                throw new ConflictException(
                    "User is already a member of this law firm.");

            var now = DateTimeOffset.UtcNow;

            var userLawFirm = new UserLawFirm
            {
                UserId = request.UserId,
                LawFirmId = request.LawFirmId,
                JoinedAt = now,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = request.UserId,
                UpdatedBy = request.UserId
            };

            await _userLawFirmRepository
                .AddAsync(userLawFirm);

            await _unitOfWork.SaveChangesAsync();

            return userLawFirm;
        }

        public async Task RemoveUserFromLawFirmAsync(
            Guid userId,
            Guid lawFirmId)
        {
            var userLawFirm =
                await _userLawFirmRepository
                    .GetByUserAndLawFirmAsync(
                        userId,
                        lawFirmId);

            if (userLawFirm == null)
                throw new NotFoundException(
                    "User is not a member of this law firm.");

            await _userLawFirmRepository
                .DeleteAsync(userLawFirm);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}