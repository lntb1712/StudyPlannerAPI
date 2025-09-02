using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Messaging
{
    public int MessageId { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public string? Content { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AccountManagement? Receiver { get; set; }

    public virtual AccountManagement? Sender { get; set; }
}
