using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ScheduleDTO;

namespace StudyPlannerAPI.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<ServiceResponse<List<ScheduleResponseDTO>>> GetAllSchedulesAsync(string studentId);
        Task<ServiceResponse<ScheduleResponseDTO>> GetScheduleByIdAsync(int scheduleId);
        Task<ServiceResponse<bool>> CreateScheduleAsync(ScheduleRequestDTO scheduleRequest);
        Task<ServiceResponse<bool>> UpdateScheduleAsync(int scheduleId, ScheduleRequestDTO scheduleRequest);
        Task<ServiceResponse<bool>> DeleteScheduleAsync(int scheduleId);
        Task<ServiceResponse<List<ScheduleResponseDTO>>> SearchSchedulesAsync(string studentId,string textToSearch);
    }
}
