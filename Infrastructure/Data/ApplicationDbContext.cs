using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Lawyer> Lawyers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lawyer>(entity =>
            {
                entity.ToTable("lawyers");

                entity.HasKey(e => e.LawyerId);

                entity.Property(e => e.LawyerId)
                    .HasColumnName("lawyer_id");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name");

                entity.Property(e => e.MobileNumber)
                    .HasColumnName("mobile_number");

                entity.Property(e => e.Email)
                    .HasColumnName("email");

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                // Indexes
                entity.HasIndex(e => e.MobileNumber)
                    .HasDatabaseName("ix_lawyers_mobile_number");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("ix_lawyers_email");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_lawyers_status");
                
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
