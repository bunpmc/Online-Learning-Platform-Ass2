using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class UserLearningPathEnrollment
{
    public Guid EnrollmentId { get; set; }

    public Guid UserId { get; set; }

    public Guid PathId { get; set; }

    public DateTime EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = null!;

    public int ProgressPercentage { get; set; }

    public virtual LearningPath Path { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
