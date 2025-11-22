using FluentValidation;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;

namespace BeemaEdgeApi.Validators.Admin;

public class UpdateProfileRequestModelValidator : AbstractValidator<UpdateProfileRequestModel>
{
    public UpdateProfileRequestModelValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(32).WithMessage("Phone Number cannot exceed 32 characters.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || g == "Male" || g == "Female" || g == "Other")
            .WithMessage("Gender must be Male, Female, or Other.")
            .When(x => !string.IsNullOrEmpty(x.Gender));

        RuleFor(x => x.MaritialStatus)
            .MaximumLength(50).WithMessage("Marital Status cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.MaritialStatus));

        RuleFor(x => x.PanNo)
            .MaximumLength(9).WithMessage("PAN No cannot exceed 9 characters.")
            .When(x => !string.IsNullOrEmpty(x.PanNo));

        RuleFor(x => x.CitizenshipNo)
            .MaximumLength(50).WithMessage("Citizenship Number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.CitizenshipNo));

        RuleFor(x => x.PassportNumber)
            .MaximumLength(50).WithMessage("Passport Number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.PassportNumber));

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50).WithMessage("License Number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.LicenseNumber));

        RuleFor(x => x.Occupation)
            .MaximumLength(100).WithMessage("Occupation cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Occupation));
    }
}

