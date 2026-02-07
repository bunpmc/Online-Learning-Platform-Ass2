using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface ILearningPathRepository
{
    Task<LearningPath?> GetByIdWithCoursesAsync(Guid id);
    Task<IEnumerable<LearningPath>> GetPublishedPathsAsync(int? limit = null);
    Task<IEnumerable<Guid>> GetUserEnrolledPathIdsAsync(Guid userId);
    Task<IEnumerable<UserLearningPathEnrollment>> GetUserEnrollmentsAsync(Guid userId);
    Task<UserLearningPathEnrollment?> GetUserEnrollmentAsync(Guid userId, Guid pathId);
    Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(Guid userId, IEnumerable<Guid> courseIds);
    Task<bool> IsEnrolledAsync(Guid userId, Guid pathId);
    Task<int> GetCourseLessonCountAsync(Guid courseId);
}
