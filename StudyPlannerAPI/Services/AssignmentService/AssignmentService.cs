using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AssignmentDetailDTO;
using StudyPlannerAPI.DTOs.AssignmentDTO;
using StudyPlannerAPI.Helper;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AssignmentRepository;
using System.Globalization;

namespace StudyPlannerAPI.Services.AssignmentService
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly StudyPlannerContext _context;

        public AssignmentService(IAssignmentRepository assignmentRepository, StudyPlannerContext context)
        {
            _assignmentRepository = assignmentRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddAssignment(AssignmentRequestDTO assignmentRequest)
        {
            if (assignmentRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(assignmentRequest.Deadline, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDeadline))
            {
                return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var assignment = new Assignment
            {

                ClassId = assignmentRequest.ClassId,
                TeacherId = assignmentRequest.TeacherId,
                Title = assignmentRequest.Title,
                Description = assignmentRequest.Description,
                Deadline = parseDeadline,
                CreatedAt = HelperTime.NowVN(),

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _assignmentRepository.AddAsync(assignment, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm bài tập thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Bài tập đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo bài tập  : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAssignment(int assignmentRequest)
        { 
            if (assignmentRequest <= 0)
            {
                return new ServiceResponse<bool>(false, "Mã bài tập sai");
            }
            var existingAssignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentRequest);
            if (existingAssignment == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy bài tập");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _assignmentRepository.DeleteAsync(assignmentRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa bài tập thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa bài tập do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa bài tập: {ex.Message}");
                }
            }
            
        }

        public async Task<ServiceResponse<List<AssignmentResponseDTO>>> GetAllAssignmentByClassAsync(string classId)
        {
            if (string.IsNullOrEmpty(classId))
            {
                return new ServiceResponse<List<AssignmentResponseDTO>>(false, "Mã lớp không được để trống");
            }
            var query = _assignmentRepository.GetAllAssignments();
            var response = query.Where(a => a.ClassId == classId)
                            .Select(a => new AssignmentResponseDTO
                            {
                                AssignmentId = a.AssignmentId,
                                ClassId = a.ClassId,
                                ClassName = a.Class!.ClassName,   // join sang bảng Class
                                TeacherId = a.TeacherId,
                                TeacherName = a.Teacher!.FullName, // join sang bảng Teacher
                                Title = a.Title,
                                Description = a.Description,
                                Deadline = a.Deadline!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                CreatedAt = a.CreatedAt!.Value.ToString("dd/MM/yyy HH:mm:ss"),

                                // map submissions (AssignmentDetails)
                                assignments = a.AssignmentDetails.Select(d => new AssignmentDetailResponseDTO
                                {
                                    AssignmentId = d.AssignmentId,
                                    StudentId = d.StudentId,
                                    StudentName = d.Student.FullName, // join sang bảng Student
                                    FilePath = d.FilePath??"",
                                    StatusId = d.StatusId,
                                    StatusName = d.Status!.StatusName, // join sang bảng Status
                                    SubmittedAt = d.SubmittedAt.HasValue ? d.SubmittedAt.Value.ToString("dd/MM/yyy HH:mm:ss") : null,
                                    Grade = d.Grade??0
                                }).ToList()
                            }).ToList();
            return new ServiceResponse<List<AssignmentResponseDTO>>(true, "Lấy danh sách theo lớp thành công", response);
                 
        }

        public async Task<ServiceResponse<List<AssignmentResponseDTO>>> GetAllAssignmentByTeacherAsync(string teacherId, string classId)
        {

            if (string.IsNullOrEmpty(teacherId))
            {
                return new ServiceResponse<List<AssignmentResponseDTO>>(false, "Mã giáo viên không được để trống");
            }
            var query = _assignmentRepository.GetAllAssignments();
            var response = query.Where(a => a.TeacherId == teacherId && a.ClassId == classId)
                            .Select(a => new AssignmentResponseDTO
                            {
                                AssignmentId = a.AssignmentId,
                                ClassId = a.ClassId,
                                ClassName = a.Class!.ClassName,   // join sang bảng Class
                                TeacherId = a.TeacherId,
                                TeacherName = a.Teacher!.FullName, // join sang bảng Teacher
                                Title = a.Title,
                                Description = a.Description,
                                Deadline = a.Deadline!.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                CreatedAt = a.CreatedAt!.Value.ToString("dd/MM/yyy HH:mm:ss"),

                                // map submissions (AssignmentDetails)
                                assignments = a.AssignmentDetails.Select(d => new AssignmentDetailResponseDTO
                                {
                                    AssignmentId = d.AssignmentId,
                                    StudentId = d.StudentId,
                                    StudentName = d.Student.FullName, // join sang bảng Student
                                    FilePath = d.FilePath ?? "",
                                    StatusId = d.StatusId,
                                    StatusName = d.Status!.StatusName, // join sang bảng Status
                                    SubmittedAt = d.SubmittedAt.HasValue ? d.SubmittedAt.Value.ToString("dd/MM/yyy HH:mm:ss") : null,
                                    Grade = d.Grade ?? 0
                                }).ToList()
                            }).ToList();
            return new ServiceResponse<List<AssignmentResponseDTO>>(true, "Lấy danh sách theo giáo viên thành công", response);
        }

        public async Task<ServiceResponse<bool>> UpdateAssignment(AssignmentRequestDTO assignmentRequest)
        {
            if (assignmentRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingAssignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentRequest.AssignmentId);
            if(existingAssignment == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy bài tập");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy HH:mm:ss",
                 "yyyy-MM-ddTHH:mm:ss.fffZ" // ISO 8601
            };
            if (!DateTime.TryParseExact(assignmentRequest.Deadline, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDeadline))
            {
                return new ServiceResponse<bool>(false, "Giờ hết hạn không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
           
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingAssignment.Description = assignmentRequest.Description;
                    existingAssignment.Deadline = parseDeadline;
                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật bài tập thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Bài tập đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo bài tập  : {ex.Message}");
                }
            }
        }
    }
}
