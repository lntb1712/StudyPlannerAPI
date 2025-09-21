using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.Services.EmailService;

namespace StudyPlannerAPI.Services.EmailService
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<ServiceResponse<bool>> SendOTPAsync(string toEmail, string otpCode, string userName = "Người dùng")
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Study Planner", _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Mã xác thực tạo tài khoản - Study Planner";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
                    <h2>Xin chào {userName},</h2>
                    <p>Mã xác thực để tạo tài khoản phụ huynh của bạn là: <strong style='font-size: 24px; color: #3b82f6;'>{otpCode}</strong></p>
                    <p>Mã này có hiệu lực trong 5 phút. Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                    <p>Trân trọng,<br>Đội ngũ Study Planner</p>
                    <hr>
                    <small>Nếu bạn gặp vấn đề, liên hệ support@studyplanner.com</small>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return new ServiceResponse<bool>(true, "Gửi mã OTP thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, $"Lỗi gửi email: {ex.Message}");
            }
        }
    }
}