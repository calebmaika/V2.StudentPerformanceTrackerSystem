using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    /// <summary>
    /// View model for subject create/edit forms
    /// </summary>
    public class SubjectViewModel
    {
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Subject code is required")]
        [StringLength(20, ErrorMessage = "Subject code cannot exceed 20 characters")]
        [Display(Name = "Subject Code")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject name is required")]
        [StringLength(200, ErrorMessage = "Subject name cannot exceed 200 characters")]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Units is required")]
        [Range(1, 10, ErrorMessage = "Units must be between 1 and 10")]
        [Display(Name = "Units/Credits")]
        public int Units { get; set; } = 3;

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(100, ErrorMessage = "Level cannot exceed 100 characters")]
        [Display(Name = "Academic Level")]
        public string? Level { get; set; }

        [StringLength(50, ErrorMessage = "Semester cannot exceed 50 characters")]
        [Display(Name = "Semester")]
        public string? Semester { get; set; }

        [StringLength(20, ErrorMessage = "School year cannot exceed 20 characters")]
        [Display(Name = "School Year")]
        public string? SchoolYear { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}