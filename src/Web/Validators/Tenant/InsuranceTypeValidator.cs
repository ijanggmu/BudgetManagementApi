using FluentValidation;
using Models.Common.Policy.Policy;
using Models.WebApi.Customer.Policy;
using System;
using System.Linq;

namespace BeemaEdgeApi.Validators.Tenant;

public class PrivateCarInsuranceModelValidator : AbstractValidator<PrivateCarInsuranceModel>
{
    public PrivateCarInsuranceModelValidator()
    {
        RuleFor(x => x.VechileType)
            .IsInEnum().WithMessage("Invalid vehicle type");

        RuleFor(x => x.EngineCapacity)
            .IsInEnum().WithMessage("Invalid engine capacity category");

        RuleFor(x => x.KilloWattRange)
            .IsInEnum().WithMessage("Invalid kilowatt range");

        RuleFor(x => x.MarketValue)
            .GreaterThan(0).WithMessage("Market value must be greater than 0");

        RuleFor(x => x.SumInsuredAmount)
            .GreaterThan(0).WithMessage("Sum insured amount must be greater than 0")
            .LessThanOrEqualTo(x => x.MarketValue).WithMessage("Sum insured cannot exceed market value");

        //RuleFor(x => x.YearOfRegistration)
        //    .InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Invalid year of registration");

        //RuleFor(x => x.ExcessOnOwnDamageOptions)
        //    .NotEmpty().When(x => x.ExcessOnOwnDamageOptions != null && x.ExcessOnOwnDamageOptions.Any())
        //    .WithMessage("At least one excess option must be selected");

        //RuleFor(x => x.SeatingCapacityIncludingDriver)
        //    .GreaterThan(0).WithMessage("Seating capacity must be at least 1 (including driver)")
        //    .LessThanOrEqualTo(100).WithMessage("Seating capacity cannot exceed 100");

        //RuleFor(x => x.NoClaimDiscount)
        //    .IsInEnum().WithMessage("Invalid no claim discount type");
    }
}

public class ThirdPartyPrivateCarInsuranceModelValidator : AbstractValidator<ThirdPartyPrivateCarInsuranceModel>
{
    public ThirdPartyPrivateCarInsuranceModelValidator()
    {
        RuleFor(x => x.VechileType)
            .IsInEnum().WithMessage("Invalid vehicle type");

        RuleFor(x => x.EngineCapacity)
            .IsInEnum().WithMessage("Invalid engine capacity category");

        RuleFor(x => x.KilloWattRange)
            .IsInEnum().WithMessage("Invalid kilowatt range");

        RuleFor(x => x.NumberOfDrivers)
            .GreaterThan(0).WithMessage("Number of drivers must be greater than 0");

        RuleFor(x => x.NumberOfPassengers)
            .GreaterThanOrEqualTo(0).WithMessage("Number of passengers cannot be negative");

        RuleFor(x => x.ManufactureCompany)
            .NotEmpty().WithMessage("Manufacture company is required")
            .MaximumLength(100).WithMessage("Manufacture company cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");
    }
}

public class BikeFullInsuranceModelValidator : AbstractValidator<BikeFullInsuranceModel>
{
    public BikeFullInsuranceModelValidator()
    {
        RuleFor(x => x.VechileType)
            .IsInEnum().WithMessage("Invalid vehicle type");

        When(x => x.VechileType == VehicleType.Fuel, () =>
        {
            RuleFor(x => x.EngineCapacity)
                .GreaterThan(0).WithMessage("Engine capacity must be greater than 0 for fuel vehicles");
        });

        When(x => x.VechileType == VehicleType.Electric, () =>
        {
            RuleFor(x => x.KilloWattRange)
                .GreaterThan(0).WithMessage("Kilowatt range must be greater than 0 for electric vehicles");
        });

        RuleFor(x => x.MarketValue)
            .NotEmpty().WithMessage("Market Value is required.")
            .Custom((value, context) =>
            {
                if (!decimal.TryParse(value, out decimal marketValue))
                {
                    context.AddFailure("Market Value must be a valid number");
                }
                else if (marketValue <= 0)
                {
                    context.AddFailure("Market Value must be greater than 0");
                }
            });

        RuleFor(x => x.SumInsuredAmount)
            .GreaterThan(0).WithMessage("Sum insured amount must be greater than 0")
            .Custom((amount, context) =>
            {
                if (decimal.TryParse(context.InstanceToValidate.MarketValue, out decimal marketValue))
                {
                    if (amount > marketValue)
                    {
                        context.AddFailure("Sum insured cannot exceed market value");
                    }
                }
            });

        //RuleFor(x => x.YearOfRegistrationBS)
        //    .NotEmpty().WithMessage("Year Of Registration (BS) is required.")
        //    .Matches(@"^\d{4}$").WithMessage("Year must be in YYYY format");

        //RuleFor(x => x.AgeOfVechile)
        //    .GreaterThanOrEqualTo(0).WithMessage("Age of vehicle cannot be negative");

        //RuleFor(x => x.NoClaimDiscount)
        //    .InclusiveBetween(0, 100).WithMessage("No claim discount must be between 0 and 100");

        //RuleFor(x => x.ManufactureCompany)
        //    .NotEmpty().WithMessage("Manufacture company is required")
        //    .MaximumLength(100).WithMessage("Manufacture company cannot exceed 100 characters");

        //RuleFor(x => x.Model)
        //    .NotEmpty().WithMessage("Model is required")
        //    .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");
    }
}

