using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.DashboardDTO;

namespace StudyPlannerAPI.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ServiceResponse<DashboardDTO>> GetDashboardDataAsync();
    }
}
