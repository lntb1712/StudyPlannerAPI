using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.MessagingRepository
{
    public class MessagingRepository:RepositoryBase<Messaging>, IMessagingRepository
    {
        private readonly StudyPlannerContext _context;

        public MessagingRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Messaging> GetAllMessaging()
        {
            return _context.Messagings
                           .Include(x => x.Sender)
                           .Include(x => x.Receiver)
                           .AsQueryable();
        }

        public async Task<Messaging> GetMessagingById(int id)
        {
            var response = await _context.Messagings
                                        .Include(x => x.Sender)
                                        .Include(x => x.Receiver)
                                        .FirstOrDefaultAsync(x=>x.MessageId == id);
            return response!;
        }
    }
}
