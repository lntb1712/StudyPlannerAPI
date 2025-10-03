namespace StudyPlannerAPI.DTOs.MessagingDTO
{
    public class MessagingResponseDTO
    {
        public int MessageId { get; set; }

        public string? SenderId { get; set; }
        public string? SenderName { get; set; }

        public string? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }

        public string? Content { get; set; }

        public bool? IsRead { get; set; }

        public string? CreatedAt { get; set; }
    }
}
