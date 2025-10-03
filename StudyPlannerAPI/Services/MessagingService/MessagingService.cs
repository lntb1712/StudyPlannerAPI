using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.DTOs.MessagingDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.MessagingRepository;
using System.Linq.Expressions;

namespace StudyPlannerAPI.Services.MessagingService
{
    public class MessagingService : IMessagingService
    {
        private readonly IMessagingRepository _messagingRepository;
        private readonly IAccountManagementRepository _accountRepository;
        private readonly StudyPlannerContext _context;

        public MessagingService(IMessagingRepository messagingRepository,IAccountManagementRepository accountRepository, StudyPlannerContext context)
        {
            _messagingRepository = messagingRepository;
            _accountRepository = accountRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> DeleteMessage(int messageId)
        {
            if (messageId < 0)
            {
                return new ServiceResponse<bool>(false, "Mã tin nhắn không được bé hơn 0");
            }
            var existingMessage = await _messagingRepository.GetMessagingById(messageId);
            if (existingMessage == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy tin nhắn");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _messagingRepository.DeleteAsync(messageId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa tin nhắn thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa tin nhắn do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa tin nhắn: {ex.Message}");
                }
            }
        }

        // ================= SEND MESSAGE =================
        public async Task<ServiceResponse<bool>> SendMessage(MessagingRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.SenderId) || string.IsNullOrWhiteSpace(request.ReceiverId))
            {
                return new ServiceResponse<bool>(false, "SenderId và ReceiverId không được để trống");
            }

            var message = new Messaging
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _messagingRepository.AddAsync(message, saveChanges: true);

                return new ServiceResponse<bool>(true, "Gửi tin nhắn thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, $"Lỗi khi gửi tin nhắn: {ex.Message}");
            }
        }

        // ================= GET CONVERSATION =================
        public async Task<ServiceResponse<List<MessagingResponseDTO>>> GetConversation(string senderId, string receiverId)
        {
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
            {
                return new ServiceResponse<List<MessagingResponseDTO>>(false, "Thiếu senderId hoặc receiverId");
            }
            var query = _messagingRepository.GetAllMessaging();
            var messages = query
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                         || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.CreatedAt)
                .ToList();

            var response = messages.Select(m => new MessagingResponseDTO
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                SenderName = m.Sender!.FullName,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver! .FullName,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return new ServiceResponse<List<MessagingResponseDTO>>(true, "Lấy cuộc trò chuyện thành công",response);
        }

        // ================= GET ALL MESSAGES BY USER =================
        public async Task<ServiceResponse<List<MessagingResponseDTO>>> GetAllMessagesByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ServiceResponse<List<MessagingResponseDTO>>(false, "UserId không được để trống");
            }
            var query = _messagingRepository.GetAllMessaging();
            var messages =  query
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            var response = messages.Select(m => new MessagingResponseDTO
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                SenderName = m.Sender!.FullName,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver!.FullName,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return new ServiceResponse<List<MessagingResponseDTO>>(true, "Lấy danh sách tin nhắn thành công",response);
        }

        // ================= MARK AS READ =================
        public async Task<ServiceResponse<bool>> MarkAsRead(int messageId)
        {
            var existingMessage = await _messagingRepository.GetMessagingById(messageId);
            if (existingMessage == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy tin nhắn");
            }

            try
            {
                existingMessage.IsRead = true;
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(true, "Đã đánh dấu tin nhắn là đã đọc");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetAllRelationship(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, "Không được để trống userName");

            var userInfo = await _accountRepository.GetAccountWithUserName(userId);
            if (userInfo == null)
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, "Không thể lấy được thông tin user");

            var query = _accountRepository.GetAllAccount()
                .Include(x => x.TeacherClasses)
                .Include(x => x.StudentClass)
                .Include(x => x.Group);

            List<AccountManagementResponseDTO> response = new List<AccountManagementResponseDTO>();

            // Case 1: user là học sinh → lấy giáo viên dạy cùng lớp + phụ huynh
            if (userInfo.StudentClass != null)
            {
                string classId = userInfo.StudentClass.ClassId?.ToString() ?? "";
                string parentEmail = userInfo.ParentEmail ?? "";

                var studentRelations = await query
                    .Where(x =>
                        // giáo viên cùng lớp
                        x.TeacherClasses.Any(tc => tc.ClassId.ToString() == classId)
                        // hoặc cha mẹ của học sinh
                        || (!string.IsNullOrEmpty(parentEmail) && x.Email == parentEmail)
                    )
                    .Select(x => new AccountManagementResponseDTO
                    {
                        UserName = x.UserName,
                        FullName = x.FullName,
                        Email = x.Email,
                        ParentEmail = x.ParentEmail,
                        GroupId = x.GroupId,
                        GroupName = x.Group!.GroupName ?? string.Empty,
                        CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy") ?? string.Empty
                    })
                    .ToListAsync();

                response.AddRange(studentRelations);
            }

            // Case 2: user là phụ huynh → lấy con + giáo viên dạy lớp con
            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                var children = await query
                    .Where(x => x.ParentEmail == userInfo.Email)
                    .Include(x => x.TeacherClasses)
                    .ToListAsync();

                foreach (var child in children)
                {
                    // thêm con vào response
                    response.Add(new AccountManagementResponseDTO
                    {
                        UserName = child.UserName,
                        FullName = child.FullName,
                        Email = child.Email,
                        ParentEmail = child.ParentEmail,
                        GroupId = child.GroupId,
                        GroupName = child.Group?.GroupName ?? string.Empty,
                        CreatedAt = child.CreatedAt?.ToString("dd/MM/yyyy") ?? string.Empty
                    });

                    string classId = child.StudentClass?.ClassId?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(classId))
                    {
                        // lấy giáo viên dạy lớp con
                        var teachers = await query
                            .Where(x => x.TeacherClasses.Any(tc => tc.ClassId.ToString() == classId))
                            .Select(x => new AccountManagementResponseDTO
                            {
                                UserName = x.UserName,
                                FullName = x.FullName,
                                Email = x.Email,
                                ParentEmail = x.ParentEmail,
                                GroupId = x.GroupId,
                                GroupName = x.Group!.GroupName ?? string.Empty,
                                CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy") ?? string.Empty
                            })
                            .ToListAsync();

                        response.AddRange(teachers);
                    }
                }
            }

            // Loại bỏ trùng lặp (nếu có)
            response = response.GroupBy(x => x.UserName).Select(g => g.First()).ToList();

            return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách thành công", response);
        }

    }
}
