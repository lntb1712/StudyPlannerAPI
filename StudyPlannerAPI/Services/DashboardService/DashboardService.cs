using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.DashboardDTO;
using StudyPlannerAPI.Helper;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.ClassRepository;
using StudyPlannerAPI.Repositories.GroupManagementRepository;
using StudyPlannerAPI.Repositories.StudentClassRepository;
using System.Net.WebSockets;

namespace StudyPlannerAPI.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IAccountManagementRepository _accountRepo;
        private readonly IGroupManagementRepository _groupRepo;
        private readonly IClassRepository _classRepo;
        private readonly IStudentClassRepository _studentClassRepo;

        public DashboardService(
            IAccountManagementRepository accountRepo,
            IGroupManagementRepository groupRepo,
            IClassRepository classRepo,
            IStudentClassRepository studentClassRepo)
        {
            _accountRepo = accountRepo;
            _groupRepo = groupRepo;
            _classRepo = classRepo;
            _studentClassRepo = studentClassRepo;
        }

        public async Task<ServiceResponse<DashboardDTO>> GetDashboardDataAsync()
        {
            var dashboard = new DashboardDTO();

            // Total Accounts
            dashboard.TotalAccounts = _accountRepo.GetAllAccount().Count();

            // Total Groups
            dashboard.TotalGroups = _groupRepo.GetAllGroup().Count();

            // Total Classes (async method, need await)
            dashboard.TotalClasses = ( _classRepo.GetAllClassesAsync()).Count();

            var now = HelperTime.NowVN();

            // New accounts in current month
            var totalNewAccounts = _accountRepo.GetAllAccount()
                .Where(x => x.CreatedAt.HasValue &&
                            x.CreatedAt.Value.Month == now.Month &&
                            x.CreatedAt.Value.Year == now.Year)
                .Count();

            // New accounts in previous month
            var previousMonth = now.AddMonths(-1);
            var totalNewAccountPrevious = _accountRepo.GetAllAccount()
                .Where(x => x.CreatedAt.HasValue &&
                            x.CreatedAt.Value.Month == previousMonth.Month &&
                            x.CreatedAt.Value.Year == previousMonth.Year)
                .Count();

            dashboard.TotalNewAccountInMonth = totalNewAccounts;

            // Percentage change compared to previous month
            if (totalNewAccountPrevious == 0)
            {
                dashboard.PercentUpDownNewRegisterAccount = totalNewAccounts > 0 ? 100 : 0;
            }
            else
            {
                dashboard.PercentUpDownNewRegisterAccount = ((double)(totalNewAccounts - totalNewAccountPrevious) / totalNewAccountPrevious) * 100;
            }

            // Total Student Enrollments (from StudentClass)
            dashboard.ClassWithStudentCounts = await _studentClassRepo.GetTotalStudentInClass();

            // Groups with User Counts
            dashboard.GroupsWithUserCounts = await _groupRepo.GetTotalUserInGroup();

            return new ServiceResponse<DashboardDTO>(true, "Lấy thông tin dashboard thành công", dashboard);
        }

    }
}
