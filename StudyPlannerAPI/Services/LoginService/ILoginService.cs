using Microsoft.AspNetCore.Identity.Data;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.LoginDTO;

namespace StudyPlannerAPI.Services.LoginService
{
    public interface ILoginService
    {
        Task<ServiceResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest);
        Task<ServiceResponse<bool>> CreateParentAccountAsync(string email, string fullName, string password, string confirmPassword);
        Task<ServiceResponse<bool>> VerifyOTPAsync(string email, string otpCode);
        Task<ServiceResponse<bool>> SendOTPAsync(string email);
    }
}
