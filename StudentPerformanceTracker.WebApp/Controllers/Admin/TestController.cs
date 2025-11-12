using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}