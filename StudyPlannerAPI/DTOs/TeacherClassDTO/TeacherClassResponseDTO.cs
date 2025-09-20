namespace StudyPlannerAPI.DTOs.TeacherClassDTO
{
    public class TeacherClassResponseDTO
    {
        public string ClassId { get; set; } = null!;

        public string TeacherId { get; set; } = null!;
        public string TeacherName { get; set; } = null!;

        public string? Subject { get; set; }
    }
}
