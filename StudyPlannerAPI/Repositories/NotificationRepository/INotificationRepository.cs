using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.NotificationRepository
{
    public interface INotificationRepository:IRepositoryBase<Notification>
    {
        IEnumerable<Notification> GetAllNotification();
        Task<Notification>GetNotificationById(int id);
    }
}
