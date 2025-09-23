using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AssignmentRepository
{
    public class AssignmentRepository:RepositoryBase<Assignment>,IAssignmentRepository
    {
        private readonly StudyPlannerContext _context;

        public AssignmentRepository (StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Assignment> GetAllAssignments()
        {
            return _context.Assignments
                           .Include(x => x.AssignmentDetails)
                           .Include(x => x.Class)
                           .Include(x => x.Teacher)
                           .AsQueryable();
        }

        public async Task<Assignment> GetAssignmentByIdAsync(int assignmentId)
        {
            var response = await _context.Assignments
                           .Include(x => x.AssignmentDetails)
                           .Include(x => x.Class)
                           .Include(x => x.Teacher)
                           .FirstOrDefaultAsync(x=>x.AssignmentId == assignmentId);
            return response!;

        }
    }
}
