using StudentPerformanceTracker.Data.Entities.AdminManagement;

namespace StudentPerformanceTracker.Services.Curriculum
{
    public interface ICurriculumService
    {
        Task<List<CurriculumManagement>> GetAllCurriculumsAsync();
        Task<CurriculumManagement?> GetCurriculumByIdAsync(int id);
        Task CreateCurriculumAsync(CurriculumManagement curriculum, List<CurriculumSubjectDto> subjects, List<int> studentIds);
        Task UpdateCurriculumAsync(CurriculumManagement curriculum, List<CurriculumSubjectDto> subjects, List<int> studentIds);
        Task DeleteCurriculumAsync(int id);
        Task<bool> CurriculumCodeExistsAsync(string code, int? excludeId = null);
        Task<List<CurriculumSubject>> GetCurriculumSubjectsAsync(int curriculumId);
        Task<List<int>> GetCurriculumStudentIdsAsync(int curriculumId);
    }

    public class CurriculumSubjectDto
    {
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
    }
}