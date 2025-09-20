using StudyPlannerAPI.DTOs.StudentClassDTO;
using StudyPlannerAPI.DTOs.TeacherClassDTO;
using StudyPlannerAPI.Models;

namespace StudyPlannerAPI.DTOs.ClassDTO
{
    public class ClassResponseDTO
    {
        public string ClassId { get; set; } = null!;

        public string? ClassName { get; set; }

    }
}
