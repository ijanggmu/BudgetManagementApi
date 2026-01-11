//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.PolicyDraftService;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using Microsoft.AspNetCore.Mvc;
//using Models.Common.Policy.Policy;
//using Models.BeemaEdgeApi.Customer.Policy;
//using Models.WebApi.Customer.Merchant;
//using Models.WebApi.Customer.Policy;

//namespace BeemaEdgeApi.Controllers.V1.Individual.Merchant;
////[Route("api/[controller]")]
////[ApiController]
//public class MerchantController(IPolicyDraftService policyDraftService) : BaseIndividualApiController
//{
//    [HttpPost("CreateComprehensiceMotorPolicy")]
//    public async Task<IActionResult> CreateComprehensiceMotorPolicy(ComprehensiveMotorPolicyRequestModel model, CancellationToken cancellationToken)
//    {
//        var request = new SubmitDraftRequestModel
//        {
//            InsuranceType = InsuranceType.FullBike,
//            EffectiveDate = DateTime.UtcNow.Date,
//            ExpiryDate = DateTime.UtcNow.Date.AddYears(1),
//            Financer = !string.IsNullOrWhiteSpace(model.Financer) ? new List<string> { model.Financer } : new List<string>(),
//            Motor = new MotorRequestModel
//            {
//                IsThirdParty = false,
//                IsComprehensive = true,
//                RiotStrike = model.RiotStrike,
//                Type = model.Type,
//                VehicleType = model.VehicleType,
//                AgeOfVehicle = model.AgeOfVehicle,
//                ManufactureYear = model.ManufactureYear,
//                ManufactureCompany = model.ManufactureCompany,
//                Financer = model.Financer,
//                Model = model.Model,
//                SubModel = model.SubModel,
//                PurchasedNewOld = model.PurchasedNewOld,
//                DateOfPurchase = model.DateOfPurchase,
//                ChasisNumber = model.ChasisNumber,
//                EngineNumber = model.EngineNumber,
//                RegistrationNumber = model.RegistrationNumber,
//                VoluntaryExcess = model.VoluntaryExcess,
//                CompulsoryExcess = model.CompulsoryExcess,
//                TotalExcess = model.TotalExcess,
//                CubicCapacity = model.CubicCapacity,
//                KiloWatt = model.KiloWatt,
//                Days = model.Days,
//                YearsFromRegistrationDateYears = model.YearsFromRegistrationDateYears,
//                YearsFromRegistrationDateYearsBS = model.YearsFromRegistrationDateYearsBS,
//                CurrentMarketPrice = model.CurrentMarketPrice,
//                RateOfDepreciation = model.RateOfDepreciation,
//                ValueOfAccessories = model.ValueOfAccessories,
//                NCDYears = model.NCDYears,
//                NCDCerticficate = model.NCDCerticficate,
//                PhotoOfVechile = model.PhotoOfVechile,
//                BlueBookCopyImage = model.BlueBookCopyImage,
//                BlueBookCopyImageUrl = model.BlueBookCopyImageUrl
//            }
//        };
//        var result = await policyDraftService.SubmitDraftAsync(request, cancellationToken);
//        return Ok(result);
//    }

//    [HttpPost("CreateThirdPartyMotorPolicy")]
//    public async Task<IActionResult> CreateThirdPartyMotorPolicy(ThirdPartyMotorPolicyRequestModel model, CancellationToken cancellationToken)
//    {
//        var request = new SubmitDraftRequestModel
//        {
//            InsuranceType = InsuranceType.ThirdPartyBike,
//            EffectiveDate = model.EffectiveDate,
//            ExpiryDate = model.ExpiryDate,
//            Motor = new MotorRequestModel
//            {
//                IsThirdParty = true,
//                IsComprehensive = false,
//                Type = model.Type,
//                VehicleType = model.VehicleType,
//                ManufactureYear = model.ManufactureYear,
//                ManufactureCompany = model.ManufactureCompany,
//                Model = model.Model,
//                SubModel = model.SubModel,
//                YearsFromRegistrationDateYears = model.YearsFromRegistrationDateYears,
//                ChasisNumber = model.ChasisNumber,
//                EngineNumber = model.EngineNumber,
//                RegistrationNumber = model.RegistrationNumber,
//                CubicCapacity = model.CubicCapacity,
//                KiloWatt = model.KiloWatt,
//                Days = model.Days,
//                PhotoOfVechile = model.PhotoOfVechile,
//                BlueBookCopyImage = model.BlueBookCopyImage,
//                BlueBookCopyImageUrl = model.BlueBookCopyImageUrl
//            }
//        };
//        var result = await policyDraftService.SubmitDraftAsync(request, cancellationToken);
//        return Ok(result);
//    }

//    [HttpPost("CreateInternationalTravelPolicy")]

//    public async Task<IActionResult> CreateInternationalTravelPolicy(InternationalTravelInsuranceRequestModel model, CancellationToken cancellationToken)
//    {
//        var request = new SubmitDraftRequestModel
//        {
//            InsuranceType = InsuranceType.InternationalTravel,
//            EffectiveDate = DateTime.UtcNow.Date,
//            ExpiryDate = DateTime.UtcNow.Date.AddDays(model.PolicyPeriodInDays),

//            ITI = new InternationalTravelInsuranceRequestModel
//            {
//                PolicyPeriodInDays = model.PolicyPeriodInDays,
//                TripType = model.TripType,
//                PassportNumber = model.PassportNumber,
//                VisitingCountry = model.VisitingCountry,
//                FatherOrHusbandName = model.FatherOrHusbandName,
//                EmergencyContactName = model.EmergencyContactName,
//                EmergencyContactNumber = model.EmergencyContactNumber,
//                TravelInsuranceType = model.TravelInsuranceType,
//                TypeOfInsured = model.TypeOfInsured,
//                PlanType = model.PlanType,
//                IsFamilyIncluded = model.IsFamilyIncluded,
//                FamilyMembers = model.FamilyMembers?.Select(fm => new ITIFamilyMemberRequestModel
//                {
//                    Relation = fm.Relation,
//                    FullName = fm.FullName,
//                    PassportNumber = fm.PassportNumber,
//                    DateOfBirth = fm.DateOfBirth,
//                    Gender = fm.Gender
//                }).ToList()
//            }
//        };
//        var result = await policyDraftService.SubmitDraftAsync(request, cancellationToken);
//        return Ok(result);
//    }
//}
