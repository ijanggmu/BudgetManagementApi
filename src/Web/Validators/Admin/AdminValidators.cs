using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Admin;

public class CreateAdminDtoValidator : AbstractValidator<CreateAdminDto>
{
    public CreateAdminDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(256).WithMessage("Username cannot exceed 256 characters.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(32).WithMessage("Phone Number cannot exceed 32 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.Password).WithMessage("Password and Confirm Password must match.");

        RuleFor(x => x.Roles)
            .Must(roles => roles == null || roles.Count > 0).WithMessage("If roles are provided, at least one role must be specified.")
            .When(x => x.Roles != null);

        RuleForEach(x => x.Roles)
            .NotEmpty().WithMessage("Role cannot be empty.")
            .MaximumLength(256).WithMessage("Role name cannot exceed 256 characters.")
            .When(x => x.Roles != null && x.Roles.Count > 0);
    }
}

public class UpdateAdminDtoValidator : AbstractValidator<UpdateAdminDto>
{
    public UpdateAdminDtoValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(32).WithMessage("Phone Number cannot exceed 32 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Roles)
            .Must(roles => roles == null || roles.Count > 0).WithMessage("If roles are provided, at least one role must be specified.")
            .When(x => x.Roles != null);

        RuleForEach(x => x.Roles)
            .NotEmpty().WithMessage("Role cannot be empty.")
            .MaximumLength(256).WithMessage("Role name cannot exceed 256 characters.")
            .When(x => x.Roles != null && x.Roles.Count > 0);
    }
}

