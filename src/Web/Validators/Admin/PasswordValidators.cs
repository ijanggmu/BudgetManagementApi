using FluentValidation;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.BeemaEdgeApi.Identity;

namespace BeemaEdgeApi.Validators.Admin;

public class ChangePasswordRequestModelValidator : AbstractValidator<ChangePasswordRequestModel>
{
    public ChangePasswordRequestModelValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old Password is required.")
            .MinimumLength(6).WithMessage("Old Password must be at least 6 characters.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New Password is required.")
            .MinimumLength(6).WithMessage("New Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("New Password cannot exceed 100 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("New Password must contain at least one uppercase letter, one lowercase letter, and one number.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.NewPassword).WithMessage("New Password and Confirm Password must match.");
    }
}

public class ForgetPasswordRequestModelValidator : AbstractValidator<ForgetPasswordRequestModel>
{
    public ForgetPasswordRequestModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(256).WithMessage("Username cannot exceed 256 characters.");
    }
}

public class ResetPasswordRequestModelValidator : AbstractValidator<ResetPasswordRequestModel>
{
    public ResetPasswordRequestModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(256).WithMessage("Username cannot exceed 256 characters.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("OTP Token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New Password is required.")
            .MinimumLength(6).WithMessage("New Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("New Password cannot exceed 100 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("New Password must contain at least one uppercase letter, one lowercase letter, and one number.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.NewPassword).WithMessage("New Password and Confirm Password must match.");
    }
}

