namespace StudyPlannerAPI.DTOs.FunctionDTO
{
    public class FunctionResponseDTO
    {
        public string FunctionId { get; set; } = null!;

        public string? FunctionName { get; set; }
        public bool IsEnable { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
