using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.TaskManagementDTO;
using StudyPlannerAPI.Services.TaskManagementService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class TaskManagementController : ControllerBase
    {
        private readonly ITaskManagementService _taskManagementService;

        public TaskManagementController(ITaskManagementService taskManagementService)
        {
            _taskManagementService = taskManagementService;
        }

        [HttpGet("GetTaskManagementAsync")]
        public async Task<IActionResult> GetTaskManagementAsync([FromQuery] string studentId)
        {
            try
            {
                var response = await _taskManagementService.GetTaskManagementAsync(studentId);
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpPost("AddTaskManagement")]
        public async Task<IActionResult> AddTaskManagement([FromBody] TaskManagementRequestDTO taskManagement)
        {
            try
            {
                var response = await _taskManagementService.AddTaskManagement(taskManagement);
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
        [HttpPut("UpdateTaskManagement")]
        public async Task<IActionResult> UpdateTaskManagement([FromBody] TaskManagementRequestDTO taskManagement)
        {
            try
            {
                var response = await _taskManagementService.UpdateTaskManagement(taskManagement);
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
        [HttpDelete("DeleteTaskManagement")]
        public async Task<IActionResult> DeleteTaskManagement([FromQuery] int taskId)
        {
            try
            {
                var response = await _taskManagementService.DeleteTaskManagement(taskId);
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
    }
}
