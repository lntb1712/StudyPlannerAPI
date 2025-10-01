using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.NotificationRepository
{
    public class NotificationRepository:RepositoryBase<Notification>,INotificationRepository    
    {
        private readonly StudyPlannerContext _context;

        public NotificationRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Notification> GetAllNotification()
        {
            return _context.Notifications
                           .Include(x=>x.UserNameNavigation)
                           .AsQueryable();
                            
        }

        public async Task<Notification> GetNotificationById(int id)
        {
            var response = await _context.Notifications
                                         .Include(x=>x.UserNameNavigation)
                                         .FirstOrDefaultAsync(x=>x.NotificationId==id);
            return response!;
        }
    }
}
