using System.Text.RegularExpressions;
using System.Web;

namespace StudentPerformanceTracker.WebApp.Helpers
{
    /// <summary>
    /// Helper class for sanitizing user input to prevent XSS attacks
    /// </summary>
    public static class InputSanitizer
    {
        /// <summary>
        /// Removes potentially dangerous HTML/JavaScript from input
        /// </summary>
        public static string SanitizeHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Encode HTML entities
            var sanitized = HttpUtility.HtmlEncode(input);

            // Remove script tags
            sanitized = Regex.Replace(sanitized, @"<script[^>]*>.*?</script>", "", RegexOptions.IgnoreCase);

            // Remove event handlers
            sanitized = Regex.Replace(sanitized, @"on\w+\s*=", "", RegexOptions.IgnoreCase);

            return sanitized;
        }

        /// <summary>
        /// Validates and sanitizes SQL-safe input (though EF Core handles this)
        /// </summary>
        public static string SanitizeSqlInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove SQL comment markers
            var sanitized = input.Replace("--", "").Replace("/*", "").Replace("*/", "");

            // Remove semicolons (end of SQL statement)
            sanitized = sanitized.Replace(";", "");

            return sanitized;
        }

        /// <summary>
        /// Validates file upload to ensure it's an allowed type
        /// </summary>
        public static bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                return false;

            // Check file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            return true;
        }
    }
}