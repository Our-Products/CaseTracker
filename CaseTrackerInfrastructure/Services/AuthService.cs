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
            // =========================================================
            // 1. Validate User Uniqueness
            // =========================================================

            if (await _userRepository.MobileNumberExistsAsync(request.MobileNumber))
                throw new InvalidOperationException(
                    "Mobile number already registered");


            // =========================================================
            // 2. Create User
            // =========================================================

            var now = DateTimeOffset.UtcNow;

            var user = new User
            {
                UserId = Guid.NewGuid(),

                MobileNumber = request.MobileNumber.Trim(),

                Email = request.Email?.Trim() ?? string.Empty,

                Password = BCrypt.Net.BCrypt.HashPassword(
                    request.Password),

                Status = "active",

                CreatedAt = now,
                UpdatedAt = now
            };


            // =========================================================
            // 3. Start Transaction
            // =========================================================

            await using var tx =
                await _db.Database.BeginTransactionAsync();

            try
            {
                _db.Users.Add(user);

                Guid? lawFirmId = null;


                // =====================================================
                // 4. Organization Registration
                // =====================================================

                if (request.RegisterType ==
                        CaseTrackerApplication.DTOs.RegisterType.Organization)
                {
                    if (request.LawFirm == null)
                    {
                        throw new InvalidOperationException(
                            "Law firm information is required.");
                    }


                    // -------------------------------------------------
                    // Find existing law firm
                    // -------------------------------------------------

                    LawFirm? lawFirm = null;

                    if (!string.IsNullOrWhiteSpace(
                            request.LawFirm.RegistrationNumber))
                    {
                        lawFirm = await _db.LawFirms
                            .FirstOrDefaultAsync(x =>
                                x.RegistrationNumber ==
                                request.LawFirm.RegistrationNumber);
                    }


                    // -------------------------------------------------
                    // If not found, search by firm name
                    // -------------------------------------------------

                    if (lawFirm == null &&
                        !string.IsNullOrWhiteSpace(
                            request.LawFirm.FirmName))
                    {
                        var firmName =
                            request.LawFirm.FirmName.Trim();

                        lawFirm = await _db.LawFirms
                            .FirstOrDefaultAsync(x =>
                                x.FirmName.ToLower() ==
                                firmName.ToLower());
                    }


                    // -------------------------------------------------
                    // Create new Law Firm if required
                    // -------------------------------------------------

                    if (lawFirm == null)
                    {
                        lawFirm = new LawFirm
                        {
                            LawFirmId = Guid.NewGuid(),

                            FirmName =
                                request.LawFirm.FirmName.Trim(),

                            RegistrationNumber = string.IsNullOrWhiteSpace(request.LawFirm.RegistrationNumber)
                                ? string.Empty
                                : request.LawFirm.RegistrationNumber.Trim(),

                            // Map optional address JSON if provided
                            AddressLine1 = string.IsNullOrWhiteSpace(request.LawFirm.AddressJson)
                                ? null
                                : request.LawFirm.AddressJson.Trim(),

                            CreatedAt = now,
                            UpdatedAt = now
                        };

                        _db.LawFirms.Add(lawFirm);

                        // ID is already generated using Guid.NewGuid()
                        lawFirmId = lawFirm.LawFirmId;
                    }
                    else
                    {
                        lawFirmId = lawFirm.LawFirmId;
                    }


                    // =================================================
                    // 5. User -> Law Firm Membership
                    // =================================================

                    var membership = new UserLawFirm
                    {
                        UserId = user.UserId,

                        LawFirmId = lawFirm.LawFirmId,

                        JoinedAt = now,

                        Status = "Active",

                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    _db.UserLawFirms.Add(membership);
                }


                // =====================================================
                // 6. Create Lawyer Profile
                // =====================================================

                var lawyer = new Lawyer
                {
                    LawyerId = Guid.NewGuid(),

                    UserId = user.UserId,

                    // Individual -> null
                    // Organization -> law firm ID
                    LawFirmId = lawFirmId,

                    // IMPORTANT:
                    // Always the PERSON's name.
                    FullName = request.FullName.Trim(),

                    Status = "Active",

                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.Lawyers.Add(lawyer);


                // =====================================================
                // 7. Assign Lawyer Role
                // =====================================================

                var lawyerRole = await _db.Roles
                    .FirstOrDefaultAsync(x =>
                        x.RoleId == "R001");

                if (lawyerRole == null)
                {
                    throw new InvalidOperationException(
                        "Lawyer role (R001) was not found.");
                }


                var userRole = new UserRole
                {
                    UserId = user.UserId,

                    RoleId = lawyerRole.RoleId,

                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.UserRoles.Add(userRole);

                // Ensure audit fields are set for UserRole
                userRole.CreatedBy = user.UserId.ToString();
                userRole.UpdatedBy = user.UserId.ToString();


                // =====================================================
                // 8. Save Everything
                // =====================================================

                await _db.SaveChangesAsync();


                // =====================================================
                // 9. Commit Transaction
                // =====================================================

                await tx.CommitAsync();


                // =====================================================
                // 10. Create JWT
                // =====================================================

                var token = await CreateTokenAsync(user);


                return new AuthResult
                {
                    Token = token,

                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(
                            GetExpiryMinutes()),

                    UserId = user.UserId,

                    LawFirmId = lawFirmId
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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
