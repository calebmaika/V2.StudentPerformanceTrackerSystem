using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.WebApp.Models.Admin;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    /// <summary>
    /// Controller for managing subjects (Admin side)
    /// Route: /Admin/Subjects/...
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("Admin/Subjects")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// GET: /Admin/Subjects
        /// Shows list of all subjects
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return View("~/Views/Admin/SubjectManagement/Index.cshtml", subjects);
        }

        /// <summary>
        /// GET: /Admin/Subjects/Create
        /// Shows create subject form
        /// </summary>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/SubjectManagement/Create.cshtml", new SubjectViewModel());
        }

        /// <summary>
        /// POST: /Admin/Subjects/Create
        /// Creates a new subject
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/SubjectManagement/Create.cshtml", model);
            }

            // Check if subject code already exists
            if (await _subjectService.SubjectCodeExistsAsync(model.SubjectCode))
            {
                ModelState.AddModelError("SubjectCode", "Subject code already exists");
                return View("~/Views/Admin/SubjectManagement/Create.cshtml", model);
            }

            // Create subject entity
            var subject = new SubjectManagement
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Description = model.Description,
                Units = model.Units,
                Category = model.Category,
                Level = model.Level,
                Semester = model.Semester,
                SchoolYear = model.SchoolYear,
                IsActive = model.IsActive
            };

            await _subjectService.CreateSubjectAsync(subject);

            TempData["SuccessMessage"] = "Subject created successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Admin/Subjects/Edit/5
        /// Shows edit subject form
        /// </summary>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found";
                return RedirectToAction("Index");
            }

            var model = new SubjectViewModel
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                Units = subject.Units,
                Category = subject.Category,
                Level = subject.Level,
                Semester = subject.Semester,
                SchoolYear = subject.SchoolYear,
                IsActive = subject.IsActive
            };

            return View("~/Views/Admin/SubjectManagement/Edit.cshtml", model);
        }

        /// <summary>
        /// POST: /Admin/Subjects/Edit/5
        /// Updates an existing subject
        /// </summary>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectViewModel model)
        {
            if (id != model.SubjectId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/SubjectManagement/Edit.cshtml", model);
            }

            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found";
                return RedirectToAction("Index");
            }

            // Check if subject code exists (excluding current subject)
            if (await _subjectService.SubjectCodeExistsAsync(model.SubjectCode, id))
            {
                ModelState.AddModelError("SubjectCode", "Subject code already exists");
                return View("~/Views/Admin/SubjectManagement/Edit.cshtml", model);
            }

            // Update subject properties
            subject.SubjectCode = model.SubjectCode;
            subject.SubjectName = model.SubjectName;
            subject.Description = model.Description;
            subject.Units = model.Units;
            subject.Category = model.Category;
            subject.Level = model.Level;
            subject.Semester = model.Semester;
            subject.SchoolYear = model.SchoolYear;
            subject.IsActive = model.IsActive;

            await _subjectService.UpdateSubjectAsync(subject);

            TempData["SuccessMessage"] = "Subject updated successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Admin/Subjects/Details/5
        /// Shows subject details
        /// </summary>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                TempData["ErrorMessage"] = "Subject not found";
                return RedirectToAction("Index");
            }

            var model = new SubjectViewModel
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                Units = subject.Units,
                Category = subject.Category,
                Level = subject.Level,
                Semester = subject.Semester,
                SchoolYear = subject.SchoolYear,
                IsActive = subject.IsActive
            };

            return View("~/Views/Admin/SubjectManagement/Details.cshtml", model);
        }

        /// <summary>
        /// POST: /Admin/Subjects/Delete/5
        /// Deletes a subject
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            
            if (subject != null)
            {
                await _subjectService.DeleteSubjectAsync(id);
                TempData["SuccessMessage"] = "Subject deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Subject not found";
            }

            return RedirectToAction("Index");
        }
    }
}