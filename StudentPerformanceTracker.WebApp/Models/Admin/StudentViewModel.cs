using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    /// <summary>
    /// View model for student create/edit forms
    /// </summary>
    public class StudentViewModel
    {
        public int StudentId { get; set; }

        [Display(Name = "Profile Picture")]
        [DataType(DataType.Upload)]
        public IFormFile? ProfilePictureFile { get; set; }

        public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-13);

        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Grade level is required")]
        [Range(7, 12, ErrorMessage = "Grade level must be between 7 and 12")]
        [Display(Name = "Grade Level")]
        public int GradeLevel { get; set; } = 7;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}