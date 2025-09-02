using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.GroupFunctionRepository
{
    public class GroupFunctionRepository : RepositoryBase<GroupFunction>, IGroupFunctionRepository
    {
        private readonly StudyPlannerContext _context;
        public GroupFunctionRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId)
        {
            return await _context.GroupFunctions
               .Where(x => x.GroupId == groupId)
               .Include(x => x.Function)
               .ToListAsync()
               .ContinueWith(t => t.Result
                   .GroupBy(x => x.FunctionId)
                   .Select(g => g.First())
                   .ToList());
        }

        public async Task<List<GroupFunction>> GetListFunctionIDOfGroup(string groupId)
        {
            var lstFunction = await _context.GroupFunctions
                                           .Include(x=>x.Function)
                                           .Where(x => x.GroupId == groupId && x.IsEnable == true)
                                           .Distinct()
                                           .ToListAsync();
            return lstFunction;
        }
    }
}
