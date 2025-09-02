using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? UserName { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Type { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AccountManagement? UserNameNavigation { get; set; }
}
