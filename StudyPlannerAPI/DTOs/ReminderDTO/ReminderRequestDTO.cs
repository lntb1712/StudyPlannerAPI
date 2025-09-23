namespace StudyPlannerAPI.DTOs.ReminderDTO
{
    public class ReminderRequestDTO
    {
        public int ReminderId { get; set; }

        public string? ParentId { get; set; }

        public string? StudentId { get; set; }

        public string? Content { get; set; }

        public string? DueDate { get; set; }

        public int? StatusId { get; set; }

        public string? CreatedAt { get; set; }
    }
}
