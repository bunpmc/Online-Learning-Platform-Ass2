using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Lesson
{
    public Guid LessonId { get; set; }

    public Guid ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? ContentUrl { get; set; }

    public int? Duration { get; set; }

    public int OrderIndex { get; set; }

    public string? Content { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<LessonComment> LessonComments { get; set; } = new List<LessonComment>();

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
