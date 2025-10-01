using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.NotificationDTO;
using StudyPlannerAPI.Services.NotificationService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("GetAllNotification")]
        public async Task<IActionResult> GetAllNotification([FromQuery] string userName)
        {
            try
            {
                var response = await _notificationService.GetAllNotification(userName);
                if (!response.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Messafe = response.Message,
                    });
                }
                return Ok(response);
            }catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi:{ex.Message}");
            }
        }

        [HttpPost("AddNotification")]
        public async Task<IActionResult> AddNotification([FromQuery] NotificationRequestDTO notification)
        {
            try
            {
                var response = await _notificationService.AddNotification(notification);
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
                return StatusCode(500, $"Lỗi:{ex.Message}");
            }
        }
        [HttpDelete("DeleteNotification")]
        public async Task<IActionResult> DeleteNotification([FromQuery] int notificationId)
        {
            try
            {
                var response = await _notificationService.DeleteNotification(notificationId);
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpPut("UpdateNotification")]
        public async Task<IActionResult> UpdateNotification([FromQuery] NotificationRequestDTO notification)
        {
            try
            {
                var response = await _notificationService.UpdateNotification(notification);
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
                return StatusCode(500, $"Lỗi:{ex.Message}");
            }
        }

    }
}
