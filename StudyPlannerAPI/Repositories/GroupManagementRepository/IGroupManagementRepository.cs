using StudyPlannerAPI.DTOs.GroupManagementDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.GroupManagementRepository
{
    public interface IGroupManagementRepository:IRepositoryBase<GroupManagement>
    {
        IQueryable<GroupManagement> GetAllGroup();
        Task<GroupManagement> GetGroupManagementWithGroupID(string GroupID);
        IQueryable<GroupManagement> SearchGroup(string textToSearch);
        Task<List<GroupManagementTotalDTO>> GetTotalUserInGroup();
    }
}
