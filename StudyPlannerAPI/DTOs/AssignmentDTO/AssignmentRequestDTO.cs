namespace StudyPlannerAPI.DTOs.AssignmentDTO
{
    public class AssignmentRequestDTO
    {
        public int AssignmentId { get; set; }

        public string? ClassId { get; set; }

        public string? TeacherId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Deadline { get; set; }

        public string? CreatedAt { get; set; }

    }
}
