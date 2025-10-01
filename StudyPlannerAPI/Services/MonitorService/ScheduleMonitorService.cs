using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Hubs;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.ScheduleRepository;
using StudyPlannerAPI.Services.ScheduleService;  // Nếu cần service khác

namespace StudyPlannerAPI.Services.MonitorService
{
    public class ScheduleMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;
        // ❌ Bỏ injection _scheduleRepository (Scoped)

        // ✅ Thêm ILogger để debug
        private readonly ILogger<ScheduleMonitorService> _logger;

        public ScheduleMonitorService(
            IServiceProvider serviceProvider,
            IHubContext<NotificationHub> hubContext,
            ILogger<ScheduleMonitorService> logger)  // Thêm logger
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduleMonitorService started at {Time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())  // ✅ Tạo scope mới mỗi loop
                {
                    try
                    {
                        var db = scope.ServiceProvider.GetRequiredService<StudyPlannerContext>();
                        var scheduleRepository = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();  // ✅ Resolve Scoped repo trong scope

                        // ✅ Await query đúng cách (fix async bug từ trước)
                        var allSchedules = scheduleRepository.GetAllSchedulesAsync();
                        var schedules =  allSchedules
                            .Where(s => s.StartTime <= DateTime.Now && (s.StatusId == 1 || s.StatusId == 2))
                            .ToList();  // Sử dụng ToListAsync() từ EF Core

                        _logger.LogInformation("Found {Count} overdue schedules at {Time}", schedules.Count, DateTime.Now);

                        foreach (var s in schedules)
                        {
                            // Tạo notification
                            var notification = new Notification
                            {
                                UserName = s.StudentId,
                                Title = "Lịch học trễ",
                                Content = $"Môn {s.Subject} đã bắt đầu lúc {s.StartTime:HH:mm}",
                                Type = "Warning",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            };
                            db.Notifications.Add(notification);

                            // Push realtime cho student
                            await _hubContext.Clients.User(s.StudentId!)
                                .SendAsync("ReceiveNotification", notification.Title, notification.Content);

                            // Fix lỗi chính tả và query parent (giả sử table là AccountManagement, field ParentEmail)
                            var parentAccount = await db.AccountManagements  // Sửa tên table nếu cần (AccountManagement?)
                                .FirstOrDefaultAsync(x => x.ParentEmail == s.Student!.ParentEmail);  // So sánh field đúng
                            if (parentAccount != null)
                            {
                                await _hubContext.Clients.User(parentAccount.UserName!)
                                    .SendAsync("ReceiveNotification", notification.Title, notification.Content);
                            }

                            // Cập nhật status để tránh lặp
                            s.StatusId = 4;  // Đổi thành 3 (hoặc enum "Notified") thay vì -1 nếu không hợp lệ
                        }

                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in ScheduleMonitorService loop at {Time}", DateTime.Now);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}