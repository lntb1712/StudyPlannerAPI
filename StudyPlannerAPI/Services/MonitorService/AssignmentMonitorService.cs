using Microsoft.AspNetCore.SignalR;
using StudyPlannerAPI.Helper;
using StudyPlannerAPI.Hubs;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AssignmentRepository;
using System.Net.WebSockets;

namespace StudyPlannerAPI.Services.MonitorService
{
    public class AssignmentMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AssignmentHub> _hubContext;
        private readonly ILogger<AssignmentMonitorService> _logger;

        public AssignmentMonitorService(IServiceProvider serviceProvider, IHubContext<AssignmentHub> hubContext, ILogger<AssignmentMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AssignmentMonitorService started at {Time}", HelperTime.NowVN());

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<StudyPlannerContext>();
                        var assignmentRepository = scope.ServiceProvider.GetRequiredService<IAssignmentRepository>();

                        var allAssignments = assignmentRepository.GetAllAssignments();
                        var overdueAssignments = allAssignments
                            .Where(x => x.Deadline <= HelperTime.NowVN())
                            .ToList();

                        foreach (var assignment in overdueAssignments)
                        {
                            // Assuming we notify students in the class
                            // Fetch students in the class (adjust table name as per your model)
                            var classStudents = assignment.AssignmentDetails
                                .Select(cs => cs.StudentId)
                                .ToList();

                            foreach (var studentId in classStudents)
                            {
                                var notification = new Notification
                                {
                                    UserName = studentId,
                                    Title = $"Bài tập quá hạn từ giáo viên {assignment.TeacherId}",
                                    Content = $"Bài tập '{assignment.Title}' đã quá hạn. Vui lòng nộp ngay.",
                                    Type = "Bài tập quá hạn",
                                    IsRead = false,
                                    CreatedAt = HelperTime.NowVN(),
                                };
                                var detail = assignment.AssignmentDetails.FirstOrDefault(x => x.StudentId == studentId);
                                if (detail != null)
                                {
                                    detail.StatusId = 4;
                                }
                                context.Notifications.Add(notification);

                                await _hubContext.Clients.User(studentId!)
                                    .SendAsync("ReceiveOverdueAssignmentNotification", notification.Title, notification.Content);
                            }
                        }
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Có lỗi xảy ra {Time}", HelperTime.NowVN());
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}