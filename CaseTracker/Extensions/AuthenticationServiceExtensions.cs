using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CaseTracker.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// Registers JWT Bearer authentication with token validation parameters.
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Get JWT configuration from appsettings
            var jwtKey = configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "CaseTracker";
            var jwtAudience = configuration["Jwt:Audience"] ?? "CaseTrackerAudience";

            // Convert key to bytes
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            // Configure authentication scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Configure token validation
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for token expiration
                };
            });

            return services;
        }
    }
}
