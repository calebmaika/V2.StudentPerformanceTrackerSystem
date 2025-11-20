using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using StudentPerformanceTracker.WebApp.Models.Teacher;
using StudentPerformanceTracker.Services.Authentication;

namespace StudentPerformanceTracker.WebApp.Controllers.Teacher
{
    [Area("Teacher")]
    [Route("Teacher/[controller]")]
    public class AccountController : Controller
    {
        private readonly ITeacherAuthenticationService _authService;

        public AccountController(ITeacherAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            // If teacher is already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Teacher"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });
            }

            return View("~/Views/Teacher/Login.cshtml");
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Teacher/Login.cshtml", model);
            }

            // Authenticate teacher
            var teacher = await _authService.AuthenticateTeacherAsync(model.Username, model.Password);

            if (teacher == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View("~/Views/Teacher/Login.cshtml", model);
            }

            // Create claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, teacher.TeacherId.ToString()),
                new Claim(ClaimTypes.Name, teacher.Username),
                new Claim("FullName", teacher.FullName),
                new Claim(ClaimTypes.Role, "Teacher")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe 
                    ? DateTimeOffset.UtcNow.AddDays(30) 
                    : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Update last login
            await _authService.UpdateLastLoginAsync(teacher.TeacherId);

            return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}