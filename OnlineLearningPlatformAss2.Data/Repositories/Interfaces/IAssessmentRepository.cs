using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IAssessmentRepository
{
    Task<IEnumerable<AssessmentQuestion>> GetActiveQuestionsAsync();
    Task<IEnumerable<AssessmentOption>> GetOptionsByIdsAsync(IEnumerable<Guid> optionIds);
    Task<UserAssessment?> GetLatestUserAssessmentAsync(Guid userId);
    Task<UserAssessment?> GetAssessmentWithAnswersAsync(Guid assessmentId);
    Task<IEnumerable<Course>> GetCoursesByCategoriesAsync(IEnumerable<string> categories, int limit);
    Task<IEnumerable<Course>> GetTopCoursesAsync(int limit);
    Task AddAssessmentAsync(UserAssessment assessment);
    Task AddUserAnswerAsync(UserAnswer answer);
    Task<int> SaveChangesAsync();
}
