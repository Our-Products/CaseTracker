using CaseTrackerApplication.DTOs;
using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaseTrackerInfrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, ApplicationDbContext db, IConfiguration config)
        {
            _userRepository = userRepository;
            _db = db;
            _config = config;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            // 1. Validate request
            ValidateRegisterRequest(request);

            // 2. Check mobile
            var existingUser = await _userRepository
                .GetByMobileNumberAsync(request.MobileNumber);

            if (existingUser != null)
                throw new InvalidOperationException(
                    "An account with this mobile number already exists.");

            // 3. Check email
            var existingEmail = await _db.Users
                .AnyAsync(u => u.Email == request.Email);

            if (existingEmail)
                throw new InvalidOperationException(
                    "An account with this email already exists.");

            // 4. Start transaction
            await using var transaction =
                await _db.Database.BeginTransactionAsync();

            try
            {
                // 5. Create User
                var user = RegisterUser(request);

                await _db.Users.AddAsync(user);

                // 6. Determine Role
                var roleName = request.RegisterType switch
                {
                    RegisterType.Individual => "Lawyer",
                    RegisterType.Organization => "Admin",
                    _ => throw new InvalidOperationException(
                        "Invalid register type.")
                };

                // 7. Create UserRole
                var userRole = await RegisterUserRoleAsync(user, roleName);

                await _db.UserRoles.AddAsync(userRole);

                // 8. Individual registration
                if (request.RegisterType == RegisterType.Individual)
                {
                    var lawyer = RegisterLawyer(request, user);

                    await _db.Lawyers.AddAsync(lawyer);
                }

                // 9. Organization registration
                if (request.RegisterType == RegisterType.Organization)
                {
                    var lawFirm = RegisterLawFirm(request, user);

                    var userLawFirm =
                        RegisterUserLawFirm(lawFirm, user);

                    await _db.LawFirms.AddAsync(lawFirm);
                    await _db.UserLawFirms.AddAsync(userLawFirm);
                }

                // 10. Save EVERYTHING
                await _db.SaveChangesAsync();

                // 11. Commit
                await transaction.CommitAsync();

                // 12. Generate JWT
                var token = await CreateTokenAsync(user);

                return new AuthResult
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        GetExpiryMinutes())
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private void ValidateRegisterRequest(RegisterRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                throw new InvalidOperationException("Mobile number is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("Password is required.");

            if (request.RegisterType == RegisterType.Organization)
            {
                if (request.LawFirm == null)
                    throw new InvalidOperationException(
                        "Law firm details are required for organization registration.");

                if (string.IsNullOrWhiteSpace(request.LawFirm.FirmName))
                    throw new InvalidOperationException("Firm name is required.");

                if (string.IsNullOrWhiteSpace(request.LawFirm.RegistrationNumber))
                    throw new InvalidOperationException(
                        "Registration number is required.");
            }
        }
        private LawFirm RegisterLawFirm(RegisterRequest request, User user)
        {
            var lawfirm = new LawFirm()
            {
                LawFirmId = new Guid(),
                FirmName = request.LawFirm.FirmName,
                RegistrationNumber = request.LawFirm.RegistrationNumber,
                AddressLine1 = request.LawFirm.AddressLine1,
                AddressLine2 = request.LawFirm.AddressLine2,
                City = request.LawFirm.City,
                District = request.LawFirm.District,
                State = request.LawFirm.State,
                Pincode = request.LawFirm.Pincode,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = user.UserId,
                UpdatedBy = user.UserId
            };

            return lawfirm;
        }
        private UserLawFirm RegisterUserLawFirm(LawFirm lawfirm, User user)
        {
            var userLawFirm = new UserLawFirm()
            {
                UserId = user.UserId,
                LawFirmId = lawfirm.LawFirmId,
                JoinedAt = DateTimeOffset.UtcNow,
                Status = "Active",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
              
            };
            return userLawFirm;
        }
        private async Task<UserRole> RegisterUserRoleAsync(User user, string roleName)
        {
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);

            if (role == null)
                throw new InvalidOperationException(
                    $"Role '{roleName}' not found.");

            return new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = user.UserId,
                UpdatedBy = user.UserId
            };
        }
        public Lawyer RegisterLawyer(RegisterRequest request, User user)
        {
            var lawyers = new Lawyer
            {
                LawyerId = Guid.NewGuid(),
                UserId = user.UserId,
                FullName = request.FullName,
                BarCouncilId = request.BarCouncilId,
                BarCouncilName = request.BarCouncilName,
                EnrollmentDate = request.EnrollmentDate,
                Status = "Active",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            return lawyers;
        }
        public User RegisterUser(RegisterRequest request)
        {
            Guid userid = Guid.NewGuid();
            var newUser = new User
            {
                UserId = userid,
                MobileNumber = request.MobileNumber,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "Active",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = userid,
                UpdatedBy = userid
            };

            return newUser;
        }
        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (user == null) throw new InvalidOperationException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new InvalidOperationException("Invalid credentials");

            var token = await CreateTokenAsync(user);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()) };
        }
        private int GetExpiryMinutes()
        {
            if (int.TryParse(_config["Jwt:ExpiryMinutes"], out var m)) return m;
            return 60;
        }
        private async Task<string> CreateTokenAsync(User user)
        {
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            // minimal claims: stable subject identifier only (UI/profile info should come from /me endpoint)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            };

            // load roles for the user and add role claims
            var roleNames = await _db.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.UserId)
                .Select(ur => ur.Role!.RoleName)
                .ToListAsync();

            foreach (var role in roleNames.Where(r => !string.IsNullOrEmpty(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
