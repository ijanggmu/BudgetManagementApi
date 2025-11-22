using FluentValidation;
using Models.BeemaEdgeApi.Roles;

namespace BeemaEdgeApi.Validators.Admin;

public class PermissionManagementViewModelValidator : AbstractValidator<PermissionManagementViewModel>
{
    public PermissionManagementViewModelValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required.");

        RuleFor(x => x.ClaimList)
            .NotNull().WithMessage("Claim List is required.")
            .Must(list => list != null && list.Count > 0).WithMessage("At least one claim must be provided.")
            .When(x => x.ClaimList != null);

        RuleForEach(x => x.ClaimList)
            .NotEmpty().WithMessage("Claim cannot be empty.")
            .MaximumLength(200).WithMessage("Claim cannot exceed 200 characters.")
            .When(x => x.ClaimList != null);
    }
}

