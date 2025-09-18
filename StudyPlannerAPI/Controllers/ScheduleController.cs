using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Services.ScheduleService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("GetAllSchedules")]
        public async Task<IActionResult> GetAllSchedule([FromQuery] string studentId)
        {
            try
            {
                var response = await _scheduleService.GetAllSchedulesAsync(studentId);
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
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("GetScheduleById")]
        public async Task<IActionResult> GetScheduleById([FromQuery] int scheduleId)
        {
            try
            {
                var response = await _scheduleService.GetScheduleByIdAsync(scheduleId);
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
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] DTOs.ScheduleDTO.ScheduleRequestDTO scheduleRequest)
        {
            if (scheduleRequest == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _scheduleService.CreateScheduleAsync(scheduleRequest);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }
        [HttpPut("UpdateSchedule")]
        public async Task<IActionResult> UpdateSchedule([FromQuery] int scheduleId, [FromBody] DTOs.ScheduleDTO.ScheduleRequestDTO scheduleRequest)
        {
            if (scheduleRequest == null)
            {
                return BadRequest("Dữ liệu nhận vào không hợp lệ");
            }
            try
            {
                var response = await _scheduleService.UpdateScheduleAsync(scheduleId, scheduleRequest);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpDelete("DeleteSchedule")]
        public async Task<IActionResult> DeleteSchedule([FromQuery] int scheduleId)
        {
            try
            {
                var response = await _scheduleService.DeleteScheduleAsync(scheduleId);
                if (response.Success == false)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
                return Ok(new { Success = true, Message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        [HttpGet("SearchSchedules")]
        public async Task<IActionResult> SearchSchedules([FromQuery] string studentId, [FromQuery] string textToSearch)
        {
            try
            {
                var response = await _scheduleService.SearchSchedulesAsync(studentId, textToSearch);
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
                return StatusCode(500, $"Lỗi {ex.Message}");
            }
        }

        

    }
}
