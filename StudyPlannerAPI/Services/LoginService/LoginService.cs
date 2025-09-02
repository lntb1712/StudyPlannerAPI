using Microsoft.AspNetCore.Identity.Data;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;
using StudyPlannerAPI.DTOs.LoginDTO;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Services.JWTService;

namespace StudyPlannerAPI.Services.LoginService
{
    public class LoginService: ILoginService
    {
        private readonly IJWTService _jwtService;
        private readonly IAccountManagementRepository _accountManagementRepository;
        private readonly IGroupFunctionRepository _groupFunctionRepository;

        public LoginService(IJWTService jwtService, IAccountManagementRepository accountManagementRepository, IGroupFunctionRepository groupFunctionRepository)
        {
            _jwtService = jwtService;
            _accountManagementRepository = accountManagementRepository;
            _groupFunctionRepository = groupFunctionRepository;
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
            var groupFunctions = permissions.Select(x=> new GroupFunctionResponseDTO
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
    }
}
