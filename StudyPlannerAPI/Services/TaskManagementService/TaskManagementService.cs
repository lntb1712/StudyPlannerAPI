using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.TaskManagementDTO;
using StudyPlannerAPI.Helper;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.TaskManagementRepository;
using System.Globalization;

namespace StudyPlannerAPI.Services.TaskManagementService
{
    public class TaskManagementService : ITaskManagementService
    {
        private readonly ITaskManagementRepository _taskManagementRepository;
        private readonly StudyPlannerContext _context;

        public TaskManagementService(ITaskManagementRepository taskManagementRepository, StudyPlannerContext context)
        {
            _taskManagementRepository = taskManagementRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddTaskManagement(TaskManagementRequestDTO taskManagementRequestDTO)
        {
            if (taskManagementRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(taskManagementRequestDTO.DueDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDueDate))
            {
                return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var taskManagement = new TaskManagement
            {
                StudentId = taskManagementRequestDTO.StudentId,
                Title = taskManagementRequestDTO.Title,
                Description = taskManagementRequestDTO.Description,
                DueDate = parseDueDate,
                StatusId = 1,
                CreatedAt = HelperTime.NowVN(),
                UpdatedAt = HelperTime.NowVN(),

            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await  _taskManagementRepository.AddAsync(taskManagement, saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm việc cần làm thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Việc cần làm đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo việc cần làm  : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTaskManagement(int taskId)
        {
            if (taskId < 0)
            {
                return new ServiceResponse<bool>(false, "Mã việc cần làm phải lớn hơn 0");
            }
            var existingTask = await _taskManagementRepository.GetTaskManagementById(taskId);
            if(existingTask== null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy việc cần xóa");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _taskManagementRepository.DeleteAsync(taskId,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa việc cần làm thành công"); 
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa việc cần làm do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa việc cần làm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<TaskManagementResponseDTO>>> GetTaskManagementAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<List<TaskManagementResponseDTO>>(false, "Mã học sinh không được để trống");
            }
            var query = _taskManagementRepository.GetAllTaskManagement();
            var lstTask = query.Where(x => x.StudentId == studentId)
                               .Select(x => new TaskManagementResponseDTO
                               {
                                   TaskId = x.TaskId,
                                   StudentId = x.StudentId,
                                   StudentName = x.Student!.FullName,
                                   Title = x.Title,
                                   Description = x.Description,
                                   DueDate = x.DueDate!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                   CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                   StatusId = x.StatusId,
                                   StatusName = x.Status!.StatusName!,
                                   UpdatedAt = x.UpdatedAt!.Value.ToString("dd/MM/yyyy HH:mm:ss")
                               }).ToList();
            return new ServiceResponse<List<TaskManagementResponseDTO>>(true, "Lấy danh sách thành công", lstTask);
        }

        public async Task<ServiceResponse<bool>> UpdateTaskManagement(TaskManagementRequestDTO taskManagementRequestDTO)
        {
            if(taskManagementRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existingTask = await _taskManagementRepository.GetTaskManagementById(taskManagementRequestDTO.TaskId);
            if (existingTask == null)
            {
                return new ServiceResponse<bool>(false, "Việc làm cần tìm không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingTask.Title = taskManagementRequestDTO.Title;
                    existingTask.Description = taskManagementRequestDTO.Description;
                    existingTask.StatusId = taskManagementRequestDTO.StatusId;
                    string[] formats = {
                        "M/d/yyyy h:mm:ss tt",
                        "MM/dd/yyyy hh:mm:ss tt",
                        "dd/MM/yyyy HH:mm:ss",
                         "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
                    };
                    if (!DateTime.TryParseExact(taskManagementRequestDTO.DueDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDueDate))
                    {
                        return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
                    }
                    existingTask.DueDate = parseDueDate;
                    existingTask.UpdatedAt = HelperTime.NowVN();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật việc cần làm thành công");

                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Việc cần làm đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo việc cần làm  : {ex.Message}");
                }
            }
        }
    }
}
