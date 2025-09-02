namespace StudyPlannerAPI.DTOs.LoginDTO
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? ApplicableLocation { get; set; }
        public string? GroupId { get; set; }
    }
}
