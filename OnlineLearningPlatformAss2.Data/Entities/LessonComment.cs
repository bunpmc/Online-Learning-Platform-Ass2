using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class LessonComment
{
    public Guid CommentId { get; set; }

    public Guid LessonId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid? ParentId { get; set; }

    public virtual ICollection<LessonComment> InverseParent { get; set; } = new List<LessonComment>();

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual LessonComment? Parent { get; set; }

    public virtual User User { get; set; } = null!;
}
