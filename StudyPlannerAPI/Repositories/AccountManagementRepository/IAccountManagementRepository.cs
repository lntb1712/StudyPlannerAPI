using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.RepositoryBase;

namespace StudyPlannerAPI.Repositories.AccountManagementRepository
{
    public interface IAccountManagementRepository : IRepositoryBase<AccountManagement>
    {
        IQueryable<AccountManagement> GetAllAccount();
        //Check pass by 3rd party (BCrypt)
        Task<AccountManagement> GetAccountWithUserName(string username);
        IQueryable<AccountManagement> SearchAccountByText(string textToSearch);
        
    }
}
