using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.TeacherClassDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.TeacherClassRepository;

namespace StudyPlannerAPI.Services.TeacherClassService
{
    public class TeacherClassService : ITeacherClassService
    {
        private readonly StudyPlannerContext _context;
        private readonly ITeacherClassRepository _teacherClassRepository;

        public TeacherClassService(StudyPlannerContext context, ITeacherClassRepository teacherClassRepository)
        {
            _context = context;
            _teacherClassRepository = teacherClassRepository;
        }

        public async Task<ServiceResponse<bool>> AddTeacherClass(TeacherClassRequestDTO teacherClassRequest)
        {
            if (teacherClassRequest == null || string.IsNullOrEmpty(teacherClassRequest.ClassId) || string.IsNullOrEmpty(teacherClassRequest.TeacherId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var newTeacherClass = new TeacherClass
            {
                ClassId = teacherClassRequest.ClassId,
                TeacherId = teacherClassRequest.TeacherId,
                Subject = teacherClassRequest.Subject,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _teacherClassRepository.AddAsync(newTeacherClass, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm phân công thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Phân công đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo phân công : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTeacherClass(string classId, string teacherId)
        {
            if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(teacherId))
            {
                return new ServiceResponse<bool>(false, "Không được để trống phân công");
            }
            var existing = await _teacherClassRepository.GetTeacherClassByIdAsync(classId, teacherId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Phân công không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _teacherClassRepository.DeleteTwoKeyAsync(classId, teacherId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa phân công thành công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa phân công do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa phân công: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<TeacherClassResponseDTO>>> GetClassByTeacherID(string teacherId)
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return new ServiceResponse<List<TeacherClassResponseDTO>>(false, "Mã giáo viên không được để trống");
            }
            var query = _teacherClassRepository.GetAllTeacherClass();
            if (query == null)
            {
                return new ServiceResponse<List<TeacherClassResponseDTO>>(false, "Phân công không tồn tại");
            }
            var response = query.Where(x => x.TeacherId == teacherId)
                                 .Select(x => new TeacherClassResponseDTO
                                 {
                                     ClassId = x.ClassId,
                                     Subject = x.Subject,
                                     TeacherId = x.TeacherId,
                                     TeacherName = x.Teacher.FullName!
                                 }).ToList();

            return new ServiceResponse<List<TeacherClassResponseDTO>>(true, "Lấy thông tin phân công thành công", response);
        }

        public async Task<ServiceResponse<List<TeacherClassResponseDTO>>> GetTeacherByClassID(string classId)
        {
            if (string.IsNullOrEmpty(classId) )
            {
                return new ServiceResponse<List<TeacherClassResponseDTO>>(false, "Mã lớp không được để trống");
            }
            var query =  _teacherClassRepository.GetAllTeacherClass();
            if (query == null)
            {
                return new ServiceResponse<List<TeacherClassResponseDTO>>(false, "Phân công không tồn tại");
            }
            var response  = query.Where(x=>x.ClassId == classId)
                                 .Select(x=>new TeacherClassResponseDTO
                                 {
                                     ClassId = x.ClassId,
                                     Subject = x.Subject,
                                     TeacherId = x.TeacherId,
                                     TeacherName = x.Teacher.FullName!
                                 }).ToList();

            return new ServiceResponse<List<TeacherClassResponseDTO>>(true, "Lấy thông tin phân công thành công", response);
        }

        public async Task<ServiceResponse<TeacherClassResponseDTO>> GetTeacherClassByID(string classId, string teacherId)
        {
            if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(teacherId))
            {
                return new ServiceResponse<TeacherClassResponseDTO>(false, "Mã phân công không được để trống");
            }
            var query = await _teacherClassRepository.GetTeacherClassByIdAsync(classId, teacherId);
            if (query == null)
            {
                return new ServiceResponse<TeacherClassResponseDTO>(false, "Phân công không tồn tại");
            }
            var teacherClassResponse = new TeacherClassResponseDTO
            {
                ClassId = classId,
                TeacherId = teacherId,
                TeacherName = query.Teacher.FullName ?? "",
                Subject = query.Subject,
            };

            return new ServiceResponse<TeacherClassResponseDTO>(true, "Lấy thông tin phân công thành công", teacherClassResponse);
        }

        public async Task<ServiceResponse<PagedResponse<TeacherClassResponseDTO>>> GetTeacherClassListAsync(string classId,int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<TeacherClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query =  _teacherClassRepository.GetAllTeacherClass();
            var totalCount =  query.Where(x => x.ClassId == classId).Count();
            var lstTeacherClass =  query.Where(x => x.ClassId == classId)
                                .Select(x => new TeacherClassResponseDTO
                                {
                                    ClassId = x.ClassId,
                                    TeacherId = x.TeacherId,
                                    TeacherName = x.Teacher.FullName ?? "",
                                    Subject = x.Subject,
                                })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<TeacherClassResponseDTO>(lstTeacherClass, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<TeacherClassResponseDTO>>(true, "Lấy danh sách phân công thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<TeacherClassResponseDTO>>> SearchTeacherClassListAsync(string classId,string textToSearch, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<TeacherClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query =  _teacherClassRepository.SearchTeacherClassByText(textToSearch);
            var totalCount =  query.Where(x => x.ClassId == classId).Count();
            var lstTeacherClass =  query.Where(x => x.ClassId == classId)
                .Select(x => new TeacherClassResponseDTO
            {
                ClassId = x.ClassId,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher.FullName??"",
                Subject = x.Subject,
            })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<TeacherClassResponseDTO>(lstTeacherClass, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<TeacherClassResponseDTO>>(true, "Lấy danh sách phân công sau khi tìm thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateTeacherClass(TeacherClassRequestDTO teacherClassRequest)
        {
            if (teacherClassRequest == null || string.IsNullOrEmpty(teacherClassRequest.ClassId) || string.IsNullOrEmpty(teacherClassRequest.TeacherId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existing = await _teacherClassRepository.GetTeacherClassByIdAsync(teacherClassRequest.ClassId, teacherClassRequest.TeacherId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Phân công không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existing.Subject = teacherClassRequest.Subject;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật phân công thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật phân công : {ex.Message}");
                }
            }
        }
    }
}