using OnlineLearningPlatformAss2.Service.Services.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.LearningPath;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class LearningPathService(ILearningPathRepository learningPathRepository) : ILearningPathService
{
    public async Task<LearningPathViewModel?> GetLearningPathDetailsAsync(Guid id)
    {
        var path = await learningPathRepository.GetByIdWithCoursesAsync(id);
        if (path == null) return null;

        return new LearningPathViewModel
        {
            Id = path.PathId,
            Title = path.Title,
            Description = path.Description ?? string.Empty,
            Price = path.Price,
            Status = path.Status,
            IsCustomPath = path.IsCustomPath,
            CreatedAt = path.CreatedAt,
            Courses = path.PathCourses.OrderBy(pc => pc.OrderIndex).Select(pc => new CourseInPathViewModel
            {
                CourseId = pc.Course.CourseId,
                Title = pc.Course.Title,
                Description = pc.Course.Description ?? string.Empty,
                ImageUrl = pc.Course.ImageUrl,
                OrderIndex = pc.OrderIndex,
                InstructorName = pc.Course.Instructor.Username,
                CategoryName = pc.Course.Category.Name
            }).ToList()
        };
    }

    public async Task<IEnumerable<LearningPathViewModel>> GetFeaturedLearningPathsAsync(Guid? userId = null)
    {
        var paths = await learningPathRepository.GetPublishedPathsAsync(4);
        var enrolledPathIds = new HashSet<Guid>();
        
        if (userId.HasValue)
        {
            enrolledPathIds = new HashSet<Guid>(await learningPathRepository.GetUserEnrolledPathIdsAsync(userId.Value));
        }

        return paths.Select(lp => new LearningPathViewModel
        {
            Id = lp.PathId,
            Title = lp.Title,
            Description = lp.Description ?? string.Empty,
            Price = lp.Price,
            Status = lp.Status,
            IsCustomPath = lp.IsCustomPath,
            CourseCount = lp.PathCourses.Count,
            IsEnrolled = enrolledPathIds.Contains(lp.PathId),
            CreatedAt = lp.CreatedAt
        });
    }

    public async Task<IEnumerable<LearningPathViewModel>> GetPublishedPathsAsync(Guid? userId = null)
    {
        var paths = await learningPathRepository.GetPublishedPathsAsync();
        var enrolledPathIds = new HashSet<Guid>();
        
        if (userId.HasValue)
        {
            enrolledPathIds = new HashSet<Guid>(await learningPathRepository.GetUserEnrolledPathIdsAsync(userId.Value));
        }

        return paths.Select(lp => new LearningPathViewModel
        {
            Id = lp.PathId,
            Title = lp.Title,
            Description = lp.Description ?? string.Empty,
            Price = lp.Price,
            Status = lp.Status,
            IsCustomPath = lp.IsCustomPath,
            CourseCount = lp.PathCourses.Count,
            IsEnrolled = enrolledPathIds.Contains(lp.PathId),
            CreatedAt = lp.CreatedAt
        });
    }

    public async Task<IEnumerable<UserLearningPathWithProgressDto>> GetUserEnrolledPathsAsync(Guid userId)
    {
        var enrollments = await learningPathRepository.GetUserEnrollmentsAsync(userId);
        var result = new List<UserLearningPathWithProgressDto>();

        foreach (var ulpe in enrollments)
        {
            var courseIds = ulpe.Path.PathCourses.Select(pc => pc.CourseId).ToList();
            var courseEnrollments = await learningPathRepository.GetCourseEnrollmentsAsync(userId, courseIds);

            var completedCourses = courseEnrollments.Count(e => e.Status == "Completed");
            var totalCourses = courseIds.Count;
            var progress = totalCourses > 0 ? (decimal)completedCourses / totalCourses * 100 : 0;

            result.Add(new UserLearningPathWithProgressDto
            {
                Id = ulpe.EnrollmentId,
                PathId = ulpe.PathId,
                PathTitle = ulpe.Path.Title,
                PathDescription = ulpe.Path.Description ?? string.Empty,
                EnrolledAt = ulpe.EnrolledAt,
                Status = ulpe.Status,
                TotalCourses = totalCourses,
                CompletedCourses = completedCourses,
                Progress = progress
            });
        }

        return result;
    }

    public async Task<UserLearningPathWithProgressDto?> GetUserPathProgressAsync(Guid userId, Guid pathId)
    {
        var enrollment = await learningPathRepository.GetUserEnrollmentAsync(userId, pathId);
        if (enrollment == null) return null;

        var courseIds = enrollment.Path.PathCourses.Select(pc => pc.CourseId).ToList();
        var courseEnrollments = await learningPathRepository.GetCourseEnrollmentsAsync(userId, courseIds);

        var completedCourses = courseEnrollments.Count(e => e.Status == "Completed");
        var totalCourses = courseIds.Count;
        var progress = totalCourses > 0 ? (decimal)completedCourses / totalCourses * 100 : 0;

        return new UserLearningPathWithProgressDto
        {
            Id = enrollment.EnrollmentId,
            PathId = enrollment.PathId,
            PathTitle = enrollment.Path.Title,
            PathDescription = enrollment.Path.Description ?? string.Empty,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt,
            Status = enrollment.Status,
            TotalCourses = totalCourses,
            CompletedCourses = completedCourses,
            Progress = progress
        };
    }

    public async Task<LearningPathDetailsWithProgressDto?> GetPathDetailsWithProgressAsync(Guid pathId, Guid? userId = null)
    {
        var path = await learningPathRepository.GetByIdWithCoursesAsync(pathId);
        if (path == null) return null;

        bool isEnrolled = false;
        decimal totalProgress = 0m;

        var coursesWithProgress = new List<PathCourseWithProgressDto>();
        var pathCourses = path.PathCourses.OrderBy(pc => pc.OrderIndex).ToList();

        if (userId.HasValue)
        {
            isEnrolled = await learningPathRepository.IsEnrolledAsync(userId.Value, pathId);
            var courseIds = pathCourses.Select(pc => pc.CourseId).ToList();
            var enrollments = (await learningPathRepository.GetCourseEnrollmentsAsync(userId.Value, courseIds)).ToList();

            int completedCourses = 0;
            for (int i = 0; i < pathCourses.Count; i++)
            {
                var pc = pathCourses[i];
                var enrollment = enrollments.FirstOrDefault(e => e.CourseId == pc.CourseId);

                decimal courseProgress = 0m;
                bool isCompleted = false;

                if (enrollment != null)
                {
                    var totalLessons = await learningPathRepository.GetCourseLessonCountAsync(pc.CourseId);
                    var completedLessons = enrollment.LessonProgresses.Count(p => p.IsCompleted);
                    courseProgress = totalLessons > 0 ? (decimal)completedLessons / totalLessons * 100 : 0;
                    isCompleted = enrollment.Status == "Completed";
                    if (isCompleted) completedCourses++;
                }

                coursesWithProgress.Add(new PathCourseWithProgressDto
                {
                    CourseId = pc.Course.CourseId,
                    Title = pc.Course.Title,
                    Description = pc.Course.Description ?? string.Empty,
                    ImageUrl = pc.Course.ImageUrl,
                    OrderIndex = pc.OrderIndex,
                    InstructorName = pc.Course.Instructor.Username,
                    Duration = 180,
                    Level = pc.Course.Level ?? "All Levels",
                    IsCompleted = isCompleted,
                    IsCurrentCourse = isEnrolled && !isCompleted && (i == 0 || coursesWithProgress[i - 1].IsCompleted),
                    IsLocked = !isEnrolled,
                    Progress = courseProgress
                });
            }

            totalProgress = pathCourses.Count > 0 ? (decimal)completedCourses / pathCourses.Count * 100 : 0;
        }
        else
        {
            coursesWithProgress = pathCourses.Select(pc => new PathCourseWithProgressDto
            {
                CourseId = pc.Course.CourseId,
                Title = pc.Course.Title,
                Description = pc.Course.Description ?? string.Empty,
                ImageUrl = pc.Course.ImageUrl,
                OrderIndex = pc.OrderIndex,
                InstructorName = pc.Course.Instructor.Username,
                Duration = 180,
                Level = pc.Course.Level ?? "All Levels",
                IsCompleted = false,
                IsCurrentCourse = false,
                IsLocked = true,
                Progress = 0m
            }).ToList();
        }

        return new LearningPathDetailsWithProgressDto
        {
            Id = path.PathId,
            Title = path.Title,
            Description = path.Description ?? string.Empty,
            Price = path.Price,
            IsCustomPath = path.IsCustomPath,
            IsEnrolled = isEnrolled,
            Progress = totalProgress,
            WhatYouWillLearn = new List<string> { "Foundational concepts", "Advanced techniques", "Practical projects", "Industry best practices" },
            Prerequisites = new List<string> { "Basic computer literacy", "Interest in the subject" },
            Courses = coursesWithProgress
        };
    }
}
