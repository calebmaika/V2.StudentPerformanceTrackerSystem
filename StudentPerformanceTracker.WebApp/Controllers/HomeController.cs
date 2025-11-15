using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.WebApp.Models;

namespace StudentPerformanceTracker.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            StatusCode = 500,
            Message = "An error occurred while processing your request."
        });
    }
}
