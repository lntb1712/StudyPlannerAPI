namespace StudyPlannerAPI.DTOs.TeacherClassDTO
{
    public class TeacherClassRequestDTO
    {
        public string ClassId { get; set; } = null!;

        public string TeacherId { get; set; } = null!;

        public string? Subject { get; set; }
    }
}
