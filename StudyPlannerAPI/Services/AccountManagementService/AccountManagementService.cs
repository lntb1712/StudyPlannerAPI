using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.AccountManagementDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.AccountManagementRepository;

namespace StudyPlannerAPI.Services.AccountManagementService
{
    public class AccountManagementService:IAccountManagementService
    {
        private readonly StudyPlannerContext _context;
        private readonly IAccountManagementRepository _accountManagementRepository;

        public AccountManagementService(StudyPlannerContext context, IAccountManagementRepository accountManagementRepository)
        {
            _context = context;
            _accountManagementRepository = accountManagementRepository;
        }

        public async Task<ServiceResponse<bool>> AddAccountManagement(AccountManagementRequestDTO account)
        {
            if (string.IsNullOrEmpty(account.UserName) || account == null)
                return new ServiceResponse<bool>(false, "Tên người dùng không được để trống hoặc thông tin nhập vào không hợp lệ");
            
            var existingAccount = await _accountManagementRepository.GetAllAccount()
                 .FirstOrDefaultAsync(x => x.Email == account.Email);
            if (existingAccount != null)
            {
                return new ServiceResponse<bool>(false, "Email đã được sử dụng");
            }
            var newAccount = new AccountManagement
            {
                UserName = account.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(account.Password, 12),
                FullName = account.FullName,
                Email = account.Email,
                ParentEmail = account.ParentEmail,
                GroupId = account.GroupId,
                CreatedAt = DateTime.Now
            };
          
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                   

                    await _accountManagementRepository.AddAsync(newAccount, saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm tài khoản thành công");
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Tài khoản đã tồn tại");
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
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAccountManagement(string username)
        {
            if(string.IsNullOrEmpty(username))
                return new ServiceResponse<bool>(false, "Tên người dùng không được để trống");
            var existingAccount = await _accountManagementRepository.GetAccountWithUserName(username);
            if(existingAccount == null)
                return new ServiceResponse<bool>(false, "Tài khoản không tồn tại");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _accountManagementRepository.DeleteAsync(username, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa tài khoản thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa tài khoản do có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page, int pageSize)
        {
            if(page<1|| pageSize <1) return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0");
            
            var query = await _accountManagementRepository.GetAllAccount().ToListAsync();
            var totalItems = query.Count;

            var lstAccounts = query
                            .Select(x=> new AccountManagementResponseDTO
                            {
                                UserName = x.UserName,
                                FullName = x.FullName,
                                Email = x.Email,
                                ParentEmail = x.ParentEmail,
                                GroupId = x.GroupId,
                                GroupName = x.Group!.GroupName,
                                CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy"),
                               
                            })
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccounts, totalItems, page, pageSize);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true,"Lấy danh sách tài khoản thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountByGroupId(string groupId, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(groupId))
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Mã nhóm không được để trống");

            if (page < 1 || pageSize < 1) 
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0");

            var query = await _accountManagementRepository.GetAllAccount().ToListAsync();
            var totalItems = query.Where(x => x.GroupId == groupId).Count();

            var lstAccounts = query
                            .Where(x => x.GroupId == groupId)
                            .Select(x => new AccountManagementResponseDTO
                            {
                                UserName = x.UserName,
                                FullName = x.FullName,
                                Email = x.Email,
                                ParentEmail = x.ParentEmail,
                                GroupId = x.GroupId,
                                GroupName = x.Group!.GroupName,
                                CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy")
                            })
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccounts, totalItems, page, pageSize);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true, "Lấy danh sách tài khoản thành công", pagedResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalAccount()
        {
            var totalItems = await _accountManagementRepository.GetAllAccount().CountAsync();
            return new ServiceResponse<int>(true, "Lấy tổng số tài khoản thành công", totalItems);
        }

        public async Task<ServiceResponse<AccountManagementResponseDTO>> GetUserInformation(string username)
        {
           if(string.IsNullOrEmpty(username))
                return new ServiceResponse<AccountManagementResponseDTO>(false, "Tên người dùng không được để trống");
            var existingAccount = await _accountManagementRepository.GetAccountWithUserName(username);
            if(existingAccount == null)
                return new ServiceResponse<AccountManagementResponseDTO>(false, "Tài khoản không tồn tại");
            var accountDto = new AccountManagementResponseDTO
            {
                UserName = existingAccount.UserName,
                FullName = existingAccount.FullName,
                Email = existingAccount.Email,
                ParentEmail = existingAccount.ParentEmail,
                GroupId = existingAccount.GroupId,
                GroupName = existingAccount.Group!.GroupId,
                CreatedAt = existingAccount.CreatedAt!.Value.ToString("dd/MM/yyyy"),
                ClassId = existingAccount.GroupId?.StartsWith("HS") == true
                ? (existingAccount.StudentClasses?.FirstOrDefault()?.ClassId ?? string.Empty)
                : string.Join(",", existingAccount.TeacherClasses?.Select(tc => tc.ClassId) ?? Enumerable.Empty<string>()),
                ClassName = existingAccount.GroupId?.StartsWith("HS") == true
                ? (existingAccount.StudentClasses?.FirstOrDefault()?.Class?.ClassName ?? string.Empty)
                : string.Join(", ", existingAccount.TeacherClasses?.Select(tc => tc.Class?.ClassName ?? string.Empty) ?? Enumerable.Empty<string>()),
            };
            return new ServiceResponse<AccountManagementResponseDTO>(true, "Lấy thông tin tài khoản thành công", accountDto);
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> SearchAccountByText(string textToSearch, int page, int pageSize)
        {
           if (string.IsNullOrEmpty(textToSearch))
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Chuỗi tìm kiếm không được để trống");
            if (page < 1 || pageSize < 1) 
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0");
            var query = await _accountManagementRepository.SearchAccountByText(textToSearch).ToListAsync();
            var totalItems = query.Count();
            var lstAccounts = query
                            .Select(x => new AccountManagementResponseDTO
                            {
                                UserName = x.UserName,
                                FullName = x.FullName,
                                Email = x.Email,
                                ParentEmail = x.ParentEmail,
                                GroupId = x.GroupId,
                                GroupName = x.Group!.GroupName,
                                CreatedAt = x.CreatedAt!.Value.ToString("dd/MM/yyyy")
                            })
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccounts, totalItems, page, pageSize);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true, "Tìm kiếm tài khoản thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO account)
        {
            if(account == null || string.IsNullOrEmpty(account.UserName))
                return new ServiceResponse<bool>(false, "Tên người dùng không được để trống hoặc thông tin nhập vào không hợp lệ");

            var existingAccount = await _accountManagementRepository.GetAccountWithUserName(account.UserName);
            if (existingAccount == null)
                return new ServiceResponse<bool>(false, "Tài khoản không tồn tại");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var password = account.Password; // Original password (plaintext or hashed)

                    // Check if the password is already hashed (e.g., starts with $2a$, $2b$, etc., typical BCrypt prefix)
                    if (!string.IsNullOrEmpty(password) && !password.StartsWith("$2"))
                    {
                        // If plaintext, hash it
                        existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);
                    }
                    else
                    {
                        // If already hashed (e.g., from database), use it as is
                        existingAccount.Password = password;
                    }
                    existingAccount.FullName = account.FullName;
                    existingAccount.GroupId = account.GroupId;
                    existingAccount.Email = account.Email;
                    existingAccount.ParentEmail = account.ParentEmail;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật tài khoản thành công", true);
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
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }

        }
    }
}
