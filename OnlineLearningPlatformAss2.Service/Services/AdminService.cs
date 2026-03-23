using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.Admin;
using OnlineLearningPlatformAss2.Service.DTOs.Course;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;
using BCrypt.Net;

namespace OnlineLearningPlatformAss2.Service.Services;

public class AdminService(
    IAdminRepository adminRepository,
    IUserRepository userRepository,
    ICourseRepository courseRepository,
    INotificationService notificationService,
    ICourseUpdateBroadcaster? broadcaster = null) : IAdminService
{
    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var stats = new AdminStatsDto
        {
            TotalUsers = await adminRepository.GetUserCountAsync(),
            TotalInstructors = await adminRepository.GetInstructorCountAsync(),
            TotalCourses = await adminRepository.GetCourseCountAsync(),
            PendingCourses = await adminRepository.GetPendingCourseCountAsync(),
            TotalEnrollments = await adminRepository.GetEnrollmentCountAsync(),
            TotalRevenue = await adminRepository.GetTotalRevenueAsync()
        };
        stats.TotalNetProfit = stats.TotalRevenue * 0.30m;

        var recentOrders = await adminRepository.GetRecentOrdersAsync(5);
        stats.RecentOrders = recentOrders.Select(o => new RecentOrderDto
        {
            OrderId = o.OrderId,
            Username = o.User.Username,
            ItemTitle = o.Course != null ? o.Course.Title : (o.Path != null ? o.Path.Title : "Bundle"),
            Amount = o.TotalAmount,
            CreatedAt = o.CreatedAt
        }).ToList();

        // Monthly chart data (last 12 months)
        var monthlyEnrollments = await adminRepository.GetMonthlyEnrollmentsAsync(12);
        var monthlyRevenue = await adminRepository.GetMonthlyRevenueAsync(12);

        var enrollmentDict = monthlyEnrollments.ToDictionary(x => $"{x.Year}-{x.Month:D2}", x => x.Count);
        var revenueDict = monthlyRevenue.ToDictionary(x => $"{x.Year}-{x.Month:D2}", x => x.Total);

        var chartData = new List<MonthlyChartDataDto>();
        for (int i = 11; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var key = $"{date.Year}-{date.Month:D2}";
            chartData.Add(new MonthlyChartDataDto
            {
                Month = date.ToString("MMM yyyy"),
                EnrollmentCount = enrollmentDict.GetValueOrDefault(key, 0),
                Revenue = revenueDict.GetValueOrDefault(key, 0m)
            });
        }
        stats.MonthlyChartData = chartData;

        return stats;
    }

    public async Task<IEnumerable<CourseViewModel>> GetPendingCoursesAsync()
    {
        var courses = await adminRepository.GetPendingCoursesAsync();
        return courses.Select(c => new CourseViewModel
        {
            Id = c.CourseId,
            Title = c.Title,
            Price = c.Price,
            CategoryName = c.Category.Name,
            InstructorName = c.Instructor.Username,
            ImageUrl = c.ImageUrl,
            Status = c.Status,
            RejectionReason = c.RejectionReason
        });
    }

    public async Task<bool> ApproveCourseAsync(Guid courseId)
    {
        var course = await adminRepository.GetCourseByIdAsync(courseId);
        if (course == null) return false;

        course.Status = "Published";
        course.RejectionReason = null;
        await adminRepository.UpdateCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        await notificationService.SendNotificationAsync(
            course.InstructorId,
            $"Congratulations! Your course '{course.Title}' has been approved and is now live.",
            "Approval"
        );

        if (broadcaster != null)
        {
            var courseVm = await GetCourseViewModelAsync(courseId);
            if (courseVm != null) await broadcaster.BroadcastCourseUpdatedAsync(courseVm);
        }

        return true;
    }

    public async Task<bool> RejectCourseAsync(Guid courseId, string reason)
    {
        var course = await adminRepository.GetCourseByIdAsync(courseId);
        if (course == null) return false;

        course.Status = "Rejected";
        course.RejectionReason = reason;
        await adminRepository.UpdateCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        await notificationService.SendNotificationAsync(
            course.InstructorId,
            $"Action Required: Your course submission '{course.Title}' was rejected. Reason: {reason}",
            "Rejection"
        );

        if (broadcaster != null)
        {
            var courseVm = await GetCourseViewModelAsync(courseId);
            if (courseVm != null) await broadcaster.BroadcastCourseUpdatedAsync(courseVm);
        }

        return true;
    }

    public async Task<bool> SuspendCourseAsync(Guid courseId)
    {
        var course = await adminRepository.GetCourseByIdAsync(courseId);
        if (course == null) return false;

        course.Status = "Suspended";
        await adminRepository.UpdateCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        if (broadcaster != null)
        {
            var courseVm = await GetCourseViewModelAsync(courseId);
            if (courseVm != null) await broadcaster.BroadcastCourseUpdatedAsync(courseVm);
        }

        return true;
    }

    public async Task<bool> UnsuspendCourseAsync(Guid courseId)
    {
        var course = await adminRepository.GetCourseByIdAsync(courseId);
        if (course == null) return false;

        course.Status = "Published";
        await adminRepository.UpdateCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        if (broadcaster != null)
        {
            var courseVm = await GetCourseViewModelAsync(courseId);
            if (courseVm != null) await broadcaster.BroadcastCourseUpdatedAsync(courseVm);
        }

        return true;
    }

    public async Task<IEnumerable<CourseViewModel>> GetAllCoursesAsync()
    {
        var courses = await adminRepository.GetAllCoursesAsync();
        var viewModels = new List<CourseViewModel>();
        
        foreach (var c in courses)
        {
            viewModels.Add(new CourseViewModel
            {
                Id = c.CourseId,
                Title = c.Title,
                Price = c.Price,
                CategoryName = c.Category.Name,
                InstructorName = c.Instructor.Username,
                ImageUrl = c.ImageUrl,
                Status = c.Status,
                StudentCount = await courseRepository.GetEnrollmentCountAsync(c.CourseId)
            });
        }
        
        return viewModels;
    }

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(string? searchTerm = null)
    {
        var users = await userRepository.SearchUsersAsync(searchTerm);
        return users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            RoleName = u.Role?.Name ?? "User",
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<bool> ToggleUserStatusAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeUserRoleAsync(Guid userId, string roleName)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var role = await userRepository.GetRoleByNameAsync(roleName);
        if (role == null) return false;

        user.RoleId = role.Id;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await userRepository.GetUserWithRoleAsync(userId);
        if (user == null || user.Username == "admin") return false;

        if (user.Role?.Name == "Instructor")
        {
            var hasStudents = await adminRepository.HasActiveStudentsAsync(userId);
            if (hasStudents) return false;
        }

        await userRepository.RemoveAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddInternalUserAsync(string username, string email, string password, string roleName)
    {
        var role = await userRepository.GetRoleByNameAsync(roleName);
        if (role == null) return false;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }
    // Course CRUD
    public async Task<(IEnumerable<CategoryDto> Categories, IEnumerable<InstructorDto> Instructors)> GetFormDataAsync()
    {
        var categories = (await adminRepository.GetAllCategoriesAsync())
            .Select(c => new CategoryDto { Id = c.CategoryId, Name = c.Name });
        var instructors = (await adminRepository.GetInstructorsAsync())
            .Select(u => new InstructorDto { Id = u.Id, Username = u.Username });
        return (categories, instructors);
    }

    public async Task<CourseViewModel?> CreateCourseAsync(AdminCreateCourseDto dto)
    {
        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            InstructorId = dto.InstructorId,
            ImageUrl = dto.ImageUrl,
            Level = dto.Level,
            Language = dto.Language,
            Status = "Published",
            CreatedAt = DateTime.UtcNow,
            IsFeatured = false
        };

        await adminRepository.AddCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        var courseVm = await GetCourseViewModelAsync(course.CourseId);
        if (courseVm != null && broadcaster != null)
        {
            await broadcaster.BroadcastCourseAddedAsync(courseVm);
        }
        return courseVm;
    }

    public async Task<CourseViewModel?> UpdateCourseAsync(AdminUpdateCourseDto dto)
    {
        var course = await adminRepository.GetCourseByIdAsync(dto.CourseId);
        if (course == null) return null;

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;
        course.CategoryId = dto.CategoryId;
        course.ImageUrl = dto.ImageUrl;
        course.Level = dto.Level;
        course.Language = dto.Language;
        course.Status = dto.Status;

        await adminRepository.UpdateCourseAsync(course);
        await adminRepository.SaveChangesAsync();

        var courseVm = await GetCourseViewModelAsync(course.CourseId);
        if (courseVm != null && broadcaster != null)
        {
            await broadcaster.BroadcastCourseUpdatedAsync(courseVm);
        }
        return courseVm;
    }

    public async Task<bool> DeleteCourseAsync(Guid courseId)
    {
        var courseVm = await GetCourseViewModelAsync(courseId);
        if (courseVm == null) return false;

        await adminRepository.DeleteCourseAsync(courseId);
        await adminRepository.SaveChangesAsync();

        if (broadcaster != null)
        {
            await broadcaster.BroadcastCourseDeletedAsync(courseId);
        }
        return true;
    }

    private async Task<CourseViewModel?> GetCourseViewModelAsync(Guid courseId)
    {
        var course = await adminRepository.GetCourseWithDetailsAsync(courseId);
        if (course == null) return null;
        
        return new CourseViewModel
        {
            Id = course.CourseId,
            Title = course.Title,
            Description = course.Description ?? "",
            Price = course.Price,
            CategoryName = course.Category.Name,
            InstructorName = course.Instructor.Username,
            ImageUrl = course.ImageUrl,
            Status = course.Status,
            Language = course.Language,
            RejectionReason = course.RejectionReason,
            StudentCount = await courseRepository.GetEnrollmentCountAsync(course.CourseId)
        };
    }
}
