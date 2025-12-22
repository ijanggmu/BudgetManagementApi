using System.Threading;
using Data.Context;
using Data.Entities.FodoEntity;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Fodo;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class FodoService(
    ApplicationDataContext db,
    UserManager<ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IFodoService
{
    public async Task<Result<List<FodoResponseDto>>> GetFodosForAdminAsync(string tenantId = null, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<List<FodoResponseDto>>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<List<FodoResponseDto>>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<List<FodoResponseDto>>.Failed("Unauthorized access.");

        IQueryable<Fodo> query = db.Fodos
            .Include(a => a.User)
            .Where(a => !a.IsDeleted && !a.User.IsDeleted);

        // For SuperAdmin: filter by tenantId if provided, otherwise show all
        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }
            // If tenantId is null, show all fodos (global query filter will be ignored)
            query = query.IgnoreQueryFilters();
        }
        // For Admin: only show fodos from their tenant (global query filter applies)

        var fodos = await query.ToListAsync(cancellationToken);

        // Get tenant names for all unique tenant IDs
        var tenantIds = fodos.Where(a => !string.IsNullOrEmpty(a.TenantId)).Select(a => a.TenantId).Distinct().ToList();
        var tenants = await db.Tenants.Where(t => tenantIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var dtos = fodos.Select(a => new FodoResponseDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Email = a.User.Email ?? string.Empty,
            PhoneNumber = a.User.PhoneNumber ?? string.Empty,
            TenantId = a.TenantId ?? string.Empty,
            TenantName = !string.IsNullOrEmpty(a.TenantId) && tenants.ContainsKey(a.TenantId) ? tenants[a.TenantId] : string.Empty,
            UserId = a.UserId,
            Username = a.User.UserName ?? string.Empty,
            CreatedOn = a.CreatedOn
        }).ToList();

        return Result<List<FodoResponseDto>>.Success(dtos);
    }

    public async Task<Result<FodoResponseDto>> GetFodoByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<FodoResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<FodoResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Fodos
            .Include(a => a.User)
            .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var fodo = await query.FirstOrDefaultAsync(cancellationToken);

        if (fodo == null)
            return Result<FodoResponseDto>.Failed("Fodo not found.");

        var tenantName = string.Empty;
        if (!string.IsNullOrEmpty(fodo.TenantId))
        {
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == fodo.TenantId, cancellationToken);
            tenantName = tenant?.Name ?? string.Empty;
        }

        var dto = new FodoResponseDto
        {
            Id = fodo.Id,
            FullName = fodo.FullName,
            Email = fodo.User.Email ?? string.Empty,
            PhoneNumber = fodo.User.PhoneNumber ?? string.Empty,
            TenantId = fodo.TenantId ?? string.Empty,
            TenantName = tenantName,
            UserId = fodo.UserId,
            Username = fodo.User.UserName ?? string.Empty,
            CreatedOn = fodo.CreatedOn
        };

        return Result<FodoResponseDto>.Success(dto);
    }

    public async Task<Result<FodoResponseDto>> CreateAsync(CreateFodoDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<FodoResponseDto>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<FodoResponseDto>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
            var isAdmin = roles.Contains(SystemRoles.Admin);

            if (!isSuperAdmin && !isAdmin)
                return Result<FodoResponseDto>.Failed("Unauthorized access.");

            // Get tenant ID from current user (for Admin) or from context
            var tenantId = isSuperAdmin ? currentUser.TenantId : db.CurrentTenantId;

            // Check if phone number already exists
            var phoneExists = await db.Users
                .AnyAsync(u => u.PhoneNumber == dto.MobileNumber && !u.IsDeleted, cancellationToken);

            if (phoneExists)
                return Result<FodoResponseDto>.Failed("Mobile number already exists.");

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

                if (emailExists)
                    return Result<FodoResponseDto>.Failed("Email already exists.");
            }

            // Get country dialing code
            var country = await db.Countries
                .FirstOrDefaultAsync(c => c.Id == dto.CountryId, cancellationToken);

            if (country == null)
                return Result<FodoResponseDto>.Failed("Invalid country ID.");

            var username = $"{country.CountryDialingCode}{dto.MobileNumber}";

            // Check if username already exists
            var usernameExists = await db.Users
                .AnyAsync(u => u.UserName == username && !u.IsDeleted, cancellationToken);

            if (usernameExists)
                return Result<FodoResponseDto>.Failed("Username already exists.");

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = username,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                PhoneNumber = dto.MobileNumber,
                PhoneCountryId = dto.CountryId,
                LockoutEnabled = true,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TenantId = tenantId
            };

            var createUserResult = await userManager.CreateAsync(user, dto.Password);
            if (!createUserResult.Succeeded)
                return Result<FodoResponseDto>.Failed(createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user.");

            // Add Agent role
            var roleResult = await userManager.AddToRoleAsync(user, SystemRoles.FoDo);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<FodoResponseDto>.Failed(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign role.");
            }

            // Create Fodo entity
            var fodo = new Fodo
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName.Trim(),
                UserId = user.Id,
                TenantId = tenantId
            };

            await db.Fodos.AddAsync(fodo, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var responseDto = new FodoResponseDto
            {
                Id = fodo.Id,
                FullName = fodo.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                TenantId = fodo.TenantId ?? string.Empty,
                UserId = fodo.UserId,
                Username = user.UserName ?? string.Empty,
                CreatedOn = fodo.CreatedOn
            };

            return Result<FodoResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<FodoResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<FodoResponseDto>> UpdateAsync(string id, UpdateFodoDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<FodoResponseDto>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<FodoResponseDto>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<FodoResponseDto>.Failed("Fodo not found.");

            // Update fodo properties
            fodo.FullName = dto.FullName.Trim();

            // Update user properties
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != fodo.User.Email)
            {
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email == dto.Email && u.Id != fodo.UserId && !u.IsDeleted, cancellationToken);

                if (emailExists)
                    return Result<FodoResponseDto>.Failed("Email already exists.");

                fodo.User.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.MobileNumber) && dto.MobileNumber != fodo.User.PhoneNumber)
            {
                var phoneExists = await db.Users
                    .AnyAsync(u => u.PhoneNumber == dto.MobileNumber && u.Id != fodo.UserId && !u.IsDeleted, cancellationToken);

                if (phoneExists)
                    return Result<FodoResponseDto>.Failed("Mobile number already exists.");

                fodo.User.PhoneNumber = dto.MobileNumber;
            }

            var updateResult = await userManager.UpdateAsync(fodo.User);
            if (!updateResult.Succeeded)
                return Result<FodoResponseDto>.Failed(updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user.");

            db.Fodos.Update(fodo);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var responseDto = new FodoResponseDto
            {
                Id = fodo.Id,
                FullName = fodo.FullName,
                Email = fodo.User.Email ?? string.Empty,
                PhoneNumber = fodo.User.PhoneNumber ?? string.Empty,
                TenantId = fodo.TenantId ?? string.Empty,
                UserId = fodo.UserId,
                Username = fodo.User.UserName ?? string.Empty,
                CreatedOn = fodo.CreatedOn
            };

            return Result<FodoResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<FodoResponseDto>.Failed($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Failed("User not authenticated.");

            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
                return Result<bool>.Failed("User not found.");

            var roles = await userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

            var query = db.Fodos
                .Include(a => a.User)
                .Where(a => a.Id == id && !a.IsDeleted && !a.User.IsDeleted);

            if (isSuperAdmin)
                query = query.IgnoreQueryFilters();

            var fodo = await query.FirstOrDefaultAsync(cancellationToken);

            if (fodo == null)
                return Result<bool>.Failed("Fodo not found.");

            // Soft delete
            fodo.IsDeleted = true;
            fodo.User.IsDeleted = true;

            db.Fodos.Update(fodo);
            db.Users.Update(fodo.User);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failed($"An error occurred: {ex.Message}");
        }
    }
}

