using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.FunctionRepository
{
    public class FunctionRepository:RepositoryBase<Function>, IFunctionRepository   
    {
        private readonly StudyPlannerContext _context;
        public FunctionRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Function>> GetFunctionsAsync()
        {
            var functions = await _context.Functions
                                  .Include(x => x.GroupFunctions)
                                  .ToListAsync();
            return functions;
        }
    }
}
