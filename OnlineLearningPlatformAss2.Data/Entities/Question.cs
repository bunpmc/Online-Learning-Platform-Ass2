using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Question
{
    public Guid QuestionId { get; set; }

    public Guid QuizId { get; set; }

    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual Quiz Quiz { get; set; } = null!;
}
