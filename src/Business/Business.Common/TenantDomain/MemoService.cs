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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Business.Common.TenantDomain;

/// <summary>Approval row with optional signature image bytes for PDF/DOCX generation.</summary>
internal sealed class ApprovalRowWithSignature(string roleName, string userName, DateTime? approvedAt, byte[]? signatureImage)
{
    public string RoleName { get; } = roleName;
    public string UserName { get; } = userName;
    public DateTime? ApprovedAt { get; } = approvedAt;
    public byte[]? SignatureImage { get; } = signatureImage;
}

public class MemoService(
    ApplicationDataContext db,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension,
    IBudgetMemoAuditService auditService,
    IHttpClientFactory httpClientFactory)
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
        if (requestModel.IncludeArchived == false)
            query = query.Where(m => m.Status != MemoStatus.Archived);

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
        return await CreateMemoEntityAsync(request, dto, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<MemoResponseDto>> CreateForRequestAsync(string budgetRequestId, CreateMemoDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var tenantId = db.CurrentTenantId;
        var request = await db.BudgetRequests.FirstOrDefaultAsync(r => r.Id == budgetRequestId && r.TenantId == tenantId, cancellationToken);
        if (request == null && isSuperAdmin)
            request = await db.BudgetRequests.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == budgetRequestId, cancellationToken);
        if (request == null) return Result<MemoResponseDto>.Failed("Budget request not found.");
        var existing = await db.Memos.AnyAsync(m => m.BudgetRequestId == budgetRequestId && m.TenantId == (request.TenantId ?? tenantId), cancellationToken);
        if (existing) return Result<MemoResponseDto>.Failed("Memo already exists for this budget request.");
        var dtoWithRequestId = new CreateMemoDto
        {
            BudgetRequestId = budgetRequestId,
            MemoTemplateId = dto.MemoTemplateId,
            BudgetHeadingId = dto.BudgetHeadingId,
            BudgetSubheadingId = dto.BudgetSubheadingId,
            Purpose = dto.Purpose,
            Notes = dto.Notes
        };
        return await CreateMemoEntityAsync(request, dtoWithRequestId, cancellationToken);
    }

    private async Task<Result<MemoResponseDto>> CreateMemoEntityAsync(BudgetRequest request, CreateMemoDto dto, CancellationToken cancellationToken)
    {
        var tenantId = db.CurrentTenantId;
        var dept = await db.Departments.FindAsync(request.DepartmentId);
        var requester = await db.Users.FindAsync(request.UserId);
        var entity = new Memo
        {
            Id = Guid.NewGuid().ToString(),
            BudgetRequestId = request.Id,
            MemoTemplateId = dto.MemoTemplateId,
            BudgetHeadingId = dto.BudgetHeadingId,
            BudgetSubheadingId = dto.BudgetSubheadingId,
            RequestedBy = requester?.UserName ?? requester?.Email ?? request.UserId,
            RequestedByDepartment = dept?.Name ?? "",
            Amount = request.Amount,
            Purpose = dto.Purpose ?? request.Purpose ?? "",
            Department = dept?.Name ?? "",
            Status = MemoStatus.Draft,
            ApproversJson = request.ApprovalHistoryJson ?? "[]",
            TenantId = request.TenantId ?? tenantId ?? "",
            CreatedBy = userProfileService.GetUserId(),
            CreatedOn = DateTime.UtcNow
        };
        await db.Memos.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync("Memo", entity.Id, "Created", $"BudgetRequest {entity.BudgetRequestId}", cancellationToken);
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
        var oldStatus = entity.Status;
        if (dto.Purpose != null) entity.Purpose = dto.Purpose;
        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<MemoStatus>(dto.Status, true, out var parsedStatus))
            entity.Status = parsedStatus;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Memos.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        if (oldStatus != entity.Status)
            await auditService.LogAsync("Memo", entity.Id, "StatusChange", $"{oldStatus} → {entity.Status}", cancellationToken);
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
        await auditService.LogAsync("Memo", entity.Id, "Deleted", null, cancellationToken);
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
            var approvalRows = await GetApprovalRowsWithSignaturesAsync(entity, cancellationToken);
            var pdfBytes = BuildMemoPdf(entity, approvalRows);
            return Result<byte[]>.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failed("Failed to generate PDF: " + ex.Message);
        }
    }

    /// <summary>Load approval data from linked BudgetRequest (so CFO/CEO signatures appear after approval) and fetch signature images.</summary>
    private async Task<List<ApprovalRowWithSignature>> GetApprovalRowsWithSignaturesAsync(Memo entity, CancellationToken cancellationToken)
    {
        var approvalHistoryJson = entity.ApproversJson ?? "[]";
        var request = await db.BudgetRequests.FirstOrDefaultAsync(r => r.Id == entity.BudgetRequestId, cancellationToken);
        if (request != null && !string.IsNullOrEmpty(request.ApprovalHistoryJson))
            approvalHistoryJson = request.ApprovalHistoryJson;

        var rows = ParseApprovalHistoryFromJson(approvalHistoryJson);
        var result = new List<ApprovalRowWithSignature>();
        var http = httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        foreach (var (roleName, userName, approvedAt, signatureUrl) in rows)
        {
            byte[]? signatureImage = null;
            if (!string.IsNullOrEmpty(signatureUrl))
            {
                try
                {
                    var bytes = await http.GetByteArrayAsync(signatureUrl, cancellationToken);
                    if (bytes != null && bytes.Length > 0)
                        signatureImage = bytes;
                }
                catch { /* leave null */ }
            }
            result.Add(new ApprovalRowWithSignature(roleName, userName ?? "-", approvedAt, signatureImage));
        }
        return result;
    }

    private static List<(string RoleName, string? UserName, DateTime? ApprovedAt, string? SignatureUrl)> ParseApprovalHistoryFromJson(string json)
    {
        var list = new List<(string, string?, DateTime?, string?)>();
        if (string.IsNullOrWhiteSpace(json)) return list;
        try
        {
            var raw = JsonSerializer.Deserialize<List<JsonElement>>(json);
            if (raw == null) return list;
            foreach (var el in raw)
            {
                var roleName = el.TryGetProperty("roleName", out var rn) ? rn.GetString() ?? "" : "";
                var userName = el.TryGetProperty("userName", out var un) ? un.GetString() : null;
                DateTime? approvedAt = null;
                if (el.TryGetProperty("approvedAt", out var dt) && dt.TryGetDateTime(out var d))
                    approvedAt = d;
                var signatureUrl = el.TryGetProperty("signatureUrl", out var sig) ? sig.GetString() : null;
                list.Add((roleName, userName, approvedAt, signatureUrl));
            }
        }
        catch { }
        return list;
    }

    private static byte[] BuildMemoPdf(Memo entity, List<ApprovalRowWithSignature> approvalRows)
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
        table.AddColumn(Unit.FromCentimeter(3));  // Signature
        var headerRow = table.AddRow();
        headerRow.HeadingFormat = true;
        headerRow.Shading.Color = Colors.LightGray;
        headerRow.Cells[0].AddParagraph("Step");
        headerRow.Cells[1].AddParagraph("Role");
        headerRow.Cells[2].AddParagraph("Approver");
        headerRow.Cells[3].AddParagraph("Date");
        headerRow.Cells[4].AddParagraph("Signature");
        for (var i = 0; i < approvalRows.Count; i++)
        {
            var row = table.AddRow();
            var ar = approvalRows[i];
            row.Cells[0].AddParagraph((i + 1).ToString());
            row.Cells[1].AddParagraph(ar.RoleName);
            row.Cells[2].AddParagraph(ar.UserName ?? "-");
            row.Cells[3].AddParagraph(ar.ApprovedAt?.ToString("dd MMM yyyy") ?? "-");
            if (ar.SignatureImage != null && ar.SignatureImage.Length > 0)
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"sig_{Guid.NewGuid():N}.png");
                try
                {
                    System.IO.File.WriteAllBytes(tempPath, ar.SignatureImage);
                    row.Cells[4].AddImage(tempPath);
                }
                finally
                {
                    try { if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath); } catch { }
                }
            }
            else
            {
                row.Cells[4].AddParagraph(ar.ApprovedAt.HasValue ? "Signed" : "-");
            }
        }

        var renderer = new PdfDocumentRenderer(true);
        renderer.Document = document;
        renderer.RenderDocument();
        using var stream = new MemoryStream();
        renderer.PdfDocument.Save(stream, false);
        return stream.ToArray();
    }

    public async Task<Result<byte[]>> GenerateDocxAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Memos.Where(m => m.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<byte[]>.Failed("Memo not found.");
        try
        {
            var approvalRows = await GetApprovalRowsWithSignaturesAsync(entity, cancellationToken);
            var docxBytes = BuildMemoDocx(entity, approvalRows);
            return Result<byte[]>.Success(docxBytes);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failed("Failed to generate document: " + ex.Message);
        }
    }

    private static byte[] BuildMemoDocx(Memo entity, List<ApprovalRowWithSignature> approvalRows)
    {
        using var stream = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new W.Document();
            var body = mainPart.Document.AppendChild(new W.Body());

            body.AppendChild(new W.Paragraph(new W.Run(new W.Text("BUDGET MEMO")))
            {
                ParagraphProperties = new W.ParagraphProperties(
                    new W.ParagraphStyleId { Val = "Title" },
                    new W.Justification { Val = W.JustificationValues.Center })
            });
            body.AppendChild(new W.Paragraph());

            var infoPara = new W.Paragraph();
            infoPara.AppendChild(new W.Run(new W.Text($"Memo ID: {entity.Id}")));
            infoPara.AppendChild(new W.Break());
            infoPara.AppendChild(new W.Run(new W.Text($"Budget Request ID: {entity.BudgetRequestId}")));
            infoPara.AppendChild(new W.Break());
            infoPara.AppendChild(new W.Run(new W.Text($"Date: {entity.CreatedOn:dd MMMM yyyy}")));
            infoPara.AppendChild(new W.Break());
            infoPara.AppendChild(new W.Run(new W.Text($"Status: {entity.Status}")));
            body.AppendChild(infoPara);
            body.AppendChild(new W.Paragraph());

            body.AppendChild(new W.Paragraph(new W.Run(new W.Text("Request Details"))));
            var reqPara = new W.Paragraph();
            reqPara.AppendChild(new W.Run(new W.Text($"Requested by: {entity.RequestedBy}")));
            reqPara.AppendChild(new W.Break());
            reqPara.AppendChild(new W.Run(new W.Text($"Department: {entity.RequestedByDepartment}")));
            reqPara.AppendChild(new W.Break());
            reqPara.AppendChild(new W.Run(new W.Text($"Amount: {entity.Amount:N2}")));
            reqPara.AppendChild(new W.Break());
            reqPara.AppendChild(new W.Run(new W.Text($"Purpose: {entity.Purpose ?? ""}")));
            body.AppendChild(reqPara);
            body.AppendChild(new W.Paragraph());

            body.AppendChild(new W.Paragraph(new W.Run(new W.Text("Approval Chain"))));
            var table = new W.Table(
                new W.TableProperties(
                    new W.TableBorders(
                        new W.TopBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 },
                        new W.BottomBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 },
                        new W.LeftBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 },
                        new W.RightBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 },
                        new W.InsideHorizontalBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 },
                        new W.InsideVerticalBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 4 })));

            var headerRow = new W.TableRow();
            foreach (var header in new[] { "Step", "Role", "Approver", "Date", "Signature" })
                headerRow.AppendChild(new W.TableCell(new W.Paragraph(new W.Run(new W.Text(header)) { RunProperties = new W.RunProperties(new W.Bold()) })));
            table.AppendChild(headerRow);

            for (var i = 0; i < approvalRows.Count; i++)
            {
                var ar = approvalRows[i];
                var row = new W.TableRow();
                row.AppendChild(NewDocxTableCell((i + 1).ToString()));
                row.AppendChild(NewDocxTableCell(ar.RoleName));
                row.AppendChild(NewDocxTableCell(ar.UserName ?? "-"));
                row.AppendChild(NewDocxTableCell(ar.ApprovedAt?.ToString("dd MMM yyyy") ?? "-"));
                if (ar.SignatureImage != null && ar.SignatureImage.Length > 0)
                {
                    var imageCell = new W.TableCell();
                    var imagePart = mainPart.AddImagePart(ImagePartType.Png);
                    using (var ms = new MemoryStream(ar.SignatureImage))
                        imagePart.FeedData(ms);
                    var relId = mainPart.GetIdOfPart(imagePart);
                    imageCell.AppendChild(NewDocxParagraphWithImage(relId));
                    row.AppendChild(imageCell);
                }
                else
                    row.AppendChild(NewDocxTableCell(ar.ApprovedAt.HasValue ? "Signed" : "-"));
                table.AppendChild(row);
            }
            body.AppendChild(table);
            mainPart.Document.Save();
        }
        return stream.ToArray();
    }

    private static W.TableCell NewDocxTableCell(string text)
    {
        return new W.TableCell(new W.Paragraph(new W.Run(new W.Text(text))));
    }

    private static W.Paragraph NewDocxParagraphWithImage(string relationshipId)
    {
        const int emuPerPixel = 9525;
        var width = (long)(80 * emuPerPixel);
        var height = (long)(40 * emuPerPixel);
        var drawing = new W.Drawing(
            new DW.Inline(
                new DW.Extent { Cx = width, Cy = height },
                new DW.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new DW.DocProperties { Id = 1U, Name = "Signature" },
                new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties { Id = 0U, Name = "Signature.png" },
                                new PIC.NonVisualPictureDrawingProperties()),
                            new PIC.BlipFill(
                                new A.Blip { Embed = relationshipId },
                                new A.Stretch(new A.FillRectangle())),
                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0, Y = 0 },
                                    new A.Extents { Cx = width, Cy = height }),
                                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })))
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            ) { DistanceFromTop = 0, DistanceFromBottom = 0, DistanceFromLeft = 0, DistanceFromRight = 0 });
        return new W.Paragraph(new W.Run(drawing));
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
            MemoTemplateId = m.MemoTemplateId,
            BudgetHeadingId = m.BudgetHeadingId,
            BudgetSubheadingId = m.BudgetSubheadingId,
            RequestedBy = m.RequestedBy,
            RequestedByDepartment = m.RequestedByDepartment,
            Amount = m.Amount,
            Purpose = m.Purpose ?? "",
            Department = m.Department,
            Approvers = approvers,
            Status = m.Status.ToString(),
            FileUrl = m.FileUrl,
            CreatedAt = m.CreatedOn,
            UpdatedAt = m.LastModifiedOn ?? m.CreatedOn
        };
    }
}
