using SendGrid;
using SendGrid.Helpers.Mail;
using StudyPlannerAPI.DTO;

namespace StudyPlannerAPI.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;

        public EmailService(IConfiguration config)
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                      ?? config["SendGrid:ApiKey"]!;
        }

        public async Task<ServiceResponse<bool>> SendOTPAsync(string toEmail, string otpCode, string userName = "Người dùng")
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress("support@studyplanner.com", "Study Planner");
                var subject = "Mã xác thực tạo tài khoản - Study Planner";
                var to = new EmailAddress(toEmail, userName);

                var htmlContent = $@"
                    <h2>Xin chào {userName},</h2>
                    <p>Mã xác thực để tạo tài khoản phụ huynh của bạn là:
                    <strong style='font-size: 24px; color: #3b82f6;'>{otpCode}</strong></p>
                    <p>Mã này có hiệu lực trong 5 phút.</p>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = await client.SendEmailAsync(msg);

                return new ServiceResponse<bool>(
                    response.IsSuccessStatusCode,
                    response.IsSuccessStatusCode ? "Gửi email thành công" : "Gửi email thất bại"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, $"Lỗi gửi email: {ex.Message}");
            }
        }
    }
}
