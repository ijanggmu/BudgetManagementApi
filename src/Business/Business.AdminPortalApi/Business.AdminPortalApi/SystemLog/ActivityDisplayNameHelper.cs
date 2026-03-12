using System.Collections.Generic;

namespace AdminPortalApi.Controllers.V1.SystemLog;

/// <summary>
/// Maps request path, alias, or module to user-friendly activity names for logs.
/// </summary>
public static class ActivityDisplayNameHelper
{
    private static readonly Dictionary<string, string> PathToDisplayName = new(StringComparer.OrdinalIgnoreCase)
    {
        { "login", "Login" },
        { "auth", "Authentication" },
        { "adminauth", "Admin login" },
        { "logout", "Logout" },
        { "budget", "Budget" },
        { "budgets", "View budgets" },
        { "budget-request", "Budget request" },
        { "budget-requests", "Budget requests" },
        { "memo", "Memo" },
        { "memos", "Memos" },
        { "department", "Department" },
        { "departments", "Departments" },
        { "approval", "Approval" },
        { "approvals", "Approvals" },
        { "dashboard", "Dashboard" },
        { "profile", "Profile" },
        { "entity", "Organization settings" },
        { "admin", "Admin" },
        { "roles", "Roles" },
        { "tenant", "Tenant" },
        { "export", "Export" },
        { "report", "Report" },
        { "reports", "Reports" },
    };

    /// <summary>
    /// Returns a user-friendly display name for the activity (request path, alias, or module).
    /// </summary>
    public static string GetDisplayName(string requestPath, string requestPathAlias, string module)
    {
        var toCheck = new[] { requestPathAlias, module, requestPath };
        foreach (var value in toCheck)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;
            var segment = value.Trim('/').ToLowerInvariant();
            foreach (var kv in PathToDisplayName)
            {
                if (segment.Contains(kv.Key)) return kv.Value;
            }
            // Fallback: humanize the last segment (e.g. "budget-requests" -> "Budget requests")
            var last = segment.Contains("/") ? segment.Substring(segment.LastIndexOf('/') + 1) : segment;
            if (!string.IsNullOrEmpty(last))
                return Humanize(last);
        }
        return string.IsNullOrWhiteSpace(requestPath) ? "Activity" : Humanize(requestPath.Trim('/'));
    }

    private static string Humanize(string segment)
    {
        if (string.IsNullOrEmpty(segment)) return "Activity";
        var replaced = segment.Replace("-", " ").Replace("_", " ");
        if (replaced.Length == 0) return "Activity";
        return char.ToUpperInvariant(replaced[0]) + replaced.Substring(1);
    }
}
