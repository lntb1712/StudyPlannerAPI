using StudyPlannerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using StudyPlannerAPI.DTOs;
using StudyPlannerAPI.DTOs.GroupFunctionDTO;
using StudyPlannerAPI.Helper;

namespace StudyPlannerAPI.Services.JWTService
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(AccountManagement accountManagement, List<GroupFunctionResponseDTO> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountManagement.UserName.ToString()),
                new Claim(ClaimTypes.Name, accountManagement.FullName!.ToString()),
                new Claim(ClaimTypes.Role, accountManagement.GroupId!.ToString()),
            };

            // Thêm Permission claims
            foreach (var permission in permissions)
            {
                var json = JsonSerializer.Serialize(new
                {
                    id =permission.FunctionId,
                    ro =permission.IsReadOnly,
                });
                claims.Add(new Claim("Permission", json));
            }

            var secretKey = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = HelperTime.NowVN().AddHours(8), // Token sống 8 tiếng chẳng hạn
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            await Task.CompletedTask;
            return tokenHandler.WriteToken(token);
        }
    }
}
