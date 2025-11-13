using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Teacher entity managed by Admin
    /// Represents teachers in the system that admins can CRUD
    /// </summary>
    public class TeacherManagement
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int TeacherId { get; set; }

        /// <summary>
        /// Teacher's login username (must be unique)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password for teacher login
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Path to profile picture (e.g., /uploads/teachers/abc.jpg)
        /// </summary>
        [MaxLength(255)]
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Teacher's last name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Teacher's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Teacher's middle name (optional)
        /// </summary>
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Teacher's date of birth
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Calculated age based on date of birth
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Teacher's residential address
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Comma-separated list of subjects assigned to teacher
        /// Example: "Math, Science, English"
        /// </summary>
        [MaxLength(500)]
        public string? SubjectsAssigned { get; set; }

        /// <summary>
        /// When this teacher record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time this teacher record was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether this teacher account is active (can login)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Full name computed property
        /// </summary>
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}