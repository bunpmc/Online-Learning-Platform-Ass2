using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class AssessmentQuestion
{
    public Guid QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public int OrderIndex { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AssessmentOption> AssessmentOptions { get; set; } = new List<AssessmentOption>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
