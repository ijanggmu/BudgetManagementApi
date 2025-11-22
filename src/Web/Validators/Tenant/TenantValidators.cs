using FluentValidation;
using Data.Entities.Tenant;

namespace BeemaEdgeApi.Validators.Tenant;

public class TenantValidator : AbstractValidator<Data.Entities.Tenant.Tenant>
{
    public TenantValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");
    }
}

