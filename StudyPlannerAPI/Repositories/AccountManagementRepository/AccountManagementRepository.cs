using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AccountManagementRepository
{
    public class AccountManagementRepository : RepositoryBase<AccountManagement>, IAccountManagementRepository
    {
        private readonly StudyPlannerContext _context;
        public AccountManagementRepository(StudyPlannerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AccountManagement> GetAccountWithUserName(string username)
        {
            var user = await _context.AccountManagements
                                     .Include(x=>x.Group)
                                     .FirstOrDefaultAsync(x => 
                                     EF.Functions.Collate(x.UserName, "SQL_Latin1_General_CP1_CS_AS") == username);
            //Catch Upper and Lower when user Typing
            return user!;
        }

        public IQueryable<AccountManagement> GetAllAccount()
        {
            return _context.AccountManagements
                           .Include(x => x.Group)
                           .ThenInclude(x=>x!.GroupFunctions)
                           .AsQueryable();
        }

        public IQueryable<AccountManagement> SearchAccountByText(string textToSearch)
        {
            return _context.AccountManagements
                           .Include(x => x.Group)
                           .ThenInclude(x => x!.GroupFunctions)
                           .Where(x=>x.UserName.Contains(textToSearch)
                           ||x.FullName!.Contains(textToSearch)
                           ||x.Group!.GroupName!.Contains(textToSearch)
                           ||x.Email!.Contains(textToSearch)
                           ||x.ParentEmail!.Contains(textToSearch))
                           .AsQueryable();
        }
    }
}
