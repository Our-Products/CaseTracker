using CaseTrackerApplication.DTOs;
using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaseTrackerInfrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
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
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = CreateToken(user);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()) };
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
