using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Junction table for Curriculum and Subject with assigned Teacher
    /// Links a subject to a curriculum and specifies which teacher teaches it
    /// </summary>
    public class CurriculumSubject
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int CurriculumSubjectId { get; set; }

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
        /// Foreign key to Subject
        /// </summary>
        [Required]
        public int SubjectId { get; set; }

        /// <summary>
        /// Navigation property to Subject
        /// </summary>
        [ForeignKey("SubjectId")]
        public SubjectManagement Subject { get; set; } = null!;

        /// <summary>
        /// Foreign key to Teacher assigned to teach this subject in this curriculum
        /// </summary>
        [Required]
        public int TeacherId { get; set; }

        /// <summary>
        /// Navigation property to Teacher
        /// </summary>
        [ForeignKey("TeacherId")]
        public TeacherManagement Teacher { get; set; } = null!;

        /// <summary>
        /// When this subject was added to the curriculum
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}