using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Order
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public Guid? CourseId { get; set; }

    public Guid? PathId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public byte[]? RowVersion { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual LearningPath? Path { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
