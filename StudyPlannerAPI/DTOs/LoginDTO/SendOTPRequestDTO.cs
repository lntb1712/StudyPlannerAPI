using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.DTOs.LoginDTO
{
    public class SendOTPRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
    }
}
