namespace StudyPlannerAPI.DTOs.GroupFunctionDTO
{
    public class GroupFunctionRequestDTO
    {
        public string GroupId { get; set; } = null!;

        public string FunctionId { get; set; } = null!;

        public bool? IsEnable { get; set; }

        public bool? IsReadOnly { get; set; }
    }
}
