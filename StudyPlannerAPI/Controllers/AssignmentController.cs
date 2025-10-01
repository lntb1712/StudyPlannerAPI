using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.AssignmentDTO;
using StudyPlannerAPI.Services.AssignmentService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet("GetAllAssignmentByTeacherAsync")]
        public async Task<IActionResult> GetAllAssignmentByTeacherAsync([FromQuery] string teacherId, [FromQuery] string classId)
        {
            try
              {
                var response = await _assignmentService.GetAllAssignmentByTeacherAsync(teacherId,classId);
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
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpGet("GetAllAssignmentByClassAsync")]
        public async Task<IActionResult> GetAllAssignmentByClassAsync([FromQuery] string classId)
        {
            try
            {
                var response = await _assignmentService.GetAllAssignmentByClassAsync(classId);
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
                return StatusCode(500, $"Lỗi :{ex.Message}");
            }
        }

        [HttpPost("AddAssignment")]
        public async Task<IActionResult> AddAssignment([FromBody] AssignmentRequestDTO assignmentRequest)
        {
            try
            {
                var response = await _assignmentService.AddAssignment(assignmentRequest);
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

        [HttpDelete("DeleteAssigment")]
        public async Task<IActionResult> DeleteAssignment([FromQuery] int assignmentId)
        {
            try
            {
                var response = await _assignmentService.DeleteAssignment(assignmentId);
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
        [HttpPut("UpdateAssignment")]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentRequestDTO assignmentRequest)
        {
            try
            {
                var response = await _assignmentService.UpdateAssignment(assignmentRequest);
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
