using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;
using System.Net.WebSockets;

namespace StudyPlannerAPI.Repositories.TaskManagementRepository
{
    public class TaskManagementRepository : RepositoryBase<TaskManagement>, ITaskManagementRepository
    {
        private readonly StudyPlannerContext _context;
        public TaskManagementRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<TaskManagement> GetAllTaskManagement()
        {
            return _context.TaskManagements
                           .Include(x => x.Status)
                           .Include(x => x.Student)
                           .AsQueryable();
        }

        public async Task<TaskManagement> GetTaskManagementById(int id)
        {
            var response = await _context.TaskManagements
                                         .Include(x => x.Status)
                                         .Include(x => x.Student)
                                         .FirstOrDefaultAsync(x=>x.TaskId == id);
            return response!;
        }
    }
}
