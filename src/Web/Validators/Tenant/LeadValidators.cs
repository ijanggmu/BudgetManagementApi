using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class CreateLeadPublicDtoValidator : AbstractValidator<CreateLeadPublicDto>
{
    public CreateLeadPublicDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Phone).MaximumLength(32);
    }
}

public class LeadActivityDtoValidator : AbstractValidator<LeadActivityDto>
{
    public LeadActivityDtoValidator()
    {
        RuleFor(x => x.Kind).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Notes).NotEmpty().MaximumLength(2000);
    }
}


