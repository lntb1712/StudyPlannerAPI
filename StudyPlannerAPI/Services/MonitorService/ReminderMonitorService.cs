using Microsoft.AspNetCore.SignalR;
using StudyPlannerAPI.Hubs;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.ReminderRepository;

namespace StudyPlannerAPI.Services.MonitorService
{
    public class ReminderMonitorService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<ReminderHub> _hubContext;

        private readonly ILogger<ReminderMonitorService> _logger;

        public ReminderMonitorService(IServiceProvider serviceProvider, IHubContext<ReminderHub> hubContext, ILogger<ReminderMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderMonitorService started at {Time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<StudyPlannerContext>();
                        var reminderRepository = scope.ServiceProvider.GetRequiredService<IReminderRepository>();

                        var allReminder = reminderRepository.GetAllReminderAsync();
                        var reminders = allReminder
                                        .Where(x => x.CreatedAt <= DateTime.Now && (x.StatusId == 1 || x.StatusId == 2))
                                        .ToList();

                        foreach (var r in reminders)
                        {
                            var notification = new Notification
                            {
                                UserName = r.StudentId,
                                Title = $"Nhắc nhở từ của phụ huynh {r.ParentId}",
                                Content = r.Content,
                                Type = "Nhắc nhở",
                                IsRead = false,
                                CreatedAt = DateTime.Now,
                            };
                            context.Notifications.Add(notification);

                            await _hubContext.Clients.User(r.StudentId!)
                                                     .SendAsync("ReceiveReminderNotification", notification.Title, notification.Content);
                            r.StatusId = 4;
                        }
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Có lỗi xảy ra {Time}", DateTime.Now);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
