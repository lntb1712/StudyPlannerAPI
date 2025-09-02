using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class StatusMaster
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; } = new List<AssignmentDetail>();

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<TaskManagement> TaskManagements { get; set; } = new List<TaskManagement>();
}
