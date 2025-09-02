using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.GroupFunctionRepository
{
    public interface IGroupFunctionRepository:IRepositoryBase<GroupFunction>
    {
        Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId);
        Task<List<GroupFunction>> GetListFunctionIDOfGroup(string groupId);
    }
}
