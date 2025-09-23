using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AssignmentDetailRepository
{
    public class AssignmentDetailRepository:RepositoryBase<AssignmentDetail>,IAssignmentDetailRepository
    {
        private readonly StudyPlannerContext _context;

        public AssignmentDetailRepository (StudyPlannerContext context): base(context)
        {
            _context = context;
        }

        public IEnumerable<AssignmentDetail> GetAllAssignmentDetailAsync(int assignmentId)
        {
            return _context.AssignmentDetails
                           .Include(x => x.Assignment)
                           .Include(x => x.Student)
                           .Include(x => x.Status)
                           .AsQueryable();
        }

        public async Task<AssignmentDetail> GetAssignmentDetailByStudentAsync(int assignmentId, string studentId)
        {
            var response = await _context.AssignmentDetails
                                        .Include(x => x.Assignment)
                                        .Include(x => x.Student)
                                        .Include(x => x.Status)
                                        .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.StudentId == studentId);
            return response!;
        }
    }
}
