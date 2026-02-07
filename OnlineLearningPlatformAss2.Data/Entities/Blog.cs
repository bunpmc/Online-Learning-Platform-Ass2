using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Blog
{
    public Guid BlogId { get; set; }

    public Guid AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
