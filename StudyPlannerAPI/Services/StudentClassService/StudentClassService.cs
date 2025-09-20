using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.StudentClassDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.StudentClassRepository;

namespace StudyPlannerAPI.Services.StudentClassService
{
    public class StudentClassService : IStudentClassService
    {
        private readonly StudyPlannerContext _context;
        private readonly IStudentClassRepository _studentClassRepository;

        public StudentClassService(StudyPlannerContext context, IStudentClassRepository studentClassRepository)
        {
            _context = context;
            _studentClassRepository = studentClassRepository;
        }

        public async Task<ServiceResponse<bool>> AddStudentClass(StudentClassRequestDTO studentClassRequest)
        {
            if (studentClassRequest == null || string.IsNullOrEmpty(studentClassRequest.ClassId) || string.IsNullOrEmpty(studentClassRequest.StudentId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var newStudentClass = new StudentClass
            {
                ClassId = studentClassRequest.ClassId,
                StudentId = studentClassRequest.StudentId,
                StudyStatus = studentClassRequest.StudyStatus,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _studentClassRepository.AddAsync(newStudentClass, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm sinh viên lớp thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Sinh viên lớp đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo sinh viên lớp : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStudentClass(string classId, string studentId)
        {
            if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<bool>(false, "Không được để trống sinh viên lớp");
            }
            var existing = await _studentClassRepository.GetStudentClassbyIdAsync(classId, studentId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Sinh viên lớp không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _studentClassRepository.DeleteTwoKeyAsync(classId, studentId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa sinh viên lớp thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa sinh viên lớp do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sinh viên lớp: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<StudentClassResponseDTO>> GetStudentClassById(string classId, string studentId)
        {
            if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(studentId))
            {
                return new ServiceResponse<StudentClassResponseDTO>(false, "Mã sinh viên lớp không được để trống");
            }
            var query = await _studentClassRepository.GetStudentClassbyIdAsync(classId, studentId);
            if (query == null)
            {
                return new ServiceResponse<StudentClassResponseDTO>(false, "Sinh viên lớp không tồn tại");
            }
            var studentClassResponse = new StudentClassResponseDTO
            {
                ClassId = classId,
                StudentId = studentId
            };

            return new ServiceResponse<StudentClassResponseDTO>(true, "Lấy thông tin sinh viên lớp thành công", studentClassResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StudentClassResponseDTO>>> GetStudentClassListAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<StudentClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query =  _studentClassRepository.GetAllStudentClass();
            var totalCount =  query.Count();
            var lstStudentClass =  query.Select(x => new StudentClassResponseDTO
            {
                ClassId = x.ClassId,
                StudentId = x.StudentId
            })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<StudentClassResponseDTO>(lstStudentClass, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<StudentClassResponseDTO>>(true, "Lấy danh sách sinh viên lớp thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StudentClassResponseDTO>>> SearchStudentClassListAsync(string textToSearch, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<StudentClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query =  _studentClassRepository.SearchStudentClassByText(textToSearch);
            var totalCount =  query.Count();
            var lstStudentClass =  query.Select(x => new StudentClassResponseDTO
            {
                ClassId = x.ClassId,
                StudentId = x.StudentId
            })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<StudentClassResponseDTO>(lstStudentClass, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<StudentClassResponseDTO>>(true, "Lấy danh sách sinh viên lớp sau khi tìm thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateStudentClass(StudentClassRequestDTO studentClassRequest)
        {
            if (studentClassRequest == null || string.IsNullOrEmpty(studentClassRequest.ClassId) || string.IsNullOrEmpty(studentClassRequest.StudentId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existing = await _studentClassRepository.GetStudentClassbyIdAsync(studentClassRequest.ClassId, studentClassRequest.StudentId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Sinh viên lớp không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // No additional fields to update in this model; commit if exists
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật sinh viên lớp thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật sinh viên lớp : {ex.Message}");
                }
            }
        }
    }
}