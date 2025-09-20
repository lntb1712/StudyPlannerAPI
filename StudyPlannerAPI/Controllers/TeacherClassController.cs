using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.TeacherClassDTO;
using StudyPlannerAPI.Services.TeacherClassService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/Class/{ClassId}/[controller]")]
    [ApiController]
    [Authorize(Policy = "PermissionPolicy")]
    [EnableCors("MyCors")]
    public class TeacherClassController : ControllerBase
    {
        private readonly ITeacherClassService _teacherClassService;

        public TeacherClassController(ITeacherClassService teacherClassService)
        {
            _teacherClassService = teacherClassService;
        }

        [HttpGet("GetTeacherClassListAsync")]
        public async Task<IActionResult> GetTeacherClassListAsync([FromRoute]string classId,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _teacherClassService.GetTeacherClassListAsync(classId,page, pageSize);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("SearchTeacherClassListAsync")]
        public async Task<IActionResult> SearchTeacherClassListAsync([FromRoute] string classId, [FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _teacherClassService.SearchTeacherClassListAsync(classId,textToSearch, page, pageSize);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddTeacherClass")]
        public async Task<IActionResult> AddTeacherClass([FromBody] TeacherClassRequestDTO requestDTO)
        {
            try
            {
                var response = await _teacherClassService.AddTeacherClass(requestDTO);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateTeacherClass")]
        public async Task<IActionResult> UpdateTeacherClass([FromBody] TeacherClassRequestDTO requestDTO)
        {
            try
            {
                var response = await _teacherClassService.UpdateTeacherClass(requestDTO);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteTeacherClass")]
        public async Task<IActionResult> DeleteTeacherClass([FromQuery] string classId, [FromQuery] string teacherId)
        {
            try
            {
                var response = await _teacherClassService.DeleteTeacherClass(classId, teacherId);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetTeacherClassByID")]
        public async Task<IActionResult> GetTeacherClassByID([FromQuery] string classId, [FromQuery] string teacherId)
        {
            try
            {
                var response = await _teacherClassService.GetTeacherClassByID(classId, teacherId);
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
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetTeacherByClassID")]
        public async Task<IActionResult> GetTeacherByClassID([FromQuery] string classId)
        {
            try
            {
                var response = await _teacherClassService.GetTeacherByClassID(classId);
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
                return StatusCode(500, ex.Message);
            }
        }
    }
}