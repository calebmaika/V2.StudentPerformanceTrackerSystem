using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceTracker.WebApp.Controllers.Teacher
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    [Route("Teacher/[controller]")]
    public class DashboardController : Controller
    {
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            ViewData["FullName"] = User.FindFirst("FullName")?.Value ?? "Teacher";
            return View("~/Views/Teacher/Dashboard/Index.cshtml");
        }
    }
}