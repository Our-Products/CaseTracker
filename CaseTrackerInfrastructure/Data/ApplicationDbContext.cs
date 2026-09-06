using CaseTrackerDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
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
        // ============================================================
        // USER
        // ============================================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.MobileNumber)
                .HasColumnName("mobile_number")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Password)
                .HasColumnName("password")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            // Nullable UUID audit fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // Indexes

            entity.HasIndex(e => e.MobileNumber)
                .IsUnique()
                .HasDatabaseName("ux_users_mobile_number");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ux_users_email");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_users_status");


            // CreatedBy → User

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → User

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // ROLE
        // ============================================================

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.RoleName)
                .HasColumnName("role_name")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // Index

            entity.HasIndex(e => e.RoleName)
                .IsUnique()
                .HasDatabaseName("ux_roles_role_name");


            // CreatedBy → User

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → User

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // Seed Roles

            entity.HasData(
                new Role
                {
                    RoleId = "R001",
                    RoleName = "Lawyer",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                },
                new Role
                {
                    RoleId = "R002",
                    RoleName = "Staff",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                },
                new Role
                {
                    RoleId = "R003",
                    RoleName = "Admin",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero)
                }
            );
        });


        // ============================================================
        // USER ROLE
        // ============================================================

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_role");

            // Composite Primary Key
            entity.HasKey(e => new
            {
                e.UserId,
                e.RoleId
            });


            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // UserId → Users.UserId

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // RoleId → Roles.RoleId

            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);


            // CreatedBy → Users.UserId

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → Users.UserId

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // LAW FIRM
        // ============================================================

        modelBuilder.Entity<LawFirm>(entity =>
        {
            entity.ToTable("law_firms");

            entity.HasKey(e => e.LawFirmId);

            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id");

            entity.Property(e => e.FirmName)
                .HasColumnName("firm_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.RegistrationNumber)
                .HasColumnName("registration_number")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.AddressLine1)
                .HasColumnName("address_line1")
                .HasMaxLength(255);

            entity.Property(e => e.AddressLine2)
                .HasColumnName("address_line2")
                .HasMaxLength(255);

            entity.Property(e => e.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            entity.Property(e => e.District)
                .HasColumnName("district")
                .HasMaxLength(100);

            entity.Property(e => e.State)
                .HasColumnName("state")
                .HasMaxLength(100);

            entity.Property(e => e.Pincode)
                .HasColumnName("pincode");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // Index

            entity.HasIndex(e => e.RegistrationNumber)
                .IsUnique()
                .HasDatabaseName("ux_law_firms_registration_number");


            // CreatedBy → User

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → User

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // LAWYER
        // ============================================================

        modelBuilder.Entity<Lawyer>(entity =>
        {
            entity.ToTable("lawyers");

            entity.HasKey(e => e.LawyerId);

            entity.Property(e => e.LawyerId)
                .HasColumnName("lawyer_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id");

            entity.Property(e => e.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.BarCouncilId)
                .HasColumnName("bar_council_id")
                .HasMaxLength(100);

            entity.Property(e => e.BarCouncilName)
                .HasColumnName("bar_council_name")
                .HasMaxLength(200);

            entity.Property(e => e.EnrollmentDate)
                .HasColumnName("enrollment_date");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            // Nullable audit fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // Indexes

            entity.HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName("ix_lawyers_user_id");

            entity.HasIndex(e => e.BarCouncilId)
                .HasDatabaseName("ix_lawyers_bar_council_id");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_lawyers_status");


            // Lawyer → User

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Lawyer → LawFirm

            entity.HasOne(e => e.LawFirm)
                .WithMany()
                .HasForeignKey(e => e.LawFirmId)
                .OnDelete(DeleteBehavior.SetNull);


            // CreatedBy → User

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → User

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // USER LAW FIRM
        // ============================================================

        modelBuilder.Entity<UserLawFirm>(entity =>
        {
            entity.ToTable("user_law_firms");

            // Composite Primary Key
            entity.HasKey(e => new
            {
                e.UserId,
                e.LawFirmId
            });


            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id")
                .IsRequired();

            entity.Property(e => e.JoinedAt)
                .HasColumnName("joined_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by");


            // UserId → Users.UserId

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // LawFirmId → LawFirms.LawFirmId

            entity.HasOne(e => e.LawFirm)
                .WithMany()
                .HasForeignKey(e => e.LawFirmId)
                .OnDelete(DeleteBehavior.Cascade);


            // CreatedBy → Users.UserId

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            // UpdatedBy → Users.UserId

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });


        base.OnModelCreating(modelBuilder);
    }
}