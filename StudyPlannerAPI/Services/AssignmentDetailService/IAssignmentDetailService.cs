using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AssignmentDetailDTO;

namespace StudyPlannerAPI.Services.AssignmentDetailService
{
    public interface IAssignmentDetailService
    {
        Task<ServiceResponse<List<AssignmentDetailResponseDTO>>> GetAllByAssignmentAsync(int assignmentId);
        Task<ServiceResponse<AssignmentDetailResponseDTO>> GetByStudentAsync(int assignmentId, string studentId);
        Task<ServiceResponse<bool>> AddAssignmentDetail(AssignmentDetailRequestDTO request);
        Task<ServiceResponse<bool>> DeleteAssignmentDetail(int assignmentId, string studentId);
        Task<ServiceResponse<bool>> UpdateAssignmentDetail(AssignmentDetailRequestDTO request);
    }
}