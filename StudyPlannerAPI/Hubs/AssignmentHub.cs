using Microsoft.AspNetCore.SignalR;

namespace StudyPlannerAPI.Hubs
{
    public class AssignmentHub:Hub
    {
        public async Task SendNotification(string userName, string title, string content)
        {
            // Gửi notification cho user cụ thể
            await Clients.User(userName).SendAsync("ReceiveOverdueAssignmentNotification", title, content);
        }
    }
}
