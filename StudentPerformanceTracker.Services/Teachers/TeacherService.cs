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
        Task<TeacherManagement> CreateTeacherAsync(TeacherManagement teacher, string password, List<int> subjectIds);  
        Task<TeacherManagement> UpdateTeacherAsync(TeacherManagement teacher, List<int> subjectIds);  
        Task<bool> DeleteTeacherAsync(int id);
        Task<bool> UsernameExistsAsync(string username, int? excludeTeacherId = null);
        Task<List<int>> GetTeacherSubjectIdsAsync(int teacherId);
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
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();
        }

        public async Task<TeacherManagement?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TeacherId == id);
        }

        public async Task<TeacherManagement> CreateTeacherAsync(TeacherManagement teacher, string password, List<int> subjectIds)
        {
            teacher.PasswordHash = _passwordService.HashPassword(password);
            teacher.CreatedAt = DateTime.UtcNow;

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            // Assign subjects
            if (subjectIds != null && subjectIds.Any())
            {
                foreach (var subjectId in subjectIds)
                {
                    var teacherSubject = new TeacherSubject
                    {
                        TeacherId = teacher.TeacherId,
                        SubjectId = subjectId,
                        AssignedAt = DateTime.UtcNow
                    };
                    _context.TeacherSubjects.Add(teacherSubject);
                }
                await _context.SaveChangesAsync();
            }

            return teacher;
        }

        public async Task<TeacherManagement> UpdateTeacherAsync(TeacherManagement teacher, List<int> subjectIds)
        {
            teacher.UpdatedAt = DateTime.UtcNow;
            _context.Teachers.Update(teacher);

            // Remove existing subject assignments
            var existingAssignments = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacher.TeacherId)
                .ToListAsync();
            _context.TeacherSubjects.RemoveRange(existingAssignments);

            // Add new subject assignments
            if (subjectIds != null && subjectIds.Any())
            {
                foreach (var subjectId in subjectIds)
                {
                    var teacherSubject = new TeacherSubject
                    {
                        TeacherId = teacher.TeacherId,
                        SubjectId = subjectId,
                        AssignedAt = DateTime.UtcNow
                    };
                    _context.TeacherSubjects.Add(teacherSubject);
                }
            }

            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return false;

            // Remove subject assignments (will be done automatically by cascade delete)
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

        public async Task<List<int>> GetTeacherSubjectIdsAsync(int teacherId)
        {
            return await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .Select(ts => ts.SubjectId)
                .ToListAsync();
        }
    }
}