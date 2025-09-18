using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTOs.ScheduleDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.ScheduleRepository
{
    public class ScheduleRepository:RepositoryBase<Schedule>, IScheduleRepository
    {
        private readonly StudyPlannerContext _context;
        public ScheduleRepository(StudyPlannerContext context):base(context)
        {
            _context = context;
        }

        public IEnumerable<Schedule> GetAllSchedulesAsync()
        {
            var schedules = _context.Schedules
                                    .Include(x=>x.Student)
                                    .Include(x=>x.Teacher)
                                    .Include(x=>x.Class)
                                    .Include(x=>x.Status)
                                    .AsNoTracking()
                                    .AsQueryable();
            return schedules; 
        }

        public Task<Schedule> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = _context.Schedules
                                   .Include(x => x.Student)
                                   .Include(x => x.Teacher)
                                   .Include(x => x.Class)
                                   .Include(x => x.Status)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            return schedule!;
        }

        public IEnumerable<Schedule> SearchScheduleAsync(string textToSearch)
        {
            return _context.Schedules
                            .Include(x => x.Student)
                            .Include(x => x.Teacher)
                            .Include(x => x.Class)
                            .Include(x => x.Status)
                            .Where(s => (s.Subject != null && s.Subject.Contains(textToSearch)) ||
                                        (s.Student != null && s.Student.FullName != null && s.Student.FullName.Contains(textToSearch)) ||
                                        (s.Teacher != null && s.Teacher.FullName != null && s.Teacher.FullName.Contains(textToSearch)) ||
                                        (s.Class != null && s.Class.ClassName != null && s.Class.ClassName.Contains(textToSearch)) ||
                                        (s.Status != null && s.Status.StatusName != null && s.Status.StatusName.Contains(textToSearch)))
                            .AsNoTracking()
                            .AsQueryable();
        }
    }
}
