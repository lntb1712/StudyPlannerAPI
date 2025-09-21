using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.DTOs.LoginDTO
{
    public class VerifyOTPRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải là 6 chữ số")]
        public string OTP { get; set; } = string.Empty;
    }
}
