namespace StudyPlannerAPI.DTOs.AssignmentDetailDTO
{
    public class AssignmentDetailResponseDTO
    {
        public int AssignmentId { get; set; }

        public string StudentId { get; set; } = null!;
        public string? StudentName { get; set; }
        public string? FilePath { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public string? SubmittedAt { get; set; }

        public double? Grade { get; set; }
    }
}
