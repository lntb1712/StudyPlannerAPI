using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class StudentClass
{
    public string ClassId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public int? StudyStatus { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AccountManagement Student { get; set; } = null!;
}
