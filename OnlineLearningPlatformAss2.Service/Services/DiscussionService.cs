using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.Discussion;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class DiscussionService(IDiscussionRepository discussionRepository) : IDiscussionService
{
    public async Task<IEnumerable<CommentViewModel>> GetLessonCommentsAsync(Guid lessonId)
    {
        var comments = await discussionRepository.GetLessonCommentsAsync(lessonId);
        return comments.Select(c => MapToViewModel(c)).ToList();
    }

    public async Task<CommentViewModel> PostCommentAsync(Guid userId, CommentRequest request)
    {
        var comment = new LessonComment
        {
            CommentId = Guid.NewGuid(),
            LessonId = request.LessonId,
            UserId = userId,
            Content = request.Content,
            ParentId = request.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        await discussionRepository.AddCommentAsync(comment);
        await discussionRepository.SaveChangesAsync();

        var savedComment = await discussionRepository.GetCommentWithUserAsync(comment.CommentId);
        return MapToViewModel(savedComment!);
    }

    public async Task<bool> DeleteCommentAsync(Guid userId, Guid commentId)
    {
        var comment = await discussionRepository.GetCommentByIdAsync(commentId);
        if (comment == null) return false;

        if (comment.UserId != userId)
        {
            var user = await discussionRepository.GetUserWithRoleAsync(userId);
            if (user?.Role?.Name != "Admin") return false;
        }

        await discussionRepository.RemoveCommentAsync(comment);
        await discussionRepository.SaveChangesAsync();
        return true;
    }

    private CommentViewModel MapToViewModel(LessonComment comment)
    {
        return new CommentViewModel
        {
            Id = comment.CommentId,
            Content = comment.Content,
            Username = comment.User.Username,
            AvatarUrl = comment.User.Profile?.AvatarUrl,
            CreatedAt = comment.CreatedAt,
            IsInstructor = comment.User.Role?.Name == "Instructor" || comment.User.Role?.Name == "Admin",
            Replies = comment.InverseParent.Select(r => MapToViewModel(r)).ToList()
        };
    }
}
