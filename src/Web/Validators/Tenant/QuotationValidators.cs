using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class CreateQuotationDtoValidator : AbstractValidator<CreateQuotationDto>
{
    public CreateQuotationDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProspectId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
    }
}

public class QuotationItemDtoValidator : AbstractValidator<QuotationItemDto>
{
    public QuotationItemDtoValidator()
    {
        RuleFor(x => x.CoverageId).NotEmpty();
        RuleFor(x => x.SumInsured).GreaterThan(0);
    }
}


