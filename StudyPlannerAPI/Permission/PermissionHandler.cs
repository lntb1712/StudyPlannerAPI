using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StudyPlannerAPI.Permision;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StudyPlannerAPI.Permission
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissionsJson = context.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var httpContext = context.Resource as DefaultHttpContext;
            if (httpContext == null)
                return Task.CompletedTask;

            var requestedPath = httpContext.Request.Path.Value?.Trim().ToLower();
            if (string.IsNullOrEmpty(requestedPath))
                return Task.CompletedTask;

            // Parse JSON permission từ claim
            var permissionIds = userPermissionsJson
                .Select(p =>
                {
                    try
                    {
                        var doc = JsonDocument.Parse(p);
                        if (doc.RootElement.TryGetProperty("id", out var idProp))
                            return idProp.GetString();
                    }
                    catch { }
                    return null;
                })
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            // Check permission theo mapping
            foreach (var permId in permissionIds)
            {
                if (PermissionToApiPatternMap.TryGetValue(permId, out var pattern) &&
                    Regex.IsMatch(requestedPath, pattern, RegexOptions.IgnoreCase))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Fail(); // không match permission nào
            return Task.CompletedTask;
        }

        // Mapping PermissionId ↔ API Route
        private static readonly Dictionary<string, string> PermissionToApiPatternMap = new Dictionary<string, string>()
        {
            {"ucAccountManagement", @"^/api/AccountManagement"},
            {"ucAssignment", @"^/api/Assignment"},
            {"ucClassManagement", @"^/api/Class"},
            {"ucGroupManagement", @"^/api/GroupManagement"},
            {"ucReminder", @"^/api/Reminder"},
            {"ucSchedule", @"^/api/Schedule"},
            {"ucTaskManagement", @"^/api/TaskManagement"},
        };
    }   
}
