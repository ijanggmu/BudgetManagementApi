using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.Draft;
using Data.Entities.Identity;
using Data.Entities.MotorEntity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.Common;
using Models.WebApi.Address;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.PolicyDraftService;

public class PolicyDraftService : IPolicyDraftService
{
    private readonly ApplicationDataContext _context;
    private readonly IUserProfileService _profileService;
    private readonly UserManager<ApplicationUser> _userManager;
    public PolicyDraftService(ApplicationDataContext context, IUserProfileService profileService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _profileService = profileService;
        _userManager = userManager;
    }


    public async Task<Result<MessageResponseModel>> SaveDraftAsync(SaveDraftRequestModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            var motor = CreateMotor(model);
            await _context.Motors.AddAsync(motor);

            var policyDraft = CreatePolicyDraft(model, "test", motor.Id);
            await _context.PolicyDrafts.AddAsync(policyDraft);

            await _context.SaveChangesAsync();



            await transaction.CommitAsync();

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Draft Saved."));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    private Motor CreateMotor(SaveDraftRequestModel model)
    {
        var motor = model.Motor;

        return new Motor
        {
            IsThirdParty = motor?.IsThirdParty ?? false,
            IsComprehensive = motor?.IsComprehensive ?? false,
            Type = motor?.Type,
            //PartyId = motor?.PartyId,
            ManufactureYear = motor?.ManufactureYear,
            ManufactureCompany = motor?.ManufactureCompany,
            Model = motor?.Model,
            SubModel = motor?.SubModel,
            PurchasedNewOld = motor?.PurchasedNewOld ?? false,
            DateOfPurchase = motor?.DateOfPurchase,
            ChasisNumber = motor?.ChasisNumber,
            EngineNumber = motor?.EngineNumber,
            RegistrationNumber = motor?.RegistrationNumber,
            VoluntaryExcess = motor?.VoluntaryExcess ?? 0,
            CompulsoryExcess = motor?.CompulsoryExcess ?? 0,
            TotalExcess = motor?.TotalExcess ?? 0,
            CubicCapacity = motor.CubicCapacity ?? 0,
            Days = motor?.Days ?? 0,
            YearsFromRegistrationDateYearsBS = motor?.YearsFromRegistrationDateYearsBS,
            CurrentMarketPrice = motor?.CurrentMarketPrice ?? 0,
            AgeOfVehicle = motor?.AgeOfVehicle,
            RateOfDepreciation = motor?.RateOfDepreciation,
            ValueOfAccessories = motor?.ValueOfAccessories,
            //VehicleForHireOrReward = motor?.VehicleForHireOrReward,
            //ParkingPlaceGarage = motor?.ParkingPlaceGarage,
            //ParkingGarageOpen = motor?.ParkingGarageOpen,
            //Maintenance = motor?.Maintenance,
            NCDYears = motor?.NCDYears ?? 0,
            //NumberofSeatsIncludingDriver = motor?.NumberofSeatsIncludingDriver ?? 0,
            RiotStrike = motor?.RiotStrike ?? false,
        };

    }
    private PolicyDraft CreatePolicyDraft(SaveDraftRequestModel model, string AdminId, string motorId)
    {
        var motorPartial = model?.Motor;

        return new PolicyDraft
        {
            NetPremium = model.NetPremium,
            PortfolioAlias = model?.PortfolioAlias,
            //PortfolioId = motorPartial?.PortfolioId,
            TypeOfParty = model?.TypeOfParty,
            EffectiveDate = model?.EffectiveDate ?? DateTime.MinValue,
            BancassuanceBankName = model?.BancassuanceBankName,
            BancassuanceBankBranch = model?.BancassuanceBankBranch,
            CustomerId = AdminId,
            MotorId = motorId
        };

    }


    private List<AddressResponseModel> MapAddress(List<CustomerAddress> addresses)
    {
        if (addresses == null || !addresses.Any())
            return new List<AddressResponseModel>();

        return addresses.Select(a => new AddressResponseModel
        {
            AddressType = a.AddressType,
            Province = a.Province,
            District = a.District,
            Municipality = a.Municipality,
            Ward = a.Ward,
            StreetAddress = a.StreetAddress
        }).ToList();
    }


    private CustomerAddress MapAdminAddress(AddressResponseModel address)
    {
        if (address == null) return null;

        return new CustomerAddress
        {
            AddressType = address.AddressType,
            Province = address.Province,
            District = address.District,
            Municipality = address.Municipality,
            Ward = address.Ward,
            StreetAddress = address.StreetAddress
        };
    }

}