public class ThirdPartyBikeInsuranceModelValidator : AbstractValidator<ThirdPartyBikeInsuranceModel>
{
    public ThirdPartyBikeInsuranceModelValidator()
    {
        RuleFor(x => x.VechileType)
            .IsInEnum().WithMessage("Invalid vehicle type");

        When(x => x.VechileType == VehicleType.Fuel, () =>
        {
            RuleFor(x => x.EngineCapacity)
                .NotNull().WithMessage("EngineCapacity is required for fuel vehicles")
                .GreaterThan(0).WithMessage("Engine capacity must be greater than 0");
        });

        When(x => x.VechileType == VehicleType.Electric, () =>
        {
            RuleFor(x => x.KilloWattRange)
                .NotNull().WithMessage("KilloWattRange is required for electric vehicles")
                .GreaterThan(0).WithMessage("Kilowatt range must be greater than 0");
        });

        RuleFor(x => x.ManufactureCompany)
            .NotEmpty().WithMessage("Manufacture company is required")
            .MaximumLength(100).WithMessage("Manufacture company cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");
    }
}

public class CommercialVehicleInsuranceModelValidator : AbstractValidator<CommercialVehicleInsuranceModel>
{
    public CommercialVehicleInsuranceModelValidator()
    {
        RuleFor(x => x.VechileType)
            .IsInEnum().WithMessage("Invalid vehicle type");

        RuleFor(x => x.CommercialVehicleClassType)
            .IsInEnum().WithMessage("Invalid commercial vehicle class type");

        RuleFor(x => x.KilloWattRange)
            .IsInEnum().WithMessage("Invalid kilowatt range");

        RuleFor(x => x.MarketValue)
            .GreaterThan(0).WithMessage("Market value must be greater than 0");

        RuleFor(x => x.SumInsuredAmount)
            .GreaterThan(0).WithMessage("Sum insured amount must be greater than 0")
            .LessThanOrEqualTo(x => x.MarketValue).WithMessage("Sum insured cannot exceed market value");

        //RuleFor(x => x.EngineCapacity)
        //    .IsInEnum().WithMessage("Invalid engine capacity category");

        //RuleFor(x => x.YearOfRegistration)
        //    .InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Invalid year of registration");

        //RuleFor(x => x.TrailorValue)
        //    .GreaterThanOrEqualTo(0).WithMessage("Trailor value cannot be negative");

        //RuleFor(x => x.SeatingCapacityIncludingDriver)
        //    .GreaterThan(0).WithMessage("Seating capacity must be at least 1 (including driver)")
        //    .LessThanOrEqualTo(200).WithMessage("Seating capacity cannot exceed 200 for commercial vehicles");

        //RuleFor(x => x.NoClaimDiscount)
        //    .IsInEnum().WithMessage("Invalid no claim discount type");
    }
}

