using StudyPlannerAPI.DTOs.StudentClassDTO;
using StudyPlannerAPI.DTOs.TeacherClassDTO;

namespace StudyPlannerAPI.DTOs.ClassDTO
{
    public class ClassRequestDTO
    {
        public string ClassId { get; set; } = null!;

        public string? ClassName { get; set; }

    }
}
