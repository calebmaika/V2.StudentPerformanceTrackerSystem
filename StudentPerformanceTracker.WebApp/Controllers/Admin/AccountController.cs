using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using StudentPerformanceTracker.WebApp.Models.Admin;
using StudentPerformanceTracker.Services.Authentication;

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    /// <summary>
    /// Handles admin login and logout functionality
    /// Route: /Admin/Account/...
    /// </summary>
    [Route("Admin/Account")]
    public class AccountController : Controller
    {
        private readonly IAdminAuthenticationService _authService;

        public AccountController(IAdminAuthenticationService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// GET: /Admin/Account/Login
        /// Shows the admin login page
        /// </summary>
        [HttpGet("Login")]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            // Show the login page from Views/Admin/Login.cshtml
            return View("~/Views/Admin/Login.cshtml");
        }

        /// <summary>
        /// POST: /Admin/Account/Login
        /// Processes the admin login form
        /// </summary>
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Login.cshtml", model);
            }

            // HARDCODED CREDENTIALS
            if (model.Username != "admin" || model.Password != "Admin@123")
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View("~/Views/Admin/Login.cshtml", model);
            }

            // Create claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "admin@studenttracker.com"),
                new Claim("FullName", "System Administrator"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Configure authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe 
                    ? DateTimeOffset.UtcNow.AddDays(30) 
                    : DateTimeOffset.UtcNow.AddHours(2)
            };

            // Sign in the admin
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Redirect to admin dashboard
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// POST: /Admin/Account/Logout
        /// Logs out the admin
        /// </summary>
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}