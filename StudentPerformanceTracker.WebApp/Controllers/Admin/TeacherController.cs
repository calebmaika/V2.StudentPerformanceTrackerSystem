using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Teachers;
using StudentPerformanceTracker.Services.Authentication;
using StudentPerformanceTracker.WebApp.Models.Admin;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    /// <summary>
    /// Controller for managing teachers (Admin side)
    /// Route: /Admin/Teachers/...
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("Admin/Teachers")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IPasswordService _passwordService;
        private readonly IWebHostEnvironment _environment;

        public TeachersController(
            ITeacherService teacherService, 
            IPasswordService passwordService,
            IWebHostEnvironment environment)
        {
            _teacherService = teacherService;
            _passwordService = passwordService;
            _environment = environment;
        }

        /// <summary>
        /// GET: /Admin/Teachers
        /// Shows list of all teachers
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return View("~/Views/Admin/Teachers/Index.cshtml", teachers);
        }

        /// <summary>
        /// GET: /Admin/Teachers/Create
        /// Shows create teacher form
        /// </summary>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Teachers/Create.cshtml", new TeacherViewModel());
        }

        /// <summary>
        /// POST: /Admin/Teachers/Create
        /// Creates a new teacher
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Teachers/Create.cshtml", model);
            }

            // Check if username already exists
            if (await _teacherService.UsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View("~/Views/Admin/Teachers/Create.cshtml", model);
            }

            // Handle profile picture upload
            string? profilePicturePath = null;
            if (model.ProfilePictureFile != null)
            {
                profilePicturePath = await SaveProfilePicture(model.ProfilePictureFile);
            }

            // Calculate age from date of birth
            var age = CalculateAge(model.DateOfBirth);

            // Create teacher entity
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
                SubjectsAssigned = model.SubjectsAssigned,
                IsActive = model.IsActive
            };

            await _teacherService.CreateTeacherAsync(teacher, model.Password!);

            TempData["SuccessMessage"] = "Teacher created successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Admin/Teachers/Edit/5
        /// Shows edit teacher form
        /// </summary>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

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
                SubjectsAssigned = teacher.SubjectsAssigned,
                IsActive = teacher.IsActive
            };

            return View("~/Views/Admin/Teachers/Edit.cshtml", model);
        }

        /// <summary>
        /// POST: /Admin/Teachers/Edit/5
        /// Updates an existing teacher
        /// </summary>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel model)
        {
            if (id != model.TeacherId)
            {
                return NotFound();
            }

            // Remove password validation for edit (password is optional when editing)
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Teachers/Edit.cshtml", model);
            }

            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

            // Check if username exists (excluding current teacher)
            if (await _teacherService.UsernameExistsAsync(model.Username, id))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View("~/Views/Admin/Teachers/Edit.cshtml", model);
            }

            // Handle profile picture upload
            if (model.ProfilePictureFile != null)
            {
                // Delete old picture if exists
                if (!string.IsNullOrEmpty(teacher.ProfilePicture))
                {
                    DeleteProfilePicture(teacher.ProfilePicture);
                }
                teacher.ProfilePicture = await SaveProfilePicture(model.ProfilePictureFile);
            }

            // Update teacher properties
            teacher.Username = model.Username;
            teacher.FirstName = model.FirstName;
            teacher.MiddleName = model.MiddleName;
            teacher.LastName = model.LastName;
            teacher.DateOfBirth = model.DateOfBirth;
            teacher.Age = CalculateAge(model.DateOfBirth);
            teacher.Address = model.Address;
            teacher.SubjectsAssigned = model.SubjectsAssigned;
            teacher.IsActive = model.IsActive;

            // Update password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                teacher.PasswordHash = _passwordService.HashPassword(model.Password);
            }

            await _teacherService.UpdateTeacherAsync(teacher);

            TempData["SuccessMessage"] = "Teacher updated successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Admin/Teachers/Details/5
        /// Shows teacher details
        /// </summary>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found";
                return RedirectToAction("Index");
            }

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
                SubjectsAssigned = teacher.SubjectsAssigned,
                IsActive = teacher.IsActive
            };

            return View("~/Views/Admin/Teachers/Details.cshtml", model);
        }

        /// <summary>
        /// POST: /Admin/Teachers/Delete/5
        /// Deletes a teacher
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            
            if (teacher != null)
            {
                // Delete profile picture if exists
                if (!string.IsNullOrEmpty(teacher.ProfilePicture))
                {
                    DeleteProfilePicture(teacher.ProfilePicture);
                }

                await _teacherService.DeleteTeacherAsync(id);
                TempData["SuccessMessage"] = "Teacher deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Teacher not found";
            }

            return RedirectToAction("Index");
        }

        #region Helper Methods

        /// <summary>
        /// Calculate age from date of birth
        /// </summary>
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            
            // Adjust if birthday hasn't occurred this year
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            
            return age;
        }

        /// <summary>
        /// Save uploaded profile picture and return path
        /// </summary>
        private async Task<string> SaveProfilePicture(IFormFile file)
        {
            // Create uploads folder if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "teachers");
            Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return web path
            return $"/uploads/teachers/{uniqueFileName}";
        }

        /// <summary>
        /// Delete profile picture from server
        /// </summary>
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