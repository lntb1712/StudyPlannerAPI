using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using StudyPlannerAPI.DTOs.ClassDTO;
    using StudyPlannerAPI.Services.ClassService;

    namespace StudyPlannerAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize(Policy = "PermissionPolicy")]
        [EnableCors("MyCors")]
        public class ClassController : ControllerBase
        {
            private readonly IClassService _classService;

            public ClassController(IClassService classService)
            {
                _classService = classService;
            }

            [HttpGet("GetClassListAsync")]
            public async Task<IActionResult> GetClassListAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                try
                {
                    var response = await _classService.GetClassListAsync(page, pageSize);
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

            [HttpGet("SearchClassListAsync")]
            public async Task<IActionResult> SearchClassListAsync([FromQuery] string textToSearch, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                try
                {
                    var response = await _classService.SearchClassListAsync(textToSearch, page, pageSize);
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

            [HttpPost("AddClass")]
            public async Task<IActionResult> AddClass([FromBody] ClassRequestDTO requestDTO)
            {
                try
                {
                    var response = await _classService.AddClass(requestDTO);
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

            [HttpPut("UpdateClass")]
            public async Task<IActionResult> UpdateClass([FromBody] ClassRequestDTO requestDTO)
            {
                try
                {
                    var response = await _classService.UpdateClass(requestDTO);
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
            [HttpDelete("DeleteClass")]
            public async Task<IActionResult> DeleteClass([FromQuery] string classId)
            {
                try
                {
                    var response = await _classService.DeleteClass(classId);
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

            [HttpGet("GetClassById")]
            public async Task<IActionResult> GetClassById([FromQuery] string classId)
            {
                try
                {
                    var response = await _classService.GetClassById(classId);
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
