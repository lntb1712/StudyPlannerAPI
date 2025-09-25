using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.AssignmentDetailDTO;
using StudyPlannerAPI.Services.AssignmentDetailService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class AssignmentDetailController : ControllerBase
    {
        private readonly IAssignmentDetailService _assignmentDetailService;

        public AssignmentDetailController(IAssignmentDetailService assignmentDetailService)
        {
            _assignmentDetailService = assignmentDetailService;
        }

        [HttpGet("GetAllByAssignmentAsync")]
        public async Task<IActionResult> GetAllByAssignmentAsync([FromQuery] int assignMentId)
        {
            try
            {
                var response = await _assignmentDetailService.GetAllByAssignmentAsync(assignMentId);
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

        [HttpGet("GetByStudentAsync")]
        public async Task<IActionResult> GetByStudentAsync([FromQuery] int assignmentId, [FromQuery] string studentId)
        {
            try
            {
                var response = await _assignmentDetailService.GetByStudentAsync(assignmentId, studentId);
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
        [HttpPost("AddAssignmentDetail")]
        [Consumes("multipart/form-data")] // 👈 bắt buộc
        public async Task<IActionResult> AddAssignmentDetail([FromForm] AssignmentDetailRequestDTO assignmentDetailRequestDTO)
        {
            try
            {
                var response = await _assignmentDetailService.AddAssignmentDetail(assignmentDetailRequestDTO);
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

        [HttpDelete("DeleteAssignmentDetail")]
        public async Task<IActionResult> DeleteAssignmentDetail([FromQuery]int assignmentId, [FromQuery] string studentId)
        {
            try
            {
                var response = await _assignmentDetailService.DeleteAssignmentDetail(assignmentId, studentId);
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
            catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi:{ex.Message}");
            }
        }

        [HttpPut("UpdateAssignmentDetail")]
        [Consumes("multipart/form-data")] // 👈 bắt buộc
        public async Task<IActionResult> UpdateAssignmentDetail([FromForm] AssignmentDetailRequestDTO assignmentDetailRequestDTO)
        {
            try
            {
                var response = await _assignmentDetailService.UpdateAssignmentDetail(assignmentDetailRequestDTO);
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
