using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Students;
using StudentPerformanceTracker.WebApp.Models.Admin;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Students")]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IWebHostEnvironment _environment;

        public StudentsController(IStudentService studentService, IWebHostEnvironment environment)
        {
            _studentService = studentService;
            _environment = environment;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View("~/Views/Admin/StudentManagement/Index.cshtml", students);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/StudentManagement/Create.cshtml", new StudentViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/StudentManagement/Create.cshtml", model);
            }

            string? profilePicturePath = null;
            if (model.ProfilePictureFile != null)
            {
                profilePicturePath = await SaveProfilePicture(model.ProfilePictureFile);
            }

            var age = CalculateAge(model.DateOfBirth);

            var student = new StudentManagement
            {
                ProfilePicture = profilePicturePath,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Age = age,
                GradeLevel = model.GradeLevel,
                Address = model.Address,
                IsActive = model.IsActive
            };

            await _studentService.CreateStudentAsync(student);

            TempData["SuccessMessage"] = "Student created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found";
                return RedirectToAction("Index");
            }

            var model = new StudentViewModel
            {
                StudentId = student.StudentId,
                ProfilePicture = student.ProfilePicture,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                Age = student.Age,
                GradeLevel = student.GradeLevel,
                Address = student.Address,
                IsActive = student.IsActive
            };

            return View("~/Views/Admin/StudentManagement/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            if (id != model.StudentId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/StudentManagement/Edit.cshtml", model);
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found";
                return RedirectToAction("Index");
            }

            if (model.ProfilePictureFile != null)
            {
                if (!string.IsNullOrEmpty(student.ProfilePicture))
                {
                    DeleteProfilePicture(student.ProfilePicture);
                }
                student.ProfilePicture = await SaveProfilePicture(model.ProfilePictureFile);
            }

            student.FirstName = model.FirstName;
            student.MiddleName = model.MiddleName;
            student.LastName = model.LastName;
            student.DateOfBirth = model.DateOfBirth;
            student.Age = CalculateAge(model.DateOfBirth);
            student.GradeLevel = model.GradeLevel;
            student.Address = model.Address;
            student.IsActive = model.IsActive;

            await _studentService.UpdateStudentAsync(student);

            TempData["SuccessMessage"] = "Student updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found";
                return RedirectToAction("Index");
            }

            var model = new StudentViewModel
            {
                StudentId = student.StudentId,
                ProfilePicture = student.ProfilePicture,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                Age = student.Age,
                GradeLevel = student.GradeLevel,
                Address = student.Address,
                IsActive = student.IsActive
            };

            return View("~/Views/Admin/StudentManagement/Details.cshtml", model);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            
            if (student != null)
            {
                if (!string.IsNullOrEmpty(student.ProfilePicture))
                {
                    DeleteProfilePicture(student.ProfilePicture);
                }

                await _studentService.DeleteStudentAsync(id);
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Student not found";
            }

            return RedirectToAction("Index");
        }

        #region Helper Methods

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            
            return age;
        }

        private async Task<string> SaveProfilePicture(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/students/{uniqueFileName}";
        }

        private void DeleteProfilePicture(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        #endregion
    }
}