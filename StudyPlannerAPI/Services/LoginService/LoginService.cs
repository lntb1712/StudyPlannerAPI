using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;
using StudyPlannerAPI.DTOs.LoginDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Services.AccountManagementService;
using StudyPlannerAPI.Services.EmailService;
using StudyPlannerAPI.Services.GroupManagementService;
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
        private readonly IGroupManagementService _groupManagementService;

        // Dùng MemoryCache thay cho DistributedCache
        private readonly IMemoryCache _cache;

        public LoginService(
            IJWTService jwtService,
            IAccountManagementRepository accountManagementRepository,
            IGroupFunctionRepository groupFunctionRepository,
            IEmailService emailService,
            IAccountManagementService accountManagementService,
            IGroupManagementService groupManagementService,
            IMemoryCache cache) // Thay IDistributedCache thành IMemoryCache
        {
            _jwtService = jwtService;
            _accountManagementRepository = accountManagementRepository;
            _groupFunctionRepository = groupFunctionRepository;
            _emailService = emailService;
            _accountManagementService = accountManagementService;
            _groupManagementService = groupManagementService;
            _cache = cache;
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

            var existingAccount = await _accountManagementRepository.GetAllAccount()
                .FirstOrDefaultAsync(x => x.Email == email);
            if (existingAccount != null)
            {
                return new ServiceResponse<bool>(false, "Email đã được sử dụng cho tài khoản khác");
            }

            // Tạo OTP 6 chữ số
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString();

            // Lưu OTP vào MemoryCache, expire sau 5 phút
            _cache.Set($"otp:{email}", otpCode, TimeSpan.FromMinutes(5));

            // Gửi email
            var emailResponse = await _emailService.SendOTPAsync(email, otpCode, "Phụ huynh");
            if (!emailResponse.Success)
            {
                _cache.Remove($"otp:{email}");
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

            // Lấy OTP từ MemoryCache
            if (!_cache.TryGetValue($"otp:{email}", out string? storedOtp) || storedOtp != otpCode)
            {
                _cache.Remove($"otp:{email}");
                return new ServiceResponse<bool>(false, "Mã OTP không hợp lệ hoặc đã hết hạn");
            }

            _cache.Remove($"otp:{email}");
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
            var existingParent = await _accountManagementRepository.GetAllAccount()
                                       .FirstOrDefaultAsync(x => x.ParentEmail == email);
            if (existingParent == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy phụ huynh trong hệ thống");
            }
            var existingAccount = await _accountManagementRepository.GetAllAccount()
                .FirstOrDefaultAsync(x => x.Email == email);
            if (existingAccount != null)
            {
                return new ServiceResponse<bool>(false, "Email đã được sử dụng");
            }

            var groupResponse = await _groupManagementService.EnsureParentGroupExists();
            if (!groupResponse.Success)
            {
                return new ServiceResponse<bool>(false, groupResponse.Message);
            }
            var parentGroupId = groupResponse.Data;

            var accountRequest = new AccountManagementRequestDTO
            {
                UserName = email,
                Password = password,
                FullName = fullName + $"(Phụ huynh của {existingParent.FullName})",
                Email = email,
                ParentEmail = "",
                GroupId = parentGroupId
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
