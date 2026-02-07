using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId);
    Task<Order?> GetOrderDetailsAsync(Guid orderId);
    Task<Course?> GetCourseByIdAsync(Guid courseId);
    Task<LearningPath?> GetLearningPathByIdAsync(Guid pathId);
    Task<bool> HasPendingCourseOrderAsync(Guid userId, Guid courseId);
    Task<bool> HasPendingPathOrderAsync(Guid userId, Guid pathId);
    Task<bool> IsEnrolledInCourseAsync(Guid userId, Guid courseId);
    Task<bool> IsEnrolledInPathAsync(Guid userId, Guid pathId);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<Guid>> GetPathCourseIdsAsync(Guid pathId);
    Task AddOrderAsync(Order order);
    Task AddTransactionAsync(Transaction transaction);
    Task AddEnrollmentAsync(Enrollment enrollment);
    Task AddPathEnrollmentAsync(UserLearningPathEnrollment enrollment);
    Task<int> SaveChangesAsync();
}
