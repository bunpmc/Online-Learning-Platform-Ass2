using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class UserAnswer
{
    public Guid AnswerId { get; set; }

    public Guid AssessmentId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid SelectedOptionId { get; set; }

    public virtual UserAssessment Assessment { get; set; } = null!;

    public virtual AssessmentQuestion Question { get; set; } = null!;

    public virtual AssessmentOption SelectedOption { get; set; } = null!;
}
