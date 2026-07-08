namespace CaseTracker.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        /// <summary>
        /// Registers authorization policies and services.
        /// </summary>
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Add default policy requiring authenticated user
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Example: Add custom policies if needed in the future
                // options.AddPolicy("AdminOnly", policy =>
                //     policy.RequireClaim("role", "admin"));
                // 
                // options.AddPolicy("LawyerAccess", policy =>
                //     policy.RequireAuthenticatedUser()
                //     .RequireClaim("type", "lawyer"));
            });

            return services;
        }
    }
}
