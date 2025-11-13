using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.AdminManagement; 
using StudentPerformanceTracker.Services.Authentication;

namespace StudentPerformanceTracker.Services.Teachers
{
    public interface ITeacherService
    {
        Task<List<TeacherManagement>> GetAllTeachersAsync();  
        Task<TeacherManagement?> GetTeacherByIdAsync(int id);  
        Task<TeacherManagement> CreateTeacherAsync(TeacherManagement teacher, string password);  
        Task<TeacherManagement> UpdateTeacherAsync(TeacherManagement teacher);  
        Task<bool> DeleteTeacherAsync(int id);
        Task<bool> UsernameExistsAsync(string username, int? excludeTeacherId = null);
    }

    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public TeacherService(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<List<TeacherManagement>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();
        }

        public async Task<TeacherManagement?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }

        public async Task<TeacherManagement> CreateTeacherAsync(TeacherManagement teacher, string password)
        {
            teacher.PasswordHash = _passwordService.HashPassword(password);
            teacher.CreatedAt = DateTime.UtcNow;

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }

        public async Task<TeacherManagement> UpdateTeacherAsync(TeacherManagement teacher)
        {
            teacher.UpdatedAt = DateTime.UtcNow;
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeTeacherId = null)
        {
            var query = _context.Teachers.Where(t => t.Username == username);
            
            if (excludeTeacherId.HasValue)
            {
                query = query.Where(t => t.TeacherId != excludeTeacherId.Value);
            }

            return await query.AnyAsync();
        }
    }
}