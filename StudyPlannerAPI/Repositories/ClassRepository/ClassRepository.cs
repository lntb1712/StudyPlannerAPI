using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;
using System.Net.WebSockets;

namespace StudyPlannerAPI.Repositories.ClassRepository
{
    public class ClassRepository:RepositoryBase<Class>,IClassRepository
    {
        private readonly StudyPlannerContext _context;

        public ClassRepository(StudyPlannerContext context):base(context)
        {
            _context = context;
        }

        public IEnumerable<Class> GetAllClassesAsync()
        {
            return _context.Classes
                           .Include(c=>c.StudentClasses)
                           .Include(c=>c.TeacherClasses)
                           .AsNoTracking()
                           .AsQueryable();
        }

        public async Task<Class> GetClassByIdAsync(string classId)
        {
            var response = await _context.Classes
                                         .Include(c=>c.StudentClasses)
                                         .Include(c=>c.TeacherClasses)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(c => c.ClassId == classId);
            return response!;
        }

        public IEnumerable<Class> SearchClassAsync(string textToSearch)
        {
            return _context.Classes
                           .Include(c=>c.StudentClasses)
                           .Include(c=>c.TeacherClasses)
                           .Where(c => (c.ClassName != null && c.ClassName.Contains(textToSearch)) ||
                                       (c.StudentClasses != null && c.StudentClasses.Any(sc => sc.Student != null && sc.Student.FullName != null && sc.Student.FullName.Contains(textToSearch))) ||
                                       (c.TeacherClasses != null && c.TeacherClasses.Any(tc => tc.Teacher != null && tc.Teacher.FullName != null && tc.Teacher.FullName.Contains(textToSearch))))
                           .AsNoTracking()
                           .AsQueryable();
        }
    }
}
