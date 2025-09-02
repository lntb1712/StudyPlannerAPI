using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;

namespace StudyPlannerAPI.Services.AccountManagementService
{
    public interface IAccountManagementService
    {
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page, int pageSize);
        Task<ServiceResponse<int>> GetTotalAccount();
        Task<ServiceResponse<AccountManagementResponseDTO>> GetUserInformation(string username);
        Task<ServiceResponse<bool>> AddAccountManagement(AccountManagementRequestDTO account);
        Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO account);
        Task<ServiceResponse<bool>> DeleteAccountManagement(string username);
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountByGroupId(string groupId,int page, int pageSize);
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> SearchAccountByText(string textToSearch, int page, int pageSize);

    }
}
