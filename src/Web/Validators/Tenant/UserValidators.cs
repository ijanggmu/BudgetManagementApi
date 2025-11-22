using FluentValidation;
using Business.Common.TenantDomain;

namespace BeemaEdgeApi.Validators.Tenant;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).MaximumLength(32).When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MaximumLength(320).When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.PhoneNumber).MaximumLength(32).When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

