using System;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaseTrackerInfrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var logger = sp.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                var context = sp.GetRequiredService<ApplicationDbContext>();
                
                // Ensure database schema is created (virtual db / dev database)
                await context.Database.EnsureCreatedAsync();

                // Check if default roles are present
                if (!await context.Roles.AnyAsync())
                {
                    context.Roles.AddRange(
                        new Role
                        {
                            RoleId = "R001",
                            RoleName = "Lawyer",
                            Description = "Legal professional who manages cases and clients",
                            Status = "Active",
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow
                        },
                        new Role
                        {
                            RoleId = "R002",
                            RoleName = "Staff",
                            Description = "Staff member who assists lawyers",
                            Status = "Active",
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow
                        },
                        new Role
                        {
                            RoleId = "R003",
                            RoleName = "Admin",
                            Description = "Law-firm administrator",
                            Status = "Active",
                            CreatedAt = DateTimeOffset.UtcNow,
                            UpdatedAt = DateTimeOffset.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded default roles (R001, R002, R003) into virtual database.");
                }

                // Check if test dummy advocate is seeded
                if (!await context.Users.AnyAsync(u => u.MobileNumber == "9876543210"))
                {
                    var dummyUser = new User
                    {
                        UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        MobileNumber = "9876543210",
                        Email = "advocate@test.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        Status = "active",
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };
                    context.Users.Add(dummyUser);

                    var userRole = new UserRole
                    {
                        UserId = dummyUser.UserId,
                        RoleId = "R001",
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "System",
                        UpdatedBy = "System"
                    };
                    context.UserRoles.Add(userRole);

                    var lawyerProfile = new Lawyer
                    {
                        LawyerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        UserId = dummyUser.UserId,
                        FullName = "Adv. R. Sundaram",
                        BarCouncilId = "TN/1042/2018",
                        BarCouncilName = "Bar Council of Tamil Nadu and Puducherry",
                        EnrollmentDate = new DateTime(2018, 5, 15),
                        Status = "Active",
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };
                    context.Lawyers.Add(lawyerProfile);

                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded dummy advocate account (9876543210 / Password123!) into virtual database.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
            }
        }
    }
}
