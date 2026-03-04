using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class AssessmentRepository(OnlineLearningSystemDbContext context) : IAssessmentRepository
{
    public async Task<IEnumerable<AssessmentQuestion>> GetActiveQuestionsAsync()
    {
        return await context.AssessmentQuestions
            .AsNoTracking()
            .Include(q => q.AssessmentOptions)
            .Where(q => q.IsActive)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssessmentOption>> GetOptionsByIdsAsync(IEnumerable<Guid> optionIds)
    {
        return await context.AssessmentOptions
            .Where(o => optionIds.Contains(o.OptionId))
            .ToListAsync();
    }

    public async Task<UserAssessment?> GetLatestUserAssessmentAsync(Guid userId)
    {
        return await context.UserAssessments
            .Include(a => a.UserAnswers)
            .ThenInclude(a => a.SelectedOption)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<UserAssessment?> GetAssessmentWithAnswersAsync(Guid assessmentId)
    {
        return await context.UserAssessments
            .Include(a => a.UserAnswers)
            .ThenInclude(a => a.SelectedOption)
            .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);
    }

    public async Task<IEnumerable<Course>> GetCoursesByCategoriesAsync(IEnumerable<string> categories, int limit)
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => categories.Contains(c.Category.Name))
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetTopCoursesAsync(int limit)
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAssessmentAsync(UserAssessment assessment)
    {
        await context.UserAssessments.AddAsync(assessment);
    }

    public async Task AddUserAnswerAsync(UserAnswer answer)
    {
        await context.UserAnswers.AddAsync(answer);
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
