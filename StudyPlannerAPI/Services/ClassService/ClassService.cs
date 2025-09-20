using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.ClassDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.ClassRepository;

namespace StudyPlannerAPI.Services.ClassService
{
    public class ClassService : IClassService
    {
        private readonly StudyPlannerContext _context;
        private readonly IClassRepository _classRepository;
        public ClassService(StudyPlannerContext context, IClassRepository classRepository)
        {
            _context = context;
            _classRepository = classRepository;
        }

        public async Task<ServiceResponse<bool>> AddClass(ClassRequestDTO classRequest)
        {
            if (classRequest == null || string.IsNullOrEmpty(classRequest.ClassId) || string.IsNullOrEmpty(classRequest.ClassName))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var newClass = new Class
            {
                ClassId = classRequest.ClassId,
                ClassName = classRequest.ClassName,

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _classRepository.AddAsync(newClass, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm lớp thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Lớp đã tồn tại");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo lớp : {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteClass(string classId)
        {
            if (string.IsNullOrEmpty(classId))
            {
                return new ServiceResponse<bool>(false, "Không được để trống lớp");
            }
            var existing = await _classRepository.GetClassByIdAsync(classId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Lớp không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _classRepository.DeleteAsync(classId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lớp thanh công");
                }
                catch (DbUpdateException dbEx)
                {

                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lớp do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lớp: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<ClassResponseDTO>> GetClassById(string classId)
        {
            if (string.IsNullOrEmpty(classId))
            {
                return new ServiceResponse<ClassResponseDTO>(false, "Mã lớp không được để trống");
            }
            var query = await _classRepository.GetClassByIdAsync(classId);
            if (query == null)
            {
                return new ServiceResponse<ClassResponseDTO>(false, "Lớp không tồn tại");
            }
            var classResponse = new ClassResponseDTO
            {
                ClassId = classId,
                ClassName = query.ClassName,
            };

            return new ServiceResponse<ClassResponseDTO>(true, "Lấy thông tin lớp thành công", classResponse);
        }

        public async Task<ServiceResponse<PagedResponse<ClassResponseDTO>>> GetClassListAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<ClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query = _classRepository.GetAllClassesAsync();
            var lstClass = query.Select(x => new ClassResponseDTO
            {
                ClassId = x.ClassId,
                ClassName = x.ClassName,
            })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<ClassResponseDTO>(lstClass, page, pageSize, query.Count());
            return new ServiceResponse<PagedResponse<ClassResponseDTO>>(true, "Lấy danh sách lớp thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<ClassResponseDTO>>> SearchClassListAsync(string textToSearch, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new ServiceResponse<PagedResponse<ClassResponseDTO>>(false, "Trang và kích thước không hợp lệ");
            }

            var query = _classRepository.SearchClassAsync(textToSearch);
            var lstClass = query.Select(x => new ClassResponseDTO
            {
                ClassId = x.ClassId,
                ClassName = x.ClassName,
            })
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagedResponse = new PagedResponse<ClassResponseDTO>(lstClass, page, pageSize, query.Count());
            return new ServiceResponse<PagedResponse<ClassResponseDTO>>(true, "Lấy danh sách lớp sau khi tìm thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateClass(ClassRequestDTO classRequest)
        {
            if (classRequest == null || string.IsNullOrEmpty(classRequest.ClassId) || string.IsNullOrEmpty(classRequest.ClassName))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existing = await _classRepository.GetClassByIdAsync(classRequest.ClassId);
            if (existing == null)
            {
                return new ServiceResponse<bool>(false, "Lớp không tồn tại");
            }    

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existing.ClassName = classRequest.ClassName;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lớp thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo lớp : {ex.Message}");
                }
            }
        }
    }
}
