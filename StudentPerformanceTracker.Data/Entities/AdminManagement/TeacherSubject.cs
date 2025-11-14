using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPerformanceTracker.Data.Entities.AdminManagement
{
    /// <summary>
    /// Junction table for many-to-many relationship between Teachers and Subjects
    /// </summary>
    public class TeacherSubject
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int TeacherSubjectId { get; set; }

        /// <summary>
        /// Foreign key to Teacher
        /// </summary>
        [Required]
        public int TeacherId { get; set; }

        /// <summary>
        /// Navigation property to Teacher
        /// </summary>
        [ForeignKey("TeacherId")]
        public TeacherManagement Teacher { get; set; } = null!;

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
        /// When this assignment was created
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}