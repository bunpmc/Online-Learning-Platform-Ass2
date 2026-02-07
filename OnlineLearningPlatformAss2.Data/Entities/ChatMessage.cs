using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class ChatMessage
{
    public Guid MessageId { get; set; }

    public Guid SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsFromAdmin { get; set; }

    public DateTime SentAt { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
