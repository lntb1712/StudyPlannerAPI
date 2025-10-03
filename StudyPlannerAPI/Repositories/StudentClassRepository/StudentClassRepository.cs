using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTOs.StudentClassDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;
using System.Net.WebSockets;

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
                           .AsQueryable();
        }

        public async Task<StudentClass> GetStudentClassbyIdAsync(string classId, string studentId)
        {
           var response= await _context.StudentClasses
                                .Include(x => x.Class)
                                .Include(x => x.Student)
                                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.ClassId == classId)!;
           return response!;
        }

        public async Task<List<StudentClassTotalDTO>> GetTotalStudentInClass()
        {
            var response = await _context.Classes
                .GroupJoin(
                    _context.StudentClasses,
                    cls => cls.ClassId,
                    sc => sc.ClassId,
                    (cls, students) => new StudentClassTotalDTO
                    {
                        ClassId = cls.ClassId,
                        ClassName = cls.ClassName,
                        TotalStudent = students.Count() // Count will be 0 if no students
                    })
                .ToListAsync();

            return response;
        }


        public IEnumerable<StudentClass> SearchStudentClassByText(string textToSearch)
        {
            return _context.StudentClasses
                           .Include(x => x.Class)
                           .Include(x => x.Student)
                           .Where(sc => (sc.Student != null && sc.Student.FullName != null && sc.Student.FullName.Contains(textToSearch)) ||
                                        (sc.Class != null && sc.Class.ClassName != null && sc.Class.ClassName.Contains(textToSearch)))
                           .AsQueryable();
        }
    }
}
