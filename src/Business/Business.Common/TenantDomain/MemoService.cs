using System.Text.Json;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Models.BeemaEdgeApi.Memo;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;
using PdfSharp.Pdf;

namespace Business.Common.TenantDomain;

public class MemoService(
    ApplicationDataContext db,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IMemoService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<Result<List<MemoResponseDto>>> GetAllAsync(MemoListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        IQueryable<Memo> query = db.Memos.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(m => m.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
        {
            var deptName = await db.Departments.Where(d => d.Id == requestModel.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
            if (deptName != null)
                query = query.Where(m => m.Department == deptName);
        }
        if (!string.IsNullOrEmpty(requestModel.Status))
            query = query.Where(m => m.Status.ToString() == requestModel.Status);
        if (!string.IsNullOrEmpty(requestModel.BudgetRequestId))
            query = query.Where(m => m.BudgetRequestId == requestModel.BudgetRequestId);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.ToListAsync(cancellationToken);
        var dtos = list.Select(MapToDto).ToList();
        return Result<List<MemoResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<MemoResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<MemoResponseDto>.Failed("Memo not found.");
        return Result<MemoResponseDto>.Success(MapToDto(entity));
    }

    public async Task<Result<MemoResponseDto>> GetByBudgetRequestIdAsync(string budgetRequestId, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.BudgetRequestId == budgetRequestId);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<MemoResponseDto>.Failed("Memo not found for this budget request.");
        return Result<MemoResponseDto>.Success(MapToDto(entity));
    }

    public async Task<Result<MemoResponseDto>> CreateAsync(CreateMemoDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var tenantId = db.CurrentTenantId;
        var request = await db.BudgetRequests.FirstOrDefaultAsync(r => r.Id == dto.BudgetRequestId && r.TenantId == tenantId, cancellationToken);
        if (request == null && isSuperAdmin)
            request = await db.BudgetRequests.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == dto.BudgetRequestId, cancellationToken);
        if (request == null) return Result<MemoResponseDto>.Failed("Budget request not found.");
        if (request.Status != BudgetRequestStatus.Approved)
            return Result<MemoResponseDto>.Failed("Budget request must be approved before creating memo.");
        var existing = await db.Memos.AnyAsync(m => m.BudgetRequestId == dto.BudgetRequestId && m.TenantId == (request.TenantId ?? tenantId), cancellationToken);
        if (existing) return Result<MemoResponseDto>.Failed("Memo already exists for this budget request.");
        var dept = await db.Departments.FindAsync(request.DepartmentId);
        var requester = await db.Users.FindAsync(request.UserId);
        var entity = new Memo
        {
            Id = Guid.NewGuid().ToString(),
            BudgetRequestId = request.Id,
            RequestedBy = requester?.UserName ?? requester?.Email ?? request.UserId,
            RequestedByDepartment = dept?.Name ?? "",
            Amount = request.Amount,
            Purpose = dto.Purpose ?? request.Purpose,
            Department = dept?.Name ?? "",
            Status = MemoStatus.Final,
            ApproversJson = request.ApprovalHistoryJson ?? "[]",
            TenantId = request.TenantId ?? tenantId ?? "",
            CreatedBy = userProfileService.GetUserId(),
            CreatedOn = DateTime.UtcNow
        };
        await db.Memos.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result<MemoResponseDto>.Success(MapToDto(entity));
    }

    public async Task<Result<MemoResponseDto>> UpdateAsync(string id, UpdateMemoDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<MemoResponseDto>.Failed("Memo not found.");
        if (dto.Purpose != null) entity.Purpose = dto.Purpose;
        if (!string.IsNullOrEmpty(dto.Status))
            entity.Status = Enum.TryParse<MemoStatus>(dto.Status, true, out var s) ? s : entity.Status;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Memos.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<MemoResponseDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Memo not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Memos.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<byte[]>> GeneratePdfAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<byte[]>.Failed("Memo not found.");
        try
        {
            var pdfBytes = BuildMemoPdf(entity);
            return Result<byte[]>.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failed("Failed to generate PDF: " + ex.Message);
        }
    }

    private static byte[] BuildMemoPdf(Memo entity)
    {
        var document = new Document();
        document.Info.Title = "Budget Memo";
        document.Info.Author = "Budget360";
        document.Info.Subject = "Approved Budget Request Memo";

        var style = document.Styles["Normal"];
        style.Font.Size = 10;
        style.Font.Color = Colors.Black;

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2);

        var title = section.AddParagraph();
        title.Format.SpaceBefore = Unit.FromCentimeter(0.5);
        title.Format.SpaceAfter = Unit.FromCentimeter(0.5);
        var titleText = title.AddFormattedText("BUDGET MEMO", TextFormat.Bold);
        titleText.Size = 18;
        titleText.Color = Colors.DarkBlue;

        section.AddParagraph().Format.Borders.Bottom.Width = 1;
        section.AddParagraph().Format.Borders.Bottom.Color = Colors.DarkBlue;
        section.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.5);

        var info = section.AddParagraph();
        info.Format.SpaceAfter = Unit.FromCentimeter(0.3);
        info.AddFormattedText("Memo ID: ", TextFormat.Bold).Size = 10;
        info.AddFormattedText(entity.Id).Size = 10;
        info.AddLineBreak();
        info.AddFormattedText("Budget Request ID: ", TextFormat.Bold).Size = 10;
        info.AddFormattedText(entity.BudgetRequestId).Size = 10;
        info.AddLineBreak();
        info.AddFormattedText("Date: ", TextFormat.Bold).Size = 10;
        info.AddFormattedText(entity.CreatedOn.ToString("dd MMMM yyyy")).Size = 10;
        info.AddLineBreak();
        info.AddFormattedText("Status: ", TextFormat.Bold).Size = 10;
        info.AddFormattedText(entity.Status.ToString()).Size = 10;

        section.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.5);
        var reqHeading = section.AddParagraph();
        reqHeading.AddFormattedText("Request Details", TextFormat.Bold).Size = 12;
        reqHeading.Format.SpaceAfter = Unit.FromCentimeter(0.3);

        var req = section.AddParagraph();
        req.AddFormattedText("Requested by: ", TextFormat.Bold).Size = 10;
        req.AddFormattedText(entity.RequestedBy).Size = 10;
        req.AddLineBreak();
        req.AddFormattedText("Department: ", TextFormat.Bold).Size = 10;
        req.AddFormattedText(entity.RequestedByDepartment).Size = 10;
        req.AddLineBreak();
        req.AddFormattedText("Amount: ", TextFormat.Bold).Size = 10;
        req.AddFormattedText(entity.Amount.ToString("N2")).Size = 10;
        req.AddLineBreak();
        req.AddFormattedText("Purpose: ", TextFormat.Bold).Size = 10;
        req.AddFormattedText(entity.Purpose ?? "").Size = 10;

        section.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.5);
        var appHeading = section.AddParagraph();
        appHeading.AddFormattedText("Approval Chain", TextFormat.Bold).Size = 12;
        appHeading.Format.SpaceAfter = Unit.FromCentimeter(0.3);

        var table = section.AddTable();
        table.Borders.Width = 0.5;
        table.Borders.Color = Colors.Black;
        table.AddColumn(Unit.FromCentimeter(2.5)); // Order
        table.AddColumn(Unit.FromCentimeter(3));  // Role
        table.AddColumn(Unit.FromCentimeter(3));  // User
        table.AddColumn(Unit.FromCentimeter(2.5)); // Date
        var headerRow = table.AddRow();
        headerRow.HeadingFormat = true;
        headerRow.Shading.Color = Colors.LightGray;
        headerRow.Cells[0].AddParagraph("Step");
        headerRow.Cells[1].AddParagraph("Role");
        headerRow.Cells[2].AddParagraph("Approver");
        headerRow.Cells[3].AddParagraph("Date");
        var approvers = ParseApproversFromJson(entity.ApproversJson);
        for (var i = 0; i < approvers.Count; i++)
        {
            var row = table.AddRow();
            row.Cells[0].AddParagraph((i + 1).ToString());
            row.Cells[1].AddParagraph(approvers[i].RoleName);
            row.Cells[2].AddParagraph(approvers[i].UserName ?? "-");
            row.Cells[3].AddParagraph(approvers[i].ApprovedAt?.ToString("dd MMM yyyy") ?? "-");
        }

        var renderer = new PdfDocumentRenderer(true);
        renderer.Document = document;
        renderer.RenderDocument();
        using var stream = new MemoryStream();
        renderer.PdfDocument.Save(stream, false);
        return stream.ToArray();
    }

    private static List<(string RoleName, string UserName, DateTime? ApprovedAt)> ParseApproversFromJson(string approversJson)
    {
        var list = new List<(string, string, DateTime?)>();
        if (string.IsNullOrWhiteSpace(approversJson)) return list;
        try
        {
            var raw = JsonSerializer.Deserialize<List<JsonElement>>(approversJson);
            if (raw == null) return list;
            foreach (var el in raw)
            {
                var roleName = el.TryGetProperty("roleName", out var rn) ? rn.GetString() ?? "" : "";
                var userName = el.TryGetProperty("userName", out var un) ? un.GetString() : null;
                DateTime? approvedAt = null;
                if (el.TryGetProperty("approvedAt", out var dt) && dt.TryGetDateTime(out var d))
                    approvedAt = d;
                list.Add((roleName, userName ?? "-", approvedAt));
            }
        }
        catch { }
        return list;
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(MemoListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        IQueryable<Memo> query = db.Memos.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(m => m.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
        {
            var deptName = await db.Departments.Where(d => d.Id == requestModel.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
            if (deptName != null)
                query = query.Where(m => m.Department == deptName);
        }
        if (!string.IsNullOrEmpty(requestModel.Status))
            query = query.Where(m => m.Status.ToString() == requestModel.Status);
        var list = await query.Take(10000).ToListAsync(cancellationToken);
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.WriteLine("Id,BudgetRequestId,RequestedBy,RequestedByDepartment,Amount,Purpose,Department,Status,CreatedOn");
        foreach (var m in list)
            writer.WriteLine($"{m.Id},{m.BudgetRequestId},{Escape(m.RequestedBy)},{Escape(m.RequestedByDepartment)},{m.Amount},{Escape(m.Purpose)},{Escape(m.Department)},{m.Status},{m.CreatedOn:O}");
        writer.Flush();
        stream.Position = 0;
        return Result<byte[]>.Success(stream.ToArray());
    }

    private static string Escape(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n')) return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }

    private static MemoResponseDto MapToDto(Memo m)
    {
        var approvers = new List<MemoApproverDto>();
        if (!string.IsNullOrEmpty(m.ApproversJson))
        {
            try
            {
                var raw = JsonSerializer.Deserialize<List<JsonElement>>(m.ApproversJson);
                if (raw != null)
                    foreach (var el in raw)
                    {
                        approvers.Add(new MemoApproverDto
                        {
                            RoleId = el.TryGetProperty("roleId", out var r) ? r.GetString() ?? "" : "",
                            RoleName = el.TryGetProperty("roleName", out var rn) ? rn.GetString() ?? "" : "",
                            UserId = el.TryGetProperty("userId", out var u) ? u.GetString() : null,
                            UserName = el.TryGetProperty("userName", out var un) ? un.GetString() : null,
                            SignatureUrl = el.TryGetProperty("signatureUrl", out var sig) ? sig.GetString() : null,
                            ApprovedAt = el.TryGetProperty("approvedAt", out var dt) && dt.TryGetDateTime(out var d) ? d : null
                        });
                    }
            }
            catch { }
        }
        return new MemoResponseDto
        {
            Id = m.Id,
            BudgetRequestId = m.BudgetRequestId,
            RequestedBy = m.RequestedBy,
            RequestedByDepartment = m.RequestedByDepartment,
            Amount = m.Amount,
            Purpose = m.Purpose,
            Department = m.Department,
            Approvers = approvers,
            Status = m.Status.ToString(),
            FileUrl = m.FileUrl,
            CreatedAt = m.CreatedOn,
            UpdatedAt = m.LastModifiedOn ?? m.CreatedOn
        };
    }
}
