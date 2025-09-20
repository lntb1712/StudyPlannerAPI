using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.TeacherClassRepository
{
    public interface ITeacherClassRepository : IRepositoryBase<TeacherClass>
    {
        IEnumerable<TeacherClass> GetAllTeacherClass();
        Task<TeacherClass> GetTeacherClassByIdAsync(string teacherId, string classId);
        IEnumerable<TeacherClass> SearchTeacherClassByText(string textToSearch);
    }
}