public class TravelInsuranceRequestModelValidator : AbstractValidator<TravelInsuranceRequestModel>
{
    public TravelInsuranceRequestModelValidator()
    {
        //RuleFor(x => x.AgeGroup)
        //    .IsInEnum().WithMessage("Invalid age group");

        //RuleFor(x => x.Region)
        //    .IsInEnum().WithMessage("Invalid travel region");

        //RuleFor(x => x.Country)
        //    .NotEmpty().WithMessage("Country is required")
        //    .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

        //RuleFor(x => x.Plan)
        //    .IsInEnum().WithMessage("Invalid travel plan");

        //RuleFor(x => x.CoverageType)
        //    .IsInEnum().WithMessage("Invalid coverage type");

        //RuleFor(x => x.TenureInDays)
        //    .GreaterThan(0).WithMessage("Tenure in days must be greater than 0")
        //    .LessThanOrEqualTo(365).WithMessage("Tenure cannot exceed 365 days");

        //RuleFor(x => x.PassportNumber)
        //    .NotEmpty().WithMessage("Passport number is required")
        //    .MaximumLength(50).WithMessage("Passport number cannot exceed 50 characters");

        //RuleFor(x => x.Phone)
        //    .NotEmpty().WithMessage("Phone number is required")
        //    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

        //RuleFor(x => x.Occupation)
        //    .NotEmpty().WithMessage("Occupation is required")
        //    .MaximumLength(100).WithMessage("Occupation cannot exceed 100 characters");

        //RuleFor(x => x.EmergencyContactName)
        //    .NotEmpty().WithMessage("Emergency contact name is required")
        //    .MaximumLength(100).WithMessage("Emergency contact name cannot exceed 100 characters");

        //RuleFor(x => x.EmergencyContactNumber)
        //    .NotEmpty().WithMessage("Emergency contact number is required")
        //    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid emergency contact number format");

        //RuleFor(x => x.Province)
        //    .NotEmpty().WithMessage("Province is required")
        //    .MaximumLength(100).WithMessage("Province cannot exceed 100 characters");

        //RuleFor(x => x.District)
        //    .NotEmpty().WithMessage("District is required")
        //    .MaximumLength(100).WithMessage("District cannot exceed 100 characters");

        //RuleFor(x => x.Municipality)
        //    .NotEmpty().WithMessage("Municipality is required")
        //    .MaximumLength(100).WithMessage("Municipality cannot exceed 100 characters");
    }
}

public class InternationalTravelInsurancePremiumCalculatorRequestModelValidator : AbstractValidator<InternationalTravelInsurancePremiumCalculatorRequestModel>
{
    public InternationalTravelInsurancePremiumCalculatorRequestModelValidator()
    {
        //RuleFor(x => x.PolicyPeriodInDays)
        //    .GreaterThan(0).WithMessage("Policy period in days must be greater than 0")
        //    .LessThanOrEqualTo(365).WithMessage("Policy period cannot exceed 365 days");

        //RuleFor(x => x.TripType)
        //    .IsInEnum().WithMessage("Invalid trip type");

        //RuleFor(x => x.VisitingCountry)
        //    .NotEmpty().WithMessage("At least one visiting country must be provided")
        //    .Must(countries => countries != null && countries.Any())
        //    .WithMessage("At least one visiting country must be specified");

        //RuleForEach(x => x.VisitingCountry)
        //    .NotEmpty().WithMessage("Country name cannot be empty")
        //    .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters");

        //RuleFor(x => x.Age)
        //    .GreaterThanOrEqualTo(19).WithMessage("Age must be 19 or older")
        //    .LessThanOrEqualTo(100).WithMessage("Age must be 100 or younger");

        //RuleFor(x => x.TypeOfInsured)
        //    .NotEmpty().WithMessage("Type of insured is required")
        //    .Length(2, 50).WithMessage("Type of insured must be between 2 and 50 characters");

        //RuleFor(x => x.PlanType)
        //    .IsInEnum().WithMessage("Invalid travel plan type");
    }
}

public class HomeInsuranceRequestModelValidator : AbstractValidator<HomeInsuranceRequestModel>
{
    public HomeInsuranceRequestModelValidator()
    {
        RuleFor(x => x.SumInsuredAmount)
            .GreaterThan(0).WithMessage("Sum insured amount must be greater than 0");

        //RuleFor(x => x.RiskCode)
        //    .NotEmpty().WithMessage("Risk code is required")
        //    .MaximumLength(50).WithMessage("Risk code cannot exceed 50 characters");

        //RuleFor(x => x.BuildingComposition)
        //    .IsInEnum().WithMessage("Invalid building composition");

        //RuleFor(x => x.LoadingMultiplier)
        //    .GreaterThanOrEqualTo(0).WithMessage("Loading multiplier cannot be negative");

        //RuleFor(x => x.Equipment)
        //    .GreaterThanOrEqualTo(0).WithMessage("Equipment value cannot be negative");

        //RuleFor(x => x.RawMaterials)
        //    .GreaterThanOrEqualTo(0).WithMessage("Raw materials value cannot be negative");

        //RuleFor(x => x.WorkInProgress)
        //    .GreaterThanOrEqualTo(0).WithMessage("Work in progress value cannot be negative");

        //RuleFor(x => x.FinishedGoods)
        //    .GreaterThanOrEqualTo(0).WithMessage("Finished goods value cannot be negative");

        //RuleFor(x => x.SemiFinishedGoods)
        //    .GreaterThanOrEqualTo(0).WithMessage("Semi-finished goods value cannot be negative");

        //RuleFor(x => x.MoneyAndJewellery)
        //    .GreaterThanOrEqualTo(0).WithMessage("Money and jewellery value cannot be negative");

        //RuleFor(x => x.FurnitureFixtureOrFitting)
        //    .GreaterThanOrEqualTo(0).WithMessage("Furniture fixture or fitting value cannot be negative");

        //RuleFor(x => x.OtherItems)
        //    .GreaterThanOrEqualTo(0).WithMessage("Other items value cannot be negative");

        //RuleFor(x => x.Art)
        //    .GreaterThanOrEqualTo(0).WithMessage("Art value cannot be negative");
    }
}

