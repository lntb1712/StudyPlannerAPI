using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.LoginDTO;
using StudyPlannerAPI.Services.AccountManagementService;
using StudyPlannerAPI.Services.LoginService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCors")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IAccountManagementService _accountManagementService;

        public LoginController(ILoginService loginService, IAccountManagementService accountManagementService)
        {
            _loginService = loginService;
            _accountManagementService = accountManagementService;
        }

        [HttpPost("Authentication")]
        public async Task<IActionResult> Authentication([FromBody] LoginRequestDTO loginRequest)
        {
            var response = await _loginService.LoginAsync(loginRequest);
            if (!response.Success)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response);
        }

        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation([FromQuery] string username)
        {
            var response = await _accountManagementService.GetUserInformation(username);
            if (!response.Success)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response);
        }

        // Endpoint gửi OTP (không cần auth, vì dùng cho đăng ký)
        [HttpPost("SendOTP")]
        [AllowAnonymous] // Cho phép gọi mà không cần token
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var response = await _loginService.SendOTPAsync(request.Email);
            if (!response.Success)
            {
                return BadRequest(new { Success = false, Message = response.Message });
            }
            return Ok(response);
        }

        // Endpoint xác thực OTP
        [HttpPost("VerifyOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var response = await _loginService.VerifyOTPAsync(request.Email, request.OTP);
            if (!response.Success)
            {
                return BadRequest(new { Success = false, Message = response.Message });
            }
            return Ok(response);
        }

        // Endpoint tạo tài khoản phụ huynh (sau khi verify OTP)
        [HttpPost("CreateParentAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateParentAccount([FromBody] RegisterParentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var response = await _loginService.CreateParentAccountAsync(
                request.Email,
                request.FullName,
                request.Password,
                request.ConfirmPassword
            );
            if (!response.Success)
            {
                return BadRequest(new { Success = false, Message = response.Message });
            }
            return Ok(response);
        }
    }
}