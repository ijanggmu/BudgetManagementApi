//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Data.Context;
//using Data.Entities.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SharedKernel.Constant.Roles;

//namespace Infrastructure.Common.UserProfile;

///// <summary>
///// Service to help with user role checking and authorization
///// </summary>
//public class UserRoleHelperService : IUserRoleHelperService
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly ApplicationDataContext _dbContext;

//    public UserRoleHelperService(
//        UserManager<ApplicationUser> userManager,
//        ApplicationDataContext dbContext)
//    {
//        _userManager = userManager;
//        _dbContext = dbContext;
//    }

//    public async Task<List<string>> GetUserRolesAsync(string userId)
//    {
//        if (string.IsNullOrEmpty(userId))
//            return new List<string>();

//        var user = await _userManager.FindByIdAsync(userId);
//        if (user == null)
//            return new List<string>();

//        var roles = await _userManager.GetRolesAsync(user);
//        return roles.ToList();
//    }

//    public async Task<bool> IsSuperAdminAsync(string userId)
//    {
//        var roles = await GetUserRolesAsync(userId);
//        return roles.Contains(SystemRoles.SuperAdmin);
//    }

//    public async Task<bool> IsAdminAsync(string userId)
//    {
//        var roles = await GetUserRolesAsync(userId);
//        return roles.Contains(SystemRoles.Admin);
//    }

//    public async Task<bool> IsMarketingExecutiveAsync(string userId)
//    {
//        var roles = await GetUserRolesAsync(userId);
//        return roles.Contains(SystemRoles.FoDo) || roles.Contains(SystemRoles.MarketingExecutive);
//    }

//    public async Task<bool> HasAnyRoleAsync(string userId, params string[] roles)
//    {
//        var userRoles = await GetUserRolesAsync(userId);
//        return roles.Any(role => userRoles.Contains(role));
//    }

//    public async Task<bool> HasAllRolesAsync(string userId, params string[] roles)
//    {
//        var userRoles = await GetUserRolesAsync(userId);
//        return roles.All(role => userRoles.Contains(role));
//    }

//    public async Task<UserRoleInfo> GetUserRoleInfoAsync(string userId)
//    {
//        if (string.IsNullOrEmpty(userId))
//            return new UserRoleInfo();

//        var user = await _userManager.FindByIdAsync(userId);
//        if (user == null)
//            return new UserRoleInfo();

//        // Get roles using the more efficient database query
//        var userRoles = await _dbContext.UserRoles
//            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
//            .Join(_dbContext.Roles.Where(r => !r.IsDeleted),
//                ur => ur.RoleId,
//                r => r.Id,
//                (ur, r) => r.Name)
//            .ToListAsync();

//        return new UserRoleInfo
//        {
//            Roles = userRoles,
//            IsSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin),
//            IsAdmin = userRoles.Contains(SystemRoles.Admin),
//            IsMarketingExecutive = userRoles.Contains(SystemRoles.FoDo) || userRoles.Contains(SystemRoles.MarketingExecutive),
//            TenantId = user.TenantId
//        };
//    }
//}

