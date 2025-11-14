using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.AdminManagement;

namespace StudentPerformanceTracker.Services.Students
{
    public interface IStudentService
    {
        Task<List<StudentManagement>> GetAllStudentsAsync();
        Task<StudentManagement?> GetStudentByIdAsync(int id);
        Task<StudentManagement> CreateStudentAsync(StudentManagement student);
        Task<StudentManagement> UpdateStudentAsync(StudentManagement student);
        Task<bool> DeleteStudentAsync(int id);
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentManagement>> GetAllStudentsAsync()
        {
            return await _context.Students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<StudentManagement?> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<StudentManagement> CreateStudentAsync(StudentManagement student)
        {
            student.CreatedAt = DateTime.UtcNow;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<StudentManagement> UpdateStudentAsync(StudentManagement student)
        {
            student.UpdatedAt = DateTime.UtcNow;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}