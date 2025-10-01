namespace StudyPlannerAPI.DTOs.TaskManagementDTO
{
    public class TaskManagementRequestDTO
    {
        public int TaskId { get; set; }

        public string? StudentId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? DueDate { get; set; }

        public int? StatusId { get; set; }

    }
}
