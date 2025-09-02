using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class TaskManagement
{
    public int TaskId { get; set; }

    public string? StudentId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual AccountManagement? Student { get; set; }
}
