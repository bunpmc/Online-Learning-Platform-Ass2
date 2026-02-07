using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class ReviewRepository(OnlineLearningSystemDbContext context) : IReviewRepository
{
    public async Task<bool> ExistsAsync(Guid userId, Guid courseId)
    {
        return await context.CourseReviews.AnyAsync(r => r.UserId == userId && r.CourseId == courseId);
    }

    public async Task<Enrollment?> GetCompletedEnrollmentAsync(Guid userId, Guid courseId)
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId && e.Status == "Completed");
    }

    public async Task<IEnumerable<CourseReview>> GetCourseReviewsAsync(Guid courseId)
    {
        return await context.CourseReviews
            .AsNoTracking()
            .Include(r => r.User)
            .ThenInclude(u => u.Profile)
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetCourseRatingsAsync(Guid courseId)
    {
        return await context.CourseReviews
            .Where(r => r.CourseId == courseId)
            .Select(r => r.Rating)
            .ToListAsync();
    }

    public async Task AddAsync(CourseReview review)
    {
        await context.CourseReviews.AddAsync(review);
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
