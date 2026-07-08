using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILawyerRepository _lawyerRepository;
        private readonly IConfiguration _config;

        public AuthService(ILawyerRepository lawyerRepository, IConfiguration config)
        {
            _lawyerRepository = lawyerRepository;
            _config = config;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            // ensure mobile uniqueness
            if (await _lawyerRepository.MobileNumberExistsAsync(request.MobileNumber))
                throw new InvalidOperationException("Mobile number already registered");

            var lawyer = new Lawyer
            {
                LawyerId = Guid.NewGuid(),
                FullName = request.FullName,
                MobileNumber = request.MobileNumber,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _lawyerRepository.AddAsync(lawyer);
            await _lawyerRepository.SaveChangesAsync();

            var token = CreateToken(lawyer);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()) };
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var lawyer = await _lawyerRepository.GetByMobileNumberAsync(request.MobileNumber);
            if (lawyer == null) throw new InvalidOperationException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, lawyer.PasswordHash))
                throw new InvalidOperationException("Invalid credentials");

            var token = CreateToken(lawyer);
            return new AuthResult { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()) };
        }

        private int GetExpiryMinutes()
        {
            if (int.TryParse(_config["Jwt:ExpiryMinutes"], out var m)) return m; 
            return 60;
        }

        private string CreateToken(Lawyer lawyer)
        {
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, lawyer.LawyerId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, lawyer.MobileNumber),
                new Claim("full_name", lawyer.FullName ?? string.Empty)
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
