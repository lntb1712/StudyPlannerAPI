namespace StudyPlannerAPI.DTOs.MessagingDTO
{
    public class MessagingRequestDTO
    {
        public int MessageId { get; set; }

        public string? SenderId { get; set; }

        public string? ReceiverId { get; set; }

        public string? Content { get; set; }

        public bool? IsRead { get; set; }
    }
}
