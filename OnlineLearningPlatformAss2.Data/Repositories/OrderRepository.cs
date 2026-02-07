using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class OrderRepository(OnlineLearningSystemDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderDetailsAsync(Guid orderId)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Transactions)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Course?> GetCourseByIdAsync(Guid courseId)
    {
        return await context.Courses.FindAsync(courseId);
    }

    public async Task<LearningPath?> GetLearningPathByIdAsync(Guid pathId)
    {
        return await context.LearningPaths.FindAsync(pathId);
    }

    public async Task<bool> HasPendingCourseOrderAsync(Guid userId, Guid courseId)
    {
        return await context.Orders.AnyAsync(o => o.UserId == userId && o.CourseId == courseId && o.Status == "Pending");
    }

    public async Task<bool> HasPendingPathOrderAsync(Guid userId, Guid pathId)
    {
        return await context.Orders.AnyAsync(o => o.UserId == userId && o.PathId == pathId && o.Status == "Pending");
    }

    public async Task<bool> IsEnrolledInCourseAsync(Guid userId, Guid courseId)
    {
        return await context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<bool> IsEnrolledInPathAsync(Guid userId, Guid pathId)
    {
        return await context.UserLearningPathEnrollments.AnyAsync(e => e.UserId == userId && e.PathId == pathId);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await context.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<Guid>> GetPathCourseIdsAsync(Guid pathId)
    {
        return await context.PathCourses
            .Where(pc => pc.PathId == pathId)
            .Select(pc => pc.CourseId)
            .ToListAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await context.Enrollments.AddAsync(enrollment);
    }

    public async Task AddPathEnrollmentAsync(UserLearningPathEnrollment enrollment)
    {
        await context.UserLearningPathEnrollments.AddAsync(enrollment);
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
