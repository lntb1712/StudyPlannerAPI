using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.TeacherClassRepository
{
    public interface ITeacherClassRepository : IRepositoryBase<TeacherClass>
    {
        IEnumerable<TeacherClass> GetAllTeacherClass();
        Task<TeacherClass> GetTeacherClassByIdAsync(string classId, string teacherId);
        IEnumerable<TeacherClass> SearchTeacherClassByText(string textToSearch);
    }
}
