using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.DTOs.MessagingDTO;

namespace StudyPlannerAPI.Services.MessagingService
{
    public interface IMessagingService
    {
        Task<ServiceResponse<bool>> SendMessage(MessagingRequestDTO request);
        Task<ServiceResponse<List<MessagingResponseDTO>>> GetConversation(string senderId, string receiverId);
        Task<ServiceResponse<List<MessagingResponseDTO>>> GetAllMessagesByUser(string userId);
        Task<ServiceResponse<bool>> MarkAsRead(int messageId);
        Task<ServiceResponse<bool>> DeleteMessage(int messageId);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetAllRelationship(string userId);
    }
}
