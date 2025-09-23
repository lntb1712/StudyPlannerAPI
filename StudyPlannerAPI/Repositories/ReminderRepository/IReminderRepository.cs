using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.ReminderRepository
{
    public interface IReminderRepository:IRepositoryBase<Reminder>
    {
        IEnumerable<Reminder> GetAllReminderAsync();
        Task<Reminder> GetReminderByIdAsync(int reminderId);
    }
}
