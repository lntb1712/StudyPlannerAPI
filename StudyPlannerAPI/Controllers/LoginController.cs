using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.LoginDTO;
using StudyPlannerAPI.Services.LoginService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCors")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
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
    }
}
