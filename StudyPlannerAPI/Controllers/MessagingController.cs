using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.MessagingDTO;
using StudyPlannerAPI.Services.MessagingService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly IMessagingService _messagingService;

        public MessagingController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpGet("GetConversation")]
        public async Task<IActionResult> GetConversation([FromQuery] string senderId, [FromQuery] string receiverId)
        {
            try
            {
                var response = await _messagingService.GetConversation(senderId, receiverId);
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
                return StatusCode(500, $"Lỗi: " + ex.Message);
            }
        }
        [HttpGet("GetAllRelationship")]
        public async Task<IActionResult> GetAllRelationship([FromQuery] string userId)
        {
            try
            {
                var response = await _messagingService.GetAllRelationship(userId);
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
                return StatusCode(500, $"Lỗi: " + ex.Message);
            }
        }
        [HttpGet("GetAllMessagesByUser")]
        public async Task<IActionResult> GetAllMessagesByUser([FromQuery] string userId)
        {
            try
            {
                var response = await _messagingService.GetAllMessagesByUser(userId);
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
                return StatusCode(500, $"Lỗi: " + ex.Message);
            }
        }
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessagingRequestDTO messagingRequestDTO)
        {
            try
            {
                var response = await _messagingService.SendMessage(messagingRequestDTO);
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
        [HttpPut("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromQuery] int messageId)
        {
            try
            {
                var response = await _messagingService.MarkAsRead(messageId);
                if(!response.Success)
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
        [HttpDelete("DeleteMessage")]
        public async Task<IActionResult> DeleteMessage([FromQuery] int messageId)
        {
            try
            {
                var response = await _messagingService.DeleteMessage(messageId);
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
