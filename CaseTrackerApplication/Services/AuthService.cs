using CaseTrackerApplication.DTOs;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    /// <summary>
    /// Handles authentication and user registration business logic.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILawyerRepository _lawyerRepository;
        private readonly ILawFirmRepository _lawFirmRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserLawFirmRepository _userLawFirmRepository;

        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ILawyerRepository lawyerRepository,
            ILawFirmRepository lawFirmRepository,
            IUserRoleRepository userRoleRepository,
            IUserLawFirmRepository userLawFirmRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _lawyerRepository = lawyerRepository;
            _lawFirmRepository = lawFirmRepository;
            _userRoleRepository = userRoleRepository;
            _userLawFirmRepository = userLawFirmRepository;

            _passwordService = passwordService;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        // =========================================================
        // REGISTER
        // =========================================================

        public async Task<AuthResult> RegisterAsync(
            RegisterRequest request)
        {
            // 1. Validate request
            ValidateRegisterRequest(request);

            // 2. Check mobile number
            var existingUser =
                await _userRepository.GetByMobileNumberAsync(
                    request.MobileNumber);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "An account with this mobile number already exists.");
            }

            // 3. Check email
            var emailExists =
                await _userRepository.EmailExistsAsync(
                    request.Email);

            if (emailExists)
            {
                throw new ConflictException(
                    "An account with this email already exists.");
            }

            // 4. Start transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 5. Create User
                var user = RegisterUser(request);

                await _userRepository.AddAsync(user);

                // 6. Determine Role
                var roleName = request.RegisterType switch
                {
                    RegisterType.Individual => "Lawyer",

                    RegisterType.Organization => "Admin",

                    _ => throw new ValidationException(
                        "Invalid register type.")
                };

                // 7. Get Role
                var role =
                    await _roleRepository.GetByNameAsync(roleName);

                if (role == null)
                {
                    throw new NotFoundException(
                        $"Role '{roleName}' not found.");
                }

                if (role.Status != "Active")
                {
                    throw new ConflictException(
                        $"Role '{roleName}' is not active.");
                }

                // 8. Create UserRole
                var userRole = RegisterUserRole(
                    user,
                    role);

                await _userRoleRepository.AddAsync(userRole);

                Guid? lawFirmId = null;

                // =====================================================
                // INDIVIDUAL REGISTRATION
                // =====================================================

                if (request.RegisterType ==
                    RegisterType.Individual)
                {
                    var lawyer =
                        RegisterLawyer(request, user);

                    await _lawyerRepository.AddAsync(lawyer);
                }

                // =====================================================
                // ORGANIZATION REGISTRATION
                // =====================================================

                if (request.RegisterType ==
                    RegisterType.Organization)
                {
                    // Check whether the registration number
                    // already exists.
                    var existingLawFirm =
                        await _lawFirmRepository
                            .GetByRegistrationNumberAsync(
                                request.LawFirm!.RegistrationNumber);

                    if (existingLawFirm != null)
                    {
                        throw new ConflictException(
                            "A law firm with this registration number already exists.");
                    }

                    // Create LawFirm
                    var lawFirm =
                        RegisterLawFirm(request, user);

                    lawFirmId = lawFirm.LawFirmId;

                    await _lawFirmRepository.AddAsync(
                        lawFirm);

                    // Create UserLawFirm
                    var userLawFirm =
                        RegisterUserLawFirm(
                            lawFirm,
                            user);

                    await _userLawFirmRepository.AddAsync(
                        userLawFirm);
                }

                // 9. Save everything
                await _unitOfWork.SaveChangesAsync();

                // 10. Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // 11. Get roles for JWT
                var roles =
                    await _userRoleRepository
                        .GetRoleNamesByUserIdAsync(
                            user.UserId);

                // 12. Generate JWT
                var token =
                    _jwtService.GenerateToken(
                        user,
                        roles);

                return new AuthResult
                {
                    Token = token,

                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(
                            _jwtService.GetExpiryMinutes()),

                    UserId = user.UserId,

                    LawFirmId = lawFirmId
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        // =========================================================
        // LOGIN
        // =========================================================

        public async Task<AuthResult> LoginAsync(
            LoginRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }

            if (string.IsNullOrWhiteSpace(
                request.MobileNumber))
            {
                throw new ValidationException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.Password))
            {
                throw new ValidationException(
                    "Password is required.");
            }

            // 1. Find user
            var user =
                await _userRepository
                    .GetByMobileNumberAsync(
                        request.MobileNumber);

            // Don't reveal whether mobile number exists.
            if (user == null)
            {
                throw new UnauthorizedException(
                    "Invalid credentials.");
            }

            // 2. Check account status
            if (user.Status != "Active")
            {
                throw new UnauthorizedException(
                    "Your account is inactive.");
            }

            // 3. Verify password
            var passwordValid =
                _passwordService.VerifyPassword(
                    request.Password,
                    user.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedException(
                    "Invalid credentials.");
            }

            // 4. Get active roles
            var roles =
                await _userRoleRepository
                    .GetRoleNamesByUserIdAsync(
                        user.UserId);

            // 5. Generate JWT
            var token =
                _jwtService.GenerateToken(
                    user,
                    roles);

            // 6. Return authentication result
            return new AuthResult
            {
                Token = token,

                ExpiresAt =
                    DateTime.UtcNow.AddMinutes(
                        _jwtService.GetExpiryMinutes()),

                UserId = user.UserId
            };
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        private void ValidateRegisterRequest(
            RegisterRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }

            if (string.IsNullOrWhiteSpace(
                request.MobileNumber))
            {
                throw new ValidationException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.Email))
            {
                throw new ValidationException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.Password))
            {
                throw new ValidationException(
                    "Password is required.");
            }

            if (request.RegisterType ==
                RegisterType.Organization)
            {
                if (request.LawFirm == null)
                {
                    throw new ValidationException(
                        "Law firm details are required for organization registration.");
                }

                if (string.IsNullOrWhiteSpace(
                    request.LawFirm.FirmName))
                {
                    throw new ValidationException(
                        "Firm name is required.");
                }

                if (string.IsNullOrWhiteSpace(
                    request.LawFirm.RegistrationNumber))
                {
                    throw new ValidationException(
                        "Registration number is required.");
                }
            }
        }

        // =========================================================
        // CREATE USER
        // =========================================================

        private User RegisterUser(
            RegisterRequest request)
        {
            var userId = Guid.NewGuid();

            var now = DateTimeOffset.UtcNow;

            return new User
            {
                UserId = userId,

                MobileNumber =
                    request.MobileNumber,

                Email =
                    request.Email,

                Password =
                    _passwordService.HashPassword(
                        request.Password),

                Status = "Active",

                CreatedAt = now,

                UpdatedAt = now,

                CreatedBy = userId,

                UpdatedBy = userId
            };
        }

        // =========================================================
        // CREATE USER ROLE
        // =========================================================

        private UserRole RegisterUserRole(
            User user,
            Role role)
        {
            var now = DateTimeOffset.UtcNow;

            return new UserRole
            {
                UserId = user.UserId,

                RoleId = role.RoleId,

                CreatedAt = now,

                UpdatedAt = now,

                CreatedBy = user.UserId,

                UpdatedBy = user.UserId
            };
        }

        // =========================================================
        // CREATE LAWYER
        // =========================================================

        private Lawyer RegisterLawyer(
            RegisterRequest request,
            User user)
        {
            var now = DateTimeOffset.UtcNow;

            return new Lawyer
            {
                LawyerId = Guid.NewGuid(),

                UserId = user.UserId,

                FullName =
                    request.FullName,

                BarCouncilId =
                    request.BarCouncilId,

                BarCouncilName =
                    request.BarCouncilName,

                EnrollmentDate =
                    request.EnrollmentDate,

                Status = "Active",

                CreatedAt = now,

                UpdatedAt = now,

                CreatedBy = user.UserId,

                UpdatedBy = user.UserId
            };
        }

        // =========================================================
        // CREATE LAW FIRM
        // =========================================================

        private LawFirm RegisterLawFirm(
            RegisterRequest request,
            User user)
        {
            var now = DateTimeOffset.UtcNow;

            return new LawFirm
            {
                LawFirmId = Guid.NewGuid(),

                FirmName =
                    request.LawFirm!.FirmName,

                RegistrationNumber =
                    request.LawFirm.RegistrationNumber,

                AddressLine1 =
                    request.LawFirm.AddressLine1,

                AddressLine2 =
                    request.LawFirm.AddressLine2,

                City =
                    request.LawFirm.City,

                District =
                    request.LawFirm.District,

                State =
                    request.LawFirm.State,

                Pincode =
                    request.LawFirm.Pincode,

                CreatedAt = now,

                UpdatedAt = now,

                CreatedBy = user.UserId,

                UpdatedBy = user.UserId
            };
        }

        // =========================================================
        // CREATE USER LAW FIRM
        // =========================================================

        private UserLawFirm RegisterUserLawFirm(
            LawFirm lawFirm,
            User user)
        {
            var now = DateTimeOffset.UtcNow;

            return new UserLawFirm
            {
                UserId =
                    user.UserId,

                LawFirmId =
                    lawFirm.LawFirmId,

                JoinedAt =
                    now,

                Status = "Active",

                CreatedAt =
                    now,

                UpdatedAt =
                    now,

                CreatedBy =
                    user.UserId,

                UpdatedBy =
                    user.UserId
            };
        }
    }
}