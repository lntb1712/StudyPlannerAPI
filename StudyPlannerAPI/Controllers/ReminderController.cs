using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.ReminderDTO;
using StudyPlannerAPI.Services.ReminderService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("GetReminderByParentOrStudent")]
        public async Task<IActionResult> GetReminderByParentOrStudent([FromQuery] string parentOrStudentId)
        {
            try
            {
                var response = await _reminderService.GetReminderByParentOrStudent(parentOrStudentId);
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex}");
            }
        }

        [HttpGet("GetReminderById")]
        public async Task<IActionResult> GetReminderById([FromQuery] int reminderId)
        {
            try
            {
                var response = await _reminderService.GetReminderById(reminderId);
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex}");
            }
        }

        [HttpPost("AddReminder")]
        public async Task<IActionResult> AddReminder([FromBody] ReminderRequestDTO reminder)
        {
            try
            {
                var response = await _reminderService.AddReminder(reminder);
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex}");
            }
        }

        [HttpDelete ("DeleteReminder")] 
        public async Task<IActionResult> DeleteReminder([FromQuery] int reminderId)
        {
            try
            {
                var response = await _reminderService.DeleteReminder(reminderId);
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
            catch( Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex}");
            }
        }

        [HttpPut("UpdateReminder")]
        public async Task<IActionResult> UpdateReminder([FromBody] ReminderRequestDTO reminder)
        {
            try
            {
                var response = await _reminderService.UpdateReminder(reminder);
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex}");
            }
        }
    }
}
