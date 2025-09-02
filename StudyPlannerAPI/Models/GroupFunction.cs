using System;
using System.Collections.Generic;

namespace StudyPlannerAPI.Models;

public partial class GroupFunction
{
    public string GroupId { get; set; } = null!;

    public string FunctionId { get; set; } = null!;

    public bool? IsEnable { get; set; }

    public bool? IsReadOnly { get; set; }

    public virtual Function Function { get; set; } = null!;

    public virtual GroupManagement Group { get; set; } = null!;
}
