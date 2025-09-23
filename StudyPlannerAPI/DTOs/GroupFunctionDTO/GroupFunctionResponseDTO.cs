namespace StudyPlannerAPI.DTOs.GroupFunctionDTO
{
    public class GroupFunctionResponseDTO
    {
        public string GroupId { get; set; } = null!;

        public string FunctionId { get; set; } = null!;
        public string? FunctionName { get; set; }

        public bool? IsEnable { get; set; }

        public bool? IsReadOnly { get; set; }
    }
}
