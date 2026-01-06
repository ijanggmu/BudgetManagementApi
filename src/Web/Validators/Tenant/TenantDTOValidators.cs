using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class CreateTenantDtoValidator : AbstractValidator<CreateTenantDto>
{
    public CreateTenantDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");
        RuleFor(x => x.CompanyBranding.Version).GreaterThan(0);

        // Validate AdminUser
        RuleFor(x => x.AdminUser).NotNull().WithMessage("Admin user information is required.");
        RuleFor(x => x.AdminUser.Email)
            .NotEmpty().WithMessage("Admin email is required.")
            .EmailAddress().WithMessage("Admin email must be a valid email address.")
            .MaximumLength(256).WithMessage("Admin email must not exceed 256 characters.");
        RuleFor(x => x.AdminUser.Username)
            .NotEmpty().WithMessage("Admin username is required.")
            .MaximumLength(256).WithMessage("Admin username must not exceed 256 characters.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Admin username must contain only letters, numbers, and underscores.");
        RuleFor(x => x.AdminUser.FullName)
            .NotEmpty().WithMessage("Admin full name is required.")
            .MaximumLength(200).WithMessage("Admin full name must not exceed 200 characters.");
        RuleFor(x => x.AdminUser.Password)
            .NotEmpty().WithMessage("Admin password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}

public class UpdateTenantDtoValidator : AbstractValidator<UpdateTenantDto>
{
    public UpdateTenantDtoValidator()
    {
        RuleFor(x => x.Name).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.Slug).MaximumLength(100).Matches("^[a-z0-9-]+$").When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");
        RuleFor(x => x.CompanyBranding.Version).GreaterThan(0);
    }
}

public class TenantLoginRequestDtoValidator : AbstractValidator<TenantLoginRequestDto>
{
    public TenantLoginRequestDtoValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Username).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}

