using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Course
{
    public Guid CourseId { get; set; }

    public Guid InstructorId { get; set; }

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsFeatured { get; set; }

    public string Level { get; set; } = "Beginner";

    public string Language { get; set; } = "English";

    public string? RejectionReason { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual User Instructor { get; set; } = null!;

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PathCourse> PathCourses { get; set; } = new List<PathCourse>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
