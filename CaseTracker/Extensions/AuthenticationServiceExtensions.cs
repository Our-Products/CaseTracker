using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                        {
                            // Ensure both "role" and ClaimTypes.Role are populated
                            var roleClaims = context.Principal.FindAll("role").ToList();
                            foreach (var roleClaim in roleClaims)
                            {
                                if (!identity.HasClaim(ClaimTypes.Role, roleClaim.Value))
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                                }
                            }

                            // Ensure "sub" is mapped to NameIdentifier
                            var subClaim = context.Principal.FindFirst("sub");
                            if (subClaim != null && !identity.HasClaim(ClaimTypes.NameIdentifier, subClaim.Value))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subClaim.Value));
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
