using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTOs.GroupManagementDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.GroupManagementRepository
{
    public class GroupManagementRepository: RepositoryBase<GroupManagement>,IGroupManagementRepository
    {
        private readonly StudyPlannerContext _context;
        public GroupManagementRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<GroupManagement> GetAllGroup()
        {
            return _context.GroupManagements
                           .Include(x=>x.GroupFunctions)
                           .AsQueryable();
        }

        public async Task<GroupManagement> GetGroupManagementWithGroupID(string GroupID)
        {
            var response = await _context.GroupManagements
                                         .Include(x=>x.GroupFunctions)
                                         .FirstOrDefaultAsync(x=>x.GroupId==GroupID);
            return response!;
        }

        public async Task<List<GroupManagementTotalDTO>> GetTotalUserInGroup()
        {
            var group = await _context.GroupManagements
                                    .Include(x => x.GroupFunctions)
                                    .Include(x => x.AccountManagements)
                                    .Select(x => new GroupManagementTotalDTO
                                    {
                                        GroupID = x.GroupId,
                                        GroupName = x.GroupName,
                                        TotalUser = x.AccountManagements.Where(t => t.GroupId == x.GroupId).Count()
                                    }).ToListAsync();
            return group;
        }

        public IQueryable<GroupManagement> SearchGroup(string textToSearch)
        {
            var group =  _context.GroupManagements
                                 .Include(x => x.GroupFunctions)
                                 .Where(x => x.GroupId.Contains(textToSearch) || x.GroupName!.Contains(textToSearch) || x.GroupDescription!.Contains(textToSearch))
                                 .AsQueryable();
            return group;
        }
    }
}
