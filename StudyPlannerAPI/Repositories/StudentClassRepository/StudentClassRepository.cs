using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.StudentClassRepository
{
    public class StudentClassRepository: RepositoryBase<StudentClass>, IStudentClassRepository
    {
        private readonly StudyPlannerContext _context;

        public StudentClassRepository(StudyPlannerContext context):base(context)
        {
            _context = context;
        }

        public IEnumerable<StudentClass> GetAllStudentClass()
        {
            return _context.StudentClasses
                           .Include(x => x.Class)
                           .Include(x => x.Student)
                           .AsNoTracking()
                           .AsQueryable();
        }

        public async Task<StudentClass> GetStudentClassbyIdAsync(string studentId, string classId)
        {
           var response= await _context.StudentClasses
                                .Include(x => x.Class)
                                .Include(x => x.Student)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.ClassId == classId)!;
           return response!;
        }

        public IEnumerable<StudentClass> SearchStudentClassByText(string textToSearch)
        {
            return _context.StudentClasses
                           .Include(x => x.Class)
                           .Include(x => x.Student)
                           .Where(sc => (sc.Student != null && sc.Student.FullName != null && sc.Student.FullName.Contains(textToSearch)) ||
                                        (sc.Class != null && sc.Class.ClassName != null && sc.Class.ClassName.Contains(textToSearch)))
                           .AsNoTracking()
                           .AsQueryable();
        }
    }
}
