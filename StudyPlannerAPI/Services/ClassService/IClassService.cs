using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ClassDTO;

namespace StudyPlannerAPI.Services.ClassService
{
    public interface IClassService
    {
        Task<ServiceResponse<PagedResponse<ClassResponseDTO>>> GetClassListAsync(int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ClassResponseDTO>>> SearchClassListAsync(string textToSearch,int page, int pageSize);
        Task<ServiceResponse<bool>> AddClass(ClassRequestDTO classRequest);
        Task<ServiceResponse<bool>> DeleteClass(string classId);
        Task<ServiceResponse<bool>> UpdateClass(ClassRequestDTO classRequest);
        Task<ServiceResponse<ClassResponseDTO>> GetClassById(string classId);

    }
}
