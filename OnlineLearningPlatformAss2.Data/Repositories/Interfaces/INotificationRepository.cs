using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid notificationId);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task<int> SaveChangesAsync();
}
