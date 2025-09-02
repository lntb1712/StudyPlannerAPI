using StudyPlannerAPI.DTOs.GroupFunctionDTO;

namespace StudyPlannerAPI.DTOs.GroupManagementDTO
{
    public class GroupManagementRequestDTO
    {
        public string GroupId { get; set; } = null!;

        public string? GroupName { get; set; }

        public string? GroupDescription { get; set; }
        public List<GroupFunctionResponseDTO> GroupFunctions { get; set; } = new List<GroupFunctionResponseDTO>();
    }
}
