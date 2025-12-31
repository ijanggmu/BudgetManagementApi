//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Data.Entities.Identity;
//using Microsoft.AspNetCore.Identity;
//using SharedKernel.Constant.Roles;

//namespace Infrastructure.Common.UserProfile;

///// <summary>
///// Service to help with user role checking and authorization
///// </summary>
//public interface IUserRoleHelperService
//{
//    /// <summary>
//    /// Gets the current user's roles
//    /// </summary>
//    Task<List<string>> GetUserRolesAsync(string userId);

//    /// <summary>
//    /// Checks if the current user is SuperAdmin
//    /// </summary>
//    Task<bool> IsSuperAdminAsync(string userId);

//    /// <summary>
//    /// Checks if the current user is Admin (Tenant Admin)
//    /// </summary>
//    Task<bool> IsAdminAsync(string userId);

//    /// <summary>
//    /// Checks if the current user is Marketing Executive (FoDo)
//    /// </summary>
//    Task<bool> IsMarketingExecutiveAsync(string userId);

//    /// <summary>
//    /// Checks if the current user has any of the specified roles
//    /// </summary>
//    Task<bool> HasAnyRoleAsync(string userId, params string[] roles);

//    /// <summary>
//    /// Checks if the current user has all of the specified roles
//    /// </summary>
//    Task<bool> HasAllRolesAsync(string userId, params string[] roles);

//    /// <summary>
//    /// Gets user role information including SuperAdmin and Admin flags
//    /// </summary>
//    Task<UserRoleInfo> GetUserRoleInfoAsync(string userId);
//}

///// <summary>
///// User role information
///// </summary>
//public class UserRoleInfo
//{
//    public List<string> Roles { get; set; } = new();
//    public bool IsSuperAdmin { get; set; }
//    public bool IsAdmin { get; set; }
//    public bool IsMarketingExecutive { get; set; }
//    public string? TenantId { get; set; }
//}

