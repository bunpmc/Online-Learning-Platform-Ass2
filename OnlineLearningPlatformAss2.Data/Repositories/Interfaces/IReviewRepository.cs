using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid courseId);
    Task<Enrollment?> GetCompletedEnrollmentAsync(Guid userId, Guid courseId);
    Task<IEnumerable<CourseReview>> GetCourseReviewsAsync(Guid courseId);
    Task<IEnumerable<int>> GetCourseRatingsAsync(Guid courseId);
    Task AddAsync(CourseReview review);
    Task<int> SaveChangesAsync();
}
