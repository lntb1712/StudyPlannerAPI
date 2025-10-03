using StudyPlannerAPI.DTOs.GroupManagementDTO;
using StudyPlannerAPI.DTOs.StudentClassDTO;

namespace StudyPlannerAPI.DTOs.DashboardDTO
{
    public class DashboardDTO
    {
        public int TotalAccounts { get; set; }
        public int TotalGroups { get; set; }
        public int TotalClasses { get; set; }
        public List<StudentClassTotalDTO> ClassWithStudentCounts { get; set; } = new List<StudentClassTotalDTO>();
        public int TotalNewAccountInMonth { get; set; }
        public double PercentUpDownNewRegisterAccount { get; set; }
        public List<GroupManagementTotalDTO> GroupsWithUserCounts { get; set; } = new List<GroupManagementTotalDTO>();
    }
}
