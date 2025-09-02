using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class TeacherClass
{
    public string ClassId { get; set; } = null!;

    public string TeacherId { get; set; } = null!;

    public string? Subject { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AccountManagement Teacher { get; set; } = null!;
}
