using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.Services.AccountManagementService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountManagementService _accountManagementService;

        public AccountManagementController(IAccountManagementService accountManagementService)
        {
            _accountManagementService = accountManagementService;
        }

        [HttpGet("GetAllAccount")]
        public async Task<IActionResult> GetAllAccount([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _accountManagementService.GetAllAccount(page, pageSize);
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

        [HttpGet("GetTotalAccount")]
        public async Task<IActionResult> GetTotalAccount()
        {
            var response = await _accountManagementService.GetTotalAccount();
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

        [HttpPost("AddAccountManagement")]
        public async Task<IActionResult> AddAccountManagement([FromBody] AccountManagementRequestDTO accountManagementRequestDTO)
        {
            var response = await _accountManagementService.AddAccountManagement(accountManagementRequestDTO);
            if (!response.Success)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response);
        }

        [HttpPut("UpdateAccountManagement")]
        public async Task<IActionResult> UpdateAccountManagement([FromBody] AccountManagementRequestDTO accountManagementRequestDTO)
        {
            var response = await _accountManagementService.UpdateAccountManagement(accountManagementRequestDTO);
            if (!response.Success)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response);
        }

        [HttpDelete("DeleteAccountManagement")]
        public async Task<IActionResult> DeleteAccountManagement([FromQuery] string username)
        {
            var response = await _accountManagementService.DeleteAccountManagement(username);
            if (!response.Success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response);
        }

        [HttpGet("GetAllAccountByGroupId")]
        public async Task<IActionResult> GetAllAccountByGroupId([FromQuery] string groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _accountManagementService.GetAllAccountByGroupId(groupId, page, pageSize);
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

        [HttpGet("SearchAccountByText")]
        public async Task<IActionResult> SearchAccountByText([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _accountManagementService.SearchAccountByText(textToSearch, page, pageSize);
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
