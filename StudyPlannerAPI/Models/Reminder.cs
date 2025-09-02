using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Reminder
{
    public int ReminderId { get; set; }

    public string? ParentId { get; set; }

    public string? StudentId { get; set; }

    public string? Content { get; set; }

    public DateTime? DueDate { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AccountManagement? Parent { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual AccountManagement? Student { get; set; }
}
