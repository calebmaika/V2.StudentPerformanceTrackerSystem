using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StudentPerformanceTracker.WebApp.Validation
{
    /// <summary>
    /// Validates academic year format (e.g., "2024-2025")
    /// </summary>
    public class AcademicYearFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let [Required] handle empty values
            }

            string academicYear = value.ToString()!;
            
            // Format: YYYY-YYYY
            var regex = new Regex(@"^\d{4}-\d{4}$");
            
            if (!regex.IsMatch(academicYear))
            {
                return new ValidationResult("Academic year must be in format YYYY-YYYY (e.g., 2024-2025)");
            }

            // Parse years
            var years = academicYear.Split('-');
            if (int.TryParse(years[0], out int year1) && int.TryParse(years[1], out int year2))
            {
                // Second year should be exactly 1 year after first
                if (year2 != year1 + 1)
                {
                    return new ValidationResult("Academic year must be consecutive (e.g., 2024-2025)");
                }
                
                // Shouldn't be too far in the future
                if (year1 > DateTime.Now.Year + 2)
                {
                    return new ValidationResult("Academic year cannot be more than 2 years in the future");
                }
            }
            
            return ValidationResult.Success;
        }
    }
}