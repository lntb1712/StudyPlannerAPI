using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.StudentClassRepository
{
    public interface IStudentClassRepository:IRepositoryBase<StudentClass>
    {
        IEnumerable<StudentClass> GetAllStudentClass();
        Task<StudentClass> GetStudentClassbyIdAsync(string classId, string studentId);
        IEnumerable<StudentClass> SearchStudentClassByText(string textToSearch);
    }
}
