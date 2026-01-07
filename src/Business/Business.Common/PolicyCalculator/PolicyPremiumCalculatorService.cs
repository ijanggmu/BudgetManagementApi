using System.Reflection;
using Business.Common.PremiumCalculation.Abstract;
using Business.Common.PremiumCalculation.Calculator.Travel;
using Business.Common.PremiumCalculation.Service;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Fire;
using Models.Common.Policy.Policy.Marine;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.ThirdPartyApi.e2e;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using SharedKernel.Operation;

namespace Business.Common.PolicyCalculator;

public class PolicyPremiumCalculatorService(
    IPolicyPremiumCalculatorFactory policyPremiumCalculatorFactory,
    ITravelRateService travelRateService) : IPolicyPremiumCalculatorService
{
    private const string DefaultPartyId = "2222222222";
    private const string DefaultBankName = "Dummy Bank Name";
    private const string DefaultBankBranch = "Dummy Bank Branch";
    private const string DefaultChasisNumber = "chasis";
    private const string DefaultEngineNumber = "engine";
    private const int DefaultPolicyDays = 365;

    public async Task<Result<ICalculationPremiumJson>> CalculatePolicyPremiumAsync(PremiumCalculateRequestModel requestModel)
    {
        var premiumAmount = 0.0m;
        string region = default;
        string planType = default;
        CreatePolicyViewModel request;

        if (requestModel.InsuranceType == InsuranceType.InternationalTravel)
        {
            var travelResult = await CreateInternationalTravelRequest(requestModel);
            request = travelResult.Request;
            premiumAmount = travelResult.PremiumAmount;
            region = travelResult.Region;
            planType = travelResult.PlanType;
        }
        else
        {
            request = requestModel.InsuranceType switch
            {
                InsuranceType.ThirdPartyBike => CreateThirdPartyBikeRequest(requestModel),
                InsuranceType.FullBike => CreateFullBikeRequest(requestModel),
                InsuranceType.FullPrivateCar => CreateFullPrivateCarRequest(requestModel),
                InsuranceType.ThirdPartyPrivateCar => CreateThirdPartyPrivateCarRequest(requestModel),
                InsuranceType.Travel => CreateTravelRequest(requestModel),
                InsuranceType.Marine => CreateMarineRequest(requestModel),
                InsuranceType.FullCommercialVehicle or InsuranceType.ThirdPartyCommercialVehicle 
                    => CreateCommercialVehicleRequest(requestModel, requestModel.InsuranceType == InsuranceType.FullCommercialVehicle),
                InsuranceType.Home or InsuranceType.Property => CreateFireInsuranceRequest(requestModel),
                _ => throw new NotSupportedException($"Insurance type {requestModel.InsuranceType} is not supported")
            };
        }

        var calculator = policyPremiumCalculatorFactory.GetCalculator(request.PortfolioAlias);
        var result = await calculator.CalculatePremium(request);

        result.PremiumUSD = premiumAmount;
        result.ExchangeRate = premiumAmount > 0 ? result.BasicPremium / premiumAmount : 0;
        result.Region = region;
        result.Plan = planType;
        var response = PremiumCalculationJsonService.GetCalculationJson(request.PortfolioAlias, result);
        return Result<ICalculationPremiumJson>.Success(response);
    }

    public async Task<decimal> CalculateUsdRate(TravelUSDRateRequestModel requestModel)
    {
        return await travelRateService.GetTravelUSDRateAsync(requestModel);
    }

    #region Bike Insurance Methods

    private CreatePolicyViewModel CreateThirdPartyBikeRequest(PremiumCalculateRequestModel requestModel)
    {
        var isElectric = requestModel.ThirdPartyBikeInsurance.VechileType == VechileType.Electric;

        return new CreatePolicyViewModel
        {
            MotorPartial = isElectric ? null : CreateMotorPartialForThirdPartyBike(requestModel),
            ElectricMotorcyclePartial = isElectric ? CreateElectricMotorcyclePartialForThirdPartyBike(requestModel) : null,
            PortfolioAlias = isElectric ? PortfolioClassConstants.ElectricMotorcycle : PortfolioClassConstants.Motorcycle,
            PartyId = "Test",
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch
        };
    }

    private MotorPartialViewModel CreateMotorPartialForThirdPartyBike(PremiumCalculateRequestModel requestModel)
    {
        return new MotorPartialViewModel
        {
            IsThirdParty = true,
            Type = requestModel.ThirdPartyBikeInsurance.VechileType.ToString(),
            ManufactureCompany = requestModel.ThirdPartyBikeInsurance.ManufactureCompany,
            Model = requestModel.ThirdPartyBikeInsurance.Model,
            PurchasedNewOld = false,
            DateOfPurchase = DateTime.UtcNow,
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            RegistrationNumber = "123456",
            CubicCapacity = (int)(requestModel.ThirdPartyBikeInsurance.EngineCapacity ?? 0),
            Days = DefaultPolicyDays.ToString(),
            YearsFromRegistrationDateYears = DateTime.UtcNow.Date.ToString(),
            YearsFromRegistrationDateYearsBS = "2080-02-22",
            CurrentMarketPrice = "100",
            PortfolioId = "MCY"
        };
    }

    private ElectricMotorcyclePartialViewModel CreateElectricMotorcyclePartialForThirdPartyBike(PremiumCalculateRequestModel requestModel)
    {
        return new ElectricMotorcyclePartialViewModel
        {
            IsThirdParty = true,
            Type = requestModel.ThirdPartyBikeInsurance.VechileType.ToString(),
            ManufactureCompany = requestModel.ThirdPartyBikeInsurance.ManufactureCompany,
            Model = requestModel.ThirdPartyBikeInsurance.Model,
            PurchasedNewOld = false,
            DateOfPurchase = DateTime.UtcNow,
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            RegistrationNumber = "123456",
            KiloWatt = requestModel.ThirdPartyBikeInsurance.KilloWattRange ?? 0,
            Days = DefaultPolicyDays,
            YearsFromRegistrationDateYears = DateTime.UtcNow.Date.ToString(),
            YearsFromRegistrationDateYearsBS = "2080-02-22",
            CurrentMarketPrice = "100",
            PortfolioId = "MCY"
        };
    }

    private CreatePolicyViewModel CreateFullBikeRequest(PremiumCalculateRequestModel requestModel)
    {
        var isElectric = requestModel.FullBikeInsurance.VechileType == VechileType.Electric;

        return new CreatePolicyViewModel
        {
            MotorPartial = isElectric ? null : CreateMotorPartialForFullBike(requestModel),
            ElectricMotorcyclePartial = isElectric ? CreateElectricMotorcyclePartialForFullBike(requestModel) : null,
            PortfolioAlias = isElectric ? PortfolioClassConstants.ElectricMotorcycle : PortfolioClassConstants.Motorcycle,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch
        };
    }

    private MotorPartialViewModel CreateMotorPartialForFullBike(PremiumCalculateRequestModel requestModel)
    {
        return new MotorPartialViewModel
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
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            RegistrationNumber = "123456",
            CubicCapacity = requestModel.FullBikeInsurance.EngineCapacity,
            Days = DefaultPolicyDays.ToString(),
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
    }

    private ElectricMotorcyclePartialViewModel CreateElectricMotorcyclePartialForFullBike(PremiumCalculateRequestModel requestModel)
    {
        return new ElectricMotorcyclePartialViewModel
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
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            RegistrationNumber = "123456",
            KiloWatt = requestModel.FullBikeInsurance.KilloWattRange,
            Days = DefaultPolicyDays,
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
    }

    #endregion

    #region Private Car Insurance Methods

    private CreatePolicyViewModel CreateFullPrivateCarRequest(PremiumCalculateRequestModel requestModel)
    {
        return new CreatePolicyViewModel
        {
            PrivateVehiclePartial = CreatePrivateVehiclePartial(requestModel, isComprehensive: true),
            PortfolioAlias = PortfolioClassConstants.PrivateVehicle,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch
        };
    }

    private CreatePolicyViewModel CreateThirdPartyPrivateCarRequest(PremiumCalculateRequestModel requestModel)
    {
        return new CreatePolicyViewModel
        {
            PrivateVehiclePartial = CreatePrivateVehiclePartial(requestModel, isComprehensive: false),
            PortfolioAlias = PortfolioClassConstants.PrivateVehicle,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch
        };
    }

    private PrivateVehiclePartialViewModel CreatePrivateVehiclePartial(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        // Handle different insurance model types separately
        var privateCarInsurance = requestModel.PrivateCarInsurance;
        var thirdPartyCarInsurance = requestModel.ThirdPartyPrivateCarInsurance;
        
        // Get engine capacity - car insurance uses enum, bike insurance uses decimal
        decimal engineCapacity;
        if (privateCarInsurance != null)
        {
            engineCapacity = GetCubicCapacityFromEngineCapacity(privateCarInsurance.EngineCapacity);
        }
        else if (thirdPartyCarInsurance != null)
        {
            engineCapacity = GetCubicCapacityFromEngineCapacity(thirdPartyCarInsurance.EngineCapacity);
        }
        else
        {
            engineCapacity = requestModel.ThirdPartyBikeInsurance?.EngineCapacity ?? 0;
        }

        var manufactureCompany = thirdPartyCarInsurance?.ManufactureCompany 
            ?? requestModel.ThirdPartyBikeInsurance?.ManufactureCompany ?? "";
        
        var model = thirdPartyCarInsurance?.Model 
            ?? requestModel.ThirdPartyBikeInsurance?.Model ?? "";
        
        var purchasedNewOld = thirdPartyCarInsurance?.PurchasedNewOld ?? true;

        return new PrivateVehiclePartialViewModel
        {
            IsComprehensive = isComprehensive,
            IsThirdParty = true,
            EnterSumInsured = isComprehensive,
            TotalExcess = 0,
            IsDirectDiscountPA = isComprehensive,
            LayupDays = 0,
            ManufactureCompany = manufactureCompany,
            Model = model,
            PurchasedNewOld = purchasedNewOld,
            DateOfPurchase = DateTime.Parse("2022-06-24T08:59:52.264Z"),
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            CubicCapacity = (int)engineCapacity,
            CompulsoryExcess = 0,
            VoluntaryExcess = 0,
            RiotStrikeAndTerrorism = isComprehensive,
            YearsFromRegistrationDateYearsBS = "string",
            CurrentMarketPrice = isComprehensive ? "5000000" : "2000000",
            NumberofSeatsIncludingDriver = 4,
            HasTailor = isComprehensive,
            ValueOfTailor = 0,
            UseOfPrivateHire = isComprehensive,
            AgeOfVehicle = 0,
            RateOfDepreciation = 0,
            ValueOfAccessories = isComprehensive ? "12,8008." : "0",
            ValueWithoutAccessories = isComprehensive ? "5000000" : "2000000",
            SumInsuredAmount = 0,
            Days = isComprehensive ? "3" : DefaultPolicyDays.ToString(),
            NCDYears = 0,
            AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = isComprehensive ? "string" : "",
            IsAgentInvolved = isComprehensive,
            IsRiotStrikeAndTerrorismForDriver = isComprehensive,
            IsRiotStrikeAndTerrorismForPassenger = isComprehensive,
            RiotStrikeAndTerrorismForPassengerSeatCount = 0,
            IsDifferentlyAble = isComprehensive,
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
        };
    }

    #endregion

    #region Travel Insurance Methods

    private CreatePolicyViewModel CreateTravelRequest(PremiumCalculateRequestModel requestModel)
    {
        return new CreatePolicyViewModel
        {
            TravelInsurancePartial = new TravelInsurancePartialViewModel
            {
                PolicyPeriodInDays = DefaultPolicyDays,
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
                ExchangeRate = 145,
                SelectedCurrency = "NPR"
            },
            PortfolioAlias = PortfolioClassConstants.TravelInsurance,
            PortfolioId = "TI",
            PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
            BancassuanceBankName = "dummyBank",
            BancassuanceBankBranch = "dummyBranch"
        };
    }

    private async Task<(CreatePolicyViewModel Request, decimal PremiumAmount, string Region, string PlanType)> CreateInternationalTravelRequest(
        PremiumCalculateRequestModel requestModel)
    {
        var regionHelperResponse = RegionHelper.GetDestinationRegion(requestModel.InternationalTravelInsurance?.VisitingCountry);
        if (!regionHelperResponse.IsSuccess)
            throw new InvalidOperationException(regionHelperResponse.Error);

        var region = regionHelperResponse.Data;
        var planType = requestModel.InternationalTravelInsurance?.PlanType.ToString();

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

        var premiumAmount = await CalculateUsdRate(usdRateModel);

        var request = new CreatePolicyViewModel
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
                SelectedCurrency = "NPR"
            },
            PortfolioAlias = PortfolioClassConstants.InternationalTravelInsurance,
            PortfolioId = "ITI",
            PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
            BancassuanceBankName = "dummyBank",
            BancassuanceBankBranch = "dummyBranch"
        };

        return (request, premiumAmount, region, planType);
    }

    #endregion

    #region Marine Insurance Methods

    private CreatePolicyViewModel CreateMarineRequest(PremiumCalculateRequestModel requestModel)
    {
        var marinePartial = new MarinePolicyParitalViewModel
        {
            MaterialsOfInsurance = new List<MaterialOfInsurance>
            {
                new MaterialOfInsurance
                {
                    PrimaryId = Guid.NewGuid().ToString(),
                    Description = requestModel.MarineInsurance.MarineType.ToString(),
                    ProductTypeCode = requestModel.MarineInsurance.MarineType.ToString(),
                    Quantity = 1,
                    UnitValue = requestModel.MarineInsurance.InvoiceValueNpr,
                    FullUnitValue = requestModel.MarineInsurance.InvoiceValueNpr,
                    IsActive = true,
                    IsExisting = false,
                    IsUpdated = false
                }
            },
            ToleranceRate = requestModel.MarineInsurance.TolerancePercent,
            IncrementalCostRate = requestModel.MarineInsurance.IsIncremental10Percent ? 10m : 0m,
            DutyRate = requestModel.MarineInsurance.DutyPercent,
            CurrencyOfValue = requestModel.MarineInsurance.Currency.ToString(),
            ModeOfTransit = ModeOftransit.MarineCargo,
            InlandTransitType = InlandTransitType.OutsideNepal,
            InlandTransitDistance = 500m,
            IsNonDeliverySelected = requestModel.MarineInsurance.HasNonDelivery,
            IsWaterDamageSelected = requestModel.MarineInsurance.HasWaterDamage,
            IsTPNDSelected = requestModel.MarineInsurance.HasTpnd,
            IsSRCCSelected = requestModel.MarineInsurance.SrccPool != SrccPool.None,
            IsContainerUsed = requestModel.MarineInsurance.HasContainerDiscount,
            Days = DefaultPolicyDays.ToString(),
            VoyageFrom = "Origin Port",
            VoyageTo = "Destination Port",
            ShipConveyance = "Ship/Airline Name",
            EstimatedDateOfDeparture = DateTime.UtcNow.AddDays(7),
            LettersOfCredit = new List<LetterOfCredit>(),
            InvoiceDetails = new List<InvoiceDetail>(),
            Deductible = "As per policy",
            WarrantyField = string.Empty,
            Exclusion = string.Empty,
            SubjectMatterOfInsurance = requestModel.MarineInsurance.MarineType.ToString(),
            IsInstallmentPayment = false,
            BasicPremiumInstallment = 0,
            SRCCPremiumInstallment = 0
        };

        return new CreatePolicyViewModel
        {
            MarinePartial = marinePartial,
            PortfolioAlias = PortfolioClassConstants.Marine,
            PortfolioId = "MAR",
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            IsCoinsurance = false,
            HGIShareRate = 0,
            IsHGILead = false,
            IsTPPolicy = false,
            AgentId = 0,
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            IsDeclaration = false
        };
    }

    #endregion

    #region Commercial Vehicle Insurance Methods

    private CreatePolicyViewModel CreateCommercialVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return requestModel.CommercialVehicleInsurance.CommercialVehicleClassType switch
        {
            CommercialVehicleClassEnum.ElectricVehicle => CreateElectricVehicleRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.ElectricVehicleExtendedWarranty => CreateElectricVehicleExtendedWarrantyRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.PassengerCarryingVehicle => CreatePassengerCarryingVehicleRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.Taxi => CreateTaxiRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.Ambulance => CreateAmbulanceRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.GoodsCarryingVehicle => CreateGoodsCarryingVehicleRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.Tractor => CreateTractorRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.ConstructionEquipment => CreateConstructionEquipmentRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.Tanker => CreateTankerRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.Tempo => CreateTempoRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.ElectricCommercialVehicle => CreateElectricCommercialVehicleRequest(requestModel, isComprehensive),
            CommercialVehicleClassEnum.AgricultureForestryVehicle => CreateAgricultureForestryVehicleRequest(requestModel, isComprehensive),
            _ => throw new NotSupportedException($"Commercial vehicle class {requestModel.CommercialVehicleInsurance.CommercialVehicleClassType} is not supported")
        };
    }

    private CreatePolicyViewModel CreateElectricVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        var commercialVehicle = requestModel.CommercialVehicleInsurance;
        var excessAmount = commercialVehicle.ExcessOnOwnDamageOptions?.FirstOrDefault() ?? 0;

        return new CreatePolicyViewModel
        {
            ElectricVehiclePartial = new ElectricVehicleParialViewModel
            {
                IsComprehensive = isComprehensive,
                IsThirdParty = true,
                IsRiotStrike = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                EnterSumInsured = isComprehensive,
                ManufactureCompany = "",
                Model = "",
                PurchasedNewOld = SafeIsPurchasedNew(commercialVehicle.YearOfRegistration),
                DateOfPurchase = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
                ChasisNumber = DefaultChasisNumber,
                EngineNumber = DefaultEngineNumber,
                KiloWatt = GetKiloWattFromEngineCapacity(commercialVehicle.EngineCapacity),
                VoluntaryExcess = isComprehensive ? excessAmount : 0,
                YearsFromRegistrationDateYearsBS = SafeCalculateYearDifference(commercialVehicle.YearOfRegistration),
                YearsFromRegistrationDateYears = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
                CurrentMarketPrice = commercialVehicle.MarketValue.ToString("N0"),
                NumberofSeatsIncludingDriver = commercialVehicle.SeatingCapacityIncludingDriver,
                HasTailor = commercialVehicle.TrailorValue > 0,
                ValueOfTailor = commercialVehicle.TrailorValue > 0 ? (int?)commercialVehicle.TrailorValue : null,
                UseOfPrivateHire = commercialVehicle.IsPrivateUse,
                AgeOfVehicle = SafeCalculateAgeOfVehicle(commercialVehicle.YearOfRegistration),
                ValueOfAccessories = 0,
                ValueWithoutAccessories = isComprehensive ? commercialVehicle.MarketValue : 0,
                SumInsuredAmount = isComprehensive ? commercialVehicle.SumInsuredAmount : 0,
                Days = DefaultPolicyDays.ToString(),
                NCDYears = (int)commercialVehicle.NoClaimDiscount,
                AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = "string",
                IsAgentInvolved = isComprehensive,
                IsRiotStrikeAndTerrorismForDriver = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                IsRiotStrikeAndTerrorismForPassenger = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                RiotStrikeAndTerrorismForPassengerSeatCount = isComprehensive && commercialVehicle.CoversRiotOrTerrorism
                    ? commercialVehicle.SeatingCapacityIncludingDriver - 1 : 0,
                IsDifferentlyAble = false,
                Transportation = true,
                Replacement = true,
                Depreciation = true,
                TransportationRate = 0,
                TransportationAmount = 0,
                ReplacementRate = 0,
                ReplacementAmount = 0,
                DepreciationRate = 0,
                DepreciationAmount = 0,
                HasSmartPolicy = false,
                HasExtendedWarrantyPolicy = false,
                LimitOfLiabilityPerVehicle = 0,
                PremiumRate = 0,
                AuthorizedServiceCenter = "",
                WarrantyPeriodList = new List<WarrantyPeriodEV>(),
                IsDirectDiscountPA = isComprehensive,
                TotalExcess = isComprehensive ? excessAmount : 0,
                CompulsoryExcess = isComprehensive ? excessAmount : 0,
                IsLayup = false,
                LayupDays = 0,
                RegistrationNumber = "",
                RegistrationNumberNepali = "",
                ProposerName = "",
                GoodsCarryingCapacity = 0,
                RiskType = "COMMERCIAL",
                IsIssued = false,
                PreviousPolicyIssuedYear = 0,
                IsPrivateTaxi = commercialVehicle.IsPrivateUse,
                IsProRataOrShortScale = false,
                IsRecoveryCharge = false,
                SpecialDiscountRate = null,
                MasterPolicyNumber = "",
                AccessoriesDetail = ""
            },
            PortfolioAlias = PortfolioClassConstants.ElectricVehicle,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            FullSumInsured = isComprehensive ? commercialVehicle.SumInsuredAmount : 0,
            IsCoinsurance = false,
            HGIShareRate = 100,
            ApplyGovtConfig = false,
            RiskTypeSelected = GetRiskTypeSelected(isComprehensive, commercialVehicle.CoversRiotOrTerrorism),
            BranchCode = "",
            Agent = null,
            AgentName = null,
            AgentId = null,
            ShortScale = null,
            VehicleNumber = ""
        };
    }

    private CreatePolicyViewModel CreateElectricVehicleExtendedWarrantyRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        var commercialVehicle = requestModel.CommercialVehicleInsurance;
        var excessAmount = commercialVehicle.ExcessOnOwnDamageOptions?.FirstOrDefault() ?? 0;

        return new CreatePolicyViewModel
        {
            ElectricVehiclePartial = new ElectricVehicleParialViewModel
            {
                HasExtendedWarrantyPolicy = true,
                LimitOfLiabilityPerVehicle = commercialVehicle.SumInsuredAmount,
                PremiumRate = commercialVehicle.SumInsuredAmount > 0 ? 2.5m : 0,
                AuthorizedServiceCenter = "Authorized Service Center",
                IsComprehensive = isComprehensive,
                IsThirdParty = !isComprehensive,
                IsRiotStrike = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                EnterSumInsured = true,
                ManufactureCompany = "",
                Model = "",
                            PurchasedNewOld = SafeIsPurchasedNew(commercialVehicle.YearOfRegistration),
                            DateOfPurchase = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
                            ChasisNumber = DefaultChasisNumber,
                            EngineNumber = DefaultEngineNumber,
                            KiloWatt = GetKiloWattFromEngineCapacity(commercialVehicle.EngineCapacity),
                            YearsFromRegistrationDateYearsBS = SafeCalculateYearDifference(commercialVehicle.YearOfRegistration),
                            YearsFromRegistrationDateYears = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
                CurrentMarketPrice = commercialVehicle.MarketValue.ToString("N0"),
                NumberofSeatsIncludingDriver = commercialVehicle.SeatingCapacityIncludingDriver,
                HasTailor = commercialVehicle.TrailorValue > 0,
                ValueOfTailor = commercialVehicle.TrailorValue > 0 ? (int?)commercialVehicle.TrailorValue : null,
                UseOfPrivateHire = commercialVehicle.IsPrivateUse,
                AgeOfVehicle = SafeCalculateAgeOfVehicle(commercialVehicle.YearOfRegistration),
                ValueOfAccessories = 0,
                ValueWithoutAccessories = commercialVehicle.MarketValue,
                SumInsuredAmount = commercialVehicle.SumInsuredAmount,
                Days = DefaultPolicyDays.ToString(),
                NCDYears = (int)commercialVehicle.NoClaimDiscount,
                VoluntaryExcess = isComprehensive ? excessAmount : 0,
                AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = "N/A",
                IsAgentInvolved = isComprehensive,
                IsRiotStrikeAndTerrorismForDriver = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                IsRiotStrikeAndTerrorismForPassenger = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
                RiotStrikeAndTerrorismForPassengerSeatCount = isComprehensive && commercialVehicle.CoversRiotOrTerrorism
                    ? commercialVehicle.SeatingCapacityIncludingDriver - 1 : 0,
                IsDifferentlyAble = false,
                Transportation = isComprehensive,
                Replacement = isComprehensive,
                Depreciation = isComprehensive,
                TransportationRate = 0,
                TransportationAmount = 0,
                ReplacementRate = 0,
                ReplacementAmount = 0,
                DepreciationRate = 0,
                DepreciationAmount = 0,
                HasSmartPolicy = false,
                IsDirectDiscountPA = isComprehensive,
                TotalExcess = isComprehensive ? excessAmount : 0,
                CompulsoryExcess = isComprehensive ? excessAmount : 0,
                IsLayup = false,
                LayupDays = 0,
                RegistrationNumber = "",
                RegistrationNumberNepali = "",
                ProposerName = "",
                GoodsCarryingCapacity = 0,
                RiskType = "EXTENDED_WARRANTY",
                IsIssued = false,
                PreviousPolicyIssuedYear = 0,
                IsPrivateTaxi = commercialVehicle.IsPrivateUse,
                IsProRataOrShortScale = false,
                IsRecoveryCharge = false,
                SpecialDiscountRate = null,
                MasterPolicyNumber = "",
                AccessoriesDetail = ""
            },
            PortfolioAlias = PortfolioClassConstants.ElectricVehicleExtendedWarranty,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            FullSumInsured = commercialVehicle.SumInsuredAmount,
            IsCoinsurance = false,
            HGIShareRate = 100,
            ApplyGovtConfig = false,
            RiskTypeSelected = GetRiskTypeSelected(isComprehensive, commercialVehicle.CoversRiotOrTerrorism),
            BranchCode = "",
            Agent = null,
            AgentName = null,
            AgentId = null,
            ShortScale = null,
            VehicleNumber = ""
        };
    }

    private CreatePolicyViewModel CreatePassengerCarryingVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return new CreatePolicyViewModel
        {
            PassengerCarryingVehiclePartial = new PassengerCarryingVehiclePartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "PASSENGER_CARRYING"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel)
            },
            PortfolioAlias = PortfolioClassConstants.PassengerCarryingVehicle,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            FullSumInsured = isComprehensive ? requestModel.CommercialVehicleInsurance.SumInsuredAmount : 0,
            IsCoinsurance = false,
            HGIShareRate = 100,
            ApplyGovtConfig = false,
            RiskTypeSelected = GetRiskTypeSelected(isComprehensive, requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism),
            BranchCode = "",
            Agent = null,
            AgentName = null,
            AgentId = null,
            ShortScale = null,
            VehicleNumber = ""
        };
    }

    private CreatePolicyViewModel CreateTaxiRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new TaxiPartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "TAXI"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel)
            },
            PortfolioClassConstants.Taxi,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateAmbulanceRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new AmbulancePartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "AMBULANCE"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                PersonalAccidentForConductor = isComprehensive,
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                SumInsuredAmountForHelper = "0",
                OwnUseForPrivateTourismSchool = requestModel.CommercialVehicleInsurance.IsOwnUse
            },
            PortfolioClassConstants.Ambulance,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateGoodsCarryingVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new GoodsCarryingVehiclePartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "GOODS_CARRYING"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                PersonalAccidentForConductor = isComprehensive,
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                SumInsuredAmountForHelper = "0",
                OwnUseForPrivateTourismSchool = requestModel.CommercialVehicleInsurance.IsOwnUse,
                GoodsCarryingCapacity = 0,
                IsAgricultureForestryVehicle = false
            },
            PortfolioClassConstants.GoodsCarryingVehicle,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateTractorRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new TractorPartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "TRACTOR"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                PersonalAccidentForConductor = isComprehensive,
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                HorsePower = 0,
                IsCubicCapacity = true,
                IsHorsePower = false,
                IsOwnUse = requestModel.CommercialVehicleInsurance.IsOwnUse
            },
            PortfolioClassConstants.Tractor,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateConstructionEquipmentRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new ConstructionEquipmentPartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "CONSTRUCTION_EQUIPMENT"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                SumInsuredAmountForHelper = "0",
                OwnUseForPrivateTourismSchool = requestModel.CommercialVehicleInsurance.IsOwnUse,
                GoodsCarryingCapacity = 0,
                IsHeavyEquipment = false
            },
            PortfolioClassConstants.ConstructionEquipment,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateTankerRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new TankerPartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "TANKER"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                PersonalAccidentForConductor = isComprehensive,
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                SumInsuredAmountForHelper = "0",
                OwnUseForPrivateTourismSchool = requestModel.CommercialVehicleInsurance.IsOwnUse,
                GoodsCarryingCapacity = 0
            },
            PortfolioClassConstants.Tanker,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateTempoRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new TempoPartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "TEMPO"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel)
            },
            PortfolioClassConstants.Tempo,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateElectricCommercialVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new ElectricCommercialVehiclePartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "ELECTRIC_COMMERCIAL"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                MasterPolicyNumber = "",
                Transportation = isComprehensive,
                Replacement = isComprehensive,
                Depreciation = isComprehensive,
                TransportationRate = 0,
                TransportationAmount = 0,
                ReplacementRate = 0,
                ReplacementAmount = 0,
                DepreciationRate = 0,
                DepreciationAmount = 0,
                HasSmartPolicy = false
            },
            PortfolioClassConstants.ElectricCommercialVehicle,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateAgricultureForestryVehicleRequest(PremiumCalculateRequestModel requestModel, bool isComprehensive)
    {
        return CreateCommercialVehicleBaseRequest(
            new AgricultureForestryVehiclePartialViewModel
            {
                CommonProperties = CreateCommonMotorProperties(requestModel, isComprehensive, "AGRICULTURE_FORESTRY"),
                CommonRSMDTModel = CreateCommonRSMDTProperties(requestModel),
                PersonalAccidentForConductor = isComprehensive,
                IsPersonalAccidentForHelper = false,
                IsRiotStrikeAndTerrorismForHelper = isComprehensive && requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism,
                SumInsuredAmountForHelper = "0",
                OwnUseForPrivateTourismSchool = requestModel.CommercialVehicleInsurance.IsOwnUse,
                GoodsCarryingCapacity = 0
            },
            PortfolioClassConstants.AgricultureForestryVehicle,
            requestModel,
            isComprehensive);
    }

    private CreatePolicyViewModel CreateCommercialVehicleBaseRequest<T>(
        T partialViewModel,
        string portfolioAlias,
        PremiumCalculateRequestModel requestModel,
        bool isComprehensive) where T : class
    {
        var request = new CreatePolicyViewModel
        {
            PortfolioAlias = portfolioAlias,
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            FullSumInsured = isComprehensive ? requestModel.CommercialVehicleInsurance.SumInsuredAmount : 0,
            IsCoinsurance = false,
            HGIShareRate = 100,
            ApplyGovtConfig = false,
            RiskTypeSelected = GetRiskTypeSelected(isComprehensive, requestModel.CommercialVehicleInsurance.CoversRiotOrTerrorism),
            BranchCode = "",
            Agent = null,
            AgentName = null,
            AgentId = null,
            ShortScale = null,
            VehicleNumber = ""
        };

        // Set the appropriate partial property using reflection or pattern matching
        SetPartialProperty(request, partialViewModel, portfolioAlias);

        return request;
    }

    private void SetPartialProperty(CreatePolicyViewModel request, object partialViewModel, string portfolioAlias)
    {
        var propertyName = portfolioAlias switch
        {
            "TAXI" => nameof(CreatePolicyViewModel.TaxiPartial),
            "AMB" => nameof(CreatePolicyViewModel.AmbulancePartial),
            "GCV" => nameof(CreatePolicyViewModel.GoodsCarryingVehiclePartial),
            "TRCT" => nameof(CreatePolicyViewModel.TractorPartial),
            "CE" => nameof(CreatePolicyViewModel.ConstructionEquipmentPartial),
            "TANKER" => nameof(CreatePolicyViewModel.TankerPartial),
            "TEMPO" => nameof(CreatePolicyViewModel.TempoPartial),
            "ECV" => nameof(CreatePolicyViewModel.ElectricCommercialVehiclePartial),
            "AFV" => nameof(CreatePolicyViewModel.AgricultureForestryVehiclePartial),
            _ => null
        };

        if (propertyName != null)
        {
            var property = typeof(CreatePolicyViewModel).GetProperty(propertyName);
            property?.SetValue(request, partialViewModel);
        }
    }

    #endregion

    #region Fire Insurance Methods (Household, Property, and LossOfProfit)

    private CreatePolicyViewModel CreateFireInsuranceRequest(PremiumCalculateRequestModel requestModel)
    {
        var fireInsurance = GetLegacyFireInsuranceModel(requestModel);
        
        if (fireInsurance == null)
        {
            throw new ArgumentException("Fire insurance data is required");
        }

        return fireInsurance.FireInsuranceType switch
        {
            FireInsuranceType.Household => CreateHouseholdInsuranceRequest(fireInsurance),
            FireInsuranceType.Property => CreatePropertyInsuranceRequest(fireInsurance),
            FireInsuranceType.LossOfProfit => CreateLossOfProfitInsuranceRequest(fireInsurance),
            _ => throw new NotSupportedException($"Fire insurance type {fireInsurance.FireInsuranceType} is not supported")
        };
    }

    private FireInsuranceRequestModel GetLegacyFireInsuranceModel(PremiumCalculateRequestModel requestModel)
    {
        // Backward compatibility: convert legacy models to unified model
        if (requestModel.InsuranceType == InsuranceType.Home && requestModel.HomeInsurance != null)
        {
            var home = requestModel.HomeInsurance;
            return new FireInsuranceRequestModel
            {
                FireInsuranceType = FireInsuranceType.Household,
                SumInsuredAmount = home.SumInsuredAmount,
                RiskCode = home.RiskCode,
                IsRSMDT = home.IsRSMDT,
                BuildingComposition = home.BuildingComposition,
                LoadingMultiplier = home.LoadingMultiplier,
                Equipment = home.Equipment,
                RawMaterials = home.RawMaterials,
                WorkInProgress = home.WorkInProgress,
                FinishedGoods = home.FinishedGoods,
                SemiFinishedGoods = home.SemiFinishedGoods,
                MoneyAndJewellery = home.MoneyAndJewellery,
                FurnitureFixtureOrFitting = home.FurnitureFixtureOrFitting,
                OtherItems = home.OtherItems,
                Art = home.Art
            };
        }
        
        if (requestModel.InsuranceType == InsuranceType.Property && requestModel.PropertyInsurance != null)
        {
            var property = requestModel.PropertyInsurance;
            return new FireInsuranceRequestModel
            {
                FireInsuranceType = FireInsuranceType.Property,
                SumInsuredAmount = property.SumInsuredAmount,
                RiskType = property.RiskType,
                IsDirectDiscountApplicable = property.IsDirectDiscountApplicable,
                RiskCode = property.RiskCode,
                IsRSMDT = property.IsRSMDT,
                BuildingComposition = property.BuildingComposition,
                LoadingMultiplier = property.LoadingMultiplier,
                ProvideSubsidy = property.ProvideSubsidy,
                SubsidyClassID = property.SubsidyClassID,
                SubsidyRate = property.SubsidyRate,
                ProvideLockdownDiscount = property.ProvideLockdownDiscount,
                Equipment = property.Equipment,
                RawMaterials = property.RawMaterials,
                WorkInProgress = property.WorkInProgress,
                FinishedGoods = property.FinishedGoods,
                SemiFinishedGoods = property.SemiFinishedGoods,
                MoneyAndJewellery = property.MoneyAndJewellery,
                FurnitureFixtureOrFitting = property.FurnitureFixtureOrFitting,
                OtherItems = property.OtherItems,
                Art = property.Art
            };
        }

        return null;
    }

    private CreatePolicyViewModel CreateHouseholdInsuranceRequest(FireInsuranceRequestModel fireInsurance)
    {
        if (!fireInsurance.SumInsuredAmount.HasValue)
        {
            throw new ArgumentException("SumInsuredAmount is required for Household insurance");
        }

        // Create a single asset with the sum insured amount
        var subjectMatter = new SubjectMatterOfInsurance
        {
            Id = Guid.NewGuid().ToString(),
            ValueOfBuilding = fireInsurance.SumInsuredAmount.Value,
            Equipment = fireInsurance.Equipment,
            RawMaterials = fireInsurance.RawMaterials,
            WorkInProgress = fireInsurance.WorkInProgress,
            FinishedGoods = fireInsurance.FinishedGoods,
            SemiFinishedGoods = fireInsurance.SemiFinishedGoods,
            MoneyAndJewellery = fireInsurance.MoneyAndJewellery,
            FurnitureFixtureOrFitting = fireInsurance.FurnitureFixtureOrFitting,
            OtherItems = fireInsurance.OtherItems,
            Art = fireInsurance.Art,
            RiskCode = !string.IsNullOrEmpty(fireInsurance.RiskCode) 
                ? fireInsurance.RiskCode 
                : "HOME_DEFAULT", // Fallback if not provided
            BuildingComposition = ((int)fireInsurance.BuildingComposition).ToString(),
            LoadingMultiplier = fireInsurance.LoadingMultiplier,
            LoadContentsFromFile = false
        };

        return new CreatePolicyViewModel
        {
            FirePartial = new FirePolicyPartialViewModel
            {
                SubjectMatterOfInsurance = new List<SubjectMatterOfInsurance> { subjectMatter },
                IsRSMDT = fireInsurance.IsRSMDT,
                ProvideSubsidy = false,
                ProvideLockdownDiscount = false
            },
            PortfolioAlias = SharedKernel.Constant.Permission.PortfolioClassConstants.Household,
            PortfolioId = "HOME",
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            PolicyPeriodInDays = DefaultPolicyDays,
            EffectiveDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(DefaultPolicyDays),
            ProposedDate = DateTime.UtcNow
        };
    }

    private CreatePolicyViewModel CreatePropertyInsuranceRequest(FireInsuranceRequestModel fireInsurance)
    {
        if (!fireInsurance.SumInsuredAmount.HasValue)
        {
            throw new ArgumentException("SumInsuredAmount is required for Property insurance");
        }

        // Determine RiskCode: use provided value, or fallback based on RiskType
        var riskCode = !string.IsNullOrEmpty(fireInsurance.RiskCode)
            ? fireInsurance.RiskCode
            : (fireInsurance.RiskType.HasValue && fireInsurance.RiskType.Value == RiskType.SimpleRisk 
                ? "PROPERTY_SIMPLE" 
                : "PROPERTY_COMPLEX");

        var subjectMatter = new SubjectMatterOfInsurance
        {
            Id = Guid.NewGuid().ToString(),
            ValueOfBuilding = fireInsurance.SumInsuredAmount.Value,
            Equipment = fireInsurance.Equipment,
            RawMaterials = fireInsurance.RawMaterials,
            WorkInProgress = fireInsurance.WorkInProgress,
            FinishedGoods = fireInsurance.FinishedGoods,
            SemiFinishedGoods = fireInsurance.SemiFinishedGoods,
            MoneyAndJewellery = fireInsurance.MoneyAndJewellery,
            FurnitureFixtureOrFitting = fireInsurance.FurnitureFixtureOrFitting,
            OtherItems = fireInsurance.OtherItems,
            Art = fireInsurance.Art,
            RiskCode = riskCode,
            BuildingComposition = ((int)fireInsurance.BuildingComposition).ToString(),
            LoadingMultiplier = fireInsurance.LoadingMultiplier,
            LoadContentsFromFile = false
        };

        return new CreatePolicyViewModel
        {
            FirePartial = new FirePolicyPartialViewModel
            {
                SubjectMatterOfInsurance = new List<SubjectMatterOfInsurance> { subjectMatter },
                IsRSMDT = fireInsurance.IsRSMDT,
                ProvideSubsidy = fireInsurance.ProvideSubsidy,
                ProvideLockdownDiscount = fireInsurance.ProvideLockdownDiscount,
                SubsidyClassID = fireInsurance.ProvideSubsidy ? fireInsurance.SubsidyClassID : null,
                SubsidyRate = fireInsurance.ProvideSubsidy ? fireInsurance.SubsidyRate : 0
            },
            PortfolioAlias = SharedKernel.Constant.Permission.PortfolioClassConstants.Property,
            PortfolioId = "PPTY",
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            PolicyPeriodInDays = DefaultPolicyDays,
            EffectiveDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(DefaultPolicyDays),
            ProposedDate = DateTime.UtcNow,
            AgentId = fireInsurance.IsDirectDiscountApplicable ? null : 0 // If direct discount applicable, no agent
        };
    }

    private CreatePolicyViewModel CreateLossOfProfitInsuranceRequest(FireInsuranceRequestModel fireInsurance)
    {
        if (!fireInsurance.AnnualRevenue.HasValue)
        {
            throw new ArgumentException("AnnualRevenue is required for LossOfProfit insurance");
        }
        if (!fireInsurance.InsuredAmount.HasValue)
        {
            throw new ArgumentException("InsuredAmount is required for LossOfProfit insurance");
        }
        if (!fireInsurance.AnnualPremium.HasValue)
        {
            throw new ArgumentException("AnnualPremium is required for LossOfProfit insurance");
        }
        if (!fireInsurance.PolicyPeriodInDays.HasValue)
        {
            throw new ArgumentException("PolicyPeriodInDays is required for LossOfProfit insurance");
        }

        var policyPeriodInDays = fireInsurance.PolicyPeriodInDays.Value;
        if (policyPeriodInDays < 1 || policyPeriodInDays > 365)
        {
            throw new ArgumentException("PolicyPeriodInDays must be between 1 and 365");
        }

        return new CreatePolicyViewModel
        {
            LOPPartial = new LOPPolicyPartialViewModel
            {
                AnnualRevenue = fireInsurance.AnnualRevenue.Value,
                InsuredAmount = fireInsurance.InsuredAmount.Value,
                AnnualPremium = fireInsurance.AnnualPremium.Value,
                PolicyPeriodInDays = policyPeriodInDays,
                IsRSMDTSelected = fireInsurance.IsRSMDTSelected,
                RSMDTPremium = fireInsurance.RSMDTPremium,
                RiskAddress = fireInsurance.RiskAddress,
                SelectedPolicyNumber = fireInsurance.SelectedPolicyNumber,
                Deductibles = fireInsurance.Deductibles,
                MaximumIndemnityPeriod = fireInsurance.MaximumIndemnityPeriod
            },
            PortfolioAlias = SharedKernel.Constant.Permission.PortfolioClassConstants.LossOfProfit,
            PortfolioId = "LOP",
            PartyId = DefaultPartyId,
            BancassuanceBankName = DefaultBankName,
            BancassuanceBankBranch = DefaultBankBranch,
            PolicyPeriodInDays = policyPeriodInDays,
            EffectiveDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(policyPeriodInDays),
            ProposedDate = DateTime.UtcNow
        };
    }

    #endregion

    #region Helper Methods

    private decimal GetKiloWattFromEngineCapacity(VechileEngineCapacityCategory engineCapacity)
    {
        return engineCapacity switch
        {
            VechileEngineCapacityCategory.Under1000 => 1000,
            VechileEngineCapacityCategory.From1000To1500 => 1500,
            VechileEngineCapacityCategory.Above1500 => 1501,
            _ => 1000
        };
    }

    private decimal GetCubicCapacityFromEngineCapacity(VechileEngineCapacityCategory engineCapacity)
    {
        return engineCapacity switch
        {
            VechileEngineCapacityCategory.Under1000 => 999,
            VechileEngineCapacityCategory.From1000To1500 => 1250,
            VechileEngineCapacityCategory.Above1500 => 1501,
            _ => throw new ArgumentOutOfRangeException(nameof(engineCapacity))
        };
    }

    private DateTime SafeCreateDateTimeFromYear(int year)
    {
        // DateTime constructor requires year between 1 and 9999
        // If invalid, use current year as fallback
        if (year < 1 || year > 9999)
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }
        return new DateTime(year, 1, 1);
    }

    private string SafeCalculateYearDifference(int yearOfRegistration)
    {
        // Ensure we don't get negative values or invalid calculations
        var currentYear = DateTime.Now.Year;
        if (yearOfRegistration < 1 || yearOfRegistration > 9999)
        {
            return "0";
        }
        var difference = currentYear - yearOfRegistration;
        return difference < 0 ? "0" : difference.ToString();
    }

    private bool SafeIsPurchasedNew(int yearOfRegistration)
    {
        // Check if vehicle is new (registered within last year)
        if (yearOfRegistration < 1 || yearOfRegistration > 9999)
        {
            return false; // Default to old if year is invalid
        }
        return DateTime.Now.Year - yearOfRegistration <= 1;
    }

    private decimal? SafeCalculateAgeOfVehicle(int yearOfRegistration)
    {
        // Calculate vehicle age safely
        if (yearOfRegistration < 1 || yearOfRegistration > 9999)
        {
            return 0; // Default to 0 if year is invalid
        }
        var age = DateTime.Now.Year - yearOfRegistration;
        return age < 0 ? 0 : age;
    }

    private CommonMotorModel CreateCommonMotorProperties(PremiumCalculateRequestModel requestModel, bool isComprehensive, string riskType)
    {
        var commercialVehicle = requestModel.CommercialVehicleInsurance;
        var excessAmount = commercialVehicle.ExcessOnOwnDamageOptions?.FirstOrDefault() ?? 0;

        return new CommonMotorModel
        {
            IsComprehensive = isComprehensive,
            IsThirdParty = true,
            EnterSumInsured = isComprehensive,
            PurchasedNewOld = DateTime.Now.Year - commercialVehicle.YearOfRegistration <= 1,
            DateOfPurchase = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
            YearsFromRegistrationDateYears = SafeCreateDateTimeFromYear(commercialVehicle.YearOfRegistration),
            YearsFromRegistrationDateYearsBS = SafeCalculateYearDifference(commercialVehicle.YearOfRegistration),
            CurrentMarketPrice = commercialVehicle.MarketValue.ToString("N0"),
            ValueWithoutAccessories = commercialVehicle.MarketValue,
            SumInsuredAmount = isComprehensive ? commercialVehicle.SumInsuredAmount : 0,
            ValueOfAccessories = 0,
            AgeOfVehicle = DateTime.Now.Year - commercialVehicle.YearOfRegistration,
            Days = DefaultPolicyDays.ToString(),
            NCDYears = (int)commercialVehicle.NoClaimDiscount,
            VoluntaryExcess = isComprehensive ? excessAmount : 0,
            TotalExcess = isComprehensive ? excessAmount : 0,
            CompulsoryExcess = isComprehensive ? excessAmount : 0,
            IsRiotStrike = isComprehensive && commercialVehicle.CoversRiotOrTerrorism,
            IsDirectDiscountPA = isComprehensive,
            IsAgentInvolved = isComprehensive,
            RegistrationNumber = "",
            AgeForPrint = "",
            AgeForPrintEnglish = "",
            RiskType = riskType,
            IsIssued = false,
            PreviousPolicyIssuedYear = 0,
            MasterPolicyNumber = "",
            AccessoriesDetail = "",
            ChasisNumber = DefaultChasisNumber,
            EngineNumber = DefaultEngineNumber,
            CubicCapacity = GetCubicCapacityFromEngineCapacity(commercialVehicle.EngineCapacity)
        };
    }

    private CommonRSMDTModel CreateCommonRSMDTProperties(PremiumCalculateRequestModel requestModel)
    {
        var commercialVehicle = requestModel.CommercialVehicleInsurance;
        return new CommonRSMDTModel
        {
            NumberofSeatsIncludingDriver = commercialVehicle.SeatingCapacityIncludingDriver,
            HasTailor = commercialVehicle.TrailorValue > 0,
            ValueOfTailor = commercialVehicle.TrailorValue > 0 ? (decimal?)commercialVehicle.TrailorValue : null,
            UseOfPrivateHire = commercialVehicle.IsPrivateUse,
            VehiclePurpose = commercialVehicle.IsOwnUse ? "OWN_USE" : "COMMERCIAL_USE"
        };
    }

    private string GetRiskTypeSelected(bool isComprehensive, bool coversRiotOrTerrorism)
    {
        return isComprehensive
            ? (coversRiotOrTerrorism ? RiskTypeSelected.AllRisks : RiskTypeSelected.BasicAndThirdParty)
            : RiskTypeSelected.ThirdParty;
    }

    #endregion
}
