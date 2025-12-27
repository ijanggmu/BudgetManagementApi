using Business.Common.PremiumCalculation.Abstract;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using SharedKernel.Operation;

namespace Business.Common.PolicyCalculator;
public class PolicyPremiumCalculatorService(IPolicyPremiumCalculatorFactory policyPremiumCalculatorFactory) : IPolicyPremiumCalculatorService
{
    public async Task<Result<PremiumCalculationResultModel>> CalculatePolicyPremiumAsync(PremiumCalculateRequestModel requestModel)
    {
        var request = new CreatePolicyViewModel();
        var premiumAmount = 0.0m;
        string region = default;
        string planType = default;

        if (requestModel.InsuranceType == InsuranceType.ThirdPartyBike)
        {
            var isElectric = requestModel.ThirdPartyBikeInsurance.VechileType
                .ToString().Equals("Electric", StringComparison.OrdinalIgnoreCase);

            var ElectricMotorPartial = new ElectricMotorcyclePartialViewModel
            {
                IsThirdParty = true,
                Type = requestModel.ThirdPartyBikeInsurance.VechileType.ToString(),
                ManufactureCompany = requestModel.ThirdPartyBikeInsurance.ManufactureCompany,
                Model = requestModel.ThirdPartyBikeInsurance.Model,
                PurchasedNewOld = false,
                DateOfPurchase = DateTime.UtcNow,
                ChasisNumber = "123456",
                EngineNumber = "123456",
                RegistrationNumber = "123456",
                KiloWatt = isElectric ? requestModel.ThirdPartyBikeInsurance.KilloWattRange : 0,
                Days = 365,
                YearsFromRegistrationDateYears = DateTime.UtcNow.Date.ToString(),
                YearsFromRegistrationDateYearsBS = "2080-02-22",
                CurrentMarketPrice = "100",
                PortfolioId = "MCY"
            };
            var motorPartial = new MotorPartialViewModel
            {
                IsThirdParty = true,
                Type = requestModel.ThirdPartyBikeInsurance.VechileType.ToString(),
                ManufactureCompany = requestModel.ThirdPartyBikeInsurance.ManufactureCompany,
                Model = requestModel.ThirdPartyBikeInsurance.Model,
                PurchasedNewOld = false,
                DateOfPurchase = DateTime.UtcNow,
                ChasisNumber = "123456",
                EngineNumber = "123456",
                RegistrationNumber = "123456",
                CubicCapacity = isElectric ? 0 : (int)requestModel.ThirdPartyBikeInsurance.EngineCapacity,
                Days = "365",
                YearsFromRegistrationDateYears = DateTime.UtcNow.Date.ToString(),
                YearsFromRegistrationDateYearsBS = "2080-02-22",
                CurrentMarketPrice = "100",
                PortfolioId = "MCY"
            };

            request = new CreatePolicyViewModel
            {
                MotorPartial = isElectric ? null : motorPartial,
                ElectricMotorcyclePartial = isElectric ? ElectricMotorPartial : null,
                PortfolioAlias = isElectric ? PortfolioClassConstants.ElectricMotorcycle : PortfolioClassConstants.Motorcycle,
                PartyId = "Test",
                BancassuanceBankName = "Dummy Bank Name",
                BancassuanceBankBranch = "Dummy Bank Branch"
            };
        }

        else if (requestModel.InsuranceType == InsuranceType.FullBike)
        {
            var isElectric = requestModel.FullBikeInsurance.VechileType
                .ToString().Equals("Electric", StringComparison.OrdinalIgnoreCase);

            var fuelPartial = new MotorPartialViewModel
            {
                IsThirdParty = true,
                IsComprehensive = true,
                RiotStrike = true,
                EnterSumInsured = false,
                PortfolioId = "MCY",
                Type = requestModel.FullBikeInsurance.VechileType.ToString(),
                ManufactureYear = requestModel.FullBikeInsurance.ManufactureYear,
                ManufactureCompany = requestModel.FullBikeInsurance.ManufactureCompany,
                Model = requestModel.FullBikeInsurance.Model,
                PurchasedNewOld = true,
                DateOfPurchase = requestModel.FullBikeInsurance.DateOfPurchase,
                ChasisNumber = "123456",
                EngineNumber = "123456",
                RegistrationNumber = "123456",
                CubicCapacity = requestModel.FullBikeInsurance.EngineCapacity,
                Days = "365",
                YearsFromRegistrationDateYears = requestModel.FullBikeInsurance.YearOfRegistrationAD,
                YearsFromRegistrationDateYearsBS = requestModel.FullBikeInsurance.YearOfRegistrationBS,
                CurrentMarketPrice = requestModel.FullBikeInsurance.MarketValue,
                AgeOfVehicle = requestModel.FullBikeInsurance.AgeOfVechile,
                RateOfDepreciation = 5,
                SumInsuredAmount = 500000,
                DepreciationRate = 0,
                DepreciationAmount = 0,
                ValueOfAccessories = "0",
                ValueWithoutAccessories = "500000",
                Depreciation = true
            };

            var electricPartial = new ElectricMotorcyclePartialViewModel
            {
                IsThirdParty = true,
                IsComprehensive = true,
                RiotStrike = true,
                EnterSumInsured = false,
                PortfolioId = "EMCY",
                Type = requestModel.FullBikeInsurance.VechileType.ToString(),
                ManufactureYear = requestModel.FullBikeInsurance.ManufactureYear,
                ManufactureCompany = requestModel.FullBikeInsurance.ManufactureCompany,
                Model = requestModel.FullBikeInsurance.Model,
                PurchasedNewOld = true,
                DateOfPurchase = requestModel.FullBikeInsurance.DateOfPurchase,
                ChasisNumber = "123456",
                EngineNumber = "123456",
                RegistrationNumber = "123456",
                KiloWatt = requestModel.FullBikeInsurance.KilloWattRange,
                Days = 365,
                YearsFromRegistrationDateYears = requestModel.FullBikeInsurance.YearOfRegistrationAD,
                YearsFromRegistrationDateYearsBS = requestModel.FullBikeInsurance.YearOfRegistrationBS,
                CurrentMarketPrice = requestModel.FullBikeInsurance.MarketValue,
                AgeOfVehicle = requestModel.FullBikeInsurance.AgeOfVechile,
                RateOfDepreciation = 5,
                SumInsuredAmount = 500000,
                DepreciationRate = 0,
                DepreciationAmount = 0,
                ValueOfAccessories = "0",
                ValueWithoutAccessories = "500000",
                Depreciation = true
            };

            request = new CreatePolicyViewModel
            {
                MotorPartial = isElectric ? null : fuelPartial,
                ElectricMotorcyclePartial = isElectric ? electricPartial : null,
                PortfolioAlias = isElectric ? PortfolioClassConstants.ElectricMotorcycle : PortfolioClassConstants.Motorcycle,
                PartyId = "2222222222",
                BancassuanceBankName = "Dummy Bank Name",
                BancassuanceBankBranch = "Dummy Bank Branch"
            };
        }
        else if (requestModel.InsuranceType == InsuranceType.FullPrivateCar)
        {
            request = new CreatePolicyViewModel
            {
                PrivateVehiclePartial = new PrivateVehiclePartialViewModel
                {
                    IsComprehensive = true,
                    IsThirdParty = true,
                    EnterSumInsured = true,
                    TotalExcess = 0,
                    IsDirectDiscountPA = true,
                    LayupDays = 0,
                    ManufactureCompany = requestModel.ThirdPartyBikeInsurance.ManufactureCompany,
                    Model = requestModel.ThirdPartyBikeInsurance.Model,
                    PurchasedNewOld = true,
                    DateOfPurchase = DateTime.Parse("2022-06-24T08:59:52.264Z"),
                    ChasisNumber = "chasis",
                    EngineNumber = "engine",
                    CubicCapacity = (int)requestModel.ThirdPartyBikeInsurance.EngineCapacity,
                    CompulsoryExcess = 0,
                    VoluntaryExcess = 0,
                    RiotStrikeAndTerrorism = true,
                    YearsFromRegistrationDateYearsBS = "string",
                    CurrentMarketPrice = "5000000",
                    NumberofSeatsIncludingDriver = 4,
                    HasTailor = true,
                    ValueOfTailor = 0,
                    UseOfPrivateHire = true,
                    AgeOfVehicle = 0,
                    RateOfDepreciation = 0,
                    ValueOfAccessories = "12,8008.",
                    ValueWithoutAccessories = "5000000",
                    SumInsuredAmount = 0,
                    Days = "3",
                    NCDYears = 0,
                    AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = "string",
                    IsAgentInvolved = true,
                    IsRiotStrikeAndTerrorismForDriver = true,
                    IsRiotStrikeAndTerrorismForPassenger = true,
                    RiotStrikeAndTerrorismForPassengerSeatCount = 0,
                    IsDifferentlyAble = true,
                    Transportation = true,
                    Replacement = true,
                    Depreciation = true,
                    TransportationRate = 0,
                    TransportationAmount = 0,
                    ReplacementRate = 0,
                    ReplacementAmount = 0,
                    DepreciationRate = 0,
                    DepreciationAmount = 0,
                    HasSmartPolicy = false
                },
                PortfolioAlias = PortfolioClassConstants.PrivateVehicle,
                PartyId = "2222222222",
                BancassuanceBankName = "Dummy Bank Name",
                BancassuanceBankBranch = "Dummy Bank Branch"
            };

        }
        else if (requestModel.InsuranceType == InsuranceType.ThirdPartyPrivateCar)
        {
            request = new CreatePolicyViewModel
            {
                PrivateVehiclePartial = new PrivateVehiclePartialViewModel
                {
                    IsComprehensive = false,
                    IsThirdParty = true,
                    EnterSumInsured = false,
                    ManufactureCompany = requestModel.ThirdPartyPrivateCarInsurance.ManufactureCompany,
                    Model = requestModel.ThirdPartyPrivateCarInsurance.Model,
                    PurchasedNewOld = requestModel.ThirdPartyPrivateCarInsurance.PurchasedNewOld,
                    DateOfPurchase = DateTime.Parse("2022-06-24T08:59:52.264Z"),
                    ChasisNumber = "chasis",
                    EngineNumber = "engine",
                    CubicCapacity = (int)requestModel.ThirdPartyPrivateCarInsurance.EngineCapacity,
                    CompulsoryExcess = 0,
                    VoluntaryExcess = 0,
                    RiotStrikeAndTerrorism = false,
                    YearsFromRegistrationDateYearsBS = "string",
                    CurrentMarketPrice = "2000000",
                    NumberofSeatsIncludingDriver = 4,
                    SumInsuredAmount = 0,
                    Days = "365",
                    NCDYears = 0,
                    Transportation = true,
                    Replacement = true,
                    Depreciation = true,
                    TransportationRate = 0,
                    TransportationAmount = 0,
                    ReplacementRate = 0,
                    ReplacementAmount = 0,
                    DepreciationRate = 0,
                    DepreciationAmount = 0,
                    HasSmartPolicy = false
                },
                PortfolioAlias = PortfolioClassConstants.PrivateVehicle,
                PartyId = "2222222222",
                BancassuanceBankName = "Dummy Bank Name",
                BancassuanceBankBranch = "Dummy Bank Branch"
            };

        }
        else if (requestModel.InsuranceType == InsuranceType.Travel)
        {
            request = new CreatePolicyViewModel
            {
                TravelInsurancePartial = new TravelInsurancePartialViewModel
                {
                    PolicyPeriodInDays = 365,
                    PassportNumber = requestModel.TravelInsurance.PassportNumber,
                    VisitingCountry = requestModel.TravelInsurance.Country,
                    Occupation = requestModel.TravelInsurance.Occupation,
                    Phone = requestModel.TravelInsurance.Phone,
                    EmergencyContactName = requestModel.TravelInsurance.EmergencyContactName,
                    EmergencyContactNumber = requestModel.TravelInsurance.EmergencyContactNumber,
                    Province = requestModel.TravelInsurance.Province,
                    District = requestModel.TravelInsurance.District,
                    Municipality = requestModel.TravelInsurance.Municipality,
                    Ward = requestModel.TravelInsurance.Ward,
                    StreetAddress = requestModel.TravelInsurance.StreetAddress,
                    PremiumAmount = 58,
                    ExchangeRate = 145
                },
                PortfolioAlias = PortfolioClassConstants.TravelInsurance,
                PortfolioId = "TI",
                PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
                BancassuanceBankName = "dummyBank",
                BancassuanceBankBranch = "dummyBranch"
            };

        }
        else if (requestModel.InsuranceType == InsuranceType.InternationalTravel)
        {
            var regionHelperResponse = RegionHelper.GetDestinationRegion(requestModel.InternationalTravelInsurance?.VisitingCountry);
            if (regionHelperResponse.IsSuccess)
                region = regionHelperResponse.Data;
            else
                return Result<PremiumCalculationResultModel>.Failed(regionHelperResponse.Error);

            planType = requestModel.InternationalTravelInsurance?.PlanType.ToString();

            var usdRateModel = new TravelUSDRateRequestModel
            {
                Issuer = "ITI",
                Group = string.Empty,
                PlanType = requestModel.InternationalTravelInsurance.PlanType.ToString(),
                Period = requestModel.InternationalTravelInsurance.PolicyPeriodInDays,
                IsIndividual = !requestModel.InternationalTravelInsurance.IsFamilyIncluded,
                DestinationIncludes = region,
                Age = requestModel.InternationalTravelInsurance.Age,
            };

            premiumAmount = await CalculateUsdRate(usdRateModel);

            request = new CreatePolicyViewModel
            {
                InternationalTravelInsurancePartial = new ITIPartialViewModel
                {
                    PolicyPeriodInDays = requestModel.InternationalTravelInsurance.PolicyPeriodInDays,
                    PassportNumber = "12131414",
                    VisitingCountry = requestModel.InternationalTravelInsurance.VisitingCountry.FirstOrDefault(),
                    Occupation = "Ocupation",
                    Phone = "+9779801911051",
                    EmergencyContactName = "Emergency Contact Name",
                    EmergencyContactNumber = "+9779801911051",
                    Province = "Province",
                    District = "District",
                    Municipality = "Municipality",
                    Ward = "6",
                    StreetAddress = "Street Address",
                    PremiumAmount = premiumAmount,
                    Age = requestModel.InternationalTravelInsurance.Age.ToString(),

                },
                PortfolioAlias = PortfolioClassConstants.InternationalTravelInsurance,
                PortfolioId = "ITI",
                PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
                BancassuanceBankName = "dummyBank",
                BancassuanceBankBranch = "dummyBranch"
            };
        }

        var calculator = policyPremiumCalculatorFactory.GetCalculator(request.PortfolioAlias);
        var result = await calculator.CalculatePremium(request);

        result.PremiumUSD = premiumAmount;
        result.ExchangeRate = premiumAmount > 0 ? result.BasicPremium / premiumAmount : 0;
        result.Region = region;
        result.Plan = planType;

        return Result<PremiumCalculationResultModel>.Success(result);

    }
    public async Task<decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel)
    {
        //var result = await coreApiService.CalculateUsdRateAsync(requestModel);
        //return result;
        return 100;
    }
}

