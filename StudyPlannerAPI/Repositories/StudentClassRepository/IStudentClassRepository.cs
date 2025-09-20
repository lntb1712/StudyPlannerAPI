using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.StudentClassRepository
{
    public interface IStudentClassRepository:IRepositoryBase<StudentClass>
    {
        IEnumerable<StudentClass> GetAllStudentClass();
        Task<StudentClass> GetStudentClassbyIdAsync(string studentId, string classId);
        IEnumerable<StudentClass> SearchStudentClassByText(string textToSearch);
    }
}
