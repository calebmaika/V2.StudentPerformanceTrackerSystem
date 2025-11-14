using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    /// <summary>
    /// View model for teacher create/edit forms
    /// </summary>
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

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
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);

        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Subjects Assigned")]
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}