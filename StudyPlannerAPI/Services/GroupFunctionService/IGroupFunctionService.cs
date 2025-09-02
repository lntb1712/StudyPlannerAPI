using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;

namespace StudyPlannerAPI.Services.GroupFunctionService
{
    public interface IGroupFunctionService
    {
        Task<ServiceResponse<List<GroupFunctionResponseDTO>>> GetGroupFunctionWithGroupID(string groupId);
        Task<ServiceResponse<bool>> DeleteGroupFunction(string groupId, string functionId);
    }
}
