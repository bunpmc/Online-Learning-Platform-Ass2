using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class QuizAttempt
{
    public Guid AttemptId { get; set; }

    public Guid UserId { get; set; }

    public Guid QuizId { get; set; }

    public double Score { get; set; }

    public bool Passed { get; set; }

    public DateTime AttemptedAt { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
