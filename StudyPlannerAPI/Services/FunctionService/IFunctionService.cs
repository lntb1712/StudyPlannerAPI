using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.FunctionDTO;

namespace StudyPlannerAPI.Services.FunctionService
{
    public interface IFunctionService
    {
        Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllFunctions();
    }
}
