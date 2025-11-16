using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Curriculum;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.Services.Teachers;
using StudentPerformanceTracker.Services.Students;
using StudentPerformanceTracker.WebApp.Models.Admin;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Curriculums")]
    public class CurriculumsController : Controller
    {
        private readonly ICurriculumService _curriculumService;
        private readonly ISubjectService _subjectService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;

        public CurriculumsController(
            ICurriculumService curriculumService,
            ISubjectService subjectService,
            ITeacherService teacherService,
            IStudentService studentService)
        {
            _curriculumService = curriculumService;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _studentService = studentService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var curriculums = await _curriculumService.GetAllCurriculumsAsync();
            return View("~/Views/Admin/CurriculumManagement/Index.cshtml", curriculums);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View("~/Views/Admin/CurriculumManagement/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CurriculumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View("~/Views/Admin/CurriculumManagement/Create.cshtml", model);
            }

            try
            {
                if (await _curriculumService.CurriculumCodeExistsAsync(model.CurriculumCode))
                {
                    ModelState.AddModelError("CurriculumCode", "Curriculum code already exists");
                    await LoadViewBagData();
                    return View("~/Views/Admin/CurriculumManagement/Create.cshtml", model);
                }

                var curriculum = new CurriculumManagement
                {
                    CurriculumName = model.CurriculumName,
                    CurriculumCode = model.CurriculumCode,
                    Description = model.Description,
                    AcademicYear = model.AcademicYear,
                    Semester = model.Semester,
                    GradeLevel = model.GradeLevel,
                    IsActive = model.IsActive
                };

                // Convert SubjectTeacherAssignments to CurriculumSubjectDto
                var subjects = model.SubjectTeacherAssignments
                    .Select(s => new CurriculumSubjectDto
                    {
                        SubjectId = s.SubjectId,
                        TeacherId = s.TeacherId
                    }).ToList();

                await _curriculumService.CreateCurriculumAsync(curriculum, subjects, model.SelectedStudentIds);

                TempData["SuccessMessage"] = "Curriculum created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating curriculum: {ex.Message}";
                await LoadViewBagData();
                return View("~/Views/Admin/CurriculumManagement/Create.cshtml", model);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var curriculum = await _curriculumService.GetCurriculumByIdAsync(id);
            if (curriculum == null)
            {
                TempData["ErrorMessage"] = "Curriculum not found";
                return RedirectToAction("Index");
            }

            var model = new CurriculumViewModel
            {
                CurriculumId = curriculum.CurriculumId,
                CurriculumName = curriculum.CurriculumName,
                CurriculumCode = curriculum.CurriculumCode,
                Description = curriculum.Description,
                AcademicYear = curriculum.AcademicYear,
                Semester = curriculum.Semester,
                GradeLevel = curriculum.GradeLevel,
                IsActive = curriculum.IsActive,
                SubjectTeacherAssignments = curriculum.CurriculumSubjects.Select(cs => new SubjectTeacherAssignment
                {
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject.SubjectName,
                    TeacherId = cs.TeacherId,
                    TeacherName = cs.Teacher.FullName
                }).ToList(),
                SelectedStudentIds = curriculum.CurriculumStudents.Select(cs => cs.StudentId).ToList()
            };

            await LoadViewBagData();
            return View("~/Views/Admin/CurriculumManagement/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CurriculumViewModel model)
        {
            if (id != model.CurriculumId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View("~/Views/Admin/CurriculumManagement/Edit.cshtml", model);
            }

            try
            {
                var curriculum = await _curriculumService.GetCurriculumByIdAsync(id);
                if (curriculum == null)
                {
                    TempData["ErrorMessage"] = "Curriculum not found";
                    return RedirectToAction("Index");
                }

                if (await _curriculumService.CurriculumCodeExistsAsync(model.CurriculumCode, id))
                {
                    ModelState.AddModelError("CurriculumCode", "Curriculum code already exists");
                    await LoadViewBagData();
                    return View("~/Views/Admin/CurriculumManagement/Edit.cshtml", model);
                }

                curriculum.CurriculumName = model.CurriculumName;
                curriculum.CurriculumCode = model.CurriculumCode;
                curriculum.Description = model.Description;
                curriculum.AcademicYear = model.AcademicYear;
                curriculum.Semester = model.Semester;
                curriculum.GradeLevel = model.GradeLevel;
                curriculum.IsActive = model.IsActive;

                var subjects = model.SubjectTeacherAssignments
                    .Select(s => new CurriculumSubjectDto
                    {
                        SubjectId = s.SubjectId,
                        TeacherId = s.TeacherId
                    }).ToList();

                await _curriculumService.UpdateCurriculumAsync(curriculum, subjects, model.SelectedStudentIds);

                TempData["SuccessMessage"] = "Curriculum updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating curriculum: {ex.Message}";
                await LoadViewBagData();
                return View("~/Views/Admin/CurriculumManagement/Edit.cshtml", model);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var curriculum = await _curriculumService.GetCurriculumByIdAsync(id);
            if (curriculum == null)
            {
                TempData["ErrorMessage"] = "Curriculum not found";
                return RedirectToAction("Index");
            }

            var model = new CurriculumViewModel
            {
                CurriculumId = curriculum.CurriculumId,
                CurriculumName = curriculum.CurriculumName,
                CurriculumCode = curriculum.CurriculumCode,
                Description = curriculum.Description,
                AcademicYear = curriculum.AcademicYear,
                Semester = curriculum.Semester,
                GradeLevel = curriculum.GradeLevel,
                IsActive = curriculum.IsActive,
                SubjectTeacherAssignments = curriculum.CurriculumSubjects.Select(cs => new SubjectTeacherAssignment
                {
                    SubjectId = cs.SubjectId,
                    SubjectName = $"{cs.Subject.SubjectCode} - {cs.Subject.SubjectName}",
                    TeacherId = cs.TeacherId,
                    TeacherName = cs.Teacher.FullName
                }).ToList(),
                SelectedStudentIds = curriculum.CurriculumStudents.Select(cs => cs.StudentId).ToList()
            };

            return View("~/Views/Admin/CurriculumManagement/Details.cshtml", model);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var curriculum = await _curriculumService.GetCurriculumByIdAsync(id);
                
                if (curriculum != null)
                {
                    await _curriculumService.DeleteCurriculumAsync(id);
                    TempData["SuccessMessage"] = "Curriculum deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Curriculum not found";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting curriculum: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private async Task LoadViewBagData()
        {
            // Load subjects
            var subjects = await _subjectService.GetAllSubjectsAsync();
            ViewBag.Subjects = subjects.Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = $"{s.SubjectCode} - {s.SubjectName}"
                }).ToList();

            // Load teachers
            var teachers = await _teacherService.GetAllTeachersAsync();
            ViewBag.Teachers = teachers.Where(t => t.IsActive)
                .Select(t => new SelectListItem
                {
                    Value = t.TeacherId.ToString(),
                    Text = t.FullName
                }).ToList();

            // Load students
            var students = await _studentService.GetAllStudentsAsync();
            ViewBag.Students = students.Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId.ToString(),
                    Text = $"{s.FullName} | (Grade {s.GradeLevel})"
                }).ToList();
        }

        #endregion
    }
}