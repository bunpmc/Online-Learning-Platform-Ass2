using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class CourseRepository(OnlineLearningSystemDbContext context) : ICourseRepository
{
    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await context.Courses.FindAsync(id);
    }

    public async Task<Course?> GetByIdWithDetailsAsync(Guid id)
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .Include(c => c.CourseReviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.CourseId == id);
    }

    public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync(int limit)
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.CourseReviews)
            .Where(c => c.IsFeatured && c.Status == "Published")
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync(string? searchTerm = null, Guid? categoryId = null, int? limit = null)
    {
        var query = context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.CourseReviews)
            .Where(c => c.Status == "Published");

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Title.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)) ||
                c.Category.Name.ToLower().Contains(term) ||
                c.Instructor.Username.ToLower().Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetInstructorCoursesAsync(Guid instructorId)
    {
        return await context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.CourseReviews)
            .Where(c => c.InstructorId == instructorId)
            .ToListAsync();
    }

    public async Task<int> GetEnrollmentCountAsync(Guid courseId)
    {
        return await context.Enrollments.CountAsync(e => e.CourseId == courseId);
    }

    public async Task UpdateAsync(Course course)
    {
        context.Courses.Update(course);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

    // Wishlist
    public async Task<Wishlist?> GetWishlistItemAsync(Guid userId, Guid courseId)
    {
        return await context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId);
    }

    public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(Guid userId)
    {
        return await context.Wishlists
            .AsNoTracking()
            .Include(w => w.Course)
            .ThenInclude(c => c.Category)
            .Include(w => w.Course)
            .ThenInclude(c => c.Instructor)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();
    }

    public async Task AddWishlistAsync(Wishlist wishlist)
    {
        await context.Wishlists.AddAsync(wishlist);
    }

    public async Task RemoveWishlistAsync(Wishlist wishlist)
    {
        context.Wishlists.Remove(wishlist);
        await Task.CompletedTask;
    }

    public async Task<bool> IsInWishlistAsync(Guid userId, Guid courseId)
    {
        return await context.Wishlists.AnyAsync(w => w.UserId == userId && w.CourseId == courseId);
    }

    // Category
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await context.Categories.AsNoTracking().ToListAsync();
    }

    // Module
    public async Task<Module?> GetModuleByIdAsync(Guid moduleId)
    {
        return await context.Modules
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
    }

    public async Task AddModuleAsync(Module module)
    {
        await context.Modules.AddAsync(module);
    }

    public async Task UpdateModuleAsync(Module module)
    {
        context.Modules.Update(module);
        await Task.CompletedTask;
    }

    public async Task RemoveModuleAsync(Module module)
    {
        context.Modules.Remove(module);
        await Task.CompletedTask;
    }

    // Lesson
    public async Task<Lesson?> GetLessonByIdAsync(Guid lessonId)
    {
        return await context.Lessons
            .Include(l => l.Module)
            .ThenInclude(m => m.Course)
            .FirstOrDefaultAsync(l => l.LessonId == lessonId);
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        await context.Lessons.AddAsync(lesson);
    }

    public async Task UpdateLessonAsync(Lesson lesson)
    {
        context.Lessons.Update(lesson);
        await Task.CompletedTask;
    }

    public async Task RemoveLessonAsync(Lesson lesson)
    {
        context.Lessons.Remove(lesson);
        await Task.CompletedTask;
    }

    // Orders/Earnings
    public async Task<decimal> GetInstructorEarningsAsync(Guid instructorId)
    {
        return await context.Orders
            .Where(o => o.Course != null && o.Course.InstructorId == instructorId && o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);
    }

    // Certificate
    public async Task<Certificate?> GetCertificateByIdAsync(Guid certificateId)
    {
        return await context.Certificates
            .AsNoTracking()
            .Include(c => c.Enrollment)
            .ThenInclude(e => e.User)
            .Include(c => c.Enrollment)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.CertificateId == certificateId);
    }

    public async Task<bool> CertificateExistsAsync(Guid enrollmentId)
    {
        return await context.Certificates.AnyAsync(c => c.EnrollmentId == enrollmentId);
    }

    public async Task<Certificate?> GetCertificateByEnrollmentIdAsync(Guid enrollmentId)
    {
        return await context.Certificates
            .Include(c => c.Enrollment)
                .ThenInclude(e => e.User)
            .Include(c => c.Enrollment)
                .ThenInclude(e => e.Course)
                .ThenInclude(co => co.Instructor)
            .FirstOrDefaultAsync(c => c.EnrollmentId == enrollmentId);
    }

    public async Task AddCertificateAsync(Certificate certificate)
    {
        await context.Certificates.AddAsync(certificate);
    }
}
