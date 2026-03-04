using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.Review;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    public async Task<bool> AddReviewAsync(Guid userId, ReviewRequest request)
    {
        bool exists = await reviewRepository.ExistsAsync(userId, request.CourseId);
        if (exists) return false;

        var enrollment = await reviewRepository.GetCompletedEnrollmentAsync(userId, request.CourseId);
        if (enrollment == null) return false;

        if (enrollment.Course.InstructorId == userId) return false;

        var review = new CourseReview
        {
            ReviewId = Guid.NewGuid(),
            CourseId = request.CourseId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await reviewRepository.AddAsync(review);
        await reviewRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReviewViewModel>> GetCourseReviewsAsync(Guid courseId)
    {
        var reviews = await reviewRepository.GetCourseReviewsAsync(courseId);
        return reviews.Select(r => new ReviewViewModel
        {
            Id = r.ReviewId,
            Username = r.User.Username,
            AvatarUrl = r.User.Profile?.AvatarUrl,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<CourseRatingSummary> GetRatingSummaryAsync(Guid courseId)
    {
        var ratings = (await reviewRepository.GetCourseRatingsAsync(courseId)).ToList();

        if (!ratings.Any())
        {
            return new CourseRatingSummary { AverageRating = 0, TotalReviews = 0 };
        }

        return new CourseRatingSummary
        {
            AverageRating = Math.Round(ratings.Average(), 1),
            TotalReviews = ratings.Count
        };
    }

    public async Task<bool> HasUserReviewedAsync(Guid userId, Guid courseId)
    {
        return await reviewRepository.ExistsAsync(userId, courseId);
    }
}
