using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Models.WebApi.TenantDTOs;
using PdfSharp.Pdf;

namespace Business.AdminPortalApi.PdfGeneration;

public class QuotationPdfService : IQuotationPdfService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public QuotationPdfService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<byte[]> GenerateQuotationPdfAsync(
        QuotationResponseDto quotation,
        string prospectName,
        string? prospectEmail = null,
        string? prospectPhone = null,
        string? companyName = null,
        string? logoUrl = null)
    {
        // Create a new MigraDoc document
        var document = new Document();
        document.Info.Title = $"Quotation #{quotation.Number}";
        document.Info.Author = companyName ?? "BeemaEdge";
        document.Info.Subject = "Insurance Quotation";

        // Define styles
        DefineStyles(document);

        // Create a section
        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.TopMargin = Unit.FromCentimeter(2);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(2);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2);

        // Add header with logo
        await AddHeader(section, companyName, logoUrl);

        // Add quotation title
        var titleParagraph = section.AddParagraph();
        titleParagraph.Format.SpaceBefore = Unit.FromCentimeter(1);
        titleParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.5);
        var title = titleParagraph.AddFormattedText("QUOTATION", TextFormat.Bold);
        title.Size = 20;
        title.Color = Colors.DarkBlue;

        // Add quotation number and date
        var infoParagraph = section.AddParagraph();
        infoParagraph.Format.SpaceAfter = Unit.FromCentimeter(0.3);
        infoParagraph.AddFormattedText($"Quotation Number: ", TextFormat.Bold).Size = 10;
        infoParagraph.AddFormattedText(quotation.Number).Size = 10;
        infoParagraph.AddLineBreak();
        infoParagraph.AddFormattedText($"Date: ", TextFormat.Bold).Size = 10;
        infoParagraph.AddFormattedText(quotation.CreatedOn.ToString("dd MMMM yyyy")).Size = 10;
        if (quotation.ValidUntil.HasValue)
        {
            infoParagraph.AddLineBreak();
            infoParagraph.AddFormattedText($"Valid Until: ", TextFormat.Bold).Size = 10;
            infoParagraph.AddFormattedText(quotation.ValidUntil.Value.ToString("dd MMMM yyyy")).Size = 10;
        }

        // Add horizontal line
        section.AddParagraph().Format.Borders.Bottom.Width = 1;
        section.AddParagraph().Format.Borders.Bottom.Color = Colors.DarkBlue;
        section.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.5);

        // Add customer information
        var customerSection = section.AddParagraph();
        customerSection.Format.SpaceAfter = Unit.FromCentimeter(0.5);
        customerSection.AddFormattedText("Customer Information", TextFormat.Bold).Size = 12;
        customerSection.AddLineBreak();
        customerSection.AddFormattedText(prospectName).Size = 10;
        if (!string.IsNullOrEmpty(prospectEmail))
        {
            customerSection.AddLineBreak();
            customerSection.AddFormattedText($"Email: {prospectEmail}").Size = 10;
        }
        if (!string.IsNullOrEmpty(prospectPhone))
        {
            customerSection.AddLineBreak();
            customerSection.AddFormattedText($"Phone: {prospectPhone}").Size = 10;
        }

        section.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.5);

        // Add quotation items table
        AddQuotationItemsTable(section, quotation);

        // Add totals section
        AddTotalsSection(section, quotation);

        // Add footer
        AddFooter(section, companyName);

        // Render the document to PDF
        var pdfRenderer = new PdfDocumentRenderer(true);
        pdfRenderer.Document = document;
        pdfRenderer.RenderDocument();

        // Convert to byte array
        using var stream = new MemoryStream();
        pdfRenderer.PdfDocument.Save(stream);
        return stream.ToArray();
    }

    private void DefineStyles(Document document)
    {
        // Normal style
        var style = document.Styles["Normal"];
        style.Font.Size = 10;
        style.Font.Color = Colors.Black;

        // Heading style
        style = document.Styles.AddStyle("Heading1", "Normal");
        style.Font.Size = 16;
        style.Font.Bold = true;
        style.Font.Color = Colors.DarkBlue;
        style.ParagraphFormat.SpaceAfter = 6;

        // Table header style
        style = document.Styles.AddStyle("TableHeader", "Normal");
        style.Font.Bold = true;
        style.Font.Color = Colors.White;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        //style.Shading.Color = Colors.DarkBlue;
    }

    private async Task AddHeader(Section section, string? companyName, string? logoUrl)
    {
        var headerTable = section.AddTable();
        headerTable.AddColumn(Unit.FromCentimeter(6));
        headerTable.AddColumn(Unit.FromCentimeter(10));

        var headerRow = headerTable.AddRow();
        headerRow.Height = Unit.FromCentimeter(3);

        // Logo cell (left)
        var logoCell = headerRow.Cells[0];
        logoCell.VerticalAlignment = VerticalAlignment.Center;

        if (!string.IsNullOrEmpty(logoUrl))
        {
            try
            {
                var logoImage = await DownloadImageAsync(logoUrl);
                if (logoImage != null && logoImage.Length > 0)
                {
                    // Save image to temporary file for MigraDoc
                    var tempFile = Path.GetTempFileName();
                    await File.WriteAllBytesAsync(tempFile, logoImage);
                    
                    try
                    {
                        var image = logoCell.AddImage(tempFile);
                        image.Width = Unit.FromCentimeter(5);
                        image.LockAspectRatio = true;
                    }
                    finally
                    {
                        // Clean up temp file
                        try { File.Delete(tempFile); } catch { }
                    }
                }
            }
            catch
            {
                // If logo download fails, continue without logo
            }
        }

        // Company name cell (right)
        var companyCell = headerRow.Cells[1];
        companyCell.VerticalAlignment = VerticalAlignment.Center;
        var companyParagraph = companyCell.AddParagraph();
        companyParagraph.Format.Alignment = ParagraphAlignment.Right;
        if (!string.IsNullOrEmpty(companyName))
        {
            var companyText = companyParagraph.AddFormattedText(companyName, TextFormat.Bold);
            companyText.Size = 16;
            companyText.Color = Colors.DarkBlue;
        }
    }

    private void AddQuotationItemsTable(Section section, QuotationResponseDto quotation)
    {
        var table = section.AddTable();
        table.Style = "Table";
        table.Borders.Color = Colors.DarkBlue;
        table.Borders.Width = 0.25;
        table.Borders.Left.Width = 0.5;
        table.Borders.Right.Width = 0.5;
        table.Rows.LeftIndent = 0;

        // Define columns
        var column = table.AddColumn(Unit.FromCentimeter(1.5));
        column.Format.Alignment = ParagraphAlignment.Center;

        column = table.AddColumn(Unit.FromCentimeter(7));
        column.Format.Alignment = ParagraphAlignment.Left;

        column = table.AddColumn(Unit.FromCentimeter(3));
        column.Format.Alignment = ParagraphAlignment.Right;

        column = table.AddColumn(Unit.FromCentimeter(3));
        column.Format.Alignment = ParagraphAlignment.Right;

        // Header row
        var headerRow = table.AddRow();
        headerRow.HeadingFormat = true;
        headerRow.Format.Alignment = ParagraphAlignment.Center;
        headerRow.Format.Font.Bold = true;
        headerRow.Format.Font.Color = Colors.White;
        headerRow.Shading.Color = Colors.DarkBlue;
        headerRow.Height = Unit.FromCentimeter(0.8);

        headerRow.Cells[0].AddParagraph("No.");
        headerRow.Cells[1].AddParagraph("Coverage");
        headerRow.Cells[2].AddParagraph("Sum Insured");
        headerRow.Cells[3].AddParagraph("Premium");

        // Data rows
        int itemNumber = 1;
        foreach (var item in quotation.Items)
        {
            var row = table.AddRow();
            row.Height = Unit.FromCentimeter(0.6);
            row.VerticalAlignment = VerticalAlignment.Center;

            row.Cells[0].AddParagraph(itemNumber.ToString());
            row.Cells[1].AddParagraph($"Coverage {item.CoverageId}");
            row.Cells[2].AddParagraph(item.SumInsured.ToString("N2"));
            row.Cells[3].AddParagraph(item.Premium.ToString("N2"));

            itemNumber++;
        }
    }

    private void AddTotalsSection(Section section, QuotationResponseDto quotation)
    {
        section.AddParagraph().Format.SpaceBefore = Unit.FromCentimeter(0.5);

        var totalsTable = section.AddTable();
        totalsTable.AddColumn(Unit.FromCentimeter(11));
        totalsTable.AddColumn(Unit.FromCentimeter(3));

        // Subtotal
        var subtotalRow = totalsTable.AddRow();
        subtotalRow.Cells[0].AddParagraph("Subtotal:").Format.Alignment = ParagraphAlignment.Right;
        subtotalRow.Cells[0].Format.Font.Bold = true;
        var subtotal = quotation.Items?.Sum(i => i.Premium) ?? 0m;
        subtotalRow.Cells[1].AddParagraph(subtotal.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;

        // Discount
        if (quotation.DiscountPercent.HasValue && quotation.DiscountPercent.Value > 0)
        {
            var discountRow = totalsTable.AddRow();
            var discountAmount = subtotal * (quotation.DiscountPercent.Value / 100m);
            discountRow.Cells[0].AddParagraph($"Discount ({quotation.DiscountPercent.Value}%):").Format.Alignment = ParagraphAlignment.Right;
            discountRow.Cells[1].AddParagraph($"-{discountAmount:N2}").Format.Alignment = ParagraphAlignment.Right;
        }

        // Total
        var totalRow = totalsTable.AddRow();
        totalRow.Format.Font.Bold = true;
        totalRow.Format.Font.Size = 12;
        totalRow.Cells[0].Shading.Color = Colors.LightGray;
        totalRow.Cells[1].Shading.Color = Colors.LightGray;
        totalRow.Cells[0].Borders.Top.Width = 1;
        totalRow.Cells[0].Borders.Top.Color = Colors.DarkBlue;
        totalRow.Cells[1].Borders.Top.Width = 1;
        totalRow.Cells[1].Borders.Top.Color = Colors.DarkBlue;

        totalRow.Cells[0].AddParagraph("Total Premium:").Format.Alignment = ParagraphAlignment.Right;
        var totalPremium = quotation.TotalPremium ?? (subtotal - (quotation.DiscountPercent.HasValue ? subtotal * (quotation.DiscountPercent.Value / 100m) : 0m));
        totalRow.Cells[1].AddParagraph(totalPremium.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
    }

    private void AddFooter(Section section, string? companyName)
    {
        section.AddParagraph().Format.SpaceBefore = Unit.FromCentimeter(1.5);

        var footerParagraph = section.AddParagraph();
        footerParagraph.Format.Alignment = ParagraphAlignment.Center;
        footerParagraph.Format.Font.Size = 9;
        footerParagraph.Format.Font.Color = Colors.Gray;
        footerParagraph.AddFormattedText("Thank you for your business!", TextFormat.Italic);
        footerParagraph.AddLineBreak();
        if (!string.IsNullOrEmpty(companyName))
        {
            footerParagraph.AddFormattedText(companyName);
        }
        footerParagraph.AddLineBreak();
        footerParagraph.AddFormattedText("This is a computer-generated document.");
    }

    private async Task<byte[]?> DownloadImageAsync(string url)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        catch
        {
            // Return null if download fails
        }
        return null;
    }
}

