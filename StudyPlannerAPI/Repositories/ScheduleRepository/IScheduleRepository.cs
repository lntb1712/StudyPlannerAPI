using StudyPlannerAPI.DTOs.ScheduleDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.ScheduleRepository
{
    public interface IScheduleRepository:IRepositoryBase<Schedule>
    {
        IEnumerable<Schedule> GetAllSchedulesAsync();
        Task<Schedule> GetScheduleByIdAsync(int scheduleId);
        IEnumerable<Schedule> SearchScheduleAsync (string textToSearch);
    }
}
