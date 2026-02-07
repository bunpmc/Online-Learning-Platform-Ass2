using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<Enrollment?> GetByIdWithDetailsAsync(Guid id);
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<IEnumerable<Enrollment>> GetStudentEnrollmentsDetailedAsync(Guid userId);
    Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
    Task AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    
    // Lesson Progress
    Task<LessonProgress?> GetLessonProgressAsync(Guid enrollmentId, Guid lessonId);
    Task AddLessonProgressAsync(LessonProgress progress);
    Task UpdateLessonProgressAsync(LessonProgress progress);
    
    Task<int> SaveChangesAsync();
}
