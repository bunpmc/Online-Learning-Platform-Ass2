using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Wishlist
{
    public Guid WishlistId { get; set; }

    public Guid UserId { get; set; }

    public Guid CourseId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
