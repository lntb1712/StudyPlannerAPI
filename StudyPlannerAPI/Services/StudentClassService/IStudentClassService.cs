using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.StudentClassDTO;

namespace StudyPlannerAPI.Services.StudentClassService
{
    public interface IStudentClassService
    {
        Task<ServiceResponse<PagedResponse<StudentClassResponseDTO>>> GetStudentClassListAsync(int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StudentClassResponseDTO>>> SearchStudentClassListAsync(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddStudentClass(StudentClassRequestDTO studentClassRequest);
        Task<ServiceResponse<bool>> DeleteStudentClass(string classId,string studentId);
        Task<ServiceResponse<bool>> UpdateStudentClass(StudentClassRequestDTO studentClassRequest);
        Task<ServiceResponse<StudentClassResponseDTO>> GetStudentClassById(string classId, string studentId);
    }
}
