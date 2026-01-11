using System;
using FluentValidation;
using Models.Common.Policy.Policy;

namespace BeemaEdgeApi.Validators.Tenant;

//public class PremiumCalculateRequestModelValidator : AbstractValidator<PremiumCalculateRequestModel>
//{
//    public PremiumCalculateRequestModelValidator()
//    {
//        RuleFor(x => x.InsuranceType)
//            .IsInEnum().WithMessage("Invalid insurance type")
//            .NotEmpty().WithMessage("Insurance type is required");

//        // Conditional validations based on InsuranceType
//        When(x => x.InsuranceType == InsuranceType.ThirdPartyBike, () =>
//        {
//            RuleFor(x => x.ThirdPartyBikeInsurance)
//                .NotNull().WithMessage("Third-party bike insurance is required.");
//            RuleFor(x => x.ThirdPartyBikeInsurance).SetValidator(new ThirdPartyBikeInsuranceModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.FullBike, () =>
//        {
//            RuleFor(x => x.FullBikeInsurance)
//                .NotNull().WithMessage("Full bike insurance is required.");
//            RuleFor(x => x.FullBikeInsurance).SetValidator(new BikeFullInsuranceModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.ThirdPartyPrivateCar, () =>
//        {
//            RuleFor(x => x.ThirdPartyPrivateCarInsurance)
//                .NotNull().WithMessage("Third-party private car insurance is required.");
//            RuleFor(x => x.ThirdPartyPrivateCarInsurance).SetValidator(new ThirdPartyPrivateCarInsuranceModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.FullPrivateCar, () =>
//        {
//            RuleFor(x => x.PrivateCarInsurance)
//                .NotNull().WithMessage("Private car insurance is required.");
//            RuleFor(x => x.PrivateCarInsurance).SetValidator(new PrivateCarInsuranceModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.FullCommercialVehicle, () =>
//        {
//            RuleFor(x => x.CommercialVehicleInsurance)
//                .NotNull().WithMessage("Commercial vehicle insurance is required.");
//            RuleFor(x => x.CommercialVehicleInsurance).SetValidator(new CommercialVehicleInsuranceModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.Travel, () =>
//        {
//            RuleFor(x => x.TravelInsurance)
//                .NotNull().WithMessage("Travel insurance is required.");
//            RuleFor(x => x.TravelInsurance).SetValidator(new TravelInsuranceRequestModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.InternationalTravel, () =>
//        {
//            RuleFor(x => x.InternationalTravelInsurance)
//                .NotNull().WithMessage("International travel insurance is required.");
//            RuleFor(x => x.InternationalTravelInsurance).SetValidator(new InternationalTravelInsurancePremiumCalculatorRequestModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.Home, () =>
//        {
//            RuleFor(x => x.HomeInsurance)
//                .NotNull().WithMessage("Home insurance is required.");
//            RuleFor(x => x.HomeInsurance).SetValidator(new HomeInsuranceRequestModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.Property, () =>
//        {
//            RuleFor(x => x.PropertyInsurance)
//                .NotNull().WithMessage("Property insurance is required.");
//            RuleFor(x => x.PropertyInsurance).SetValidator(new PropertyInsuranceRequestModelValidator());
//        });

//        When(x => x.InsuranceType == InsuranceType.Marine, () =>
//        {
//            RuleFor(x => x.MarineInsurance)
//                .NotNull().WithMessage("Marine insurance is required.");
//            RuleFor(x => x.MarineInsurance).SetValidator(new MarineInsuranceRequestModelValidator());
//        });

//    }
//}

public class PremiumCalculateRequestModelValidator : AbstractValidator<PremiumCalculateRequestModel>
{
    public PremiumCalculateRequestModelValidator()
    {
        // Validate InsuranceType
        RuleFor(x => x.InsuranceType)
            .NotNull().WithMessage("Insurance type is required")
            .IsValidEnumValue().WithMessage("Invalid insurance type");

        // Conditional validations
        ConfigureConditionalValidations();
    }

