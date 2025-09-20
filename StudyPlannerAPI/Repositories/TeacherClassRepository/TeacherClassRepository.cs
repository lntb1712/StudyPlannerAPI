using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;
using System.Net.WebSockets;

namespace StudyPlannerAPI.Repositories.TeacherClassRepository
{
    public class TeacherClassRepository : RepositoryBase<TeacherClass>, ITeacherClassRepository
    {
        private readonly StudyPlannerContext _context;

        public TeacherClassRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<TeacherClass> GetAllTeacherClass()
        {
            return _context.TeacherClasses
                           .Include(x => x.Teacher)
                           .Include(x => x.Class)
                           .AsEnumerable();
        }

        public async Task<TeacherClass> GetTeacherClassByIdAsync(string classId, string teacherId)
        {
            var response = await _context.TeacherClasses
                                         .Include(x => x.Teacher)
                                         .Include(x => x.Class)
                                         .FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.ClassId == classId);
            return response!;
        }

        public IEnumerable<TeacherClass> SearchTeacherClassByText(string textToSearch)
        {
            return _context.TeacherClasses
                           .Include(x => x.Teacher)
                           .Include(x => x.Class)
                           .Where(x => x.ClassId.Contains(textToSearch) ||
                                  x.Class.ClassName!.Contains(textToSearch) ||
                                  x.Teacher.FullName!.Contains(textToSearch) ||
                                  x.Subject!.Contains(textToSearch))
                           .AsEnumerable();
        }
    }
}
