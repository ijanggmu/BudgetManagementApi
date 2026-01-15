using Models.WebApi.TenantDTOs;

namespace Business.Common.PdfGeneration;

public interface IQuotationPdfService
{
    /// <summary>
    /// Generates a PDF document for a quotation with company branding
    /// </summary>
    /// <param name="quotation">Quotation data</param>
    /// <param name="prospectName">Prospect/Contact full name</param>
    /// <param name="prospectEmail">Prospect/Contact email</param>
    /// <param name="prospectPhone">Prospect/Contact phone</param>
    /// <param name="companyName">Company/Tenant name</param>
    /// <param name="logoUrl">Company logo URL (optional)</param>
    /// <returns>PDF document as byte array</returns>
    Task<byte[]> GenerateQuotationPdfAsync(
        QuotationResponseDto quotation,
        string prospectName,
        string? prospectEmail = null,
        string? prospectPhone = null,
        string? companyName = null,
        string? logoUrl = null);
}

