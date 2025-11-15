using System.ComponentModel.DataAnnotations;
using StudentPerformanceTracker.WebApp.Validation;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    public class CurriculumViewModel
    {
        public int CurriculumId { get; set; }

        [Required(ErrorMessage = "Curriculum name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Curriculum name must be between 3 and 200 characters")]
        [Display(Name = "Curriculum Name")]
        public string CurriculumName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Curriculum code is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Curriculum code must be between 2 and 50 characters")]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Curriculum code can only contain uppercase letters, numbers, and hyphens")]
        [Display(Name = "Curriculum Code")]
        public string CurriculumCode { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Academic year is required")]
        [AcademicYearFormat]
        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester is required")]
        [StringLength(50, ErrorMessage = "Semester cannot exceed 50 characters")]
        [Display(Name = "Semester")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade level is required")]
        [Range(7, 12, ErrorMessage = "Grade level must be between 7 and 12")]
        [Display(Name = "Grade Level")]
        public int GradeLevel { get; set; } = 7;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Subjects with Teachers")]
        [MinLength(1, ErrorMessage = "At least one subject must be assigned")]
        public List<SubjectTeacherAssignment> SubjectTeacherAssignments { get; set; } = new List<SubjectTeacherAssignment>();

        [Display(Name = "Enrolled Students")]
        public List<int> SelectedStudentIds { get; set; } = new List<int>();
    }

    public class SubjectTeacherAssignment
    {
        [Required]
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        
        [Required]
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }
}