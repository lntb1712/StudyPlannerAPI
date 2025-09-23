using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AssignmentDetailRepository
{
    public interface IAssignmentDetailRepository:IRepositoryBase<AssignmentDetail>
    {
        IEnumerable<AssignmentDetail> GetAllAssignmentDetailAsync(int assignmentId);
        Task<AssignmentDetail> GetAssignmentDetailByStudentAsync(int assignmentId, string studentId);
    }
}
