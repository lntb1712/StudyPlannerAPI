using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace StudyPlannerAPI.Permision
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public PermissionRequirement() { }
    }
}
