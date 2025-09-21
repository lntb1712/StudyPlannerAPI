using StudyPlannerAPI.DTO;

namespace StudyPlannerAPI.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<bool>> SendOTPAsync(string toEmail, string otpCode, string userName = "Người dùng");
    }
}