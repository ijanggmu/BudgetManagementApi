using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForQuotation : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Quotation>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<Quotation>(p => p.Number)
           .CanFilter()
           .CanSort();

        mapper.Property<Quotation>(p => p.Status)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.ProductId)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.ProspectId)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.TotalPremium)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.DiscountPercent)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.ValidUntil)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.PdfUrl)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<Quotation>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Quotation>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<Quotation>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<Quotation>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

