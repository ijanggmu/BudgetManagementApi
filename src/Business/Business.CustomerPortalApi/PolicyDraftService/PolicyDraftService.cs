using Business.Common.DraftNumber;
using Business.Common.File;
using Business.Common.JobHelper.CustomerPolicyJob;
using Business.Common.PolicyCalculator;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.ITIEntity;
using Data.Entities.MotorEntity;
using Data.Entities.PrivateVehicleEntity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer;
using Models.WebApi.Customer.Policy;
using Models.WebApi.ITI;
using Models.WebApi.Policy.Motor;
using SharedKernel.Helper;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;
using SharedKernel.SystemEnum.Payment;
using Data.Entities.PolicyE2e;

namespace Business.BeemaEdgeApi.PolicyDraftService;

public class PolicyDraftService(ApplicationDataContext context, IUserProfileService profileService,
    ISieveExtension sieveExtension,
    IPolicyCalculatorService policyCalculator,
    IFileService fileService,
    IDraftNumberService draftNumberService, IPolicyCalculatorService policyCalculatorService, ICustomerPolicyCreateJobService customerPolicyCreateJobService
    ) : IPolicyDraftService
{
    public async Task<Result<MessageResponseModel>> SaveDraftAsync(SaveDraftRequestModel model, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var userId = profileService.GetUserId();
        try
        {
            var customer = await context.Customers
              .Include(c => c.User)
              .Include(c => c.Addresses)
              .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted, cancellationToken);

            if (customer.KycStatus == KYCStatus.Approved)
                return Result<MessageResponseModel>.Failed("Kyc not approved.");

            var temporaryAddress = customer.Addresses.FirstOrDefault(a => a.AddressType.ToLower() == AddressTypeEnums.Temporary.ToString().ToLower());

            //if (model.InsuranceType == InsuranceType.FullBike)
            //{
            //    var motor = CreateMotor(model.Motor);
            //    await context.Motors.AddAsync(motor, cancellationToken);

            //    var policyDraft = CreatePolicyDraft(model, customer.Id, motor.Id);
            //    await context.PolicyDrafts.AddAsync(policyDraft, cancellationToken);
            //}

            //if (model.InsuranceType == InsuranceType.Travel)
            //{
            //    var iti = CreateITI(customer, temporaryAddress, model.InternationTravelInsurance);
            //    if (!iti.IsSuccess)
            //    {
            //        return Result<MessageResponseModel>.Failed(iti.Error);
            //    }

            //    await context.ITI.AddAsync(iti.Data, cancellationToken);

            //    var policyDraft = CreateITIPolicyDraft(model, customer.Id, iti.Data.Id);
            //    await context.PolicyDrafts.AddAsync(policyDraft, cancellationToken);
            //}

            if (model.InsuranceType == InsuranceType.FullPrivateCar)
            {
                var pv = CreatePV(model.PrivateVechile);
                await context.PrivateVehicles.AddAsync(pv, cancellationToken);

                var policyDraft = CreatePVPolicyDraft(model, customer.Id, pv.Id);
                await context.PolicyDrafts.AddAsync(policyDraft, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Draft Saved."));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    public static int CalculateAge(string dobString)
    {
        if (DateTime.TryParse(dobString, out var dob))
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age;
        }

        return 0; // or throw exception / return -1 based on your use case
    }
    public async Task<Result<SubmitDraftResponseModel>> SubmitDraftAsync(SubmitDraftRequestModel model, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var customer = await context.Customers
                .Include(c => c.User)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted, cancellationToken);

            if (customer == null)
                return Result<SubmitDraftResponseModel>.Failed("Customer not found.");

            if (customer.KycStatus != KYCStatus.Approved)
                return Result<SubmitDraftResponseModel>.Failed("Kyc not approved.");

            var temporaryAddress = customer.Addresses
                .FirstOrDefault(a => a.AddressType.Equals(AddressTypeEnums.Temporary.ToString(), StringComparison.OrdinalIgnoreCase));

            string draftNo = string.Empty;

            if (string.IsNullOrEmpty(model.PolicyId))
            {
                draftNo = await draftNumberService.GetNextDraftNumberAsync();
            }

            var premiumCalculationResponseModel = new PremiumCalculationResponseModel();

            string policyId = model.PolicyId;

            if (model.InsuranceType == InsuranceType.ThirdPartyBike)
            {
                var result = await HandleThirdPartyBikeDraftAsync(model, customer.Id, draftNo, policyId, cancellationToken);
                if (!result.IsSuccess) return result;
                premiumCalculationResponseModel = result.Data.CalculationDetail;
                policyId = result.Data.PolicyId;
            }

            else if (model.InsuranceType == InsuranceType.InternationalTravel)
            {
                var result = await HandleITIDraftAsync(model, customer, temporaryAddress, draftNo, policyId, cancellationToken);
                if (!result.IsSuccess) return result;
                premiumCalculationResponseModel = result.Data.CalculationDetail;
                policyId = result.Data.PolicyId;
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);


            return Result<SubmitDraftResponseModel>.Success(new SubmitDraftResponseModel
            {
                PolicyId = policyId,
                Message = "Policy submitted successfully.",
                CalculationDetail = premiumCalculationResponseModel
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    private async Task<Result<SubmitDraftResponseModel>> HandleITIDraftAsync(
    SubmitDraftRequestModel model, Customer customer, CustomerAddress temporaryAddress, string draftNo, string existingPolicyId, CancellationToken cancellationToken)
    {
        var age = CalculateAge(customer.DobAD);

        var regionResponse = RegionHelper.GetDestinationRegion(model.ITI.VisitingCountry);
        if (!regionResponse.IsSuccess)
            return Result<SubmitDraftResponseModel>.Failed(regionResponse.Error);

        var region = regionResponse.Data;

        var policyDraft = await GetOrCreateDraftAsync(existingPolicyId, cancellationToken);

        var itiResult = CreateITI(policyDraft?.ITI, customer, temporaryAddress, model.ITI);
        var iti = itiResult.Data;

        if (!itiResult.IsSuccess)
            return Result<SubmitDraftResponseModel>.Failed(itiResult.Error);

        if (policyDraft != null)
        {
            context.ITI.Update(iti);
        }
        else
        {
            await context.ITI.AddAsync(iti, cancellationToken);
        }

        policyDraft = CreateITIPolicyDraft(policyDraft, model, customer.Id, iti.Id, "ITI", "ITI", "International Travel");

        var usdRateRequest = new TravelUSDRateRequestModel
        {
            Issuer = "ITI",
            PlanType = model.ITI.PlanType.ToString(),
            Period = model.ITI.PolicyPeriodInDays,
            IsIndividual = !model.ITI.IsFamilyIncluded,
            DestinationIncludes = region,
            Age = age,
            Group = string.Empty
        };

        iti.PremiumAmount = await policyCalculatorService.CalculateUsdRate(usdRateRequest);

        var premiumRequest = new PremiumCalculateRequestModel
        {
            InsuranceType = InsuranceType.InternationalTravel,
            InternationalTravelInsurance = new InternationalTravelInsurancePremiumCalculatorRequestModel
            {
                PolicyPeriodInDays = model.ITI.PolicyPeriodInDays,
                TripType = model.ITI.TripType,
                Age = age,
                VisitingCountry = model.ITI.VisitingCountry,
                TypeOfInsured = TravelInsuranceType.Individual.ToString(),
                PlanType = model.ITI.PlanType,
                IsFamilyIncluded = model.ITI.IsFamilyIncluded
            }
        };

        var premiumResponse = await policyCalculator.CalculatePremiumAsync(premiumRequest);
        var premium = premiumResponse.Data;


        if (string.IsNullOrEmpty(policyDraft.DraftNo))
        {
            policyDraft.DraftNo = draftNo;
        }

        MapPremiumValues(policyDraft, premium, policyDraft.DraftNo);
        policyDraft.ExpiryDate = model.ExpiryDate;
        iti.ExchangeRate = premium.ExchangeRate;

        if (string.IsNullOrWhiteSpace(existingPolicyId))
        {
            await context.PolicyDrafts.AddAsync(policyDraft, cancellationToken);
        }
        else
        {
            context.PolicyDrafts.Update(policyDraft);
        }

        return Result<SubmitDraftResponseModel>.Success(new SubmitDraftResponseModel
        {
            PolicyId = policyDraft.Id,
            CalculationDetail = premium
        });
    }
    private async Task<Result<SubmitDraftResponseModel>> HandleThirdPartyBikeDraftAsync(
    SubmitDraftRequestModel model,
    string customerId,
    string draftNo,
    string existingPolicyId,
    CancellationToken cancellationToken)
    {
        // Get or create the policy draft
        var policyDraft = await GetOrCreateDraftAsync(existingPolicyId, cancellationToken);
        var isNewDraft = policyDraft == null;

        // Prepare Motor entity
        var motorEntity = isNewDraft
            ? CreateMotor(new Motor(), model.Motor)
            : CreateMotor(policyDraft.Motor, model.Motor);

        if (isNewDraft)
        {
            await context.Motors.AddAsync(motorEntity, cancellationToken);
        }
        else
        {
            motorEntity.Id = policyDraft.MotorId;
            context.Motors.Update(motorEntity);
        }

        // Determine policy codes based on vehicle type
        var isFuel = motorEntity.VehicleType == (int)VechileType.Fuel;
        var policyCode = isFuel ? "MCY" : "EMCY";

        policyDraft = CreatePolicyDraft(
            policyDraft,
            model,
            customerId,
            motorEntity.Id,
            policyCode,
            policyCode,
            "Motorcycle"
        );

        // Prepare premium request model
        var premiumRequest = new PremiumCalculateRequestModel
        {
            InsuranceType = InsuranceType.ThirdPartyBike,
            ThirdPartyBikeInsurance = new ThirdPartyBikeInsuranceModel
            {
                Model = model.Motor.Model,
                VechileType = model.Motor.VechileType,
                ManufactureCompany = model.Motor.ManufactureCompany,
                EngineCapacity = isFuel ? model.Motor.CubicCapacity : null,
                KilloWattRange = model.Motor.VechileType == VechileType.Electric
                    ? model.Motor.KiloWatt
                    : null
            }
        };

        // Calculate premium
        var premiumResponse = await policyCalculator.CalculatePremiumAsync(premiumRequest);
        var premium = premiumResponse.Data;

        if (string.IsNullOrEmpty(policyDraft.DraftNo))
        {
            policyDraft.DraftNo = draftNo;
        }
        // Map premium values to policy draft
        MapPremiumValues(policyDraft, premium, policyDraft.DraftNo);

        // Save or update the policy draft
        if (isNewDraft)
        {
            await context.PolicyDrafts.AddAsync(policyDraft, cancellationToken);
        }
        else
        {
            policyDraft.Id = existingPolicyId;
            context.PolicyDrafts.Update(policyDraft);
        }

        // Build and return response
        return Result<SubmitDraftResponseModel>.Success(new SubmitDraftResponseModel
        {
            PolicyId = policyDraft.Id,
            CalculationDetail = premium
        });
    }

    private async Task<PolicyDraft> GetOrCreateDraftAsync(string policyId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(policyId)) return null;

        return await context.PolicyDrafts
                            .Include(x => x.ITI)
                            .Include(x => x.Motor)
                            .FirstOrDefaultAsync(p => p.Id == policyId, cancellationToken);
    }

    private static void MapPremiumValues(PolicyDraft draft, PremiumCalculationResponseModel premium, string draftNo)
    {
        draft.DraftNo = draftNo;
        draft.NetPremium = premium.NetPremium;
        draft.ThirdPartyPremium = premium.ThirdPartyPremium;
        draft.BasicPremium = premium.BasicPremium;
        draft.RSMDTPremium = premium.RSMDTPremium;
        draft.GrossPremium = premium.GrossPremium;
        draft.PayableAmount = premium.PayableAmount;
        draft.TotalPremium = premium.TotalPremium;
        draft.GovernmentSubsidyAmount = premium.GovernmentSubsidyAmount;
        draft.PersonalAccidentPremium = premium.PersonalAccidentPremium;
        draft.SumInsured = premium.SumInsured;
        draft.StampDuty = premium.StampDuty;
        draft.VatAmount = premium.VatAmount;
    }

    public async Task<Result<List<GetAllDraftResponseModel>>> GetDraftPoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();

        var draftPolicies = context.PolicyDrafts
                                        .Include(d => d.Motor)
                                        .Include(d => d.ITI)
                                        .Include(d => d.Customer)
                                        .Where(x => x.Customer.UserId == userId && x.Status != PurchaseStatus.Acknowledged && x.Status != PurchaseStatus.Paid)
                                        .AsNoTracking();

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(draftPolicies, requestModel);

        var policies = await result.Select(model => new GetAllDraftResponseModel
        {
            Id = model.Id,
            DraftNo = model.DraftNo,
            NetPremium = model.NetPremium,
            PortfolioAlias = model.PortfolioAlias,
            Class = model.Class,
            Model = model.Motor != null ? model.Motor.Model : default,
            InsuredName = model.Customer != null ? model.Customer.FullName : default,
            VistingCountry = model.ITI != null ? model.ITI.VisitingCountry : default,
            PolicyStatus = model.Status.ToString(),
            CreatedOn = model.CreatedOn.ToString(),
            ExpiryDate = model.ExpiryDate.ToString(),
            ManufacturedDate = model.Motor.ManufactureYear,
            EffectiveDate = model.EffectiveDate.ToString()
        }).ToListAsync(cancellationToken);


        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<GetAllDraftResponseModel>>.Success(policies, pagination);
    }

    private Motor CreateMotor(Motor motor, MotorRequestModel model)
    {
        var isElectric = model.VechileType == VechileType.Electric;
        var isFuel = model.VechileType == VechileType.Fuel;


        motor.IsThirdParty = model?.IsThirdParty ?? false;
        motor.IsComprehensive = model?.IsComprehensive ?? false;
        motor.Type = model?.Type;
        motor.VehicleType = (int)model.VechileType;
        motor.ManufactureYear = model?.ManufactureYear;
        motor.ManufactureCompany = model?.ManufactureCompany;
        motor.Financer = model.Financer;
        motor.Model = model?.Model;
        motor.PurchasedNewOld = model?.PurchasedNewOld ?? false;
        motor.DateOfPurchase = model?.DateOfPurchase;
        motor.ChasisNumber = model?.ChasisNumber;
        motor.EngineNumber = model?.EngineNumber;
        motor.RegistrationNumber = model?.RegistrationNumber;
        motor.VoluntaryExcess = model?.VoluntaryExcess ?? 0;
        motor.CompulsoryExcess = model?.CompulsoryExcess ?? 0;
        motor.TotalExcess = model?.TotalExcess ?? 0;
        motor.CubicCapacity = isFuel ? model.CubicCapacity ?? 0 : null;
        motor.KilloWatt = isElectric ? model.KiloWatt ?? 0 : null;
        motor.Days = model?.Days ?? 0;
        motor.YearsFromRegistrationDateYears = model.YearsFromRegistrationDateYears;
        motor.YearsFromRegistrationDateYearsBS = model?.YearsFromRegistrationDateYearsBS;
        motor.CurrentMarketPrice = model?.CurrentMarketPrice ?? 0;
        motor.AgeOfVehicle = model?.AgeOfVehicle;
        motor.RateOfDepreciation = model?.RateOfDepreciation;
        motor.ValueOfAccessories = model?.ValueOfAccessories;
        motor.NCDYears = model?.NCDYears ?? 0;
        motor.RiotStrike = model?.RiotStrike ?? false;
        motor.BlueBookCopyImage = string.Join(",", model.BlueBookCopyImage);
        motor.SubModel = model?.Model;
        return motor;
    }

    private PolicyDraft CreatePolicyDraft(PolicyDraft draft, SaveDraftRequestModel model, string customerId, string motorId)
    {

        draft.NetPremium = model.NetPremium;
        draft.PortfolioAlias = model?.PortfolioAlias;
        //draft.//PortfolioId = motorPartial?.PortfolioId;
        draft.TypeOfParty = model?.TypeOfParty;
        draft.EffectiveDate = model?.EffectiveDate ?? DateTime.MinValue;
        draft.BancassuanceBankName = model?.BancassuanceBankName;
        draft.BancassuanceBankBranch = model?.BancassuanceBankBranch;
        draft.CustomerId = customerId;
        draft.MotorId = motorId;
        return draft;
    }
    private PolicyDraft CreatePolicyDraft(PolicyDraft draft, SubmitDraftRequestModel model, string customerId, string motorId, string portfolioAlias, string portfolioId, string policyClass)
    {
        if (draft == null)
        {
            draft = new PolicyDraft();
        }
        //NetPremium = model.NetPremium;
        draft.PortfolioAlias = portfolioAlias;
        draft.PortfolioId = portfolioId;
        draft.EffectiveDate = model?.EffectiveDate.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime();
        //BancassuanceBankName = model?.BancassuanceBankName;
        //BancassuanceBankBranch = model?.BancassuanceBankBranch;
        draft.CustomerId = customerId;
        draft.MotorId = motorId;
        draft.Class = policyClass;
        draft.InsuranceType = InsuranceType.ThirdPartyBike;
        draft.ExpiryDate = model?.ExpiryDate.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime();
        return draft;
    }
    private PolicyDraft CreateITIPolicyDraft(PolicyDraft policy, SubmitDraftRequestModel model, string customerId, string itiId, string portfolioAlias, string portfolioId, string policyClass)
    {
        if (policy == null)
            policy = new PolicyDraft();
        //NetPremium = model.NetPremium,
        policy.InsuranceType = InsuranceType.InternationalTravel;
        policy.PortfolioAlias = portfolioAlias;
        policy.PortfolioId = portfolioId;
        policy.EffectiveDate = model?.EffectiveDate.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime();
        policy.ExpiryDate = DateTime.MinValue.ToUniversalTime();
        //BancassuanceBankName = model?.BancassuanceBankName;
        //BancassuanceBankBranch = model?.BancassuanceBankBranch;
        policy.CustomerId = customerId;
        policy.ITIId = itiId;
        policy.Class = policyClass;

        return policy;
    }

    private Result<InternationalTravelInsurance> CreateITI(InternationalTravelInsurance insuranceTravelInsurance, Customer customer, CustomerAddress address, InternationalTravelInsuranceRequestModel model)
    {
        var region = string.Empty;
        var regionHelperResponse = RegionHelper.GetDestinationRegion(model.VisitingCountry);

        if (regionHelperResponse.IsSuccess)
        {
            region = regionHelperResponse.Data;
        }
        else
        {
            return Result<InternationalTravelInsurance>.Failed(regionHelperResponse.Error);
        }
        insuranceTravelInsurance ??= new InternationalTravelInsurance();
        insuranceTravelInsurance.PolicyPeriodInDays = model.PolicyPeriodInDays;
        insuranceTravelInsurance.TripType = model.TripType;
        insuranceTravelInsurance.PassportNumber = model.PassportNumber;
        insuranceTravelInsurance.VisitingCountry = string.Join(",", model.VisitingCountry);
        insuranceTravelInsurance.FatherHusbandName = model.FatherOrHusbandName;
        insuranceTravelInsurance.EmergencyContactName = model.EmergencyContactName;
        insuranceTravelInsurance.EmergencyContactNumber = model.EmergencyContactNumber;
        insuranceTravelInsurance.TravellingCountry = region;
        insuranceTravelInsurance.InsuranceType = model.TravelInsuranceType;
        insuranceTravelInsurance.TypeOfInsured = model.TypeOfInsured;
        insuranceTravelInsurance.FamilyMembers = model.FamilyMembers?.Select(fm => new ITIFamilyMember
        {
            Relation = fm.Relation,
            FullName = fm.FullName,
            PassportNumber = fm.PassportNumber,
            DateOfBirth = fm.DateOfBirth,
            Gender = fm.Gender
        }).ToList();

        return Result<InternationalTravelInsurance>.Success(insuranceTravelInsurance);
    }

    private PolicyDraft CreateITIPolicyDraft(SaveDraftRequestModel model, string customerId, string itiId)
    {

        return new PolicyDraft
        {
            NetPremium = model.NetPremium,
            PortfolioAlias = model?.PortfolioAlias,
            TypeOfParty = model?.TypeOfParty,
            EffectiveDate = model?.EffectiveDate ?? DateTime.MinValue,
            BancassuanceBankName = model?.BancassuanceBankName,
            BancassuanceBankBranch = model?.BancassuanceBankBranch,
            CustomerId = customerId,
            ITIId = itiId,
        };

    }

    private PrivateVehicle CreatePV(PrivateVehiclePartialViewModel model)
    {
        var pv = model;
        return new PrivateVehicle
        {
            PartyId = pv.PartyId,
            PortfolioId = pv.PortfolioId,
            Type = pv.Type,
            IsComprehensive = pv.IsComprehensive,
            IsThirdParty = pv.IsThirdParty,
            RiotStrikeAndTerrorism = pv.RiotStrikeAndTerrorism,
            EnterSumInsured = pv.EnterSumInsured,
            CompulsoryExcess = pv.CompulsoryExcess,
            TotalExcess = pv.TotalExcess,
            IsDirectDiscountPA = pv.IsDirectDiscountPA,
            ManufactureYear = pv.ManufactureYear,
            ManufactureCompany = pv.ManufactureCompany,
            Model = pv.Model,
            SubModel = pv.SubModel,
            PurchasedNewOld = pv.PurchasedNewOld,
            DateOfPurchase = pv.DateOfPurchase,
            ChasisNumber = pv.ChasisNumber,
            EngineNumber = pv.EngineNumber,
            AgeForPrint = pv.AgeForPrint,
            AgeForPrintEnglish = pv.AgeForPrintEnglish,
            RegistrationNumber = pv.RegistrationNumber,
            RegistrationNumberNepali = pv.RegistrationNumberNepali,
            ProposerName = pv.ProposerName,
            TypeOfInsurance = (int)pv.TypeOfInsurance,
            GoodsCarryingCapacity = pv.GoodsCarryingCapacity,
            CubicCapacity = pv.CubicCapacity,
            VoluntaryExcess = pv.VoluntaryExcess,
            YearsFromRegistrationDateYears = pv.YearsFromRegistrationDateYears,
            YearsFromRegistrationDateYearsBS = pv.YearsFromRegistrationDateYearsBS,
            CurrentMarketPrice = pv.CurrentMarketPrice,
            IsPersonalAccidentForPaidForDriver = pv.IsPersonalAccidentForPaidForDriver,
            IsPersonalAccidentForPaidForPassenger = pv.IsPersonalAccidentForPaidForPassenger,
            PersonalAccidentForPassengerSeatCount = pv.PersonalAccidentForPassengerSeatCount,
            NumberofSeatsIncludingDriver = pv.NumberofSeatsIncludingDriver,
            SumInsuredAmountForPaidDriver = pv.SumInsuredAmountForPaidDriver,
            SumInsuredAmountForPassenger = pv.SumInsuredAmountForPassenger,
            HasTailor = pv.HasTailor,
            ValueOfTailor = pv.ValueOfTailor,
            UseOfPrivateHire = pv.UseOfPrivateHire,
            AgeOfVehicle = pv.AgeOfVehicle,
            RateOfDepreciation = pv.RateOfDepreciation,
            ValueOfAccessories = pv.ValueOfAccessories,
            ValueWithoutAccessories = pv.ValueWithoutAccessories,
            SumInsuredAmount = pv.SumInsuredAmount,
            IsVehicleDutyFree = pv.IsVehicleDutyFree,
            VehiclePurpose = pv.VehiclePurpose,
            AccessoriesDetail = pv.AccessoriesDetail,
            IsProRataOrShortScale = pv.IsProRataOrShortScale,
            Days = pv.Days,
            NCDYears = pv.NCDYears,
            VehicleForHireOrReward = pv.VehicleForHireOrReward,
            ParkingPlaceGarage = pv.ParkingPlaceGarage,
            ParkingGarageOpen = pv.ParkingGarageOpen,
            Maintenance = pv.Maintenance,
            PurposedVehicleUsedOtherThanThePurposer = pv.PurposedVehicleUsedOtherThanThePurposer,
            AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = pv.AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion,
            AnyOtherInsuranceProposedVehicle = pv.AnyOtherInsuranceProposedVehicle,
            InsuranceCompanyName = pv.InsuranceCompanyName,
            IsEntitledForNoClaimDiscountFromOtherInsuranceCompany = pv.IsEntitledForNoClaimDiscountFromOtherInsuranceCompany,
            EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany = pv.EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany,
            RenewalNoticeNCD = pv.RenewalNoticeNCD,
            HasAnyComputerOrInsurer = pv.HasAnyComputerOrInsurer,
            AccidentOrLossInThreeYears = pv.AccidentOrLossInThreeYears,
            HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled = pv.HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled,
            CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany = pv.CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany,
            RiskType = pv.RiskType,
            IsIssued = pv.IsIssued,
            IsAgentInvolved = pv.IsAgentInvolved,
            PreviousPolicyIssuedYear = pv.PreviousPolicyIssuedYear,
            PaToRiderAndOnePillionRiderSumInsuredAmount = pv.PaToRiderAndOnePillionRiderSumInsuredAmount,
            IsPrivateTaxi = pv.IsPrivateTaxi,
            IsRiotStrikeAndTerrorismForDriver = pv.IsRiotStrikeAndTerrorismForDriver,
            IsRiotStrikeAndTerrorismForPassenger = pv.IsRiotStrikeAndTerrorismForPassenger,
            RiotStrikeAndTerrorismForPassengerSeatCount = pv.RiotStrikeAndTerrorismForPassengerSeatCount,
            IsDifferentlyAble = pv.IsDifferentlyAble,
            SpecialDiscountRate = pv.SpecialDiscountRate,
            ISRecoveryCharge = pv.ISRecoveryCharge,
            MasterPolicyNumber = pv.MasterPolicyNumber,
            Transportation = pv.Transportation,
            Replacement = pv.Replacement,
            Depreciation = pv.Depreciation,
            TransportationRate = pv.TransportationRate,
            TransportationAmount = pv.TransportationAmount,
            ReplacementRate = pv.ReplacementRate,
            ReplacementAmount = pv.ReplacementAmount,
            DepreciationRate = pv.DepreciationRate,
            DepreciationAmount = pv.DepreciationAmount,
            HasSmartPolicy = pv.HasSmartPolicy
        };
    }

    private PolicyDraft CreatePVPolicyDraft(SaveDraftRequestModel model, string customerId, string pvId)
    {
        return new PolicyDraft
        {
            NetPremium = model.NetPremium,
            PortfolioAlias = model?.PortfolioAlias,
            TypeOfParty = model?.TypeOfParty,
            EffectiveDate = model?.EffectiveDate ?? DateTime.MinValue,
            BancassuanceBankName = model?.BancassuanceBankName,
            BancassuanceBankBranch = model?.BancassuanceBankBranch,
            CustomerId = customerId,
            PrivateVehicleId = pvId
        };
    }
    public async Task<Result<PolicyDraftByIdResponseModel>> GetDraftPolicyByIdAsync(string draftId, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();

        var draft = context.PolicyDrafts
            .Include(d => d.Motor)
            .Include(d => d.ITI)
            .Include(d => d.Customer)
                .ThenInclude(c => c.User)
            .Where(d => d.Id == draftId &&
                        d.Customer.UserId == userId &&
                        d.Status != PurchaseStatus.Acknowledged &&
                        d.Status != PurchaseStatus.Paid)
        .AsNoTracking();


        if (draft == null)
            return Result<PolicyDraftByIdResponseModel>.Failed("Draft not found");

        var response = await draft.Select(model => new PolicyDraftByIdResponseModel
        {
            DraftNo = model.DraftNo,
            GateWay = model.PaymentGateway,
            TransactionId = model.PaymentTransactionId,
            NetPremium = model.NetPremium,
            CreatedOn = model.CreatedOn,
            StaffEmail = model.Customer.User.Email,
            ExpiryDate = model.ExpiryDate,
            EffectiveDate = model.EffectiveDate,
            Customer = model.Customer == null ? null : new CustomerResponseModel
            {
                FirstName = model.Customer.FirstName,
                MiddleName = model.Customer.MiddleName,
                LastName = model.Customer.LastName,
                DobAD = model.Customer.DobAD,
                DobBS = model.Customer.DobBS,
                Phone = model.Customer.User.PhoneNumber,
                Gender = model.Customer.Gender,
                PanNo = model.Customer.PanNo,
                MaritialStatus = model.Customer.MaritalStatus,
            },
            Motor = model.Motor == null ? null : new MotorResponseModel
            {
                IsThirdParty = model.Motor.IsThirdParty,
                IsComprehensive = model.Motor.IsComprehensive,
                RiotStrike = model.Motor.RiotStrike,
                Type = model.Motor.Type,
                VechileType = (VechileType)model.Motor.VehicleType,
                AgeOfVehicle = model.Motor.AgeOfVehicle,
                ManufactureYear = model.Motor.ManufactureYear,
                ManufactureCompany = model.Motor.ManufactureCompany,
                Financer = model.Motor.Financer,
                Model = model.Motor.Model,
                SubModel = model.Motor.SubModel,
                PurchasedNewOld = model.Motor.PurchasedNewOld,
                DateOfPurchase = model.Motor.DateOfPurchase,
                ChasisNumber = model.Motor.ChasisNumber,
                EngineNumber = model.Motor.EngineNumber,
                RegistrationNumber = model.Motor.RegistrationNumber,
                VoluntaryExcess = model.Motor.VoluntaryExcess,
                CompulsoryExcess = model.Motor.CompulsoryExcess,
                TotalExcess = model.Motor.TotalExcess,
                CubicCapacity = model.Motor.CubicCapacity,
                KiloWatt = model.Motor.KilloWatt,
                Days = model.Motor.Days,
                YearsFromRegistrationDateYears = model.Motor.YearsFromRegistrationDateYears,
                YearsFromRegistrationDateYearsBS = model.Motor.YearsFromRegistrationDateYearsBS,
                CurrentMarketPrice = model.Motor.CurrentMarketPrice,
                RateOfDepreciation = model.Motor.RateOfDepreciation,
                ValueOfAccessories = model.Motor.ValueOfAccessories,
                NCDYears = model.Motor.NCDYears,
                NCDCerticficate = model.Motor.NCDCerticficate,
                PhotoOfVechileString = model.Motor.PhotoOfVechile,
                BlueBookCopyImageString = model.Motor.BlueBookCopyImage,
                BlueBookCopyImageurl = new()

            },
            ITI = model.ITI == null ? null : new ITIResponseModel
            {
                PolicyPeriodInDays = model.ITI.PolicyPeriodInDays,
                TripType = model.ITI.TripType,
                PassportNumber = model.ITI.PassportNumber,
                VisitingCountryString = model.ITI.VisitingCountry,
                FatherOrHusbandName = model.ITI.FatherHusbandName,
                EmergencyContactName = model.ITI.EmergencyContactName,
                EmergencyContactNumber = model.ITI.EmergencyContactNumber,
                TravelInsuranceType = model.ITI.InsuranceType,
                TypeOfInsured = model.ITI.TypeOfInsured,
                TravellingCountry = model.ITI.TravellingCountry,
                PlanType = model.ITI.PlanType,
                //IsFamilyIncluded = model.ITI.
                FamilyMembers = model.ITI.FamilyMembers.Select(fm => new ITIFamilyMemberResponseModel
                {
                    FullName = fm.FullName,
                    PassportNumber = fm.PassportNumber,
                    Relation = fm.Relation,
                    Gender = fm.Gender,
                    DateOfBirth = fm.DateOfBirth
                }).ToList(),
                IsFamilyIncluded = model.ITI.FamilyMembers != null && model.ITI.FamilyMembers.Any()

            }
        }).FirstOrDefaultAsync(cancellationToken);


        if (response.Motor != null)
        {
            response.Motor.PhotoOfVechile = response.Motor.PhotoOfVechileString != null ? response.Motor.PhotoOfVechileString.Split(",").ToList() : new();
            response.Motor.BlueBookCopyImage = response.Motor.BlueBookCopyImageString != null ? response.Motor.BlueBookCopyImageString.Split(",").ToList() : new();
            if (response.Motor.BlueBookCopyImage?.Any() == true)
            {
                response.Motor.BlueBookCopyImageurl = new List<string>();

                foreach (var img in response.Motor.BlueBookCopyImage)
                {
                    var url = await fileService.GetFilePresignedUrlAsync(img);
                    response.Motor.BlueBookCopyImageurl.Add(url);
                }
            }
        }
        if (response.ITI != null)
        {
            response.ITI.VisitingCountry = response.ITI.VisitingCountryString.Split(",").ToList();
        }

        return Result<PolicyDraftByIdResponseModel>.Success(response);
    }

}
