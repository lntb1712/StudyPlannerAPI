using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public string? ClassId { get; set; }

    public string? TeacherId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; } = new List<AssignmentDetail>();

    public virtual Class? Class { get; set; }

    public virtual AccountManagement? Teacher { get; set; }
}
