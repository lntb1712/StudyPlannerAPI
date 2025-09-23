using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.ReminderRepository
{
    public class ReminderRepository:RepositoryBase<Reminder>, IReminderRepository
    {
        private readonly StudyPlannerContext _context; 

        public ReminderRepository(StudyPlannerContext context):base(context) 
        {
            _context = context;
        }

        public IEnumerable<Reminder> GetAllReminderAsync()
        {
            return _context.Reminders
                           .Include(x => x.Parent)
                           .Include(x => x.Student)
                           .Include(x => x.Status)
                           .AsQueryable();
        }

        public async Task<Reminder> GetReminderByIdAsync(int reminderId)
        {
            var response = await _context.Reminders
                          .Include(x => x.Parent)
                          .Include(x => x.Student)
                          .Include(x => x.Status)
                          .FirstOrDefaultAsync(x => x.ReminderId == reminderId);
            return response!;
                  
        }

      
    }
}
