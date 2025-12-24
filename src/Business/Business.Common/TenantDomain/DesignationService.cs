using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Designation;
using Models.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class DesignationService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService)
    : IDesignationService
{
    public async Task<Result<List<DesignationResponseDto>>> GetAllAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<List<DesignationResponseDto>>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<List<DesignationResponseDto>>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        IQueryable<Designation> query = db.Designations
            .Where(d => !d.IsDeleted);

        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
                query = query.Where(d => d.TenantId == tenantId);
            query = query.IgnoreQueryFilters();
        }

        var designations = await query.ToListAsync(cancellationToken);

        var dtos = designations.Select(d => new DesignationResponseDto
        {
            Id = d.Id,
            Title = d.Title,
            Description = d.Description,
            TenantId = d.TenantId ?? string.Empty,
            CreatedOn = d.CreatedOn
        }).ToList();

        return Result<List<DesignationResponseDto>>.Success(dtos);
    }

    public async Task<Result<DesignationResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<DesignationResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<DesignationResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Designations
            .Where(d => d.Id == id && !d.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var designation = await query.FirstOrDefaultAsync(cancellationToken);

        if (designation == null)
            return Result<DesignationResponseDto>.Failed("Designation not found.");

        var dto = new DesignationResponseDto
        {
            Id = designation.Id,
            Title = designation.Title,
            Description = designation.Description,
            TenantId = designation.TenantId ?? string.Empty,
            CreatedOn = designation.CreatedOn
        };

        return Result<DesignationResponseDto>.Success(dto);
    }

    public async Task<Result<DesignationResponseDto>> CreateAsync(CreateDesignationDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<DesignationResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<DesignationResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<DesignationResponseDto>.Failed("Unauthorized access.");

        var tenantId = isSuperAdmin ? user.TenantId : db.CurrentTenantId;

        // Check if Title already exists for this tenant
        var titleExists = await db.Designations
            .AnyAsync(d => d.Title == dto.Title && d.TenantId == tenantId && !d.IsDeleted, cancellationToken);

        if (titleExists)
            return Result<DesignationResponseDto>.Failed("Designation Title already exists.");

        var designation = new Designation
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            TenantId = tenantId
        };

        await db.Designations.AddAsync(designation, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var responseDto = new DesignationResponseDto
        {
            Id = designation.Id,
            Title = designation.Title,
            Description = designation.Description,
            TenantId = designation.TenantId ?? string.Empty,
            CreatedOn = designation.CreatedOn
        };

        return Result<DesignationResponseDto>.Success(responseDto);
    }

    public async Task<Result<DesignationResponseDto>> UpdateAsync(string id, UpdateDesignationDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<DesignationResponseDto>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<DesignationResponseDto>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);

        var query = db.Designations
            .Where(d => d.Id == id && !d.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var designation = await query.FirstOrDefaultAsync(cancellationToken);

        if (designation == null)
            return Result<DesignationResponseDto>.Failed("Designation not found.");

        // Check if Title already exists (if changed)
        if (dto.Title != designation.Title)
        {
            var titleExists = await db.Designations
                .AnyAsync(d => d.Title == dto.Title && d.TenantId == designation.TenantId && d.Id != id && !d.IsDeleted, cancellationToken);

            if (titleExists)
                return Result<DesignationResponseDto>.Failed("Designation Title already exists.");
        }

        designation.Title = dto.Title.Trim();
        designation.Description = dto.Description?.Trim();

        db.Designations.Update(designation);
        await db.SaveChangesAsync(cancellationToken);

        var responseDto = new DesignationResponseDto
        {
            Id = designation.Id,
            Title = designation.Title,
            Description = designation.Description,
            TenantId = designation.TenantId ?? string.Empty,
            CreatedOn = designation.CreatedOn
        };

        return Result<DesignationResponseDto>.Success(responseDto);
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

        var query = db.Designations
            .Where(d => d.Id == id && !d.IsDeleted);

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var designation = await query.FirstOrDefaultAsync(cancellationToken);

        if (designation == null)
            return Result<bool>.Failed("Designation not found.");

        // Check if designation is used by any marketing executives
        var isUsed = await db.Fodos
            .AnyAsync(f => f.DesignationId == id && !f.IsDeleted, cancellationToken);

        if (isUsed)
            return Result<bool>.Failed("Cannot delete designation. It is assigned to one or more marketing executives.");

        designation.IsDeleted = true;
        db.Designations.Update(designation);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ImportResult>> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<ImportResult>.Failed("User not authenticated.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<ImportResult>.Failed("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
        var isAdmin = roles.Contains(SystemRoles.Admin);

        if (!isSuperAdmin && !isAdmin)
            return Result<ImportResult>.Failed("Unauthorized access.");

        var tenantId = isSuperAdmin ? user.TenantId : db.CurrentTenantId;

        var importResult = new ImportResult();
        var designationsToAdd = new List<Designation>();
        var errors = new List<ImportError>();

        try
        {
            var workbook = new XSSFWorkbook(fileStream);
            var sheet = workbook.GetSheetAt(0);

            if (sheet == null || sheet.LastRowNum < 1)
                return Result<ImportResult>.Failed("Excel file is empty or invalid.");

            // Validate header row
            var headerRow = sheet.GetRow(0);
            if (headerRow == null)
                return Result<ImportResult>.Failed("Header row is missing.");

            var titleIndex = -1;
            var descriptionIndex = -1;

            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.ToString()?.Trim().ToLower();
                if (cellValue == "title")
                    titleIndex = i;
                else if (cellValue == "description")
                    descriptionIndex = i;
            }

            if (titleIndex == -1)
                return Result<ImportResult>.Failed("Required column 'Title' is missing in the Excel file.");

            // Process data rows
            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                var title = row.GetCell(titleIndex)?.ToString()?.Trim();
                var description = row.GetCell(descriptionIndex)?.ToString()?.Trim();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(title))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "Title",
                        ErrorMessage = "Title is required."
                    });
                    importResult.FailureCount++;
                    continue;
                }

                // Check if designation already exists
                var exists = await db.Designations
                    .AnyAsync(d => d.Title == title && d.TenantId == tenantId && !d.IsDeleted, cancellationToken);

                if (exists)
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "Title",
                        ErrorMessage = $"Designation '{title}' already exists."
                    });
                    importResult.FailureCount++;
                    continue;
                }

                designationsToAdd.Add(new Designation
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description,
                    TenantId = tenantId
                });

                importResult.SuccessCount++;
            }

            // Bulk insert
            if (designationsToAdd.Any())
            {
                await db.Designations.AddRangeAsync(designationsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            importResult.TotalRows = sheet.LastRowNum;
            importResult.Errors = errors;

            return Result<ImportResult>.Success(importResult);
        }
        catch (Exception ex)
        {
            return Result<ImportResult>.Failed($"Error importing Excel file: {ex.Message}");
        }
    }
}

