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

    }
}