    private void ConfigureConditionalValidations()
    {
        // ThirdPartyBike - 0
        When(x => x.InsuranceType == InsuranceType.ThirdPartyBike, () =>
        {
            RuleFor(x => x.ThirdPartyBikeInsurance)
                .NotNull().WithMessage("Third-party bike insurance details are required")
                .SetValidator(new ThirdPartyBikeInsuranceModelValidator());
        });

        // FullBike - 1
        When(x => x.InsuranceType == InsuranceType.FullBike, () =>
        {
            RuleFor(x => x.FullBikeInsurance)
                .NotNull().WithMessage("Full bike insurance details are required")
                .SetValidator(new BikeFullInsuranceModelValidator());
        });

        // FullPrivateCar - 2
        When(x => x.InsuranceType == InsuranceType.FullPrivateCar, () =>
        {
            RuleFor(x => x.PrivateCarInsurance)
                .NotNull().WithMessage("Private car insurance details are required")
                .SetValidator(new PrivateCarInsuranceModelValidator());
        });

        // ThirdPartyPrivateCar - 3
        When(x => x.InsuranceType == InsuranceType.ThirdPartyPrivateCar, () =>
        {
            RuleFor(x => x.ThirdPartyPrivateCarInsurance)
                .NotNull().WithMessage("Third-party private car insurance details are required")
                .SetValidator(new ThirdPartyPrivateCarInsuranceModelValidator());
        });

        // FullCommercialVehicle - 4
        When(x => x.InsuranceType == InsuranceType.FullCommercialVehicle, () =>
        {
            RuleFor(x => x.CommercialVehicleInsurance)
                .NotNull().WithMessage("Commercial vehicle insurance details are required")
                .SetValidator(new CommercialVehicleInsuranceModelValidator());
        });

        // ThirdPartyCommercialVehicle - 5 (if you have this model)
        When(x => x.InsuranceType == InsuranceType.ThirdPartyCommercialVehicle, () =>
        {
            // Add validation if you create this model
            RuleFor(x => x.ThirdPartyPrivateCarInsurance) // or create specific model
                .NotNull().WithMessage("Third-party commercial vehicle insurance details are required");
        });

        // Travel - 6
        When(x => x.InsuranceType == InsuranceType.Travel, () =>
        {
            RuleFor(x => x.TravelInsurance)
                .NotNull().WithMessage("Travel insurance details are required")
                .SetValidator(new TravelInsuranceRequestModelValidator());
        });

        // InternationalTravel - 7
        When(x => x.InsuranceType == InsuranceType.InternationalTravel, () =>
        {
            RuleFor(x => x.InternationalTravelInsurance)
                .NotNull().WithMessage("International travel insurance details are required")
                .SetValidator(new InternationalTravelInsurancePremiumCalculatorRequestModelValidator());
        });

        // Marine - 8
        When(x => x.InsuranceType == InsuranceType.Marine, () =>
        {
            RuleFor(x => x.MarineInsurance)
                .NotNull().WithMessage("Marine insurance details are required")
                .SetValidator(new MarineInsuranceRequestModelValidator());
        });

        // Home - 9
        When(x => x.InsuranceType == InsuranceType.Home, () =>
        {
            RuleFor(x => x.HomeInsurance)
                .NotNull().WithMessage("Home insurance details are required")
                .SetValidator(new HomeInsuranceRequestModelValidator());
        });

        // Property - 10
        When(x => x.InsuranceType == InsuranceType.Property, () =>
        {
            RuleFor(x => x.PropertyInsurance)
                .NotNull().WithMessage("Property insurance details are required")
                .SetValidator(new PropertyInsuranceRequestModelValidator());
        });
    }
}
public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> IsValidEnumValue<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : struct, Enum
    {
        return ruleBuilder.Must(value => Enum.IsDefined(typeof(TProperty), value))
            .WithMessage("{PropertyName} has an invalid value");
    }
}
