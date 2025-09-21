using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;
using StudyPlannerAPI.DTOs.LoginDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Services.AccountManagementService;
using StudyPlannerAPI.Services.EmailService;
using StudyPlannerAPI.Services.GroupManagementService; // Thêm import
using StudyPlannerAPI.Services.JWTService;

namespace StudyPlannerAPI.Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly IJWTService _jwtService;
        private readonly IAccountManagementRepository _accountManagementRepository;
        private readonly IGroupFunctionRepository _groupFunctionRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountManagementService _accountManagementService;
        private readonly IGroupManagementService _groupManagementService; // Thêm dependency

        // Dictionary để lưu OTP tạm thời (in-memory, chỉ cho demo. Production dùng Redis/DB với expire)
        private readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStorage = new();

        public LoginService(
            IJWTService jwtService,
            IAccountManagementRepository accountManagementRepository,
            IGroupFunctionRepository groupFunctionRepository,
            IEmailService emailService,
            IAccountManagementService accountManagementService,
            IGroupManagementService groupManagementService) // Thêm constructor param
        {
            _jwtService = jwtService;
            _accountManagementRepository = accountManagementRepository;
            _groupFunctionRepository = groupFunctionRepository;
            _emailService = emailService;
            _accountManagementService = accountManagementService;
            _groupManagementService = groupManagementService; // Khởi tạo
        }

        public async Task<ServiceResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return new ServiceResponse<LoginResponseDTO>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var account = await _accountManagementRepository.GetAccountWithUserName(loginRequest.Username.Trim());

            if (account == null)
            {
                return new ServiceResponse<LoginResponseDTO>(false, "Tài khoản không tồn tại");
            }
            else if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password.Trim(), account.Password!))
            {
                // Kiểm tra mật khẩu
                return new ServiceResponse<LoginResponseDTO>(false, "Mật khẩu không đúng");
            }

            var permissions = await _groupFunctionRepository.GetListFunctionIDOfGroup(account.GroupId!);
            var groupFunctions = permissions.Select(x => new GroupFunctionResponseDTO
            {
                GroupId = account.GroupId!,
                FunctionId = x.FunctionId,
                FunctionName = x.Function!.FunctionName!,
                IsEnable = x.IsEnable,
                IsReadOnly = x.IsReadOnly,
            }).ToList();

            var token = await _jwtService.GenerateToken(account, groupFunctions);
            if (string.IsNullOrEmpty(token))
            {
                return new ServiceResponse<LoginResponseDTO>(false, "Lỗi trong quá trình tạo token");
            }

            var loginResponse = new LoginResponseDTO
            {
                Token = token,
                GroupId = account.GroupId,
                Username = account.UserName,
            };
            return new ServiceResponse<LoginResponseDTO>(true, "Đăng nhập thành công", loginResponse);
        }

        // Gửi OTP đến email
        public async Task<ServiceResponse<bool>> SendOTPAsync(string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                return new ServiceResponse<bool>(false, "Email không hợp lệ");
            }

            // Kiểm tra nếu email đã tồn tại (tùy chọn, để tránh spam)
            var existingAccount = await _accountManagementRepository.GetAllAccount()
                .FirstOrDefaultAsync(x => x.Email == email || x.ParentEmail == email);
            if (existingAccount != null)
            {
                return new ServiceResponse<bool>(false, "Email đã được sử dụng cho tài khoản khác");
            }

            // Tạo OTP 6 chữ số
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5); // Hết hạn sau 5 phút

            // Lưu OTP vào storage tạm (in-memory, production dùng Redis)
            _otpStorage[email] = (otpCode, expiry);

            // Gửi email
            var emailResponse = await _emailService.SendOTPAsync(email, otpCode, "Phụ huynh");
            if (!emailResponse.Success)
            {
                _otpStorage.Remove(email); // Xóa nếu gửi thất bại
                return new ServiceResponse<bool>(false, emailResponse.Message);
            }

            return new ServiceResponse<bool>(true, "Gửi mã OTP thành công");
        }

        // Xác thực OTP
        public async Task<ServiceResponse<bool>> VerifyOTPAsync(string email, string otpCode)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpCode))
            {
                return new ServiceResponse<bool>(false, "Email hoặc mã OTP không được để trống");
            }

            if (!_otpStorage.TryGetValue(email, out var stored) || DateTime.UtcNow > stored.Expiry)
            {
                _otpStorage.Remove(email); // Xóa nếu hết hạn
                return new ServiceResponse<bool>(false, "Mã OTP không hợp lệ hoặc đã hết hạn");
            }

            if (stored.Otp != otpCode)
            {
                return new ServiceResponse<bool>(false, "Mã OTP không đúng");
            }

            // Xóa OTP sau verify thành công
            _otpStorage.Remove(email);
            return new ServiceResponse<bool>(true, "Xác thực OTP thành công");
        }

        // Tạo tài khoản phụ huynh sau khi verify OTP
        public async Task<ServiceResponse<bool>> CreateParentAccountAsync(string email, string fullName, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return new ServiceResponse<bool>(false, "Thông tin không được để trống");
            }

            if (password != confirmPassword)
            {
                return new ServiceResponse<bool>(false, "Mật khẩu xác nhận không khớp");
            }

            if (password.Length < 6)
            {
                return new ServiceResponse<bool>(false, "Mật khẩu phải ít nhất 6 ký tự");
            }

            // Kiểm tra email đã tồn tại chưa
            var existingAccount = await _accountManagementRepository.GetAllAccount()
                .FirstOrDefaultAsync(x => x.Email == email || x.ParentEmail == email);
            if (existingAccount != null)
            {
                return new ServiceResponse<bool>(false, "Email đã được sử dụng");
            }

            // Gọi EnsureParentGroupExists để lấy hoặc tạo group cho phụ huynh
            var groupResponse = await _groupManagementService.EnsureParentGroupExists();
            if (!groupResponse.Success)
            {
                return new ServiceResponse<bool>(false, groupResponse.Message);
            }
            var parentGroupId = groupResponse.Data; // Lấy GroupId từ response

            var accountRequest = new AccountManagementRequestDTO
            {
                UserName = email, // Sử dụng email làm username cho parent
                Password = password,
                FullName = fullName,
                Email = email,
                ParentEmail = email, // Parent email là chính email này
                GroupId = parentGroupId // Sử dụng GroupId từ EnsureParentGroupExists
            };

            var createResponse = await _accountManagementService.AddAccountManagement(accountRequest);
            return createResponse;
        }

        // Helper method kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}