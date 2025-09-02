using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Class
{
    public string ClassId { get; set; } = null!;

    public string? ClassName { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

    public virtual ICollection<TeacherClass> TeacherClasses { get; set; } = new List<TeacherClass>();
}
