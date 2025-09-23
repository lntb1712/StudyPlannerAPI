using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AssignmentRepository
{
    public interface IAssignmentRepository:IRepositoryBase<Assignment>
    {
        IEnumerable<Assignment> GetAllAssignments();
        Task<Assignment> GetAssignmentByIdAsync(int assignmentId);
    }
}
