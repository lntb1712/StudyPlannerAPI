namespace StudyPlannerAPI.DTOs.StudentClassDTO
{
    public class StudentClassResponseDTO
    {
        public string ClassId { get; set; } = null!;

        public string StudentId { get; set; } = null!;

        public string StudentName { get; set; } = null!;

        public int? StudyStatus { get; set; }
    }
}
