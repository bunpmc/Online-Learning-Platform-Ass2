using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class LearningPath
{
    public Guid PathId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public bool IsCustomPath { get; set; }

    public Guid? SourceAssessmentId { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PathCourse> PathCourses { get; set; } = new List<PathCourse>();

    public virtual ICollection<UserLearningPathEnrollment> UserLearningPathEnrollments { get; set; } = new List<UserLearningPathEnrollment>();
}
