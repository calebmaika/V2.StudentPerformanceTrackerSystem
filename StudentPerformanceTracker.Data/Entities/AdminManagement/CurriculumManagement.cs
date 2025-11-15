using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Curriculum entity managed by Admin
    /// Represents curriculum/program combinations of subjects, teachers, and students
    /// </summary>
    public class CurriculumManagement
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int CurriculumId { get; set; }

        /// <summary>
        /// Curriculum name (e.g., "Grade 7 - Section A", "Grade 8 Science Program")
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CurriculumName { get; set; } = string.Empty;

        /// <summary>
        /// Curriculum code (e.g., "G7-A", "G8-SCI")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CurriculumCode { get; set; } = string.Empty;

        /// <summary>
        /// Description of the curriculum
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Academic year (e.g., "2024-2025")
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AcademicYear { get; set; } = string.Empty;

        /// <summary>
        /// Semester (e.g., "First Semester", "Second Semester")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Semester { get; set; } = string.Empty;

        /// <summary>
        /// Grade level this curriculum is for
        /// </summary>
        [Required]
        public int GradeLevel { get; set; }

        /// <summary>
        /// When this curriculum record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time this curriculum record was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether this curriculum is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property for curriculum subjects (with assigned teachers)
        /// </summary>
        public ICollection<CurriculumSubject> CurriculumSubjects { get; set; } = new List<CurriculumSubject>();

        /// <summary>
        /// Navigation property for enrolled students
        /// </summary>
        public ICollection<CurriculumStudent> CurriculumStudents { get; set; } = new List<CurriculumStudent>();
    }
}