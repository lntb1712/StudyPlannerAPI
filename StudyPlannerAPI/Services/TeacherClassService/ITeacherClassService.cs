using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ClassDTO;
using StudyPlannerAPI.DTOs.TeacherClassDTO;

namespace StudyPlannerAPI.Services.TeacherClassService
{
    public interface ITeacherClassService
    {
        Task<ServiceResponse<PagedResponse<TeacherClassResponseDTO>>> GetTeacherClassListAsync(int page, int pageSize);
        Task<ServiceResponse<PagedResponse<TeacherClassResponseDTO>>> SearchTeacherClassListAsync(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddTeacherClass(TeacherClassRequestDTO teacherClassRequest);
        Task<ServiceResponse<bool>> DeleteTeacherClass(string classId,string teacherId);
        Task<ServiceResponse<bool>> UpdateTeacherClass(TeacherClassRequestDTO teacherClassRequest);
        Task<ServiceResponse<TeacherClassResponseDTO>> GetTeacherClassByID(string classId,string teacherId);
    }
}
 