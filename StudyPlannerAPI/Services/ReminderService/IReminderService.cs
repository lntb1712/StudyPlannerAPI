using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ReminderDTO;

namespace StudyPlannerAPI.Services.ReminderService
{
    public interface IReminderService
    {
        Task<ServiceResponse<List<ReminderResponseDTO>>> GetReminderByParentOrStudent(string userName);
        Task<ServiceResponse<ReminderResponseDTO>> GetReminderById(int reminderId);
        Task<ServiceResponse<bool>> AddReminder(ReminderRequestDTO reminderRequest);
        Task<ServiceResponse<bool>> DeleteReminder(int reminderId);
        Task<ServiceResponse<bool>> UpdateReminder(ReminderRequestDTO reminderRequest);

    }
}
