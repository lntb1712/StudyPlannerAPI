namespace StudyPlannerAPI.DTOs.NotificationDTO
{
    public class NotificationRequestDTO
    {
        public int NotificationId { get; set; }

        public string? UserName { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Type { get; set; }

        public bool? IsRead { get; set; }
    }
}
