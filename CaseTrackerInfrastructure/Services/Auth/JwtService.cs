using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaseTrackerInfrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int GetExpiryMinutes()
        {
            if (int.TryParse(
                _configuration["Jwt:ExpiryMinutes"],
                out var minutes))
            {
                return minutes;
            }

            return 60;
        }

        public string GenerateToken(
            User user,
            IEnumerable<string> roles)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key not configured.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString())
            };

            foreach (var role in roles.Where(
                r => !string.IsNullOrEmpty(r)))
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, role));

                claims.Add(
                    new Claim("role", role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    GetExpiryMinutes()),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}