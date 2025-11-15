using Microsoft.EntityFrameworkCore;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.AdminManagement;

namespace StudentPerformanceTracker.Services.Curriculum
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ApplicationDbContext _context;

        public CurriculumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CurriculumManagement>> GetAllCurriculumsAsync()
        {
            return await _context.Curriculums
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Teacher)
                .Include(c => c.CurriculumStudents)
                    .ThenInclude(cs => cs.Student)
                .OrderBy(c => c.CurriculumName)
                .ToListAsync();
        }

        public async Task<CurriculumManagement?> GetCurriculumByIdAsync(int id)
        {
            return await _context.Curriculums
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Teacher)
                .Include(c => c.CurriculumStudents)
                    .ThenInclude(cs => cs.Student)
                .FirstOrDefaultAsync(c => c.CurriculumId == id);
        }

        public async Task CreateCurriculumAsync(CurriculumManagement curriculum, List<CurriculumSubjectDto> subjects, List<int> studentIds)
        {
            _context.Curriculums.Add(curriculum);
            await _context.SaveChangesAsync();

            // Add subjects with teachers
            foreach (var subjectDto in subjects)
            {
                var curriculumSubject = new CurriculumSubject
                {
                    CurriculumId = curriculum.CurriculumId,
                    SubjectId = subjectDto.SubjectId,
                    TeacherId = subjectDto.TeacherId
                };
                _context.CurriculumSubjects.Add(curriculumSubject);
            }

            // Add students
            foreach (var studentId in studentIds)
            {
                var curriculumStudent = new CurriculumStudent
                {
                    CurriculumId = curriculum.CurriculumId,
                    StudentId = studentId
                };
                _context.CurriculumStudents.Add(curriculumStudent);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCurriculumAsync(CurriculumManagement curriculum, List<CurriculumSubjectDto> subjects, List<int> studentIds)
        {
            curriculum.UpdatedAt = DateTime.UtcNow;
            _context.Curriculums.Update(curriculum);

            // Remove old subjects
            var existingSubjects = await _context.CurriculumSubjects
                .Where(cs => cs.CurriculumId == curriculum.CurriculumId)
                .ToListAsync();
            _context.CurriculumSubjects.RemoveRange(existingSubjects);

            // Add new subjects
            foreach (var subjectDto in subjects)
            {
                var curriculumSubject = new CurriculumSubject
                {
                    CurriculumId = curriculum.CurriculumId,
                    SubjectId = subjectDto.SubjectId,
                    TeacherId = subjectDto.TeacherId
                };
                _context.CurriculumSubjects.Add(curriculumSubject);
            }

            // Remove old students
            var existingStudents = await _context.CurriculumStudents
                .Where(cs => cs.CurriculumId == curriculum.CurriculumId)
                .ToListAsync();
            _context.CurriculumStudents.RemoveRange(existingStudents);

            // Add new students
            foreach (var studentId in studentIds)
            {
                var curriculumStudent = new CurriculumStudent
                {
                    CurriculumId = curriculum.CurriculumId,
                    StudentId = studentId
                };
                _context.CurriculumStudents.Add(curriculumStudent);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCurriculumAsync(int id)
        {
            var curriculum = await _context.Curriculums.FindAsync(id);
            if (curriculum != null)
            {
                _context.Curriculums.Remove(curriculum);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CurriculumCodeExistsAsync(string code, int? excludeId = null)
        {
            return await _context.Curriculums
                .AnyAsync(c => c.CurriculumCode == code && (excludeId == null || c.CurriculumId != excludeId));
        }

        public async Task<List<CurriculumSubject>> GetCurriculumSubjectsAsync(int curriculumId)
        {
            return await _context.CurriculumSubjects
                .Include(cs => cs.Subject)
                .Include(cs => cs.Teacher)
                .Where(cs => cs.CurriculumId == curriculumId)
                .ToListAsync();
        }

        public async Task<List<int>> GetCurriculumStudentIdsAsync(int curriculumId)
        {
            return await _context.CurriculumStudents
                .Where(cs => cs.CurriculumId == curriculumId)
                .Select(cs => cs.StudentId)
                .ToListAsync();
        }

        public async Task<List<CurriculumManagement>> GetCurriculumsByTeacherIdAsync(int teacherId)
        {
            return await _context.Curriculums
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Teacher)
                .Include(c => c.CurriculumStudents)
                    .ThenInclude(cs => cs.Student)
                .Where(c => c.CurriculumSubjects.Any(cs => cs.TeacherId == teacherId))
                .OrderBy(c => c.CurriculumName)
                .ToListAsync();
        }
    }
}