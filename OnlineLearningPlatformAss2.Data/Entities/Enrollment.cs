using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Enrollment
{
    public Guid EnrollmentId { get; set; }

    public Guid UserId { get; set; }

    public Guid CourseId { get; set; }

    public DateTime EnrolledAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CompletedAt { get; set; }

    public Guid? LastViewedLessonId { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual User User { get; set; } = null!;
}
