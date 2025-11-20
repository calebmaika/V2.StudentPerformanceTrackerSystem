using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudentPerformanceTracker.WebApp.Controllers.Teacher
{
    [Authorize(Roles = "Teacher")]
    [Route("Teacher/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Get teacher information from claims
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacherName = User.FindFirst("FullName")?.Value ?? "Teacher";
            
            ViewData["TeacherName"] = teacherName;
            ViewData["TeacherId"] = teacherId;
            
            return View("~/Views/Teacher/Dashboard/Index.cshtml");
        }
    }
}