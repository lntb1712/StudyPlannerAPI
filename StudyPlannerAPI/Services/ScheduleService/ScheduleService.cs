using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ScheduleDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.ScheduleRepository;
using System.Globalization;
using System.Security.Cryptography.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StudyPlannerAPI.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly StudyPlannerContext _context;

        public ScheduleService(IScheduleRepository scheduleRepository, StudyPlannerContext context)
        {
            _scheduleRepository = scheduleRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> CreateScheduleAsync(ScheduleRequestDTO scheduleRequest)
        {
            if (string.IsNullOrEmpty(scheduleRequest.StudentId) || string.IsNullOrEmpty(scheduleRequest.TeacherId) || scheduleRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(scheduleRequest.StartTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseStartTime))
            {
                return new ServiceResponse<bool>(false, "Giờ bắt đầu không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            if (!DateTime.TryParseExact(scheduleRequest.EndTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseEndTime))
            {
                return new ServiceResponse<bool>(false, "Giờ kết thúc không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var schedule = new Schedule
            {
                StudentId = scheduleRequest.StudentId,
                ClassId = scheduleRequest.ClassId,
                TeacherId = scheduleRequest.TeacherId,
                Subject = scheduleRequest.Subject,
                DayOfWeek = scheduleRequest.DayOfWeek,
                StartTime = parseStartTime,
                EndTime = parseEndTime,
                StatusId = scheduleRequest.StatusId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _scheduleRepository.AddAsync(schedule, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Tạo lịch  thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Lịch đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo lịch : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteScheduleAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                return new ServiceResponse<bool>(false, "ID lịch không hợp lệ");
            }
            var existingSchedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (existingSchedule == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lịch");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _scheduleRepository.DeleteAsync(scheduleId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lịch thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lịch do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lịch: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<ScheduleResponseDTO>>> GetAllSchedulesAsync(string studentId)
        {

            if (string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<List<ScheduleResponseDTO>>(false, "ID sinh viên không hợp lệ");
            }

            var query = _scheduleRepository.GetAllSchedulesAsync();
            var filteredSchedules = query.Where(s => s.StudentId == studentId)
                                         .Select(x => new ScheduleResponseDTO
                                         {
                                             ScheduleId = x.ScheduleId,
                                             StudentId = x.StudentId,
                                             StudentName = x.Student != null ? x.Student.FullName : null,
                                             ClassId = x.ClassId,
                                             ClassName = x.Class != null ? x.Class.ClassName : null,
                                             TeacherId = x.TeacherId,
                                             TeacherName = x.Teacher != null ? x.Teacher.FullName : null,
                                             Subject = x.Subject,
                                             DayOfWeek = x.DayOfWeek,
                                             StartTime = x.StartTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             EndTime = x.EndTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             StatusId = x.StatusId,
                                             StatusName = x.Status != null ? x.Status.StatusName : null,
                                             CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             UpdatedAt = x.UpdatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")

                                         }).ToList();
            return new ServiceResponse<List<ScheduleResponseDTO>>(true, "Lấy lịch thành công", filteredSchedules);
        }

        public async Task<ServiceResponse<ScheduleResponseDTO>> GetScheduleByIdAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                return new ServiceResponse<ScheduleResponseDTO>(false, "ID lịch không hợp lệ");
            }

            var query = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            var filteredSchedules = new ScheduleResponseDTO
            {
                ScheduleId = query.ScheduleId,
                StudentId = query.StudentId,
                StudentName = query.Student != null ? query.Student.FullName : null,
                ClassId = query.ClassId,
                ClassName = query.Class != null ? query.Class.ClassName : null,
                TeacherId = query.TeacherId,
                TeacherName = query.Teacher != null ? query.Teacher.FullName : null,
                Subject = query.Subject,
                DayOfWeek = query.DayOfWeek,
                StartTime = query.StartTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                EndTime = query.EndTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                StatusId = query.StatusId,
                StatusName = query.Status != null ? query.Status.StatusName : null,
                CreatedAt = query.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedAt = query.UpdatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")

            };
            return new ServiceResponse<ScheduleResponseDTO>(true, "Lấy lịch thành công", filteredSchedules);
        }

        public async Task<ServiceResponse<List<ScheduleResponseDTO>>> SearchSchedulesAsync(string studentId, string textToSearch)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<List<ScheduleResponseDTO>>(false, "ID sinh viên không hợp lệ");
            }

            var query =  _scheduleRepository.SearchScheduleAsync(textToSearch);
            var filteredSchedules = query.Where(s => s.StudentId == studentId)
                                         .Select(x => new ScheduleResponseDTO
                                         {
                                             ScheduleId = x.ScheduleId,
                                             StudentId = x.StudentId,
                                             StudentName = x.Student != null ? x.Student.FullName : null,
                                             ClassId = x.ClassId,
                                             ClassName = x.Class != null ? x.Class.ClassName : null,
                                             TeacherId = x.TeacherId,
                                             TeacherName = x.Teacher != null ? x.Teacher.FullName : null,
                                             Subject = x.Subject,
                                             DayOfWeek = x.DayOfWeek,
                                             StartTime = x.StartTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             EndTime = x.EndTime!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             StatusId = x.StatusId,
                                             StatusName = x.Status != null ? x.Status.StatusName : null,
                                             CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                             UpdatedAt = x.UpdatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")

                                         }).ToList();
            return new ServiceResponse<List<ScheduleResponseDTO>>(true, "Lấy lịch thành công", filteredSchedules);
        }

        public async Task<ServiceResponse<bool>> UpdateScheduleAsync(int scheduleId, ScheduleRequestDTO scheduleRequest)
        {
            if (string.IsNullOrEmpty(scheduleRequest.StudentId) || string.IsNullOrEmpty(scheduleRequest.TeacherId) || scheduleRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(scheduleRequest.StartTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseStartTime))
            {
                return new ServiceResponse<bool>(false, "Giờ bắt đầu không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            if (!DateTime.TryParseExact(scheduleRequest.EndTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseEndTime))
            {
                return new ServiceResponse<bool>(false, "Giờ kết thúc không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }

            var existingSchedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (existingSchedule == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lịch");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingSchedule.StudentId = scheduleRequest.StudentId;
                    existingSchedule.ClassId = scheduleRequest.ClassId;
                    existingSchedule.TeacherId = scheduleRequest.TeacherId;
                    existingSchedule.Subject = scheduleRequest.Subject;
                    existingSchedule.DayOfWeek = scheduleRequest.DayOfWeek;
                    existingSchedule.StartTime = parseStartTime;
                    existingSchedule.EndTime = parseEndTime;
                    existingSchedule.StatusId = scheduleRequest.StatusId;
                    existingSchedule.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lịch thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Lịch đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lịch: {ex.Message}");
                }
            }
        }
    }
}
