using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.AdminManagement;

namespace StudentPerformanceTracker.Services.Subject
{
    public interface ISubjectService
    {
        Task<List<SubjectManagement>> GetAllSubjectsAsync();
        Task<SubjectManagement?> GetSubjectByIdAsync(int id);
        Task<SubjectManagement> CreateSubjectAsync(SubjectManagement subject);
        Task<SubjectManagement> UpdateSubjectAsync(SubjectManagement subject);
        Task<bool> DeleteSubjectAsync(int id);
        Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null);
    }

    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;

        public SubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectManagement>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.SubjectCode)
                .ToListAsync();
        }

        public async Task<SubjectManagement?> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<SubjectManagement> CreateSubjectAsync(SubjectManagement subject)
        {
            subject.CreatedAt = DateTime.UtcNow;
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<SubjectManagement> UpdateSubjectAsync(SubjectManagement subject)
        {
            subject.UpdatedAt = DateTime.UtcNow;
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null)
        {
            var query = _context.Subjects.Where(s => s.SubjectCode == subjectCode);
            
            if (excludeSubjectId.HasValue)
            {
                query = query.Where(s => s.SubjectId != excludeSubjectId.Value);
            }

            return await query.AnyAsync();
        }
    }
}