namespace OnlineLearningPlatformAss2.Service.DTOs.Admin;

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalCourses { get; set; }
    public int PendingCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalNetProfit { get; set; }
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<MonthlyChartDataDto> MonthlyChartData { get; set; } = new();
}

public class MonthlyChartDataDto
{
    public string Month { get; set; } = null!;
    public int EnrollmentCount { get; set; }
    public decimal Revenue { get; set; }
}

public class RecentOrderDto
{
    public Guid OrderId { get; set; }
    public string Username { get; set; } = null!;
    public string ItemTitle { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminCreateCourseDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Guid InstructorId { get; set; }
    public string? ImageUrl { get; set; }
    public string Level { get; set; } = "Beginner";
    public string Language { get; set; } = "English";
}

public class AdminUpdateCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public string Level { get; set; } = "Beginner";
    public string Language { get; set; } = "English";
    public string Status { get; set; } = "Published";
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class InstructorDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
}
