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

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}