using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class AccountManagement
{
    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public string? GroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ParentEmail { get; set; }

    public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; } = new List<AssignmentDetail>();

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual GroupManagement? Group { get; set; }

    public virtual ICollection<Messaging> MessagingReceivers { get; set; } = new List<Messaging>();

    public virtual ICollection<Messaging> MessagingSenders { get; set; } = new List<Messaging>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Reminder> ReminderParents { get; set; } = new List<Reminder>();

    public virtual ICollection<Reminder> ReminderStudents { get; set; } = new List<Reminder>();

    public virtual ICollection<Schedule> ScheduleStudents { get; set; } = new List<Schedule>();

    public virtual ICollection<Schedule> ScheduleTeachers { get; set; } = new List<Schedule>();

    public virtual StudentClass? StudentClass { get; set; }

    public virtual ICollection<TaskManagement> TaskManagements { get; set; } = new List<TaskManagement>();

    public virtual ICollection<TeacherClass> TeacherClasses { get; set; } = new List<TeacherClass>();
}
