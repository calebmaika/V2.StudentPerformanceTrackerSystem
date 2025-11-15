using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Teachers;
using StudentPerformanceTracker.Services.Authentication;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.WebApp.Models.Admin;
using StudentPerformanceTracker.Services.Auditing;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Teachers")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ISubjectService _subjectService;
        private readonly IPasswordService _passwordService;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuditService _auditService;

        public TeachersController(
            ITeacherService teacherService,
            ISubjectService subjectService,
            IPasswordService passwordService,
            IWebHostEnvironment environment,
            IAuditService auditService)
        {
            _teacherService = teacherService;
            _subjectService = subjectService;
            _passwordService = passwordService;
            _environment = environment;
            _auditService = auditService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return View("~/Views/Admin/TeacherManagement/Index.cshtml", teachers);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            ViewBag.Subjects = subjects.Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = $"{s.SubjectCode} - {s.SubjectName}"
                }).ToList();

            return View("~/Views/Admin/TeacherManagement/Create.cshtml", new TeacherViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            // Server-side validation: Check username uniqueness
            if (await _teacherService.UsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "This username is already taken");
            }

            // Server-side validation: Validate profile picture
            if (model.ProfilePictureFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(model.ProfilePictureFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ProfilePictureFile", "Only image files (JPG, PNG, GIF) are allowed");
                }
                
                // Check file size (max 5MB)
                if (model.ProfilePictureFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfilePictureFile", "File size cannot exceed 5MB");
                }
            }

            // If validation fails, return to form
            if (!ModelState.IsValid)
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects.Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SubjectId.ToString(),
                        Text = $"{s.SubjectCode} - {s.SubjectName}"
                    }).ToList();
                return View("~/Views/Admin/TeacherManagement/Create.cshtml", model);
            }

            try
            {
                string? profilePicturePath = null;
                if (model.ProfilePictureFile != null)
                {
                    profilePicturePath = await SaveProfilePicture(model.ProfilePictureFile);
                }

                var age = CalculateAge(model.DateOfBirth);

                var teacher = new TeacherManagement
                {
                    Username = model.Username,
                    ProfilePicture = profilePicturePath,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Age = age,
                    Address = model.Address,
                    IsActive = model.IsActive
                };

                await _teacherService.CreateTeacherAsync(teacher, model.Password!, model.SelectedSubjectIds);

                // Log the audit action
                await _auditService.LogActionAsync(
                    action: "Create",
                    entityType: "Teacher",
                    entityId: teacher.TeacherId,
                    username: User.Identity?.Name ?? "Unknown",
                    details: $"Created teacher: {teacher.FullName} (Username: {teacher.Username})"
                );

                TempData["SuccessMessage"] = "Teacher created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (we'll implement logging next)
                TempData["ErrorMessage"] = $"Error creating teacher: {ex.Message}";
                
                var subjects = await _subjectService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects.Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SubjectId.ToString(),
                        Text = $"{s.SubjectCode} - {s.SubjectName}"
                    }).ToList();
                
                return View("~/Views/Admin/TeacherManagement/Create.cshtml", model);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

            var subjects = await _subjectService.GetAllSubjectsAsync();
            var selectedSubjectIds = await _teacherService.GetTeacherSubjectIdsAsync(id);

            ViewBag.Subjects = subjects.Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = $"{s.SubjectCode} - {s.SubjectName}",
                    Selected = selectedSubjectIds.Contains(s.SubjectId)
                }).ToList();

            var model = new TeacherViewModel
            {
                TeacherId = teacher.TeacherId,
                Username = teacher.Username,
                ProfilePicture = teacher.ProfilePicture,
                FirstName = teacher.FirstName,
                MiddleName = teacher.MiddleName,
                LastName = teacher.LastName,
                DateOfBirth = teacher.DateOfBirth,
                Age = teacher.Age,
                Address = teacher.Address,
                SelectedSubjectIds = selectedSubjectIds,
                IsActive = teacher.IsActive
            };

            return View("~/Views/Admin/TeacherManagement/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel model)
        {
            if (id != model.TeacherId)
            {
                return NotFound();
            }

            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects.Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SubjectId.ToString(),
                        Text = $"{s.SubjectCode} - {s.SubjectName}",
                        Selected = model.SelectedSubjectIds.Contains(s.SubjectId)
                    }).ToList();
                return View("~/Views/Admin/TeacherManagement/Edit.cshtml", model);
            }

            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

            if (await _teacherService.UsernameExistsAsync(model.Username, id))
            {
                ModelState.AddModelError("Username", "Username already exists");
                var subjects = await _subjectService.GetAllSubjectsAsync();
                ViewBag.Subjects = subjects.Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SubjectId.ToString(),
                        Text = $"{s.SubjectCode} - {s.SubjectName}",
                        Selected = model.SelectedSubjectIds.Contains(s.SubjectId)
                    }).ToList();
                return View("~/Views/Admin/TeacherManagement/Edit.cshtml", model);
            }

            if (model.ProfilePictureFile != null)
            {
                if (!string.IsNullOrEmpty(teacher.ProfilePicture))
                {
                    DeleteProfilePicture(teacher.ProfilePicture);
                }
                teacher.ProfilePicture = await SaveProfilePicture(model.ProfilePictureFile);
            }

            teacher.Username = model.Username;
            teacher.FirstName = model.FirstName;
            teacher.MiddleName = model.MiddleName;
            teacher.LastName = model.LastName;
            teacher.DateOfBirth = model.DateOfBirth;
            teacher.Age = CalculateAge(model.DateOfBirth);
            teacher.Address = model.Address;
            teacher.IsActive = model.IsActive;

            if (!string.IsNullOrEmpty(model.Password))
            {
                teacher.PasswordHash = _passwordService.HashPassword(model.Password);
            }

            await _teacherService.UpdateTeacherAsync(teacher, model.SelectedSubjectIds);

            // Log the audit action
            await _auditService.LogActionAsync(
                action: "Update",
                entityType: "Teacher",
                entityId: teacher.TeacherId,
                username: User.Identity?.Name ?? "Unknown",
                details: $"Updated teacher: {teacher.FullName} (Username: {teacher.Username})"
            );

            TempData["SuccessMessage"] = "Teacher updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

            var selectedSubjectIds = await _teacherService.GetTeacherSubjectIdsAsync(id);

            var model = new TeacherViewModel
            {
                TeacherId = teacher.TeacherId,
                Username = teacher.Username,
                ProfilePicture = teacher.ProfilePicture,
                FirstName = teacher.FirstName,
                MiddleName = teacher.MiddleName,
                LastName = teacher.LastName,
                DateOfBirth = teacher.DateOfBirth,
                Age = teacher.Age,
                Address = teacher.Address,
                SelectedSubjectIds = selectedSubjectIds,
                IsActive = teacher.IsActive
            };

            return View("~/Views/Admin/TeacherManagement/Details.cshtml", model);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            
            if (teacher != null)
            {
                if (!string.IsNullOrEmpty(teacher.ProfilePicture))
                {
                    DeleteProfilePicture(teacher.ProfilePicture);
                }

                // Store teacher info before deletion for audit log
                var teacherName = teacher.FullName;
                var teacherUsername = teacher.Username;

                await _teacherService.DeleteTeacherAsync(id);

                // Log the audit action
                await _auditService.LogActionAsync(
                    action: "Delete",
                    entityType: "Teacher",
                    entityId: id,
                    username: User.Identity?.Name ?? "Unknown",
                    details: $"Deleted teacher: {teacherName} (Username: {teacherUsername})"
                );

                TempData["SuccessMessage"] = "Teacher deleted successfully!";
            }
            
            else
            {
                TempData["ErrorMessage"] = "Teacher not found";
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
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "teachers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/teachers/{uniqueFileName}";
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