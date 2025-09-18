namespace StudyPlannerAPI.DTOs.ScheduleDTO
{
    public class ScheduleRequestDTO
    {
        public int ScheduleId { get; set; }

        public string? StudentId { get; set; }

        public string? ClassId { get; set; }

        public string? TeacherId { get; set; }

        public string? Subject { get; set; }

        public int? DayOfWeek { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public int? StatusId { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
