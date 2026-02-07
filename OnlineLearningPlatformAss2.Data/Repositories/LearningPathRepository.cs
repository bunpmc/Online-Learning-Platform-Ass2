using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class LearningPathRepository(OnlineLearningSystemDbContext context) : ILearningPathRepository
{
    public async Task<LearningPath?> GetByIdWithCoursesAsync(Guid id)
    {
        return await context.LearningPaths
            .AsNoTracking()
            .Include(lp => lp.PathCourses)
            .ThenInclude(pc => pc.Course)
            .ThenInclude(c => c.Category)
            .Include(lp => lp.PathCourses)
            .ThenInclude(pc => pc.Course.Instructor)
            .FirstOrDefaultAsync(lp => lp.PathId == id);
    }

    public async Task<IEnumerable<LearningPath>> GetPublishedPathsAsync(int? limit = null)
    {
        var query = context.LearningPaths
            .AsNoTracking()
            .Include(lp => lp.PathCourses)
            .Where(lp => lp.Status == "Published")
            .OrderByDescending(lp => lp.CreatedAt);

        if (limit.HasValue)
            return await query.Take(limit.Value).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserEnrolledPathIdsAsync(Guid userId)
    {
        return await context.UserLearningPathEnrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.PathId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserLearningPathEnrollment>> GetUserEnrollmentsAsync(Guid userId)
    {
        return await context.UserLearningPathEnrollments
            .AsNoTracking()
            .Include(ulpe => ulpe.Path)
            .ThenInclude(lp => lp.PathCourses)
            .Where(ulpe => ulpe.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserLearningPathEnrollment?> GetUserEnrollmentAsync(Guid userId, Guid pathId)
    {
        return await context.UserLearningPathEnrollments
            .Include(ulpe => ulpe.Path)
            .ThenInclude(lp => lp.PathCourses)
            .FirstOrDefaultAsync(ulpe => ulpe.UserId == userId && ulpe.PathId == pathId);
    }

    public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(Guid userId, IEnumerable<Guid> courseIds)
    {
        return await context.Enrollments
            .Include(e => e.LessonProgresses)
            .Where(e => e.UserId == userId && courseIds.Contains(e.CourseId))
            .ToListAsync();
    }

    public async Task<bool> IsEnrolledAsync(Guid userId, Guid pathId)
    {
        return await context.UserLearningPathEnrollments
            .AnyAsync(ulpe => ulpe.UserId == userId && ulpe.PathId == pathId);
    }

    public async Task<int> GetCourseLessonCountAsync(Guid courseId)
    {
        return await context.Lessons
            .CountAsync(l => context.Modules.Any(m => m.CourseId == courseId && m.ModuleId == l.ModuleId));
    }
}
