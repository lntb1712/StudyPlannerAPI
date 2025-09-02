using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public string? StudentId { get; set; }

    public string? ClassId { get; set; }

    public string? TeacherId { get; set; }

    public string? Subject { get; set; }

    public int? DayOfWeek { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Class? Class { get; set; }

    public virtual StatusMaster? Status { get; set; }

    public virtual AccountManagement? Student { get; set; }

    public virtual AccountManagement? Teacher { get; set; }
}
