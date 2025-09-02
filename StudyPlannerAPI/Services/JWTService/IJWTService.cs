using StudyPlannerAPI.DTOs;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;
using StudyPlannerAPI.Models;

namespace StudyPlannerAPI.Services.JWTService
{
    public interface IJWTService
    {
        Task<string> GenerateToken(AccountManagement accountManagement, List<GroupFunctionResponseDTO> permissions);
    }
}
