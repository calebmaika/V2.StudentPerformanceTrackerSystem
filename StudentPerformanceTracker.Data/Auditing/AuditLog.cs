using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities.Auditing
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // "Create", "Update", "Delete"

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty; // "Teacher", "Student", etc.

        public int EntityId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Details { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}