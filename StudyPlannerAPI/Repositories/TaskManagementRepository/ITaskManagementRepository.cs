using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.TaskManagementRepository
{
    public interface ITaskManagementRepository : IRepositoryBase<TaskManagement>
    {
        IEnumerable<TaskManagement> GetAllTaskManagement();
        Task<TaskManagement> GetTaskManagementById(int id);
    }
}
