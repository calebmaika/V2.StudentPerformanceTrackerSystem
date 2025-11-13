using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.Data.Entities
{
    /// <summary>
    /// Represents an administrator user in the system
    /// This will create a table called "Admins" in the database
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Primary key - unique ID for each admin
        /// </summary>
        [Key]
        public int AdminId { get; set; }

        /// <summary>
        /// Admin's login username (must be unique)
        /// Max 100 characters, required field
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password using BCrypt
        /// NEVER store plain text passwords!
        /// Max 255 characters to store the hash
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Admin's email address (must be unique)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Admin's full name for display
        /// </summary>
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// When this admin account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time this admin logged in
        /// Nullable because they might never login
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Whether this admin account is active (can login)
        /// Default is true
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}