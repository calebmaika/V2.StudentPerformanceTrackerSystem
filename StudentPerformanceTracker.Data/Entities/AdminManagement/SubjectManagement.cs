using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Subject entity managed by Admin
    /// Represents subjects/courses in the system
    /// </summary>
    public class SubjectManagement
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int SubjectId { get; set; }

        /// <summary>
        /// Subject code (e.g., "MATH101", "ENG201")
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string SubjectCode { get; set; } = string.Empty;

        /// <summary>
        /// Subject name (e.g., "Mathematics", "English Literature")
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string SubjectName { get; set; } = string.Empty;

        /// <summary>
        /// Subject description
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Number of units/credits
        /// </summary>
        [Required]
        public int Units { get; set; } = 3;

        /// <summary>
        /// Subject category (e.g., "Core", "Elective", "Major")
        /// </summary>
        [MaxLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Academic level (e.g., "Grade 7", "Grade 8", "Year 1")
        /// </summary>
        [MaxLength(100)]
        public string? Level { get; set; }

        /// <summary>
        /// Semester (e.g., "1st Semester", "2nd Semester")
        /// </summary>
        [MaxLength(50)]
        public string? Semester { get; set; }

        /// <summary>
        /// School year (e.g., "2024-2025")
        /// </summary>
        [MaxLength(20)]
        public string? SchoolYear { get; set; }

        /// <summary>
        /// When this subject record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time this subject record was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether this subject is active (can be assigned)
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}