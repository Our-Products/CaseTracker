using CaseTrackerApplication.DTOs.Auth;
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
            ValidateRegisterRequest(request);

            // -----------------------------------------------------
            // Check existing mobile number
            // -----------------------------------------------------

            var existingUser =
                await _userRepository.GetByMobileNumberAsync(
                    request.MobileNumber);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "An account with this mobile number already exists.");
            }

            // -----------------------------------------------------
            // Check existing email
            // -----------------------------------------------------

            var emailExists =
                await _userRepository.EmailExistsAsync(
                    request.Email);

            if (emailExists)
            {
                throw new ConflictException(
                    "An account with this email already exists.");
            }

            // -----------------------------------------------------
            // Begin transaction
            // -----------------------------------------------------

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // -------------------------------------------------
                // 1. Create User
                // -------------------------------------------------

                var user = RegisterUser(request);

                await _userRepository.AddAsync(user);

                // -------------------------------------------------
                // 2. Determine Role
                // -------------------------------------------------

                var roleName = request.RegisterType switch
                {
                    RegisterType.Individual => "Lawyer",
                    RegisterType.Organization => "Admin",

                    _ => throw new ValidationException(
                        "Invalid register type.")
                };

                // -------------------------------------------------
                // 3. Get Role
                // -------------------------------------------------

                var role =
                    await _roleRepository.GetByNameAsync(
                        roleName);

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

                // -------------------------------------------------
                // 4. Create UserRole
                // -------------------------------------------------

                var userRole =
                    RegisterUserRole(
                        user,
                        role);

                await _userRoleRepository.AddAsync(
                    userRole);

                Guid? lawFirmId = null;

                // =================================================
                // INDIVIDUAL REGISTRATION
                // =================================================

                if (request.RegisterType ==
                    RegisterType.Individual)
                {
                    var lawyer =
                        RegisterIndividualLawyer(
                            request,
                            user);

                    await _lawyerRepository.AddAsync(
                        lawyer);
                }

                // =================================================
                // ORGANIZATION REGISTRATION
                // =================================================

                if (request.RegisterType ==
                    RegisterType.Organization)
                {
                    // ---------------------------------------------
                    // LawFirm is guaranteed by validation
                    // ---------------------------------------------

                    var lawFirmRequest =
                        request.LawFirm!;

                    // ---------------------------------------------
                    // Check duplicate registration number
                    // ---------------------------------------------

                    var existingLawFirm =
                        await _lawFirmRepository
                            .GetByRegistrationNumberAsync(
                                lawFirmRequest.RegistrationNumber!);

                    if (existingLawFirm != null)
                    {
                        throw new ConflictException(
                            "A law firm with this registration number already exists.");
                    }

                    // ---------------------------------------------
                    // Create LawFirm
                    // ---------------------------------------------

                    var lawFirm =
                        RegisterLawFirm(
                            request,
                            user);

                    lawFirmId =
                        lawFirm.LawFirmId;

                    await _lawFirmRepository.AddAsync(
                        lawFirm);

                    // ---------------------------------------------
                    // Create UserLawFirm membership
                    // ---------------------------------------------

                    var userLawFirm =
                        RegisterUserLawFirm(
                            lawFirm,
                            user);

                    await _userLawFirmRepository.AddAsync(
                        userLawFirm);

                    // ---------------------------------------------
                    // Create Lawyer associated with LawFirm
                    // ---------------------------------------------

                    var lawyer =
                        RegisterOrganizationalLawyer(
                            request,
                            user,
                            lawFirm);

                    await _lawyerRepository.AddAsync(
                        lawyer);
                }

                // -------------------------------------------------
                // 5. Save all changes
                // -------------------------------------------------

                await _unitOfWork.SaveChangesAsync();

                // -------------------------------------------------
                // 6. Commit transaction
                // -------------------------------------------------

                await _unitOfWork.CommitTransactionAsync();

                // -------------------------------------------------
                // 7. Get roles for JWT
                // -------------------------------------------------

                var roles =
                    await _userRoleRepository
                        .GetRoleNamesByUserIdAsync(
                            user.UserId);

                // -------------------------------------------------
                // 8. Generate JWT
                // -------------------------------------------------

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

                    UserId =
                        user.UserId,

                    LawFirmId =
                        lawFirmId
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
            // -----------------------------------------------------
            // Validate request
            // -----------------------------------------------------

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

            // -----------------------------------------------------
            // Find user
            // -----------------------------------------------------

            var user =
                await _userRepository
                    .GetByMobileNumberAsync(
                        request.MobileNumber);

            // Do not reveal whether mobile number exists.
            if (user == null)
            {
                throw new UnauthorizedException(
                    "Invalid credentials.");
            }

            // -----------------------------------------------------
            // Check account status
            // -----------------------------------------------------

            if (user.Status != "Active")
            {
                throw new UnauthorizedException(
                    "Your account is inactive.");
            }

            // -----------------------------------------------------
            // Verify password
            // -----------------------------------------------------

            var passwordValid =
                _passwordService.VerifyPassword(
                    request.Password,
                    user.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedException(
                    "Invalid credentials.");
            }

            // -----------------------------------------------------
            // Get user roles
            // -----------------------------------------------------

            var roles =
                await _userRoleRepository
                    .GetRoleNamesByUserIdAsync(
                        user.UserId);

            // -----------------------------------------------------
            // Generate JWT
            // -----------------------------------------------------

            var token =
                _jwtService.GenerateToken(
                    user,
                    roles);

            // -----------------------------------------------------
            // Return authentication result
            // -----------------------------------------------------

            return new AuthResult
            {
                Token = token,

                ExpiresAt =
                    DateTime.UtcNow.AddMinutes(
                        _jwtService.GetExpiryMinutes()),

                UserId =
                    user.UserId
            };
        }

        // =========================================================
        // VALIDATE REGISTER REQUEST
        // =========================================================

        private void ValidateRegisterRequest(
            RegisterRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }

            // -----------------------------------------------------
            // Basic validation
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                request.FullName))
            {
                throw new ValidationException(
                    "Full name is required.");
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

            // -----------------------------------------------------
            // Validate RegisterType
            // -----------------------------------------------------

            if (!Enum.IsDefined(
                typeof(RegisterType),
                request.RegisterType))
            {
                throw new ValidationException(
                    "Invalid register type.");
            }

            // =====================================================
            // ORGANIZATION VALIDATION
            // =====================================================

            if (request.RegisterType ==
                RegisterType.Organization)
            {
                // LawFirm is required only for Organization.

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

            // =====================================================
            // INDIVIDUAL VALIDATION
            // =====================================================

            // LawFirm is intentionally not validated here.
            //
            // For Individual registration:
            //
            // request.LawFirm == null
            //
            // is completely valid.
        }

        // =========================================================
        // CREATE USER
        // =========================================================

        private User RegisterUser(
            RegisterRequest request)
        {
            var userId =
                Guid.NewGuid();

            var now =
                DateTimeOffset.UtcNow;

            return new User
            {
                UserId =
                    userId,

                MobileNumber =
                    request.MobileNumber,

                Email =
                    request.Email,

                Password =
                    _passwordService.HashPassword(
                        request.Password),

                Status =
                    "Active",

                CreatedAt =
                    now,

                UpdatedAt =
                    now,

                CreatedBy =
                    userId,

                UpdatedBy =
                    userId
            };
        }

        // =========================================================
        // CREATE USER ROLE
        // =========================================================

        private UserRole RegisterUserRole(
            User user,
            Role role)
        {
            var now =
                DateTimeOffset.UtcNow;

            return new UserRole
            {
                UserId =
                    user.UserId,

                RoleId =
                    role.RoleId,

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

        // =========================================================
        // CREATE INDIVIDUAL LAWYER
        // =========================================================

        private Lawyer RegisterIndividualLawyer(
            RegisterRequest request,
            User user)
        {
            var now =
                DateTimeOffset.UtcNow;

            return new Lawyer
            {
                LawyerId =
                    Guid.NewGuid(),

                UserId =
                    user.UserId,

                LawFirmId =
                    null,

                FullName =
                    request.FullName,

                BarCouncilId =
                    request.BarCouncilId,

                BarCouncilName =
                    request.BarCouncilName,

                EnrollmentDate =
                    request.EnrollmentDate,

                Status =
                    "Active",

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

        // =========================================================
        // CREATE ORGANIZATIONAL LAWYER
        // =========================================================

        private Lawyer RegisterOrganizationalLawyer(
            RegisterRequest request,
            User user,
            LawFirm lawFirm)
        {
            var now =
                DateTimeOffset.UtcNow;

            return new Lawyer
            {
                LawyerId =
                    Guid.NewGuid(),

                UserId =
                    user.UserId,

                LawFirmId =
                    lawFirm.LawFirmId,

                FullName =
                    request.FullName,

                BarCouncilId =
                    request.BarCouncilId,

                BarCouncilName =
                    request.BarCouncilName,

                EnrollmentDate =
                    request.EnrollmentDate,

                Status =
                    "Active",

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

        // =========================================================
        // CREATE LAW FIRM
        // =========================================================

        private LawFirm RegisterLawFirm(
            RegisterRequest request,
            User user)
        {
            var now =
                DateTimeOffset.UtcNow;

            var lawFirmRequest =
                request.LawFirm!;

            return new LawFirm
            {
                LawFirmId =
                    Guid.NewGuid(),

                FirmName =
                    lawFirmRequest.FirmName,

                RegistrationNumber =
                    lawFirmRequest.RegistrationNumber!,

                AddressLine1 =
                    lawFirmRequest.AddressLine1,

                AddressLine2 =
                    lawFirmRequest.AddressLine2,

                City =
                    lawFirmRequest.City,

                District =
                    lawFirmRequest.District,

                State =
                    lawFirmRequest.State,

                Pincode =
                    lawFirmRequest.Pincode,

                Status =
                    "Active",

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

        // =========================================================
        // CREATE USER LAW FIRM
        // =========================================================

        private UserLawFirm RegisterUserLawFirm(
            LawFirm lawFirm,
            User user)
        {
            var now =
                DateTimeOffset.UtcNow;

            return new UserLawFirm
            {
                UserId =
                    user.UserId,

                LawFirmId =
                    lawFirm.LawFirmId,

                JoinedAt =
                    now,

                Status =
                    "Active",

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
