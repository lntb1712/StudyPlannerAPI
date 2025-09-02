namespace StudyPlannerAPI.DTOs.AccountManagementDTO
{
    public class AccountManagementResponseDTO
    {
        public string UserName { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? ParentEmail { get; set; }

        public string? GroupId { get; set; }

        public string? GroupName { get; set; }

        public string? CreatedAt { get; set; }
    }
}
