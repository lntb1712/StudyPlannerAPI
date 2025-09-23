using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class AssignmentDetail
{
    public int AssignmentId { get; set; }

    public string StudentId { get; set; } = null!;

    public int? StatusId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public double? Grade { get; set; }

    public string? FilePath { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual StatusMaster? Status { get; set; }

    public virtual AccountManagement Student { get; set; } = null!;
}
