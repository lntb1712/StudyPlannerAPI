using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.TaskManagementDTO;

namespace StudyPlannerAPI.Services.TaskManagementService
{
    public interface ITaskManagementService
    {
        Task<ServiceResponse<List<TaskManagementResponseDTO>>> GetTaskManagementAsync (string studentId);
        Task<ServiceResponse<bool>> AddTaskManagement (TaskManagementRequestDTO taskManagementRequestDTO);
        Task<ServiceResponse<bool>> DeleteTaskManagement (int taskId);
        Task<ServiceResponse<bool>> UpdateTaskManagement (TaskManagementRequestDTO taskManagementRequestDTO);

    }
}
