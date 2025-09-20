using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.ClassRepository
{
    public interface IClassRepository:IRepositoryBase<Class>
    {
        IEnumerable<Class> GetAllClassesAsync();
        Task<Class> GetClassByIdAsync(string classId);
        IEnumerable<Class> SearchClassAsync(string textToSearch);
        
    }
}
