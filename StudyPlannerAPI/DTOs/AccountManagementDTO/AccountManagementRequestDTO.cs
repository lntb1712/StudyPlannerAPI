namespace StudyPlannerAPI.DTOs.AccountManagementDTO
{
    public class AccountManagementRequestDTO
    {
        public string UserName { get; set; } = null!;

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? ParentEmail { get; set; }

        public string? GroupId { get; set; }

    }
}
