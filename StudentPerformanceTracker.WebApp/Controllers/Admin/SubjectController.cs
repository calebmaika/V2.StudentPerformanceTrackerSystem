using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Subject;
using StudentPerformanceTracker.WebApp.Models.Admin;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Subjects")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return View("~/Views/Admin/SubjectManagement/Index.cshtml", subjects);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/SubjectManagement/Create.cshtml", new SubjectViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/SubjectManagement/Create.cshtml", model);
            }

            if (await _subjectService.SubjectCodeExistsAsync(model.SubjectCode))
            {
                ModelState.AddModelError("SubjectCode", "Subject code already exists");
                return View("~/Views/Admin/SubjectManagement/Create.cshtml", model);
            }

            var subject = new SubjectManagement
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Description = model.Description,
                IsActive = model.IsActive
            };

            await _subjectService.CreateSubjectAsync(subject);

            TempData["SuccessMessage"] = "Subject created successfully!";
            return RedirectToAction("Index");
        }

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
                IsActive = subject.IsActive
            };

            return View("~/Views/Admin/SubjectManagement/Edit.cshtml", model);
        }

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

            if (await _subjectService.SubjectCodeExistsAsync(model.SubjectCode, id))
            {
                ModelState.AddModelError("SubjectCode", "Subject code already exists");
                return View("~/Views/Admin/SubjectManagement/Edit.cshtml", model);
            }

            subject.SubjectCode = model.SubjectCode;
            subject.SubjectName = model.SubjectName;
            subject.Description = model.Description;
            subject.IsActive = model.IsActive;

            await _subjectService.UpdateSubjectAsync(subject);

            TempData["SuccessMessage"] = "Subject updated successfully!";
            return RedirectToAction("Index");
        }

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
                IsActive = subject.IsActive
            };

            return View("~/Views/Admin/SubjectManagement/Details.cshtml", model);
        }

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