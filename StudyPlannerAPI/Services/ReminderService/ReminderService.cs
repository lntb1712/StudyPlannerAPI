using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ReminderDTO;
using StudyPlannerAPI.DTOs.ScheduleDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.ReminderRepository;
using System.Globalization;

namespace StudyPlannerAPI.Services.ReminderService
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IAccountManagementRepository _accountManagementRepository;
        private readonly StudyPlannerContext _context;

        public ReminderService(IReminderRepository reminderRepository, IAccountManagementRepository accountManagementRepository, StudyPlannerContext context)
        {
            _reminderRepository = reminderRepository;
            _accountManagementRepository = accountManagementRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddReminder(ReminderRequestDTO reminderRequest)
        {
            if (string.IsNullOrEmpty(reminderRequest.ParentId)  || reminderRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var studentOfParent = await _accountManagementRepository
               .GetAllAccount()
               .FirstOrDefaultAsync(x => x.ParentEmail == reminderRequest.ParentId);

            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(reminderRequest.DueDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDueDate))
            {
                return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var reminder = new Reminder
            {
                ParentId = reminderRequest.ParentId,
                StudentId = studentOfParent!.UserName,
                Content = reminderRequest.Content,
                DueDate = parseDueDate,
                StatusId = 1,
                CreatedAt = DateTime.Now
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _reminderRepository.AddAsync(reminder, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm nhắc nhở thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Nhắc nhở đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo nhắc nhở  : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteReminder(int reminderId)
        {
            if (reminderId < 0)
            {
                return new ServiceResponse<bool>(false, "Mã nhắc nhở sai ");
            }

            var existingReminder = await _reminderRepository.GetReminderByIdAsync(reminderId);
            if (existingReminder == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy nhắc nhở");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _reminderRepository.DeleteAsync(reminderId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa nhắc nhở thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa nhắc nhở do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa nhắc nhở: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<ReminderResponseDTO>> GetReminderById(int reminderId)
        {
            if (reminderId < 0)
            {
                return new ServiceResponse<ReminderResponseDTO>(false, "Mã nhắc nhở không đúng");
            }

            var reminder = await _reminderRepository.GetReminderByIdAsync(reminderId);
            var reminderResponse = new ReminderResponseDTO
            {
                ReminderId = reminder.ReminderId,
                ParentId = reminder.ParentId,
                ParentFullName = reminder.Parent!.FullName,
                StudentId = reminder.StudentId,
                StudentFullName = reminder.Student!.FullName,
                Content = reminder.Content,
                StatusId = reminder.StatusId,
                StatusName = reminder.Status!.StatusName!,
                DueDate = reminder.DueDate!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                CreatedAt = reminder.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")

            };
            return new ServiceResponse<ReminderResponseDTO>(true, "Lấy thông tin nhắc nhở thành công", reminderResponse);
        }

        public async Task<ServiceResponse<List<ReminderResponseDTO>>> GetReminderByParentOrStudent(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new ServiceResponse<List<ReminderResponseDTO>>(false, "Tên tìm nhắc nhở không đúng");
            }
            var studentOfParent = await _accountManagementRepository
              .GetAllAccount()
              .FirstOrDefaultAsync(x => x.ParentEmail == userName);

            var query = _reminderRepository.GetAllReminderAsync();
            var response = query.Where(x => x.ParentId == userName || x.StudentId == studentOfParent!.UserName)
                                .OrderBy(x => x.StatusId)
                                .Select(x => new ReminderResponseDTO
                                {
                                    ReminderId = x.ReminderId,
                                    ParentId = x.ParentId,
                                    ParentFullName = x.Parent!.FullName,
                                    StudentId = x.StudentId,
                                    StudentFullName = x.Student!.FullName,
                                    Content = x.Content,
                                    StatusId = x.StatusId,
                                    StatusName = x.Status!.StatusName!,
                                    DueDate = x.DueDate!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                    CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")
                                }).ToList();
            return new ServiceResponse<List<ReminderResponseDTO>>(true, "Lấy danh sách nhắc nhở thành công", response);
        }

        public async Task<ServiceResponse<bool>> UpdateReminder(ReminderRequestDTO reminderRequest)
        {
            if (string.IsNullOrEmpty(reminderRequest.ParentId) || string.IsNullOrEmpty(reminderRequest.StudentId) || reminderRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingReminder = await _reminderRepository.GetReminderByIdAsync(reminderRequest.ReminderId);
            if (existingReminder == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy nhắc nhở");
            }

            var studentOfParent = await _accountManagementRepository
               .GetAllAccount()
               .FirstOrDefaultAsync(x => x.ParentEmail == reminderRequest.ParentId);

            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(reminderRequest.DueDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDueDate))
            {
                return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingReminder.Content = reminderRequest.Content;
                    existingReminder.DueDate = parseDueDate;
                    existingReminder.StatusId = reminderRequest.StatusId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật nhắc nhở thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Nhắc nhở đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật nhắc nhở: {ex.Message}");
                }
            }
        }
    }
}
