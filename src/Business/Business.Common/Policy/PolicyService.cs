using Business.Common.File;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Common.Policy.Manufacturer;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.ThirdPartyApi;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Address;
using Models.WebApi.Customer;
using Models.WebApi.Customer.Policy;
using Models.WebApi.ITI;
using Models.WebApi.Policy;
using Models.WebApi.Policy.Motor;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;
using SharedKernel.SystemEnum.Payment;

namespace Business.Common.Policy;
public class PolicyService(ApplicationDataContext context,
    ICoreApiService coreApiService,
    IUserProfileService profileService,
    UserManager<ApplicationUser> userManager,
    ISieveExtension sieveExtension,
    IFileService fileService,
    IPolicyAcknowlegeService policyAcknowlegeService
    ) : IPolicyService
{
    public async Task<Result<ThirdPartyPolicyResponseModel>> GetPolicyDetailsAsync(string draftNo, CancellationToken cancellationToken = default)
    {
        var result = await coreApiService.GetPolicyDetailsAsync(draftNo);
        if (result == null)
            Result<ThirdPartyPolicyResponseModel>.Failed("Policy details not found.");
        return Result<ThirdPartyPolicyResponseModel>.Success(result);
    }

    public async Task<string> PrintPolicyAsync(PrintPolicyRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var coreApiRequestModel = new PrintPolicyCoreRequestModel()
        {
            draftNo = requestModel.DraftNumber,
            ishtml = true,
            isNepali = requestModel.isNepali,
            isEnglish = !requestModel.isNepali,
            isCertificate = false,
            isReceipt = false
        };
        var result = await coreApiService.PrintPolicyAsync(coreApiRequestModel);
        return result;
    }
    public async Task<string> PrintReceiptAsync(PrintReceiptRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var coreApiRequestModel = new PrintPolicyCoreRequestModel()
        {
            draftNo = requestModel.DraftNumber,
            ishtml = true,
            isNepali = true,
            isEnglish = false,
            isCertificate = false,
            isReceipt = true
        };
        var result = await coreApiService.PrintPolicyAsync(coreApiRequestModel);
        return result;
    }
    public async Task<string> PrintCertificateAsync(PrintCertificateRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var coreApiRequestModel = new PrintPolicyCoreRequestModel()
        {
            draftNo = requestModel.DraftNumber,
            ishtml = true,
            isNepali = requestModel.isNepali,
            isEnglish = !requestModel.isNepali,
            isCertificate = true,
            isReceipt = false
        };
        var result = await coreApiService.PrintPolicyAsync(coreApiRequestModel);
        return result;
    }

    public async Task<Result<IEnumerable<PolicyResponseModel>>> GetMyPoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var result = await coreApiService.GetPolicyAsync();
        return Result<IEnumerable<PolicyResponseModel>>.Success(result.Data);
    }

    public async Task<Result<List<PolicyIssuanceViewModel>>> GetPolicyList(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        string partyCode = "Testing";
        var result = await coreApiService.GetPolicyList(partyCode);
        return Result<List<PolicyIssuanceViewModel>>.Success(result);
    }

    public async Task<Result<List<ManufacturerViewModel>>> GetAllManufacture(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var result = await coreApiService.GetAllManufacture();
        return Result<List<ManufacturerViewModel>>.Success(result);
    }
    public async Task<Result<List<ManufacturerImportExcelData>>> GetAllManufactureByClassName(string className)
    {
        var result = await coreApiService.GetAllManufacture(className);
        return Result<List<ManufacturerImportExcelData>>.Success(result);
    }

    public async Task<Result<List<PolicyDetailResponse>>> PolicyDetails(PolicyDetailRequest model, CancellationToken cancellationToken = default)
    {
        var result = await coreApiService.PolicyDetails(model);
        return Result<List<PolicyDetailResponse>>.Success(result);
    }

    public async Task<Result<MessageResponseModel>> CreateMotorcyclePolicy(MotorPolicyRequestModel requestModel, string userId, CancellationToken cancellationToken = default)
    {
        var customer = await context.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted, cancellationToken);

        var permanentAddress = customer.Addresses.FirstOrDefault(a => a.AddressType.ToLower() == AddressTypeEnums.Permanent.ToString().ToLower());
        var temporaryAddress = customer.Addresses.FirstOrDefault(a => a.AddressType.ToLower() == AddressTypeEnums.Temporary.ToString().ToLower());

        var contactKyc = new ContactKYCViewModel
        {
            CitizenshipNo = customer.CitizenshipNo,
            CitizenshipIssueDate = customer.CitizenshipIssueDate,
            CitizenshipIssueDistrict = customer.CitizenshipIssueDistrict,
            DobBS = DateTime.TryParse(customer.DobBS, out var parsedDobBs) ? parsedDobBs : DateTime.MinValue,
            DobAD = DateTime.Parse(customer.DobAD),
            PassportNumber = customer.PassportNumber,
            PassportIssueDate = customer.PassportIssueDate,
            PassportExpiryDate = customer.PassportExpiryDate,
            PassportIssuePlace = customer.PassportIssuePlace,
            VoterIdNumber = customer.VoterIdNumber,
            LicenseNumber = customer.LicenseNumber,
            OccupationJson = new List<string> { customer.Occupation } ?? [],
        };

        var contactViewModel = new ContactViewModel
        {
            CourtesyTitle = customer.CourtesyTitle,
            FirstName = customer.FirstName,
            MiddleName = customer.MiddleName,
            LastName = customer.LastName,
            Gender = customer.Gender,
            PanNo = customer.PanNo,
            MaritialStatus = customer.MaritalStatus,
            ContactKYCViewModel = contactKyc,
            PaProvince = permanentAddress.Province,
            PaDistrict = permanentAddress.District,
            PaMunicipality = permanentAddress.Municipality,
            PaWard = permanentAddress.Ward,
            PaStreetAddress = permanentAddress.StreetAddress,
            TmProvince = temporaryAddress.Province,
            TmDistrict = temporaryAddress.District,
            TmMunicipality = temporaryAddress.Municipality,
            TmWard = temporaryAddress.Ward,
            TmStreetAddress = temporaryAddress.StreetAddress,
            Individual_Type = customer.IndividualType
        };

        var createPolicyViewModel = new CreatePolicyViewModel
        {
            ContactViewModel = contactViewModel,

            PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
            TypeOfParty = "Individual",
            PortfolioParent = "Motor",
            Class = "Motorcycle",
            EffectiveDate = requestModel.EffectiveDate,
            ExpiryDate = requestModel.ExpiryDate,
            ProposedDate = requestModel.ProposedDate,
            BancassuanceBankName = "Dummy Bank Name",
            BancassuanceBankBranch = "Dummy Bank Branch",
        };

        if (requestModel.MotorPartial.VechileType == (int)VechileType.Fuel)
        {
            createPolicyViewModel.PortfolioAlias = "MCY";
            createPolicyViewModel.PortfolioId = "MCY";

            var motorPartial = new MotorPartialViewModel
            {

                IsThirdParty = requestModel.MotorPartial.IsThirdParty,
                IsComprehensive = requestModel.MotorPartial.IsComprehensive,
                Type = requestModel.MotorPartial.Type,
                ManufactureYear = requestModel.MotorPartial.ManufactureYear,
                ManufactureCompany = requestModel.MotorPartial.ManufactureCompany,
                Model = requestModel.MotorPartial.Model,
                DateOfPurchase = DateTime.TryParse(requestModel.MotorPartial.DateOfPurchase, out var parsedDate)
                            ? parsedDate
                            : new DateTime(2005, 06, 20),
                ChasisNumber = requestModel.MotorPartial.ChasisNumber,
                EngineNumber = requestModel.MotorPartial.EngineNumber,
                RegistrationNumber = requestModel.MotorPartial.RegistrationNumber,
                CubicCapacity = requestModel.MotorPartial.CubicCapacity ?? 0,
                Days = requestModel.MotorPartial.Days,
                YearsFromRegistrationDateYears = string.IsNullOrEmpty(requestModel.MotorPartial.YearsFromRegistrationDateYears) ? "2002-03-10" : requestModel.MotorPartial.YearsFromRegistrationDateYears,
                YearsFromRegistrationDateYearsBS = string.IsNullOrEmpty(requestModel.MotorPartial.YearsFromRegistrationDateYearsBS) ? "2056-02-20" : requestModel.MotorPartial.YearsFromRegistrationDateYearsBS,
                CurrentMarketPrice = requestModel.MotorPartial.CurrentMarketPrice,
                AgeOfVehicle = requestModel.MotorPartial.AgeOfVehicle,
                RiotStrike = requestModel.MotorPartial.RiotStrike,
                SubModel = requestModel.MotorPartial.Model
            };
            createPolicyViewModel.MotorPartial = motorPartial;
        }
        if (requestModel.MotorPartial.VechileType == (int)VechileType.Electric)
        {
            var electricMotorPartial = new ElectricMotorcyclePartialViewModel
            {
                IsThirdParty = requestModel.MotorPartial.IsThirdParty,
                IsComprehensive = requestModel.MotorPartial.IsComprehensive,
                Type = requestModel.MotorPartial.Type,
                ManufactureYear = requestModel.MotorPartial.ManufactureYear,
                ManufactureCompany = requestModel.MotorPartial.ManufactureCompany,
                Model = requestModel.MotorPartial.Model,
                DateOfPurchase = DateTime.TryParse(requestModel.MotorPartial.DateOfPurchase, out var parsedDate)
                                ? parsedDate
                                : new DateTime(2005, 06, 20),
                ChasisNumber = requestModel.MotorPartial.ChasisNumber,
                EngineNumber = requestModel.MotorPartial.EngineNumber,
                RegistrationNumber = requestModel.MotorPartial.RegistrationNumber,
                KiloWatt = requestModel.MotorPartial.KilloWatt ?? 0,
                Days = Convert.ToInt32(requestModel.MotorPartial.Days),
                YearsFromRegistrationDateYears = string.IsNullOrEmpty(requestModel.MotorPartial.YearsFromRegistrationDateYears) ? "2002-03-10" : requestModel.MotorPartial.YearsFromRegistrationDateYears,
                YearsFromRegistrationDateYearsBS = string.IsNullOrEmpty(requestModel.MotorPartial.YearsFromRegistrationDateYearsBS) ? "2056-02-20" : requestModel.MotorPartial.YearsFromRegistrationDateYearsBS,
                CurrentMarketPrice = requestModel.MotorPartial.CurrentMarketPrice,
                AgeOfVehicle = requestModel.MotorPartial.AgeOfVehicle,
                RiotStrike = requestModel.MotorPartial.RiotStrike,
                SubModel = requestModel.MotorPartial.Model,
            };

            createPolicyViewModel.PortfolioAlias = "EMCY";
            createPolicyViewModel.PortfolioId = "EMCY";

            createPolicyViewModel.ElectricMotorcyclePartial = electricMotorPartial;
        }


        var request = new CorePolicyCreateViewModel
        {
            DraftNo = requestModel.DraftNumber,
            TransactionId = Guid.NewGuid().ToString(),
            CreatePolicyViewModel = createPolicyViewModel,
            ContactViewModel = contactViewModel,
            NetPremium = requestModel.NetPremium,
            PayablePremium = requestModel.PayablePremium,
            GateWay = requestModel.Gateway.ToString()
        };
        await coreApiService.CreatePolicyAsync(request);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Motor policy queued successfully."));
    }

    public async Task<Result<MessageResponseModel>> CreateITIPolicy(string policyId, CancellationToken cancellationToken = default)
    {
        var policy = await context.PolicyDrafts
            .Include(x => x.ITI)
            .Include(x => x.Customer)
            .ThenInclude(c => c.Addresses)
            .Include(x => x.Customer)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(c => c.PaymentTransactionId == policyId && !c.Customer.User.IsDeleted, cancellationToken);

        var customer = policy.Customer;
        var permanentAddress = customer.Addresses.FirstOrDefault(a => a.AddressType.ToLower() == AddressTypeEnums.Permanent.ToString().ToLower());
        var temporaryAddress = customer.Addresses.FirstOrDefault(a => a.AddressType.ToLower() == AddressTypeEnums.Temporary.ToString().ToLower());

        var contactKyc = new ContactKYCViewModel
        {
            CitizenshipNo = customer.CitizenshipNo,
            CitizenshipIssueDate = customer.CitizenshipIssueDate,
            CitizenshipIssueDistrict = customer.CitizenshipIssueDistrict,
            DobBS = DateTime.TryParse(customer.DobBS, out var parsedDobBs) ? parsedDobBs : DateTime.MinValue,
            DobAD = DateTime.Parse(customer.DobAD),
            PassportNumber = customer.PassportNumber,
            PassportIssueDate = customer.PassportIssueDate,
            PassportExpiryDate = customer.PassportExpiryDate,
            PassportIssuePlace = customer.PassportIssuePlace,
            VoterIdNumber = customer.VoterIdNumber,
            LicenseNumber = customer.LicenseNumber,
            OccupationJson = new List<string> { customer.Occupation } ?? [],
        };

        var contactViewModel = new ContactViewModel
        {
            CourtesyTitle = customer.CourtesyTitle,
            FirstName = customer.FirstName,
            MiddleName = customer.MiddleName,
            LastName = customer.LastName,
            Gender = customer.Gender,
            PanNo = customer.PanNo,
            MaritialStatus = customer.MaritalStatus,
            ContactKYCViewModel = contactKyc,
            PaProvince = permanentAddress.Province,
            PaDistrict = permanentAddress.District,
            PaMunicipality = permanentAddress.Municipality,
            PaWard = permanentAddress.Ward,
            PaStreetAddress = permanentAddress.StreetAddress,
            TmProvince = temporaryAddress.Province,
            TmDistrict = temporaryAddress.District,
            TmMunicipality = temporaryAddress.Municipality,
            TmWard = temporaryAddress.Ward,
            TmStreetAddress = temporaryAddress.StreetAddress,
            Individual_Type = customer.IndividualType
        };

        var itiPartial = new ITIPartialViewModel
        {
            PolicyPeriodInDays = policy.ITI.PolicyPeriodInDays,
            TripType = policy.ITI.TripType,
            IdType = policy.ITI.IdType,
            PassportNumber = policy.ITI.PassportNumber,
            VisitingCountry = policy.ITI.VisitingCountry,
            EmergencyContactName = policy.ITI.EmergencyContactName,
            EmergencyContactNumber = policy.ITI.EmergencyContactNumber,
            TravellingCountry = policy.ITI.TravellingCountry,
            InsuranceType = policy.ITI.InsuranceType,
            TypeOfInsured = policy.ITI.TypeOfInsured,
            EffectiveDateBeforeEndorsement = policy.EffectiveDate.ToUniversalTime(),
            ExpiryDateBeforeEndorsement = policy.ExpiryDate.ToUniversalTime(),
            Province = temporaryAddress.Province,
            District = temporaryAddress.District,
            Municipality = temporaryAddress.Municipality,
            Ward = temporaryAddress.Ward.ToString(),
            Phone = policy.Customer.User.PhoneNumber,
            StreetAddress = temporaryAddress.StreetAddress,
            Occupation = customer.Occupation,
            FamilyMembers = policy.ITI.FamilyMembers?.Select(fm => new ITIFamilyMembers
            {
                Relation = fm.Relation,
                FullName = fm.FullName,
                PassportNumber = fm.PassportNumber,
                DateOfBirth = fm.DateOfBirth,
                Gender = fm.Gender
            }).ToList(),
            Age = CalculateAge(customer.DobAD).ToString(),
            FatherHusbandName = policy.ITI.FatherHusbandName,
            ExchangeRate = policy.ITI.ExchangeRate,
            PremiumAmount = policy.ITI.PremiumAmount,
            DateOfBirth = DateTime.Parse(customer.DobAD),
            Email = customer.User.Email,
            Gender = customer.Gender,
        };

        var createPolicyViewModel = new CreatePolicyViewModel
        {
            InternationalTravelInsurancePartial = itiPartial,
            ContactViewModel = contactViewModel,
            PortfolioAlias = "ITI",
            PortfolioId = "ITI",
            PartyId = "68361328-5dc7-4d99-b952-01a0fd66a890",
            TypeOfParty = "Individual",
            PortfolioParent = "ITI",
            Class = "International Travel Insurance",
            EffectiveDate = policy.EffectiveDate,
            ExpiryDate = policy.ExpiryDate,
            ProposedDate = policy.CreatedOn,
            BancassuanceBankName = "Dummy Bank Name",
            BancassuanceBankBranch = "Dummy Bank Branch",
        };
        var request = new CorePolicyCreateViewModel
        {
            DraftNo = policy.DraftNo,
            TransactionId = Guid.NewGuid().ToString(),
            CreatePolicyViewModel = createPolicyViewModel,
            ContactViewModel = contactViewModel,
            NetPremium = policy.NetPremium,
            PayablePremium = policy.PayableAmount,
            GateWay = policy.PaymentGateway.ToString()
        };

        await coreApiService.CreatePolicyAsync(request);


        return Result<MessageResponseModel>.Success(new MessageResponseModel("International travel insurance policy queued successfully."));
    }

    public async Task<Result<PolicyByIdResponseModel>> GetPolicyByIdAsync(string id, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();

        var response = await context.PolicyDrafts
            .AsNoTracking()
            .Where(d => d.Id == id &&
                        d.Customer.UserId == userId &&
                        (d.Status == PurchaseStatus.Acknowledged ||
                        d.Status == PurchaseStatus.Paid))
            .Select(model => new PolicyByIdResponseModel
            {
                PolicyId = model.Id,
                DraftNo = model.DraftNo,
                GateWay = model.PaymentGateway,
                TransactionId = model.PaymentTransactionId,
                NetPremium = model.NetPremium,
                CreatedOn = model.CreatedOn,
                Class = model.PortfolioAlias,
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
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response == null)
            return Result<PolicyByIdResponseModel>.Failed("Draft not found");

        if (response.Motor != null)
        {
            if (!string.IsNullOrWhiteSpace(response.Motor.PhotoOfVechileString))
            {
                response.Motor.PhotoOfVechile = response.Motor.PhotoOfVechileString.Split(",").ToList();
            }
            response.Motor.BlueBookCopyImage = response.Motor.BlueBookCopyImageString.Split(",").ToList();
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

        return Result<PolicyByIdResponseModel>.Success(response);
    }


    public async Task<Result<List<GetAllActivePoliciesResponseModel>>> GetActivePoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();

        var today = DateTime.UtcNow.Date;

        var draftPolicies = context.PolicyDrafts
                                        .Include(d => d.Motor)
                                        .Include(d => d.ITI)
                                        .Include(d => d.Customer)
                                        .Where(x => x.Customer.UserId == userId && (x.Status == PurchaseStatus.Paid || x.Status == PurchaseStatus.Acknowledged) && x.ExpiryDate >= today)
                                        .AsNoTracking();

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(draftPolicies, requestModel);

        var policies = await result.Select(model => new GetAllActivePoliciesResponseModel
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
            DocumentNumber = model.DocumentNumber,
            PolicyNumber = model.PolicyNumber,
            CreatedOn = model.CreatedOn.ToString(),
            ExpiryDate = model.ExpiryDate.ToString(),
            EffectiveDate = model.EffectiveDate.ToString()
        }).ToListAsync(cancellationToken);

        var policieList = policies.Select(x =>
        {
            var effectiveDate = DateTime.Parse(x.EffectiveDate).Date;
            var expiryDate = DateTime.Parse(x.ExpiryDate).Date;

            var baseDate = effectiveDate >= today ? effectiveDate : today;
            var remainingDays = (expiryDate - baseDate).Days;

            return new GetAllActivePoliciesResponseModel
            {
                Class = x.Class,
                RemainingDays = remainingDays < 0 ? 0 : remainingDays,
                CreatedOn = x.CreatedOn,
                DocumentNumber = x.DocumentNumber,
                DraftNo = x.DraftNo,
                EffectiveDate = x.EffectiveDate,
                ExpiryDate = x.ExpiryDate,
                Id = x.Id,
                PolicyNumber = x.PolicyNumber,
                InsuredName = x.InsuredName,
                Model = x.Model,
                NetPremium = x.NetPremium,
                PolicyStatus = x.PolicyStatus,
                PortfolioAlias = x.PortfolioAlias,
                VistingCountry = x.VistingCountry,
                IsExpired = remainingDays < 0
            };
        }).ToList();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<GetAllActivePoliciesResponseModel>>.Success(policieList, pagination);
    }
    public async Task<Result<List<GetAllActivePoliciesResponseModel>>> GetAllPoliciesAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken)
    {
        var userId = profileService.GetUserId();
        var draftPolicies = context.PolicyDrafts
                                        .Include(d => d.Motor)
                                        .Include(d => d.ITI)
                                        .Include(d => d.Customer)
                                        .Where(x => x.Customer.UserId == userId && (x.Status == PurchaseStatus.Paid || x.Status == PurchaseStatus.Acknowledged))
                                        .AsNoTracking();

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(draftPolicies, requestModel);

        var policies = await result.Select(model => new GetAllActivePoliciesResponseModel
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
            DocumentNumber = model.DocumentNumber,
            PolicyNumber = model.PolicyNumber,
            CreatedOn = model.CreatedOn.ToString(),
            ExpiryDate = model.ExpiryDate.ToString(),
            EffectiveDate = model.EffectiveDate.ToString()
        }).ToListAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;

        var policieList = policies.Select(x =>
        {
            var effectiveDate = DateTime.Parse(x.EffectiveDate).Date;
            var expiryDate = DateTime.Parse(x.ExpiryDate).Date;

            var baseDate = effectiveDate >= today ? effectiveDate : today;
            var remainingDays = (expiryDate - baseDate).Days;

            return new GetAllActivePoliciesResponseModel
            {
                Class = x.Class,
                RemainingDays = remainingDays < 0 ? 0 : remainingDays,
                CreatedOn = x.CreatedOn,
                DocumentNumber = x.DocumentNumber,
                DraftNo = x.DraftNo,
                EffectiveDate = x.EffectiveDate,
                ExpiryDate = x.ExpiryDate,
                Id = x.Id,
                PolicyNumber = x.PolicyNumber,
                InsuredName = x.InsuredName,
                Model = x.Model,
                NetPremium = x.NetPremium,
                PolicyStatus = x.PolicyStatus,
                PortfolioAlias = x.PortfolioAlias,
                VistingCountry = x.VistingCountry,
                IsExpired = remainingDays < 0
            };
        }).ToList();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<GetAllActivePoliciesResponseModel>>.Success(policieList, pagination);
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
    public static int CalculateAge(string? dobString)
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

}
