using StudyPlannerAPI.Models;

namespace StudyPlannerAPI.DTOs.TaskManagementDTO
{
    public class TaskManagementResponseDTO
    {
        public int TaskId { get; set; }

        public string? StudentId { get; set; }
        public string? StudentName { get; set; }    

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? DueDate { get; set; }

        public int? StatusId { get; set; }
        public string StatusName { get; set; }  

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
