using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Validation
{
    /// <summary>
    /// Validates that a date is not in the future
    /// Useful for Date of Birth fields
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue > DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future");
                }
            }
            
            return ValidationResult.Success;
        }
    }
}