namespace StudyPlannerAPI.DTOs.StudentClassDTO
{
    public class StudentClassRequestDTO
    {
        public string ClassId { get; set; } = null!;

        public string StudentId { get; set; } = null!;

        public int? StudyStatus { get; set; }
    }
}
