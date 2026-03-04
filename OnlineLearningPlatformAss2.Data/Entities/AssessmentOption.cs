using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class AssessmentOption
{
    public Guid OptionId { get; set; }

    public Guid QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public string? SkillLevel { get; set; }

    public int OrderIndex { get; set; }

    public string? Category { get; set; }

    public virtual AssessmentQuestion Question { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
