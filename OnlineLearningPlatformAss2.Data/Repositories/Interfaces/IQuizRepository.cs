using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IQuizRepository
{
    Task<Quiz?> GetQuizByLessonAsync(Guid lessonId);
    Task<Quiz?> GetQuizWithDetailsAsync(Guid quizId);
    Task<bool> HasPassedQuizAsync(Guid userId, Guid quizId);
    Task AddAttemptAsync(QuizAttempt attempt);
    Task<int> SaveChangesAsync();
}
