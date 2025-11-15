using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Validation
{
    /// <summary>
    /// Validates minimum age requirement
    /// Useful for teacher age validation (must be 18+)
    /// </summary>
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                
                if (dateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"Must be at least {_minimumAge} years old");
                }
            }
            
            return ValidationResult.Success;
        }
    }
}