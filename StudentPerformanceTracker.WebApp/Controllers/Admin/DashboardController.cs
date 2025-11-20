using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    /// <summary>
    /// Admin dashboard controller
    /// Route: /Admin/Dashboard/...
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/Dashboard")]
    public class DashboardController : Controller
    {
        /// <summary>
        /// GET: /Admin/Dashboard or /Admin/Dashboard/Index
        /// Shows the admin dashboard
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Get the admin's full name from claims
            ViewData["FullName"] = User.FindFirst("FullName")?.Value ?? "Admin";
            
            // Show the dashboard view from Views/Admin/Dashboard/Index.cshtml
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}