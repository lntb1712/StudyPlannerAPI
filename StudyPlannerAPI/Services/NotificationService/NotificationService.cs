using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.NotificationDTO;
using StudyPlannerAPI.Hubs;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.NotificationRepository;
using System.Reflection.Metadata.Ecma335;

namespace StudyPlannerAPI.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly StudyPlannerContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository notificationRepository, StudyPlannerContext context, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ServiceResponse<bool>> AddNotification(NotificationRequestDTO notification)
        {
            if (notification ==null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var response = new Notification
            {
                UserName = notification.UserName,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = DateTime.Now
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _notificationRepository.AddAsync(response, saveChanges: false);
                    await _context.SaveChangesAsync();
                    // 🔥 Push realtime đến client ngay sau khi lưu DB
                    await _hubContext.Clients.User(notification.UserName!)
                        .SendAsync("ReceiveNotification", notification.Title, notification.Content);
                    await _hubContext.Clients.User(notification.UserName!)
                        .SendAsync("ReceiveReminderNotification", notification.Title, notification.Content);
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm thông báo thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Thông báo đã tồn tại");
                        }

                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không hợp lệ");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dBEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo thông báo  : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteNotification(int notificationId)
        {
            if (notificationId < 0)
            {
                return new ServiceResponse<bool>(false, "Mã thông báo không được bé hơn 0");
            }

            var existing = await _notificationRepository.GetNotificationById(notificationId);
            if(existing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy thông báo cần xóa");
            }

            using (var transaction= await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _notificationRepository.DeleteAsync(notificationId);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa thông báo thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa thông báo do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa thông báo: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<NotificationResponseDTO>>> GetAllNotification(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new ServiceResponse<List<NotificationResponseDTO>>(false, "Tên người dùng không được để trống");
            }
            var query = _notificationRepository.GetAllNotification();
            var response = query.Where(x => x.UserName == userName)
                                .Select(x => new NotificationResponseDTO
                                {
                                    NotificationId = x.NotificationId,
                                    UserName = x.UserName,
                                    FullName = x.UserNameNavigation!.FullName,
                                    Title = x.Title,
                                    Content = x.Content,
                                    IsRead = x.IsRead,
                                    Type = x.Type,
                                    CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")
                                }).OrderBy(x=>x.IsRead).ToList();
            return new ServiceResponse<List<NotificationResponseDTO>>(true, "Lấy danh sách thông báo thành công",response);

        }

        public async Task<ServiceResponse<bool>> UpdateNotification(NotificationRequestDTO notification)
        {
            if(notification == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingNotification = await _notificationRepository.GetNotificationById(notification.NotificationId);
            if (existingNotification == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy thông báo để thông báo");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingNotification.IsRead = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Đã đọc thông báo");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Thông báo đã tồn tại");
                        }

                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không hợp lệ");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dBEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo thông báo  : {ex.Message}");
                }
            }
        }
    }
}
