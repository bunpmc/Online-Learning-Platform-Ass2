using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class DiscussionRepository(OnlineLearningSystemDbContext context) : IDiscussionRepository
{
    public async Task<IEnumerable<LessonComment>> GetLessonCommentsAsync(Guid lessonId)
    {
        return await context.LessonComments
            .AsNoTracking()
            .Include(c => c.User)
            .ThenInclude(u => u.Role)
            .Include(c => c.User)
            .ThenInclude(u => u.Profile)
            .Include(c => c.InverseParent)
            .ThenInclude(r => r.User)
            .ThenInclude(u => u.Role)
            .Where(c => c.LessonId == lessonId && c.ParentId == null)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<LessonComment?> GetCommentByIdAsync(Guid commentId)
    {
        return await context.LessonComments.FindAsync(commentId);
    }

    public async Task<LessonComment?> GetCommentWithUserAsync(Guid commentId)
    {
        return await context.LessonComments
            .Include(c => c.User)
            .ThenInclude(u => u.Role)
            .Include(c => c.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task<User?> GetUserWithRoleAsync(Guid userId)
    {
        return await context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddCommentAsync(LessonComment comment)
    {
        await context.LessonComments.AddAsync(comment);
    }

    public async Task RemoveCommentAsync(LessonComment comment)
    {
        context.LessonComments.Remove(comment);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
