using FluentValidation;
using Models.Common.Policy.Policy;

namespace BeemaEdgeApi.Validators.Tenant;

public class PremiumCalculateRequestModelValidator : AbstractValidator<PremiumCalculateRequestModel>
{
    public PremiumCalculateRequestModelValidator()
    {
        //RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(64).WithMessage("Product code is required and must be valid");
        // Add more validation rules based on PremiumCalculateRequestModel properties
    }
}

