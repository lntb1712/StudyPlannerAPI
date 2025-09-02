using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.GroupManagementDTO;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Repositories.GroupManagementRepository;

namespace StudyPlannerAPI.Services.GroupManagementService
{
    public class GroupManagementService: IGroupManagementService
    {
        private readonly IGroupManagementRepository _groupManagementRepository;
        private readonly IGroupFunctionRepository _groupFunctionRepository;
        private readonly StudyPlannerContext _context;

        public GroupManagementService(IGroupManagementRepository groupManagementRepository, IGroupFunctionRepository groupFunctionRepository, StudyPlannerContext context)
        {
            _groupManagementRepository = groupManagementRepository;
            _groupFunctionRepository = groupFunctionRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddGroupManagement(GroupManagementRequestDTO group)
        {
            if (group == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var groupManagement = new GroupManagement
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                GroupDescription = group.GroupDescription,
                GroupFunctions = new List<GroupFunction>()
            };

            // Duyệt từng function
            foreach (var function in group.GroupFunctions)
            {


                var groupFunction = new GroupFunction
                {
                    GroupId = group.GroupId,
                    FunctionId = function.FunctionId,
                    IsEnable = function.IsEnable,
                    IsReadOnly = function.IsReadOnly,
                };

                groupManagement.GroupFunctions.Add(groupFunction);


            }


            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _groupManagementRepository.AddAsync(groupManagement, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm mới nhóm người dùng thành công");
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
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteGroupManagement(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var group = await _groupManagementRepository.GetGroupManagementWithGroupID(groupId);
            if (group == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy nhóm cần xóa");
            }
            //throw new NotImplementedException();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _groupManagementRepository.DeleteAsync(groupId, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa nhóm người dùng thành công", true);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa nhóm người dùng");
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

        public async Task<ServiceResponse<PagedResponse<GroupManagementResponseDTO>>> GetAllGroupManagement(int page, int pageSize)
        {

            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(false, "Trang không hợp lệ", null!);
            }
            var lstGroup = _groupManagementRepository.GetAllGroup();
            var totalItems = await lstGroup.CountAsync();
            if (lstGroup == null || totalItems == 0)
            {
                return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(false, "Không tìm thấy nhóm người dùng nào", null!);
            }

            var lstGroupManagement = lstGroup
                                    .Select(x => new GroupManagementResponseDTO
                                    {
                                        GroupId = x.GroupId,
                                        GroupName = x.GroupName,
                                        GroupDescription = x.GroupDescription,
                                    })
                                    .OrderBy(x => x.GroupId) // Sắp xếp theo GroupId để đảm bảo tính nhất quán
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var pagedResponse = new PagedResponse<GroupManagementResponseDTO>(lstGroupManagement, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(true, "Lấy danh sách nhóm người dùng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<GroupManagementResponseDTO>> GetGroupManagementWithGroupId(string groupId)
        {
            //throw new NotImplementedException();
            var groupManagementWithGroupId = await _groupManagementRepository.GetGroupManagementWithGroupID(groupId);
            if (groupManagementWithGroupId == null)
            {
                return new ServiceResponse<GroupManagementResponseDTO>(false, "Không tìm thấy nhóm người dùng nào", null!);
            }

            var groupManagementResponse = new GroupManagementResponseDTO
            {
                GroupId = groupManagementWithGroupId.GroupId,
                GroupName = groupManagementWithGroupId.GroupName,
                GroupDescription = groupManagementWithGroupId.GroupDescription,
            };
            return new ServiceResponse<GroupManagementResponseDTO>(true, "Lấy thông tin nhóm người dùng thành công", groupManagementResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalGroupCount()
        {
            try
            {
                var response = await _groupManagementRepository.GetAllGroup().CountAsync();
                return new ServiceResponse<int>(true, "Lấy tổng nhóm thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Lỗi:" + ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GroupManagementTotalDTO>>> GetTotalUserInGroup()
        {
            var totalUserInGroup = await _groupManagementRepository.GetTotalUserInGroup();
            if (totalUserInGroup == null || totalUserInGroup.Count == 0)
            {
                return new ServiceResponse<List<GroupManagementTotalDTO>>(false, "Không tìm thấy nhóm người dùng nào", null!);
            }
            return new ServiceResponse<List<GroupManagementTotalDTO>>(true, "Lấy danh sách nhóm người dùng thành công", totalUserInGroup);
        }

        public async Task<ServiceResponse<PagedResponse<GroupManagementResponseDTO>>> SearchGroup(string textToSearch, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(false, "Trang không hợp lệ", null!);
            }
            var lstGroup = _groupManagementRepository.SearchGroup(textToSearch);
            var totalItems = await lstGroup.CountAsync();
            if (lstGroup == null || totalItems == 0)
            {
                return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(false, "Không tìm thấy nhóm người dùng nào", null!);
            }

            var lstGroupManagement = lstGroup
                .Select(x => new GroupManagementResponseDTO
                {
                    GroupId = x.GroupId,
                    GroupName = x.GroupName,
                    GroupDescription = x.GroupDescription,
                
                })
                .OrderBy(x => x.GroupId) // Sắp xếp theo GroupId để đảm bảo tính nhất quán
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResponse = new PagedResponse<GroupManagementResponseDTO>(lstGroupManagement, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<GroupManagementResponseDTO>>(true, "Lấy danh sách nhóm người dùng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateGroupManagement(GroupManagementRequestDTO group)
        {
            //throw new NotImplementedException();
            if (group == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var groupManagement = await _groupManagementRepository.GetGroupManagementWithGroupID(group.GroupId);
            if (groupManagement == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy nhóm người dùng hợp lệ");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    groupManagement.GroupName = group.GroupName;
                    groupManagement.GroupDescription = group.GroupDescription;
                   


                    var existingFunctions = groupManagement.GroupFunctions.ToDictionary(x => $"{x.FunctionId}");

                    foreach (var function in group.GroupFunctions)
                    {



                        var key = $"{function.FunctionId}";

                        if (existingFunctions.TryGetValue(key, out var existingFunction))
                        {
                            existingFunction.IsEnable = function.IsEnable;
                            existingFunction.IsReadOnly = function.IsReadOnly;
                        }

                        else
                        {
                            groupManagement.GroupFunctions.Add(new GroupFunction
                            {
                                GroupId = group.GroupId,
                                FunctionId = function.FunctionId,
                                IsReadOnly = function.IsReadOnly,
                                IsEnable = function.IsEnable
                            });
                        }

                    }


                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công", true);
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
