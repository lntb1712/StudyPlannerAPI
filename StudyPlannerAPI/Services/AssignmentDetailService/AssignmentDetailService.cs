using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AssignmentDetailDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AssignmentDetailRepository;
using StudyPlannerAPI.Services.CloudinaryService;
using System.Globalization;

namespace StudyPlannerAPI.Services.AssignmentDetailService
{
    public class AssignmentDetailService : IAssignmentDetailService
    {
        private readonly IAssignmentDetailRepository _assignmentDetailRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly StudyPlannerContext _context;

        public AssignmentDetailService(IAssignmentDetailRepository assignmentDetailRepository,ICloudinaryService cloudinaryService, StudyPlannerContext context)
        {
            _assignmentDetailRepository = assignmentDetailRepository;
            _cloudinaryService = cloudinaryService;
            _context = context;
        }

        public async Task<ServiceResponse<List<AssignmentDetailResponseDTO>>> GetAllByAssignmentAsync(int assignmentId)
        {
            if (assignmentId <= 0)
            {
                return new ServiceResponse<List<AssignmentDetailResponseDTO>>(false, "Mã bài tập không hợp lệ");
            }

            var query = _context.AssignmentDetails
                .Include(d => d.Student)
                .Include(d => d.Status)
                .Where(d => d.AssignmentId == assignmentId);

            var response = await query.Select(d => new AssignmentDetailResponseDTO
            {
                AssignmentId = d.AssignmentId,
                StudentId = d.StudentId,
                StudentName = d.Student!.FullName,
                FilePath = d.FilePath,
                StatusId = d.StatusId,
                StatusName = d.Status!.StatusName,
                SubmittedAt = d.SubmittedAt.HasValue ? d.SubmittedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                Grade = d.Grade
            }).ToListAsync();

            return new ServiceResponse<List<AssignmentDetailResponseDTO>>(true, "Lấy danh sách nộp bài thành công", response);
        }

        public async Task<ServiceResponse<AssignmentDetailResponseDTO>> GetByStudentAsync(int assignmentId, string studentId)
        {
            if (assignmentId <= 0 || string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<AssignmentDetailResponseDTO>(false, "Mã bài tập hoặc mã học sinh không hợp lệ");
            }

            var entity = await _assignmentDetailRepository.GetAssignmentDetailByStudentAsync(assignmentId, studentId);
            if (entity == null)
            {
                return new ServiceResponse<AssignmentDetailResponseDTO>(false, "Không tìm thấy bài nộp");
            }

            var response = new AssignmentDetailResponseDTO
            {
                AssignmentId = entity.AssignmentId,
                StudentId = entity.StudentId,
                StudentName = entity.Student!.FullName,
                FilePath = entity.FilePath,
                StatusId = entity.StatusId,
                StatusName = entity.Status!.StatusName,
                SubmittedAt = entity.SubmittedAt.HasValue ? entity.SubmittedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                Grade = entity.Grade
            };

            return new ServiceResponse<AssignmentDetailResponseDTO>(true, "Lấy bài nộp thành công", response);
        }

        public async Task<ServiceResponse<bool>> AddAssignmentDetail(AssignmentDetailRequestDTO request)
        {
            if (request.AssignmentId <= 0 || string.IsNullOrEmpty(request.StudentId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu không hợp lệ");
            }

            var existing = await _assignmentDetailRepository.GetAssignmentDetailByStudentAsync(request.AssignmentId, request.StudentId);
            if (existing != null)
            {
                return new ServiceResponse<bool>(false, "Học sinh đã nộp bài cho bài tập này");
            }

            // Lấy studentName từ DB (nếu có)
            var student = await _context.AccountManagements
                .AsNoTracking()
                .Where(s => s.UserName == request.StudentId)
                .Select(s => new { s.UserName, s.FullName })
                .FirstOrDefaultAsync();

            string folderNameSafe;
            if (student != null && !string.IsNullOrWhiteSpace(student.FullName))
            {
                // sanitise: thay space -> underscore, loại ký tự đặc biệt đơn giản
                string safe = student.FullName.Trim()
                    .Replace(" ", "_")
                    .Replace("/", "_")
                    .Replace("\\", "_");
                folderNameSafe = $"assignments/{safe}";
            }
            else
            {
                folderNameSafe = $"assignments/{request.StudentId}";
            }

            string? fileUrl = null;
            if (request.File != null && request.File.Length > 0)
            {
                fileUrl = await _cloudinaryService.UploadFileAsync(request.File, folderNameSafe);
                if (fileUrl == null)
                    return new ServiceResponse<bool>(false, "Không thể upload file lên storage");
            }

            var detail = new AssignmentDetail
            {
                AssignmentId = request.AssignmentId,
                StudentId = request.StudentId,
                StatusId = 1, // ví dụ mặc định trạng thái = đã nộp
                FilePath = fileUrl,
                SubmittedAt = DateTime.Now,
                Grade = null
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _assignmentDetailRepository.AddAsync(detail, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Nộp bài thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không hợp lệ");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi nộp bài: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAssignmentDetail(int assignmentId, string studentId)
        {
            if (assignmentId <= 0 || string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<bool>(false, "Mã bài tập hoặc mã học sinh không hợp lệ");
            }

            var existing = await _assignmentDetailRepository.GetAssignmentDetailByStudentAsync(assignmentId, studentId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy bài nộp");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _assignmentDetailRepository.DeleteTwoKeyAsync(assignmentId, studentId, saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa bài nộp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa bài nộp do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa bài nộp: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAssignmentDetail(AssignmentDetailRequestDTO request)
        {
            if (request == null || request.AssignmentId <= 0 || string.IsNullOrEmpty(request.StudentId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existing = await _assignmentDetailRepository.GetAssignmentDetailByStudentAsync(request.AssignmentId, request.StudentId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy bài nộp");
            }

            DateTime? submittedAt = existing.SubmittedAt;
            if (!string.IsNullOrEmpty(request.SubmittedAt))
            {
                string[] formats = {
                    "M/d/yyyy h:mm:ss tt",
                    "MM/dd/yyyy hh:mm:ss tt",
                    "dd/MM/yyyy HH:mm:ss",
                    "yyyy-MM-ddTHH:mm:ss.fffZ"
                };
                if (!DateTime.TryParseExact(request.SubmittedAt, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseSubmittedAt))
                {
                    return new ServiceResponse<bool>(false, "Thời gian nộp không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy HH:mm:ss hoặc M/d/yyyy h:mm:ss tt.");
                }
                submittedAt = parseSubmittedAt;
            }

       

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existing.StatusId = request.StatusId ?? existing.StatusId;
                    existing.FilePath = request.FilePath ?? existing.FilePath;
                    existing.SubmittedAt = submittedAt;
                    existing.Grade = request.Grade;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật bài nộp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không hợp lệ");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật bài nộp: {ex.Message}");
                }
            }
        }
    }
}