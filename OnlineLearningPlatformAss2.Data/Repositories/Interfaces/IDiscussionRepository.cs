using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IDiscussionRepository
{
    Task<IEnumerable<LessonComment>> GetLessonCommentsAsync(Guid lessonId);
    Task<LessonComment?> GetCommentByIdAsync(Guid commentId);
    Task<LessonComment?> GetCommentWithUserAsync(Guid commentId);
    Task<User?> GetUserWithRoleAsync(Guid userId);
    Task AddCommentAsync(LessonComment comment);
    Task RemoveCommentAsync(LessonComment comment);
    Task<int> SaveChangesAsync();
}
