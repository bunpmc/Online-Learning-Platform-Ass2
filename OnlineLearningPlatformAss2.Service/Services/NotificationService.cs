using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task SendNotificationAsync(Guid userId, string message, string type = "General")
    {
        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = userId,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false)
    {
        return await _notificationRepository.GetUserNotificationsAsync(userId, onlyUnread);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
        await _notificationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId);
    }
}

