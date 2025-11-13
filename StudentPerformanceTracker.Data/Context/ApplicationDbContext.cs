using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Entities;

namespace StudentPerformanceTracker.Data.Context
{
    /// <summary>
    /// Main database context for the application
    /// This is the bridge between your C# code and SQLite database
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor - accepts configuration options
        /// ASP.NET Core will automatically pass the configuration
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet represents the "Admins" table in the database
        /// </summary>
        public DbSet<Admin> Admins { get; set; }

        /// <summary>
        /// Configure database options and suppress warnings
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress the pending model changes warning
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        /// <summary>
        /// This method configures the database schema and seeds initial data
        /// Called automatically by EF Core
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                // Set AdminId as primary key
                entity.HasKey(e => e.AdminId);
                
                // Make Username unique - no two admins can have same username
                entity.HasIndex(e => e.Username).IsUnique();
                
                // Make Email unique - no two admins can have same email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed the database with a default admin account
            // This creates one admin user when database is first created
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Username = "admin",
                    // Password is "Admin@123" - already hashed with BCrypt
                    // This is a BCrypt hash, NOT the actual password
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