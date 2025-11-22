using FluentValidation;
using Models.BeemaEdgeApi.Roles;

namespace BeemaEdgeApi.Validators.Admin;

public class CreateRoleRequestModelValidator : AbstractValidator<CreateRoleRequestModel>
{
    public CreateRoleRequestModelValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role Name is required.")
            .MaximumLength(256).WithMessage("Role Name cannot exceed 256 characters.")
            .Matches(@"^\s*(\w+\s)*\w+\s*$").WithMessage("Role Name can contain only one space between words.")
            .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("Role Name cannot be empty or whitespace.");

        RuleFor(x => x.RoleDescription)
            .MaximumLength(500).WithMessage("Role Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.RoleDescription));

        RuleFor(x => x.RoleType)
            .NotEmpty().WithMessage("Role Type is required.")
            .MaximumLength(50).WithMessage("Role Type cannot exceed 50 characters.");
    }
}

public class UpdateRoleRequestModelValidator : AbstractValidator<UpdateRoleRequestModel>
{
    public UpdateRoleRequestModelValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required.");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role Name is required.")
            .MaximumLength(256).WithMessage("Role Name cannot exceed 256 characters.")
            .Matches(@"^\s*(\w+\s)*\w+\s*$").WithMessage("Role Name can contain only one space between words.")
            .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("Role Name cannot be empty or whitespace.");

        RuleFor(x => x.RoleDescription)
            .MaximumLength(500).WithMessage("Role Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.RoleDescription));
    }
}

