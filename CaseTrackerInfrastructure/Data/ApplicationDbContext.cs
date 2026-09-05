using CaseTrackerDomain.Models;
using System;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        public DbSet<Lawyer> Lawyers { get; set; } = null!;
        public DbSet<LawFirm> LawFirms { get; set; } = null!;
        public DbSet<UserLawFirm> UserLawFirms { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.MobileNumber)
                    .HasColumnName("mobile_number");

                entity.Property(e => e.Email)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasColumnName("password");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by");

                entity.HasIndex(e => e.MobileNumber)
                    .IsUnique()
                    .HasDatabaseName("ux_users_mobile_number");

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("ux_users_email");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("ix_users_status");
            });

            modelBuilder.Entity<LawFirm>(entity =>
            {
                entity.ToTable("law_firms");

                entity.HasKey(e => e.LawFirmId);

                entity.Property(e => e.LawFirmId)
                    .HasColumnName("law_firm_id");

                entity.Property(e => e.FirmName)
                    .HasColumnName("firm_name")
                    .HasColumnType("character varying(255)")
                    .IsRequired();

                entity.Property(e => e.RegistrationNumber)
                    .HasColumnName("registration_number")
                    .HasColumnType("character varying(255)")
                    .IsRequired();

                entity.Property(e => e.AddressLine1)
                    .HasColumnName("address_line1")
                    .HasColumnType("json");

                entity.Property(e => e.AddressLine2)
                    .HasColumnName("address_line2")
                    .HasColumnType("json");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.Pincode)
                    .HasColumnName("pincode");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasIndex(e => e.FirmName)
                    .HasDatabaseName("ix_law_firms_firm_name");

                entity.HasIndex(e => e.RegistrationNumber)
                    .IsUnique()
                    .HasDatabaseName("ux_law_firms_registration_number");
            });

            modelBuilder.Entity<Lawyer>(entity =>
            {
                entity.ToTable("lawyers");

                entity.HasKey(e => e.LawyerId);

                entity.Property(e => e.LawyerId)
                    .HasColumnName("lawyer_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.LawFirmId)
                    .HasColumnName("law_firm_id");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name");

                entity.Property(e => e.BarCouncilId)
                    .HasColumnName("bar_council_id");

                entity.Property(e => e.BarCouncilName)
                    .HasColumnName("bar_council_name");

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnName("enrollment_date");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("ix_lawyers_user_id");

                // Additional indexes for queries
                entity.HasIndex(e => e.BarCouncilId)
                    .HasDatabaseName("ix_lawyers_bar_council_id");
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("ix_lawyers_status");

                entity.HasOne(l => l.User)
                    .WithMany()
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.LawFirm)
                    .WithMany()
                    .HasForeignKey(l => l.LawFirmId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<UserLawFirm>(entity =>
            {
                entity.ToTable("user_law_firms");

                entity.HasKey(e => new { e.UserId, e.LawFirmId });

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.LawFirmId)
                    .HasColumnName("law_firm_id");

                entity.Property(e => e.JoinedAt)
                    .HasColumnName("joined_at");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasOne(ulf => ulf.User)
                    .WithMany()
                    .HasForeignKey(ulf => ulf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ulf => ulf.LawFirm)
                    .WithMany()
                    .HasForeignKey(ulf => ulf.LawFirmId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.RoleName)
                    .HasColumnName("role_name")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamptz");

                entity.HasIndex(e => e.RoleName)
                    .IsUnique()
                    .HasDatabaseName("ix_roles_role_name");

                // Seed default roles required during migration
                entity.HasData(
                    new Role
                    {
                        RoleId = "R001",
                        RoleName = "Lawyer",
                        Description = "Legal professional who manages cases and clients",
                        Status = "Active",
                        CreatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                        UpdatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                    },
                    new Role
                    {
                        RoleId = "R002",
                        RoleName = "Staff",
                        Description = "Staff member who assists lawyers",
                        Status = "Active",
                        CreatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                        UpdatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                    },
                    new Role
                    {
                        RoleId = "R003",
                        RoleName = "Admin",
                        Description = "Law-firm administrator",
                        Status = "Active",
                        CreatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                        UpdatedAt = new DateTimeOffset(2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                    }
                );
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role");

                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasOne(ur => ur.User)
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
