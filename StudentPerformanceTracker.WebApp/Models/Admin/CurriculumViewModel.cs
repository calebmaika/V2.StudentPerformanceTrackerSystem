using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    public class CurriculumViewModel
    {
        public int CurriculumId { get; set; }

        [Required(ErrorMessage = "Curriculum name is required")]
        [StringLength(200, ErrorMessage = "Curriculum name cannot exceed 200 characters")]
        [Display(Name = "Curriculum Name")]
        public string CurriculumName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Curriculum code is required")]
        [StringLength(50, ErrorMessage = "Curriculum code cannot exceed 50 characters")]
        [Display(Name = "Curriculum Code")]
        public string CurriculumCode { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Academic year is required")]
        [StringLength(20, ErrorMessage = "Academic year cannot exceed 20 characters")]
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

        // Subject-Teacher assignments
        [Display(Name = "Subjects with Teachers")]
        public List<SubjectTeacherAssignment> SubjectTeacherAssignments { get; set; } = new List<SubjectTeacherAssignment>();

        // Students enrolled
        [Display(Name = "Enrolled Students")]
        public List<int> SelectedStudentIds { get; set; } = new List<int>();
    }

    public class SubjectTeacherAssignment
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }
}