public class PropertyInsuranceRequestModelValidator : AbstractValidator<PropertyInsuranceRequestModel>
{
    public PropertyInsuranceRequestModelValidator()
    {
        RuleFor(x => x.SumInsuredAmount)
            .GreaterThan(0).WithMessage("Sum insured amount must be greater than 0");

        //RuleFor(x => x.RiskType)
        //    .IsInEnum().WithMessage("Invalid risk type");

        //RuleFor(x => x.RiskCode)
        //    .NotEmpty().WithMessage("Risk code is required")
        //    .MaximumLength(50).WithMessage("Risk code cannot exceed 50 characters");

        //RuleFor(x => x.BuildingComposition)
        //    .IsInEnum().WithMessage("Invalid building composition");

        //RuleFor(x => x.LoadingMultiplier)
        //    .GreaterThanOrEqualTo(0).WithMessage("Loading multiplier cannot be negative");

        //RuleFor(x => x.SubsidyRate)
        //    .InclusiveBetween(0, 100).WithMessage("Subsidy rate must be between 0 and 100");

        //When(x => x.ProvideSubsidy, () =>
        //{
        //    RuleFor(x => x.SubsidyClassID)
        //        .NotEmpty().WithMessage("Subsidy class ID is required when subsidy is provided");
        //});

        //RuleFor(x => x.Equipment)
        //    .GreaterThanOrEqualTo(0).WithMessage("Equipment value cannot be negative");

        //RuleFor(x => x.RawMaterials)
        //    .GreaterThanOrEqualTo(0).WithMessage("Raw materials value cannot be negative");

        //RuleFor(x => x.WorkInProgress)
        //    .GreaterThanOrEqualTo(0).WithMessage("Work in progress value cannot be negative");

        //RuleFor(x => x.FinishedGoods)
        //    .GreaterThanOrEqualTo(0).WithMessage("Finished goods value cannot be negative");

        //RuleFor(x => x.SemiFinishedGoods)
        //    .GreaterThanOrEqualTo(0).WithMessage("Semi-finished goods value cannot be negative");

        //RuleFor(x => x.MoneyAndJewellery)
        //    .GreaterThanOrEqualTo(0).WithMessage("Money and jewellery value cannot be negative");

        //RuleFor(x => x.FurnitureFixtureOrFitting)
        //    .GreaterThanOrEqualTo(0).WithMessage("Furniture fixture or fitting value cannot be negative");

        //RuleFor(x => x.OtherItems)
        //    .GreaterThanOrEqualTo(0).WithMessage("Other items value cannot be negative");

        //RuleFor(x => x.Art)
        //    .GreaterThanOrEqualTo(0).WithMessage("Art value cannot be negative");
    }
}

public class MarineInsuranceRequestModelValidator : AbstractValidator<MarineInsuranceRequestModel>
{
    public MarineInsuranceRequestModelValidator()
    {
        //RuleFor(x => x.MarineType)
        //    .IsInEnum().WithMessage("Invalid marine type");

        //RuleFor(x => x.InvoiceValueNpr)
        //    .GreaterThan(0).WithMessage("Invoice value must be greater than 0");

        //RuleFor(x => x.TolerancePercent)
        //    .InclusiveBetween(0, 100).WithMessage("Tolerance percent must be between 0 and 100");

        //RuleFor(x => x.DutyPercent)
        //    .InclusiveBetween(0, 100).WithMessage("Duty percent must be between 0 and 100");

        //RuleFor(x => x.Currency)
        //    .IsInEnum().WithMessage("Invalid currency");

        //RuleFor(x => x.TransitDiscount)
        //    .IsInEnum().WithMessage("Invalid transit discount");

        //RuleFor(x => x.SrccPool)
        //    .IsInEnum().WithMessage("Invalid SRCC pool");

        //RuleFor(x => x.VolumeDiscount)
        //    .IsInEnum().WithMessage("Invalid volume discount");
    }
}
