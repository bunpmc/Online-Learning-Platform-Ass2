using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class PathCourse
{
    public Guid PathId { get; set; }

    public Guid CourseId { get; set; }

    public int OrderIndex { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual LearningPath Path { get; set; } = null!;
}
