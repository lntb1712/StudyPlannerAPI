using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.NotificationDTO;

namespace StudyPlannerAPI.Services.NotificationService
{
    public interface INotificationService
    {
        Task<ServiceResponse<List<NotificationResponseDTO>>> GetAllNotification(string userName);
        Task<ServiceResponse<bool>> AddNotification(NotificationRequestDTO notification);
        Task<ServiceResponse<bool>> DeleteNotification(int notificationId);
        Task<ServiceResponse<bool>> UpdateNotification(NotificationRequestDTO notification);
    }
}
