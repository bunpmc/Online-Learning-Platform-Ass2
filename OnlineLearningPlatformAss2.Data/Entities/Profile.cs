using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Profile
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
