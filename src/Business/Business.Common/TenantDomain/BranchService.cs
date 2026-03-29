using System.Threading;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Branch;
using Models.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BranchService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService, ISieveExtension sieveExtension)
    : IBranchService
{
    public async Task<Result<List<BranchResponseDto>>> GetAllAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        // Only check role for query filtering logic
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        IQueryable<Branch> query = db.Branches;
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

        // Apply Sieve filtering and pagination
        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var branches = await result.ToListAsync(cancellationToken);

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

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<BranchResponseDto>>.Success(dtos, pagination);
    }

    public async Task<Result<BranchResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var query = db.Branches
            .Where(b => b.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

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
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;

        // Check if BranchCode already exists
        // Note: IsDeleted filter is now applied globally
        var codeExists = await db.Branches
            .AnyAsync(b => b.BranchCode == dto.BranchCode && b.TenantId == tenantId, cancellationToken);

        if (codeExists)
            return Result<BranchResponseDto>.Failed("Branch Code already exists.");

        // Check if BranchName already exists for this tenant
        // Note: IsDeleted filter is now applied globally
        var nameExists = await db.Branches
            .AnyAsync(b => b.BranchName == dto.BranchName && b.TenantId == tenantId, cancellationToken);

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
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var query = db.Branches
            .Where(b => b.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

        var branch = await query.FirstOrDefaultAsync(cancellationToken);

        if (branch == null)
            return Result<BranchResponseDto>.Failed("Branch not found.");

        // Check if BranchCode already exists (if changed)
        if (dto.BranchCode != branch.BranchCode)
        {
            // Note: IsDeleted filter is now applied globally
            var codeExists = await db.Branches
                .AnyAsync(b => b.BranchCode == dto.BranchCode && b.TenantId == branch.TenantId && b.Id != id, cancellationToken);

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
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var query = db.Branches
            .Where(b => b.Id == id);
        // Note: IsDeleted and TenantId filters are now applied globally via query filters

        if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

        var branch = await query.FirstOrDefaultAsync(cancellationToken);

        if (branch == null)
            return Result<bool>.Failed("Branch not found.");

        // Check if branch is used by any marketing executives
        ////var isUsed = await db.Fodos
        ////    .AnyAsync(f => f.BranchId == id && !f.IsDeleted, cancellationToken);

        //if (isUsed)
        //    return Result<bool>.Failed("Cannot delete branch. It is assigned to one or more marketing executives.");

        branch.IsDeleted = true;
        db.Branches.Update(branch);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ImportResult>> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;

        var importResult = new ImportResult();
        var branchesToAdd = new List<Branch>();
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

            var branchNameIndex = -1;
            var branchCodeIndex = -1;
            var provinceIndex = -1;
            var districtIndex = -1;
            var municipalityIndex = -1;
            var wardIndex = -1;
            var isActiveIndex = -1;

            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.ToString()?.Trim().ToLower();
                if (cellValue == "branchname" || cellValue == "branch name")
                    branchNameIndex = i;
                else if (cellValue == "branchcode" || cellValue == "branch code")
                    branchCodeIndex = i;
                else if (cellValue == "province")
                    provinceIndex = i;
                else if (cellValue == "district")
                    districtIndex = i;
                else if (cellValue == "municipality")
                    municipalityIndex = i;
                else if (cellValue == "ward")
                    wardIndex = i;
                else if (cellValue == "isactive" || cellValue == "is active")
                    isActiveIndex = i;
            }

            if (branchNameIndex == -1 || branchCodeIndex == -1 || provinceIndex == -1 ||
                districtIndex == -1 || municipalityIndex == -1 || wardIndex == -1)
                return Result<ImportResult>.Failed("Required columns are missing in the Excel file.");

            // Process data rows
            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                var branchName = row.GetCell(branchNameIndex)?.ToString()?.Trim();
                var branchCode = row.GetCell(branchCodeIndex)?.ToString()?.Trim();
                var province = row.GetCell(provinceIndex)?.ToString()?.Trim();
                var district = row.GetCell(districtIndex)?.ToString()?.Trim();
                var municipality = row.GetCell(municipalityIndex)?.ToString()?.Trim();
                var wardCell = row.GetCell(wardIndex);
                var isActive = true; // Default to true

                if (isActiveIndex != -1)
                {
                    var isActiveCell = row.GetCell(isActiveIndex);
                    if (isActiveCell != null)
                    {
                        if (isActiveCell.CellType == CellType.Boolean)
                            isActive = isActiveCell.BooleanCellValue;
                        else if (isActiveCell.CellType == CellType.String)
                            bool.TryParse(isActiveCell.StringCellValue, out isActive);
                        else if (isActiveCell.CellType == CellType.Numeric)
                            isActive = isActiveCell.NumericCellValue != 0;
                    }
                }

                // Validate required fields
                var validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(branchName))
                    validationErrors.Add("BranchName is required.");
                if (string.IsNullOrWhiteSpace(branchCode))
                    validationErrors.Add("BranchCode is required.");
                if (string.IsNullOrWhiteSpace(province))
                    validationErrors.Add("Province is required.");
                if (string.IsNullOrWhiteSpace(district))
                    validationErrors.Add("District is required.");
                if (string.IsNullOrWhiteSpace(municipality))
                    validationErrors.Add("Municipality is required.");

                int ward = 0;
                if (wardCell == null || !int.TryParse(wardCell.ToString(), out ward))
                    validationErrors.Add("Ward must be a valid integer.");

                if (validationErrors.Any())
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "Validation",
                        ErrorMessage = string.Join(" ", validationErrors)
                    });
                    importResult.FailureCount++;
                    continue;
                }

                // Check if branch code already exists
                var codeExists = await db.Branches
                    .AnyAsync(b => b.BranchCode == branchCode && b.TenantId == tenantId && !b.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "BranchCode",
                        ErrorMessage = $"Branch Code '{branchCode}' already exists."
                    });
                    importResult.FailureCount++;
                    continue;
                }

                // Check if branch name already exists
                var nameExists = await db.Branches
                    .AnyAsync(b => b.BranchName == branchName && b.TenantId == tenantId && !b.IsDeleted, cancellationToken);

                if (nameExists)
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowIndex + 1,
                        Field = "BranchName",
                        ErrorMessage = $"Branch Name '{branchName}' already exists."
                    });
                    importResult.FailureCount++;
                    continue;
                }

                branchesToAdd.Add(new Branch
                {
                    Id = Guid.NewGuid().ToString(),
                    BranchName = branchName,
                    BranchCode = branchCode,
                    Province = province,
                    District = district,
                    Municipality = municipality,
                    Ward = ward,
                    IsActive = isActive,
                    TenantId = tenantId
                });

                importResult.SuccessCount++;
            }

            // Bulk insert
            if (branchesToAdd.Any())
            {
                await db.Branches.AddRangeAsync(branchesToAdd, cancellationToken);
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

