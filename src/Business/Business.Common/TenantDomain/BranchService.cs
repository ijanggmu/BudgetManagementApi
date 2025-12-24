using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Branch;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BranchService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService)
    : IBranchService
{
    public async Task<Result<List<BranchResponseDto>>> GetAllAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<List<BranchResponseDto>>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<List<BranchResponseDto>>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        IQueryable<Branch> query = db.Branches
            .Where(b => !b.IsDeleted);

        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
                query = query.Where(b => b.TenantId == tenantId);
            query = query.IgnoreQueryFilters();
        }

        var branches = await query.ToListAsync(cancellationToken);

        var dtos = branches.Select(b => new BranchResponseDto
        {
            Id = b.Id,
            BranchName = b.BranchName,
            BranchCode = b.BranchCode,
            Province = b.Province,
            District = b.District,
            Municipality = b.Municipality,
            Ward = b.Ward,
            IsActive = b.IsActive,
            TenantId = b.TenantId ?? string.Empty,
            CreatedOn = b.CreatedOn
        }).ToList();

        return Result<List<BranchResponseDto>>.Success(dtos);
    }

    public async Task<Result<BranchResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<BranchResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<BranchResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Branches
            .Where(b => b.Id == id && !b.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var branch = await query.FirstOrDefaultAsync(cancellationToken);

        if (branch == null)
            return Result<BranchResponseDto>.Failed("Branch not found.");

        var dto = new BranchResponseDto
        {
            Id = branch.Id,
            BranchName = branch.BranchName,
            BranchCode = branch.BranchCode,
            Province = branch.Province,
            District = branch.District,
            Municipality = branch.Municipality,
            Ward = branch.Ward,
            IsActive = branch.IsActive,
            TenantId = branch.TenantId ?? string.Empty,
            CreatedOn = branch.CreatedOn
        };

        return Result<BranchResponseDto>.Success(dto);
    }

    public async Task<Result<BranchResponseDto>> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<BranchResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<BranchResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<BranchResponseDto>.Failed("Unauthorized access.");

        var tenantId = isSuperAdmin ? user.TenantId : db.CurrentTenantId;

        // Check if BranchCode already exists
        var codeExists = await db.Branches
            .AnyAsync(b => b.BranchCode == dto.BranchCode && b.TenantId == tenantId && !b.IsDeleted, cancellationToken);

        if (codeExists)
            return Result<BranchResponseDto>.Failed("Branch Code already exists.");

        // Check if BranchName already exists for this tenant
        var nameExists = await db.Branches
            .AnyAsync(b => b.BranchName == dto.BranchName && b.TenantId == tenantId && !b.IsDeleted, cancellationToken);

        if (nameExists)
            return Result<BranchResponseDto>.Failed("Branch Name already exists.");

        var branch = new Branch
        {
            Id = Guid.NewGuid().ToString(),
            BranchName = dto.BranchName.Trim(),
            BranchCode = dto.BranchCode.Trim(),
            Province = dto.Province.Trim(),
            District = dto.District.Trim(),
            Municipality = dto.Municipality.Trim(),
            Ward = dto.Ward,
            IsActive = dto.IsActive,
            TenantId = tenantId
        };

        await db.Branches.AddAsync(branch, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var responseDto = new BranchResponseDto
        {
            Id = branch.Id,
            BranchName = branch.BranchName,
            BranchCode = branch.BranchCode,
            Province = branch.Province,
            District = branch.District,
            Municipality = branch.Municipality,
            Ward = branch.Ward,
            IsActive = branch.IsActive,
            TenantId = branch.TenantId ?? string.Empty,
            CreatedOn = branch.CreatedOn
        };

        return Result<BranchResponseDto>.Success(responseDto);
    }

    public async Task<Result<BranchResponseDto>> UpdateAsync(string id, UpdateBranchDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<BranchResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<BranchResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Branches
            .Where(b => b.Id == id && !b.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var branch = await query.FirstOrDefaultAsync(cancellationToken);

        if (branch == null)
            return Result<BranchResponseDto>.Failed("Branch not found.");

        // Check if BranchCode already exists (if changed)
        if (dto.BranchCode != branch.BranchCode)
        {
            var codeExists = await db.Branches
                .AnyAsync(b => b.BranchCode == dto.BranchCode && b.TenantId == branch.TenantId && b.Id != id && !b.IsDeleted, cancellationToken);

            if (codeExists)
                return Result<BranchResponseDto>.Failed("Branch Code already exists.");
        }

        // Check if BranchName already exists (if changed)
        if (dto.BranchName != branch.BranchName)
        {
            var nameExists = await db.Branches
                .AnyAsync(b => b.BranchName == dto.BranchName && b.TenantId == branch.TenantId && b.Id != id && !b.IsDeleted, cancellationToken);

            if (nameExists)
                return Result<BranchResponseDto>.Failed("Branch Name already exists.");
        }

        branch.BranchName = dto.BranchName.Trim();
        branch.BranchCode = dto.BranchCode.Trim();
        branch.Province = dto.Province.Trim();
        branch.District = dto.District.Trim();
        branch.Municipality = dto.Municipality.Trim();
        branch.Ward = dto.Ward;
        branch.IsActive = dto.IsActive;

        db.Branches.Update(branch);
        await db.SaveChangesAsync(cancellationToken);

        var responseDto = new BranchResponseDto
        {
            Id = branch.Id,
            BranchName = branch.BranchName,
            BranchCode = branch.BranchCode,
            Province = branch.Province,
            District = branch.District,
            Municipality = branch.Municipality,
            Ward = branch.Ward,
            IsActive = branch.IsActive,
            TenantId = branch.TenantId ?? string.Empty,
            CreatedOn = branch.CreatedOn
        };

        return Result<BranchResponseDto>.Success(responseDto);
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<bool>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<bool>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Branches
            .Where(b => b.Id == id && !b.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var branch = await query.FirstOrDefaultAsync(cancellationToken);

        if (branch == null)
            return Result<bool>.Failed("Branch not found.");

        // Check if branch is used by any marketing executives
        var isUsed = await db.Fodos
            .AnyAsync(f => f.BranchId == id && !f.IsDeleted, cancellationToken);

        if (isUsed)
            return Result<bool>.Failed("Cannot delete branch. It is assigned to one or more marketing executives.");

        branch.IsDeleted = true;
        db.Branches.Update(branch);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

