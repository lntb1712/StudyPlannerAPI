using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AssignmentDTO;

namespace StudyPlannerAPI.Services.AssignmentService
{
    public interface IAssignmentService
    {
        Task<ServiceResponse<List<AssignmentResponseDTO>>> GetAllAssignmentByTeacherAsync(string teacherId);
        Task<ServiceResponse<List<AssignmentResponseDTO>>> GetAllAssignmentByClassAsync(string classId);
        Task<ServiceResponse<bool>> AddAssignment(AssignmentRequestDTO assignmentRequest);
        Task<ServiceResponse<bool>> DeleteAssignment(int assignmentRequest);
        Task<ServiceResponse<bool>> UpdateAssignment(AssignmentRequestDTO assignmentRequest);
    }
}
