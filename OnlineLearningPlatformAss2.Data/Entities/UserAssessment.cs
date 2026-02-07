using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class UserAssessment
{
    public Guid AssessmentId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CompletedAt { get; set; }

    public string? ResultSummary { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
