using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Entities;
using StudentPerformanceTracker.Data.Entities.AdminManagement;

namespace StudentPerformanceTracker.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<TeacherManagement> Teachers { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<TeacherManagement>(entity =>
            {
                entity.HasKey(e => e.TeacherId);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.ToTable("Teachers");
            });

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$xN8PkDz7Y.X/VGqKb6Cv3e8YqJH8xZGxHvZHKYNJzKvNx6mW5yTCq",
                    Email = "admin@studenttracker.com",
                    FullName = "System Administrator",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}