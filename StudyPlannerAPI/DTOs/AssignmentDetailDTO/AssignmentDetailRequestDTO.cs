namespace StudyPlannerAPI.DTOs.AssignmentDetailDTO
{
    public class AssignmentDetailRequestDTO
    {
        public int AssignmentId { get; set; }

        public string StudentId { get; set; } = null!;

        public int? StatusId { get; set; }
        public string? FilePath { get; set; }

        public string? SubmittedAt { get; set; }

        public double? Grade { get; set; }
        public IFormFile? File { get; set; }
    }
}
