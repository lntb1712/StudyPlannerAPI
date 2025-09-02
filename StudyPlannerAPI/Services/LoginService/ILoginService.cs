using Microsoft.AspNetCore.Identity.Data;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.LoginDTO;

namespace StudyPlannerAPI.Services.LoginService
{
    public interface ILoginService
    {
        Task<ServiceResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest);
    }
}
