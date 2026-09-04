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
            // ensure mobile uniqueness
            if (await _userRepository.MobileNumberExistsAsync(request.MobileNumber))
                throw new InvalidOperationException("Mobile number already registered");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                MobileNumber = request.MobileNumber,
                Email = request.Email ?? string.Empty,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "active",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            // Persist user and optional related entities in a single transaction
            await using var tx = await _db.Database.BeginTransactionAsync();

            _db.Users.Add(user);

            Guid? lawFirmId = null;

            if (request.RegisterType == CaseTrackerApplication.DTOs.RegisterType.Organization && request.LawFirm != null)
            {
                // Try to find existing law firm by registration number (preferred) or by name
                LawFirm? existing = null;
                if (!string.IsNullOrWhiteSpace(request.LawFirm.RegistrationNumber))
                {
                    existing = await _db.LawFirms
                        .FirstOrDefaultAsync(l => l.RegistrationNumber == request.LawFirm.RegistrationNumber);
                }

                if (existing == null && !string.IsNullOrWhiteSpace(request.LawFirm.FirmName))
                {
                    existing = await _db.LawFirms
                        .FirstOrDefaultAsync(l => l.FirmName.ToLower() == request.LawFirm.FirmName.ToLower());
                }

                LawFirm lf;
                if (existing != null)
                {
                    lf = existing;
                }
                else
                {
                    lf = new LawFirm
                    {
                        LawFirmId = Guid.NewGuid(),
                        FirmName = request.LawFirm.FirmName,
                        RegistrationNumber = request.LawFirm.RegistrationNumber,
                        AddressLine1 = request.LawFirm.AddressJson ?? string.Empty,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    _db.LawFirms.Add(lf);
                }

                await _db.SaveChangesAsync(); // ensure lf.LawFirmId is generated if new
                lawFirmId = lf.LawFirmId;

                var membership = new UserLawFirm
                {
                    UserId = user.UserId,
                    LawFirmId = lf.LawFirmId,
                    JoinedAt = DateTimeOffset.UtcNow,
                    Status = "Active",
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                _db.UserLawFirms.Add(membership);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var token = CreateToken(user);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()), UserId = user.UserId, LawFirmId = lawFirmId };
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (user == null) throw new InvalidOperationException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new InvalidOperationException("Invalid credentials");

            var token = CreateToken(user);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()) };
        }

        private int GetExpiryMinutes()
        {
            if (int.TryParse(_config["Jwt:ExpiryMinutes"], out var m)) return m;
            return 60;
        }

        private string CreateToken(User user)
        {
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.MobileNumber),
            };

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
