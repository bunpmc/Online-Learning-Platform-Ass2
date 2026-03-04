using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class EnrollmentRepository(OnlineLearningSystemDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await context.Enrollments.FindAsync(id);
    }

    public async Task<Enrollment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .ThenInclude(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .Include(e => e.LessonProgresses)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);
    }

    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.LessonProgresses)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsDetailedAsync(Guid userId)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .ThenInclude(c => c.Category)
            .Include(e => e.Course.Instructor)
            .Include(e => e.Course.Modules)
            .ThenInclude(m => m.Lessons)
            .Include(e => e.LessonProgresses)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }

    public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId)
    {
        return await context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await context.Enrollments.AddAsync(enrollment);
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        context.Enrollments.Update(enrollment);
        await Task.CompletedTask;
    }

    // Lesson Progress
    public async Task<LessonProgress?> GetLessonProgressAsync(Guid enrollmentId, Guid lessonId)
    {
        return await context.LessonProgresses
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId);
    }

    public async Task AddLessonProgressAsync(LessonProgress progress)
    {
        await context.LessonProgresses.AddAsync(progress);
    }

    public async Task UpdateLessonProgressAsync(LessonProgress progress)
    {
        context.LessonProgresses.Update(progress);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
