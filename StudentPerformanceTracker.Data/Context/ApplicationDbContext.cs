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
        public DbSet<SubjectManagement> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<StudentManagement> Students { get; set; }
        public DbSet<CurriculumManagement> Curriculums { get; set; }
        public DbSet<CurriculumSubject> CurriculumSubjects { get; set; }
        public DbSet<CurriculumStudent> CurriculumStudents { get; set; }

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

            modelBuilder.Entity<SubjectManagement>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.HasIndex(e => e.SubjectCode).IsUnique();
                entity.ToTable("Subjects");
            });

            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.HasKey(e => e.TeacherSubjectId);
                entity.ToTable("TeacherSubjects");

                entity.HasOne(ts => ts.Teacher)
                    .WithMany(t => t.TeacherSubjects)
                    .HasForeignKey(ts => ts.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ts => ts.Subject)
                    .WithMany(s => s.TeacherSubjects)
                    .HasForeignKey(ts => ts.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.TeacherId, e.SubjectId }).IsUnique();
            });

            modelBuilder.Entity<StudentManagement>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.ToTable("Students");
            });

            // Curriculum
            modelBuilder.Entity<CurriculumManagement>(entity =>
            {
                entity.HasKey(e => e.CurriculumId);
                entity.HasIndex(e => e.CurriculumCode).IsUnique();
                entity.ToTable("Curriculums");
            });

            // CurriculumSubject
            modelBuilder.Entity<CurriculumSubject>(entity =>
            {
                entity.HasKey(e => e.CurriculumSubjectId);
                entity.ToTable("CurriculumSubjects");

                entity.HasOne(cs => cs.Curriculum)
                    .WithMany(c => c.CurriculumSubjects)
                    .HasForeignKey(cs => cs.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cs => cs.Subject)
                    .WithMany()
                    .HasForeignKey(cs => cs.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cs => cs.Teacher)
                    .WithMany()
                    .HasForeignKey(cs => cs.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CurriculumId, e.SubjectId }).IsUnique();
            });

            // CurriculumStudent
            modelBuilder.Entity<CurriculumStudent>(entity =>
            {
                entity.HasKey(e => e.CurriculumStudentId);
                entity.ToTable("CurriculumStudents");

                entity.HasOne(cs => cs.Curriculum)
                    .WithMany(c => c.CurriculumStudents)
                    .HasForeignKey(cs => cs.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cs => cs.Student)
                    .WithMany()
                    .HasForeignKey(cs => cs.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CurriculumId, e.StudentId }).IsUnique();
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