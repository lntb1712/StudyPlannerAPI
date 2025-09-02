using StudyPlannerAPI.Permision;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StudyPlannerAPI.Permission
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissions = context.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var httpContext = context.Resource as DefaultHttpContext;
            if (httpContext == null)
                return Task.CompletedTask;

            var requestedPath = httpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(requestedPath))
                return Task.CompletedTask;

            var warehouseId = httpContext.Request.Query["warehouseId"].ToString();


            if (userPermissions.Any(permission =>
                PermissionToApiPatternMap.TryGetValue(permission, out var apiPattern)
                && Regex.IsMatch(requestedPath, apiPattern, RegexOptions.IgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(); // chặn request nếu không match permission
            }

            return Task.CompletedTask;
        }

        private static readonly Dictionary<string, string> PermissionToApiPatternMap = new Dictionary<string, string>()
        {
            {"ucAccountManagement", @"^/api/AccountManagement"},
            {"ucGroupManagement", @"^/api/GroupManagement"},
        };
    }
}
