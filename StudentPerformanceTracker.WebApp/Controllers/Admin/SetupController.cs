using Microsoft.AspNetCore.Mvc;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities;  // Already there
using StudentPerformanceTracker.Services.Authentication;
using AdminEntity = StudentPerformanceTracker.Data.Entities.Admin;  // ← ADD THIS LINE

namespace StudentPerformanceTracker.WebApp.Controllers.Admin
{
    [Route("Admin/Setup")]
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public SetupController(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            // Check if admin exists
            var existingAdmin = _context.Admins.FirstOrDefault(a => a.Username == "admin");
            
            if (existingAdmin != null)
            {
                return Content("Admin already exists!");
            }

            // Create admin - USE AdminEntity instead of Admin
            var admin = new AdminEntity 
            {
                Username = "admin",
                PasswordHash = _passwordService.HashPassword("Admin@123"),
                Email = "admin@studenttracker.com",
                FullName = "System Administrator",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Content("Admin created! Now login with: admin / Admin@123");
        }
    }
}