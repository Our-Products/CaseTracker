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

    // ============================================================
    // DB SETS
    // ============================================================

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<Lawyer> Lawyers { get; set; } = null!;
    public DbSet<LawFirm> LawFirms { get; set; } = null!;
    public DbSet<UserLawFirm> UserLawFirms { get; set; } = null!;

    public DbSet<State> States { get; set; } = null!;
    public DbSet<District> Districts { get; set; } = null!;
    public DbSet<CourtComplex> CourtComplexes { get; set; } = null!;
    public DbSet<CourtType> CourtTypes { get; set; } = null!;
    public DbSet<Court> Courts { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Case> Cases { get; set; } = null!;
    public DbSet<CaseClient> CaseClients { get; set; } = null!;
    public DbSet<CaseLawyer> CaseLawyers { get; set; } = null!;
    public DbSet<CaseLawyerRole> CaseLawyerRoles { get; set; } = null!;
    public DbSet<CaseHearing> CaseHearings { get; set; } = null!;
    public DbSet<CaseOrder> CaseOrders { get; set; } = null!;
    public DbSet<ECourtSyncLog> ECourtSyncLogs { get; set; } = null!;
    public DbSet<CaseDocument> CaseDocuments { get; set; } = null!;
    public DbSet<ECourtApiLog> ECourtApiLogs { get; set; } = null!;
    public DbSet<CourtMasterSyncHistory> CourtMasterSyncHistories { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ============================================================
        // USER
        // ============================================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            // Primary Key
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");


            // Mobile Number
            entity.Property(e => e.MobileNumber)
                .HasColumnName("mobile_number")
                .HasMaxLength(20)
                .IsRequired();


            // Email
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();


            // Password
            entity.Property(e => e.Password)
                .HasColumnName("password")
                .HasColumnType("text")
                .IsRequired();


            // Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");


            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // INDEXES
            // ========================================================

            entity.HasIndex(e => e.MobileNumber)
                .IsUnique()
                .HasDatabaseName("ux_users_mobile_number");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ux_users_email");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_users_status");


            // ========================================================
            // AUDIT RELATIONSHIPS
            // ========================================================

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
        // ROLE
        // ============================================================

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            // Primary Key
            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasMaxLength(20)
                .IsRequired();


            // Role Name
            entity.Property(e => e.RoleName)
                .HasColumnName("role_name")
                .HasMaxLength(50)
                .IsRequired();


            // Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");


            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // INDEXES
            // ========================================================

            entity.HasIndex(e => e.RoleName)
                .IsUnique()
                .HasDatabaseName("ux_roles_role_name");


            // ========================================================
            // AUDIT RELATIONSHIPS
            // ========================================================

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


            // ========================================================
            // SEED DATA
            // ========================================================

            entity.HasData(
                new Role
                {
                    RoleId = "R000",
                    RoleName = "SuperAdmin",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = null,
                    UpdatedBy = null
                },

                new Role
                {
                    RoleId = "R001",
                    RoleName = "Lawyer",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = null,
                    UpdatedBy = null
                },

                new Role
                {
                    RoleId = "R002",
                    RoleName = "Staff",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = null,
                    UpdatedBy = null
                },

                new Role
                {
                    RoleId = "R003",
                    RoleName = "Admin",
                    Status = "Active",
                    CreatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(
                        2026, 9, 4, 0, 0, 0, TimeSpan.Zero),
                    CreatedBy = null,
                    UpdatedBy = null
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


            // User ID
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();


            // Role ID
            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasMaxLength(20)
                .IsRequired();


            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // RELATIONSHIPS
            // ========================================================

            // UserId → Users.UserId
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // RoleId → Roles.RoleId
            //
            // Restrict prevents accidental deletion of a role
            // that is already assigned to users.
            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


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

            // Primary Key
            entity.HasKey(e => e.LawFirmId);


            // Law Firm ID
            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id");


            // Firm Name
            entity.Property(e => e.FirmName)
                .HasColumnName("firm_name")
                .HasMaxLength(255)
                .IsRequired();


            // Registration Number
            entity.Property(e => e.RegistrationNumber)
                .HasColumnName("registration_number")
                .HasMaxLength(255)
                .IsRequired();


            // Address
            entity.Property(e => e.AddressLine1)
                .HasColumnName("address_line1")
                .HasMaxLength(255);

            entity.Property(e => e.AddressLine2)
                .HasColumnName("address_line2")
                .HasMaxLength(255);


            // Location
            entity.Property(e => e.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            entity.Property(e => e.District)
                .HasColumnName("district")
                .HasMaxLength(100);

            entity.Property(e => e.State)
                .HasColumnName("state")
                .HasMaxLength(100);


            // Pincode
            entity.Property(e => e.Pincode)
                .HasColumnName("pincode");


            entity.Property(e => e.Status)
               .HasColumnName("status")
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue("Active");

            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // INDEXES
            // ========================================================

            entity.HasIndex(e => e.RegistrationNumber)
                .IsUnique()
                .HasDatabaseName(
                    "ux_law_firms_registration_number");


            // ========================================================
            // AUDIT RELATIONSHIPS
            // ========================================================

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
        // LAWYER
        // ============================================================

        modelBuilder.Entity<Lawyer>(entity =>
        {
            entity.ToTable("lawyers");

            // Primary Key
            entity.HasKey(e => e.LawyerId);


            // Lawyer ID
            entity.Property(e => e.LawyerId)
                .HasColumnName("lawyer_id");


            // User ID
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();


            // Law Firm ID
            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id")
                .IsRequired(false);


            // Full Name
            entity.Property(e => e.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(200)
                .IsRequired();


            // Bar Council ID
            entity.Property(e => e.BarCouncilId)
                .HasColumnName("bar_council_id")
                .HasMaxLength(100);


            // Bar Council Name
            entity.Property(e => e.BarCouncilName)
                .HasColumnName("bar_council_name")
                .HasMaxLength(200);


            // Enrollment Date
            entity.Property(e => e.EnrollmentDate)
                .HasColumnName("enrollment_date");


            // Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");


            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // INDEXES
            // ========================================================

            // One lawyer profile per user
            entity.HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName("ix_lawyers_user_id");


            entity.HasIndex(e => e.BarCouncilId)
                .HasDatabaseName("ix_lawyers_bar_council_id");


            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_lawyers_status");


            // ========================================================
            // RELATIONSHIPS
            // ========================================================

            // Lawyer → User
            //
            // User deletion removes the dependent lawyer profile.
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Lawyer → LawFirm
            //
            // Deleting a law firm does not delete the lawyer.
            // The lawyer simply becomes unassigned.
            entity.HasOne(e => e.LawFirm)
                .WithMany(f => f.Lawyers)
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


            // User ID
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();


            // Law Firm ID
            entity.Property(e => e.LawFirmId)
                .HasColumnName("law_firm_id")
                .IsRequired();


            // Joined At
            entity.Property(e => e.JoinedAt)
                .HasColumnName("joined_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");


            // Created At
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Updated At
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();


            // Audit Fields
            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired(false);

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .IsRequired(false);


            // ========================================================
            // RELATIONSHIPS
            // ========================================================

            // UserId → Users.UserId
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // LawFirmId → LawFirms.LawFirmId
            entity.HasOne(e => e.LawFirm)
                .WithMany(f => f.UserLawFirms)
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



        // ============================================================
        // STATE
        // ============================================================

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("states");
            entity.HasKey(e => e.StateId);

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.StateName).HasColumnName("state_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.StateCode).HasColumnName("state_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.StateCode).IsUnique().HasDatabaseName("ux_states_state_code");
            entity.HasIndex(e => e.StateName).IsUnique().HasDatabaseName("ux_states_state_name");

            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // DISTRICT
        // ============================================================

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("districts");
            entity.HasKey(e => e.DistrictId);

            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.DistrictName).HasColumnName("district_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.StateId).HasColumnName("state_id").IsRequired();
            entity.Property(e => e.DistrictCode).HasColumnName("district_code").HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasOne(e => e.State).WithMany(s => s.Districts).HasForeignKey(e => e.StateId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // COURT COMPLEX
        // ============================================================

        modelBuilder.Entity<CourtComplex>(entity =>
        {
            entity.ToTable("court_complexes");
            entity.HasKey(e => e.CourtComplexId);

            entity.Property(e => e.CourtComplexId).HasColumnName("court_complex_id");
            entity.Property(e => e.ComplexName).HasColumnName("complex_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.DistrictId).HasColumnName("district_id").IsRequired();
            entity.Property(e => e.ComplexCode).HasColumnName("complex_code").HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasOne(e => e.District).WithMany(d => d.CourtComplexes).HasForeignKey(e => e.DistrictId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // COURT TYPE
        // ============================================================

        modelBuilder.Entity<CourtType>(entity =>
        {
            entity.ToTable("court_types");
            entity.HasKey(e => e.CourtTypeId);

            entity.Property(e => e.CourtTypeId).HasColumnName("court_type_id").HasMaxLength(20);
            entity.Property(e => e.CourtTypeName).HasColumnName("court_type_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // COURT
        // ============================================================

        modelBuilder.Entity<Court>(entity =>
        {
            entity.ToTable("courts");
            entity.HasKey(e => e.CourtId);

            entity.Property(e => e.CourtId).HasColumnName("court_id");
            entity.Property(e => e.CourtName).HasColumnName("court_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.CourtCode).HasColumnName("court_code").HasMaxLength(50);
            entity.Property(e => e.CourtComplexId).HasColumnName("court_complex_id").IsRequired();
            entity.Property(e => e.CourtTypeId).HasColumnName("court_type_id").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasOne(e => e.CourtComplex).WithMany(c => c.Courts).HasForeignKey(e => e.CourtComplexId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CourtType).WithMany(t => t.Courts).HasForeignKey(e => e.CourtTypeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CLIENT
        // ============================================================

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("clients");
            entity.HasKey(e => e.ClientId);

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.LawFirmId).HasColumnName("law_firm_id").IsRequired(false);
            entity.Property(e => e.ClientType).HasColumnName("client_type").HasMaxLength(20).IsRequired().HasDefaultValue("Individual");
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.PrimaryPhone).HasColumnName("primary_phone").HasMaxLength(20).IsRequired();
            entity.Property(e => e.SecondaryPhone).HasColumnName("secondary_phone").HasMaxLength(20);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.AddressLine1).HasColumnName("address_line1").HasMaxLength(255);
            entity.Property(e => e.AddressLine2).HasColumnName("address_line2").HasMaxLength(255);
            entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(100);
            entity.Property(e => e.Pincode).HasColumnName("pincode");
            entity.Property(e => e.ContactPerson).HasColumnName("contact_person").HasMaxLength(200);
            entity.Property(e => e.GstNumber).HasColumnName("gst_number").HasMaxLength(50);
            entity.Property(e => e.PanNumber).HasColumnName("pan_number").HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.FullName).HasDatabaseName("ix_clients_full_name");
            entity.HasIndex(e => e.PrimaryPhone).HasDatabaseName("ix_clients_primary_phone");
            entity.HasIndex(e => e.Email).HasDatabaseName("ix_clients_email");

            entity.HasOne(e => e.LawFirm).WithMany(f => f.Clients).HasForeignKey(e => e.LawFirmId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE
        // ============================================================

        modelBuilder.Entity<Case>(entity =>
        {
            entity.ToTable("cases");
            entity.HasKey(e => e.CaseId);

            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.LawFirmId).HasColumnName("law_firm_id").IsRequired(false);
            entity.Property(e => e.CourtId).HasColumnName("court_id").IsRequired();
            entity.Property(e => e.CnrNumber).HasColumnName("cnr_number").HasMaxLength(16);
            entity.Property(e => e.CaseNumber).HasColumnName("case_number").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CaseType).HasColumnName("case_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.FilingNumber).HasColumnName("filing_number").HasMaxLength(100);
            entity.Property(e => e.FilingDate).HasColumnName("filing_date").HasColumnType("date");
            entity.Property(e => e.RegistrationNumber).HasColumnName("registration_number").HasMaxLength(100);
            entity.Property(e => e.RegistrationDate).HasColumnName("registration_date").HasColumnType("date");
            entity.Property(e => e.CaseTitle).HasColumnName("case_title").HasMaxLength(300).IsRequired();
            entity.Property(e => e.CaseStage).HasColumnName("case_stage").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CaseStatus).HasColumnName("case_status").HasMaxLength(30).IsRequired().HasDefaultValue("Pending");
            entity.Property(e => e.ActsSections).HasColumnName("acts_sections").HasColumnType("text");
            entity.Property(e => e.PoliceStation).HasColumnName("police_station").HasMaxLength(100);
            entity.Property(e => e.FirNumber).HasColumnName("fir_number").HasMaxLength(50);
            entity.Property(e => e.FirYear).HasColumnName("fir_year");
            entity.Property(e => e.IsEcourtSynced).HasColumnName("is_ecourt_synced").IsRequired().HasDefaultValue(false);
            entity.Property(e => e.LastSyncedAt).HasColumnName("last_synced_at").HasColumnType("timestamptz");
            entity.Property(e => e.LastSuccessfulSyncAt).HasColumnName("last_successful_sync_at").HasColumnType("timestamptz");
            entity.Property(e => e.LastSyncAttemptAt).HasColumnName("last_sync_attempt_at").HasColumnType("timestamptz");
            entity.Property(e => e.SyncStatus).HasColumnName("sync_status").HasMaxLength(30);
            entity.Property(e => e.SyncError).HasColumnName("sync_error").HasColumnType("text");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.CnrNumber).IsUnique().HasDatabaseName("ux_cases_cnr_number");
            entity.HasIndex(e => e.CaseNumber).HasDatabaseName("ix_cases_case_number");
            entity.HasIndex(e => e.CaseType).HasDatabaseName("ix_cases_case_type");
            entity.HasIndex(e => e.CaseStatus).HasDatabaseName("ix_cases_case_status");
            entity.HasIndex(e => e.CourtId).HasDatabaseName("ix_cases_court_id");

            entity.HasOne(e => e.LawFirm).WithMany(f => f.Cases).HasForeignKey(e => e.LawFirmId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Court).WithMany(c => c.Cases).HasForeignKey(e => e.CourtId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE CLIENT (Composite Key: CaseId + ClientId)
        // ============================================================

        modelBuilder.Entity<CaseClient>(entity =>
        {
            entity.ToTable("case_clients");
            entity.HasKey(e => new { e.CaseId, e.ClientId });

            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.PartyType).HasColumnName("party_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.PartySequence).HasColumnName("party_sequence").IsRequired().HasDefaultValue(1);
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary").IsRequired().HasDefaultValue(true);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.PartyType).HasDatabaseName("ix_case_clients_party_type");

            entity.HasOne(e => e.Case).WithMany(c => c.CaseClients).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Client).WithMany(c => c.CaseClients).HasForeignKey(e => e.ClientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE LAWYER ROLE
        // ============================================================

        modelBuilder.Entity<CaseLawyerRole>(entity =>
        {
            entity.ToTable("case_lawyer_roles");
            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleId).HasColumnName("role_id").HasMaxLength(20);
            entity.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");

            entity.HasIndex(e => e.RoleName).IsUnique().HasDatabaseName("ux_case_lawyer_roles_name");
        });


        // ============================================================
        // CASE LAWYER (Composite Key: CaseId + LawyerId)
        // ============================================================

        modelBuilder.Entity<CaseLawyer>(entity =>
        {
            entity.ToTable("case_lawyers");
            entity.HasKey(e => new { e.CaseId, e.LawyerId });

            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.LawyerId).HasColumnName("lawyer_id");
            entity.Property(e => e.LawyerRoleId).HasColumnName("lawyer_role_id").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasOne(e => e.Case).WithMany(c => c.CaseLawyers).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Lawyer).WithMany(l => l.CaseLawyers).HasForeignKey(e => e.LawyerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.LawyerRole).WithMany(r => r.CaseLawyers).HasForeignKey(e => e.LawyerRoleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE HEARING
        // ============================================================

        modelBuilder.Entity<CaseHearing>(entity =>
        {
            entity.ToTable("case_hearings");
            entity.HasKey(e => e.HearingId);

            entity.Property(e => e.HearingId).HasColumnName("hearing_id");
            entity.Property(e => e.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(e => e.HearingDate).HasColumnName("hearing_date").HasColumnType("date").IsRequired();
            entity.Property(e => e.ItemNumber).HasColumnName("item_number");
            entity.Property(e => e.CourtHall).HasColumnName("court_hall").HasMaxLength(100);
            entity.Property(e => e.JudgeName).HasColumnName("judge_name").HasMaxLength(200);
            entity.Property(e => e.PurposeOfHearing).HasColumnName("purpose_of_hearing").HasMaxLength(150).IsRequired();
            entity.Property(e => e.BusinessOnDate).HasColumnName("business_on_date").HasColumnType("text");
            entity.Property(e => e.NextHearingDate).HasColumnName("next_hearing_date").HasColumnType("date");
            entity.Property(e => e.NextPurpose).HasColumnName("next_purpose").HasMaxLength(150);
            entity.Property(e => e.HearingStatus).HasColumnName("hearing_status").HasMaxLength(30).IsRequired().HasDefaultValue("Scheduled");
            entity.Property(e => e.DailyOrderSummary).HasColumnName("daily_order_summary").HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.CaseId).HasDatabaseName("ix_case_hearings_case_id");
            entity.HasIndex(e => e.HearingDate).HasDatabaseName("ix_case_hearings_hearing_date");
            entity.HasIndex(e => e.NextHearingDate).HasDatabaseName("ix_case_hearings_next_hearing_date");

            entity.HasOne(e => e.Case).WithMany(c => c.CaseHearings).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE ORDER
        // ============================================================

        modelBuilder.Entity<CaseOrder>(entity =>
        {
            entity.ToTable("case_orders");
            entity.HasKey(e => e.OrderId);

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(e => e.HearingId).HasColumnName("hearing_id").IsRequired(false);
            entity.Property(e => e.OrderDate).HasColumnName("order_date").HasColumnType("date").IsRequired();
            entity.Property(e => e.OrderType).HasColumnName("order_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.OrderUrl).HasColumnName("order_url").HasMaxLength(500);
            entity.Property(e => e.PdfStoragePath).HasColumnName("pdf_storage_path").HasMaxLength(500);
            entity.Property(e => e.IsCertified).HasColumnName("is_certified").IsRequired().HasDefaultValue(true);
            entity.Property(e => e.OrderMarkdownContent).HasColumnName("order_markdown_content").HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired(false);

            entity.HasIndex(e => e.CaseId).HasDatabaseName("ix_case_orders_case_id");
            entity.HasIndex(e => e.OrderDate).HasDatabaseName("ix_case_orders_order_date");

            entity.HasOne(e => e.Case).WithMany(c => c.CaseOrders).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Hearing).WithMany(h => h.CaseOrders).HasForeignKey(e => e.HearingId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // ECOURT SYNC LOG
        // ============================================================

        modelBuilder.Entity<ECourtSyncLog>(entity =>
        {
            entity.ToTable("ecourt_sync_logs");
            entity.HasKey(e => e.SyncId);

            entity.Property(e => e.SyncId).HasColumnName("sync_id");
            entity.Property(e => e.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(e => e.CnrNumber).HasColumnName("cnr_number").HasMaxLength(16).IsRequired();
            entity.Property(e => e.SyncStatus).HasColumnName("sync_status").HasMaxLength(30).IsRequired();
            entity.Property(e => e.SyncSource).HasColumnName("sync_source").HasMaxLength(50).IsRequired().HasDefaultValue("AutoCron");
            entity.Property(e => e.HearingsUpdatedCount).HasColumnName("hearings_updated_count").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.OrdersDownloadedCount).HasColumnName("orders_downloaded_count").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.RawPayload).HasColumnName("raw_payload").HasColumnType("jsonb");
            entity.Property(e => e.ErrorDetails).HasColumnName("error_details").HasColumnType("text");
            entity.Property(e => e.SyncedAt).HasColumnName("synced_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.TriggeredBy).HasColumnName("triggered_by").IsRequired(false);

            entity.HasIndex(e => e.CaseId).HasDatabaseName("ix_ecourt_sync_logs_case_id");
            entity.HasIndex(e => e.CnrNumber).HasDatabaseName("ix_ecourt_sync_logs_cnr_number");
            entity.HasIndex(e => e.SyncStatus).HasDatabaseName("ix_ecourt_sync_logs_sync_status");

            entity.HasOne(e => e.Case).WithMany(c => c.ECourtSyncLogs).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.TriggeredByUser).WithMany().HasForeignKey(e => e.TriggeredBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // CASE DOCUMENT
        // ============================================================

        modelBuilder.Entity<CaseDocument>(entity =>
        {
            entity.ToTable("case_documents");
            entity.HasKey(e => e.DocumentId);

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.CaseId).HasColumnName("case_id").IsRequired();
            entity.Property(e => e.HearingId).HasColumnName("hearing_id").IsRequired(false);
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(50).IsRequired();
            entity.Property(e => e.DocumentTitle).HasColumnName("document_title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FileUrl).HasColumnName("file_url").HasMaxLength(500).IsRequired();
            entity.Property(e => e.MimeType).HasColumnName("mime_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by").IsRequired(false);

            entity.HasIndex(e => e.CaseId).HasDatabaseName("ix_case_documents_case_id");
            entity.HasIndex(e => e.Category).HasDatabaseName("ix_case_documents_category");

            entity.HasOne(e => e.Case).WithMany(c => c.CaseDocuments).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Hearing).WithMany(h => h.CaseDocuments).HasForeignKey(e => e.HearingId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.UploadedByUser).WithMany().HasForeignKey(e => e.UploadedBy).OnDelete(DeleteBehavior.Restrict);
        });


        // ============================================================
        // ECOURT API LOG
        // ============================================================

        modelBuilder.Entity<ECourtApiLog>(entity =>
        {
            entity.ToTable("ecourt_api_logs");
            entity.HasKey(e => e.LogId);

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Endpoint).HasColumnName("endpoint").HasMaxLength(200).IsRequired();
            entity.Property(e => e.HttpMethod).HasColumnName("http_method").HasMaxLength(10).IsRequired().HasDefaultValue("GET");
            entity.Property(e => e.CnrNumber).HasColumnName("cnr_number").HasMaxLength(16);
            entity.Property(e => e.StatusCode).HasColumnName("status_code").IsRequired();
            entity.Property(e => e.RequestId).HasColumnName("request_id").HasMaxLength(100);
            entity.Property(e => e.CreditsCharged).HasColumnName("credits_charged").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.DurationMs).HasColumnName("duration_ms").IsRequired();
            entity.Property(e => e.IsSuccess).HasColumnName("is_success").IsRequired();
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasColumnType("text");
            entity.Property(e => e.RequestedAt).HasColumnName("requested_at").HasColumnType("timestamptz").IsRequired();

            entity.HasIndex(e => e.RequestedAt).HasDatabaseName("ix_ecourt_api_logs_requested_at");
            entity.HasIndex(e => e.CnrNumber).HasDatabaseName("ix_ecourt_api_logs_cnr_number");
            entity.HasIndex(e => e.Endpoint).HasDatabaseName("ix_ecourt_api_logs_endpoint");
        });

        // ============================================================
        // COURT MASTER SYNC HISTORY
        // ============================================================

        modelBuilder.Entity<CourtMasterSyncHistory>(entity =>
        {
            entity.ToTable("court_master_sync_histories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(10).IsRequired();
            entity.Property(e => e.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Completed");
            entity.Property(e => e.RecordsRead).HasColumnName("records_read").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.RecordsInserted).HasColumnName("records_inserted").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.RecordsUpdated).HasColumnName("records_updated").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.RecordsSkipped).HasColumnName("records_skipped").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.RecordsFailed).HasColumnName("records_failed").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.ApiRequests).HasColumnName("api_requests").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasColumnType("text");
            entity.Property(e => e.DurationMs).HasColumnName("duration_ms").IsRequired().HasDefaultValue(0);

            entity.HasIndex(e => e.StartedAt).HasDatabaseName("ix_court_master_sync_histories_started_at");
            entity.HasIndex(e => e.State).HasDatabaseName("ix_court_master_sync_histories_state");
        });

        // ============================================================
        // APPLY BASE CONFIGURATION
        // ============================================================

        base.OnModelCreating(modelBuilder);
    }
}