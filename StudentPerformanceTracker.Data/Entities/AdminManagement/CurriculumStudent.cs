using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Junction table for Curriculum and Student
    /// Links students to a curriculum (enrollment)
    /// </summary>
    public class CurriculumStudent
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int CurriculumStudentId { get; set; }

        /// <summary>
        /// Foreign key to Curriculum
        /// </summary>
        [Required]
        public int CurriculumId { get; set; }

        /// <summary>
        /// Navigation property to Curriculum
        /// </summary>
        [ForeignKey("CurriculumId")]
        public CurriculumManagement Curriculum { get; set; } = null!;

        /// <summary>
        /// Foreign key to Student
        /// </summary>
        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// Navigation property to Student
        /// </summary>
        [ForeignKey("StudentId")]
        public StudentManagement Student { get; set; } = null!;

        /// <summary>
        /// When the student was enrolled in this curriculum
        /// </summary>
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}