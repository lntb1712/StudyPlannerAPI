using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.FunctionRepository
{
    public interface IFunctionRepository:IRepositoryBase<Function>
    {
        Task<List<Function>> GetFunctionsAsync();
    }
}
