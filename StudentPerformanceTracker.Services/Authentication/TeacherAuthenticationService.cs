using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using Microsoft.EntityFrameworkCore;

namespace StudentPerformanceTracker.Services.Authentication
{
    public interface ITeacherAuthenticationService
    {
        Task<TeacherManagement?> AuthenticateTeacherAsync(string username, string password);
        Task UpdateLastLoginAsync(int teacherId);
    }

    public class TeacherAuthenticationService : ITeacherAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public TeacherAuthenticationService(
            ApplicationDbContext context, 
            IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<TeacherManagement?> AuthenticateTeacherAsync(string username, string password)
        {
            // Find teacher by username and check if active
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Username == username && t.IsActive);

            if (teacher == null)
            {
                return null;
            }

            // Verify password against stored hash
            bool isPasswordValid = _passwordService.VerifyPassword(password, teacher.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            return teacher;
        }

        public async Task UpdateLastLoginAsync(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            
            if (teacher != null)
            {
                // You can add a LastLoginAt field to TeacherManagement entity if you want to track this
                await _context.SaveChangesAsync();
            }
        }
    }
}