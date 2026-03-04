using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class QuizRepository(OnlineLearningSystemDbContext context) : IQuizRepository
{
    public async Task<Quiz?> GetQuizByLessonAsync(Guid lessonId)
    {
        return await context.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.LessonId == lessonId);
    }

    public async Task<Quiz?> GetQuizWithDetailsAsync(Guid quizId)
    {
        return await context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);
    }

    public async Task<bool> HasPassedQuizAsync(Guid userId, Guid quizId)
    {
        return await context.QuizAttempts.AnyAsync(a => a.UserId == userId && a.QuizId == quizId && a.Passed);
    }

    public async Task AddAttemptAsync(QuizAttempt attempt)
    {
        await context.QuizAttempts.AddAsync(attempt);
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
