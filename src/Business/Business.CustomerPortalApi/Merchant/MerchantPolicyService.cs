using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Common.DraftNumber;
using Business.Common.PolicyCalculator;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.ITIEntity;
using Data.Entities.MotorEntity;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.EntityFrameworkCore;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.ThirdPartyApi;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer.Policy;
using Models.WebApi.Merchant;
using SharedKernel.Helper;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;
using Data.Entities.PolicyE2e;

namespace Business.BeemaEdgeApi.Merchant;
public class MerchantPolicyService(ICoreApiService coreApiService, ApplicationDataContext context, IUserProfileService profileService,
                                    IDraftNumberService draftNumberService, IPolicyCalculatorService policyCalculator, IPolicyCalculatorService policyCalculatorService) : IMerchantPolicyService
{
    public async Task<Result<MerchantPolicyResponseModel>> SubmitDraftAsync(CreateMerchantPolicyRequestModel model, CancellationToken cancellationToken)
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
                return Result<MerchantPolicyResponseModel>.Failed("Customer not found.");

            if (customer.KycStatus != KYCStatus.Approved)
                return Result<MerchantPolicyResponseModel>.Failed("Kyc not approved.");

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


            return Result<MerchantPolicyResponseModel>.Success(new MerchantPolicyResponseModel
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

    private async Task<Result<MerchantPolicyResponseModel>> HandleThirdPartyBikeDraftAsync(
   CreateMerchantPolicyRequestModel model,
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
        return Result<MerchantPolicyResponseModel>.Success(new MerchantPolicyResponseModel
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
    private PolicyDraft CreatePolicyDraft(PolicyDraft draft, CreateMerchantPolicyRequestModel model, string customerId, string motorId, string portfolioAlias, string portfolioId, string policyClass)
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
    private async Task<Result<MerchantPolicyResponseModel>> HandleITIDraftAsync(
                                CreateMerchantPolicyRequestModel model, Customer customer, CustomerAddress temporaryAddress, string draftNo, string existingPolicyId, CancellationToken cancellationToken)
    {
        var age = CalculateAge(customer.DobAD);

        var regionResponse = RegionHelper.GetDestinationRegion(model.ITI.VisitingCountry);
        if (!regionResponse.IsSuccess)
            return Result<MerchantPolicyResponseModel>.Failed(regionResponse.Error);

        var region = regionResponse.Data;

        var policyDraft = await GetOrCreateDraftAsync(existingPolicyId, cancellationToken);

        var itiResult = CreateITI(policyDraft?.ITI, customer, temporaryAddress, model.ITI);
        var iti = itiResult.Data;

        if (!itiResult.IsSuccess)
            return Result<MerchantPolicyResponseModel>.Failed(itiResult.Error);

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

        return Result<MerchantPolicyResponseModel>.Success(new MerchantPolicyResponseModel
        {
            PolicyId = policyDraft.Id,
            CalculationDetail = premium
        });
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
    private PolicyDraft CreateITIPolicyDraft(PolicyDraft policy, CreateMerchantPolicyRequestModel model, string customerId, string itiId, string portfolioAlias, string portfolioId, string policyClass)
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

}
