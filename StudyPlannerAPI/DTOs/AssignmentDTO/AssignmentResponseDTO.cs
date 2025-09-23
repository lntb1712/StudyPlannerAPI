using StudyPlannerAPI.DTOs.AssignmentDetailDTO;

namespace StudyPlannerAPI.DTOs.AssignmentDTO
{
    public class AssignmentResponseDTO
    {
        public int AssignmentId { get; set; }

        public string? ClassId { get; set; }
        public string? ClassName { get; set; }

        public string? TeacherId { get; set; }
        public string? TeacherName { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Deadline { get; set; }

        public string? CreatedAt { get; set; }

        public List<AssignmentDetailResponseDTO> assignments { get; set; } = new List<AssignmentDetailResponseDTO>();
    }
}
