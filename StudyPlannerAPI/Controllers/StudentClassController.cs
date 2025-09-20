using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.DTOs.StudentClassDTO;
using StudyPlannerAPI.Services.StudentClassService;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassController : ControllerBase
    {
        private readonly IStudentClassService _studentClassService;

        public StudentClassController(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        [HttpGet("GetStudentClassListAsync")]
        public async Task<IActionResult> GetStudentClassListAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _studentClassService.GetStudentClassListAsync(page, pageSize);
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

        [HttpGet("SearchStudentClassListAsync")]
        public async Task<IActionResult> SearchStudentClassListAsync([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _studentClassService.SearchStudentClassListAsync(textToSearch, page, pageSize);
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

        [HttpPost("AddStudentClass")]
        public async Task<IActionResult> AddStudentClass([FromBody] StudentClassRequestDTO requestDTO)
        {
            try
            {
                var response = await _studentClassService.AddStudentClass(requestDTO);
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

        [HttpPut("UpdateStudentClass")]
        public async Task<IActionResult> UpdateStudentClass([FromBody] StudentClassRequestDTO requestDTO)
        {
            try
            {
                var response = await _studentClassService.UpdateStudentClass(requestDTO);
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

        [HttpDelete("DeleteStudentClass")]
        public async Task<IActionResult> DeleteStudentClass([FromQuery] string classId, [FromQuery] string studentId)
        {
            try
            {
                var response = await _studentClassService.DeleteStudentClass(classId, studentId);
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

        [HttpGet("GetStudentClassById")]
        public async Task<IActionResult> GetStudentClassById([FromQuery] string classId, [FromQuery] string studentId)
        {
            try
            {
                var response = await _studentClassService.GetStudentClassById(classId, studentId);
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