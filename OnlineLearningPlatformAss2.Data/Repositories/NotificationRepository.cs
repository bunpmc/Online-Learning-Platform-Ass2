using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class NotificationRepository(OnlineLearningSystemDbContext context) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(Guid notificationId)
    {
        return await context.Notifications.FindAsync(notificationId);
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool onlyUnread = false)
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        if (onlyUnread)
        {
            return await query.Where(n => !n.IsRead).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task AddAsync(Notification notification)
    {
        await context.Notifications.AddAsync(notification);
    }

    public Task UpdateAsync(Notification notification)
    {
        context.Notifications.Update(notification);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
