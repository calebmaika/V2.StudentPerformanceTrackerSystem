using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Student entity managed by Admin
    /// Represents students in the system that admins can CRUD
    /// </summary>
    public class StudentManagement
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int StudentId { get; set; }

        /// <summary>
        /// Path to profile picture (e.g., /uploads/students/abc.jpg)
        /// </summary>
        [MaxLength(255)]
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Student's last name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Student's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Student's middle name (optional)
        /// </summary>
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Student's date of birth
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Calculated age based on date of birth
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Student's grade level (e.g., 7, 8, 9, 10, 11, 12)
        /// </summary>
        [Required]
        public int GradeLevel { get; set; }

        /// <summary>
        /// Student's residential address
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// When this student record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time this student record was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether this student account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Full name computed property
        /// </summary>
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}