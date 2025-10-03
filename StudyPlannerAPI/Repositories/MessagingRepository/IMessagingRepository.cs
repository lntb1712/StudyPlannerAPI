using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.MessagingRepository
{
    public interface IMessagingRepository:IRepositoryBase<Messaging> 
    {
        IEnumerable<Messaging> GetAllMessaging();
        Task<Messaging> GetMessagingById(int id);
    }
}
