using System.Linq;
using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class CreateQuotationDtoValidator : AbstractValidator<CreateQuotationDto>
{
    public CreateQuotationDtoValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
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

public class UpdateQuotationDtoValidator : AbstractValidator<UpdateQuotationDto>
{
    public UpdateQuotationDtoValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) || 
                new[] { "Draft", "Submitted", "Approved", "Declined", "Accepted" }.Contains(status))
            .WithMessage("Status must be one of: Draft, Submitted, Approved, Declined, Accepted")
            .When(x => !string.IsNullOrEmpty(x.Status));
        
        RuleFor(x => x.TotalPremium)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TotalPremium.HasValue);
        
        RuleFor(x => x.DiscountPercent)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercent.HasValue);
        
        RuleForEach(x => x.Items)
            .SetValidator(new QuotationItemDtoValidator())
            .When(x => x.Items != null);
    }
}


