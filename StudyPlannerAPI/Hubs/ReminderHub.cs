using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;

namespace StudyPlannerAPI.Hubs
{
    public class ReminderHub : Hub
    {
        public async Task SendNotification(string userName, string title, string content)
        {
            await Clients.User(userName).SendAsync("ReceiveReminderNotification", title, content);
        }
    }
}
