﻿using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.GroupManagementDTO;

namespace StudyPlannerAPI.Services.GroupManagementService
{
    public interface IGroupManagementService
    {
        Task<ServiceResponse<PagedResponse<GroupManagementResponseDTO>>> GetAllGroupManagement(int page, int pageSize);
        Task<ServiceResponse<bool>> AddGroupManagement(GroupManagementRequestDTO group);
        Task<ServiceResponse<bool>> DeleteGroupManagement(string groupId);
        Task<ServiceResponse<bool>> UpdateGroupManagement(GroupManagementRequestDTO group);
        Task<ServiceResponse<GroupManagementResponseDTO>> GetGroupManagementWithGroupId(string groupId);
        Task<ServiceResponse<PagedResponse<GroupManagementResponseDTO>>> SearchGroup(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<List<GroupManagementTotalDTO>>> GetTotalUserInGroup();
        Task<ServiceResponse<int>> GetTotalGroupCount();
        Task<ServiceResponse<string>> EnsureParentGroupExists();
    }
}
