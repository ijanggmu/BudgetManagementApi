using Business.Common.Helper;
using Data.Context;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Calculation.Motor;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator.Motor;
public class CommercialVehiclePremiumCalculator : IPolicyPremiumCalculator
{
    private bool _isProRata;
    private decimal shortScaleRate;
    private int days;
    private bool isMinRSMDAmount;
    private readonly ApplicationDataContext _db;

    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.CommercialVehcile,
            PortfolioClassConstants.ElectricVehicle,
            PortfolioClassConstants.PassengerCarryingVehicle,
            PortfolioClassConstants.Ambulance,
            PortfolioClassConstants.GoodsCarryingVehicle,
            PortfolioClassConstants.Taxi,
            PortfolioClassConstants.Tractor,
            PortfolioClassConstants.ConstructionEquipment,
            PortfolioClassConstants.Tanker,
            PortfolioClassConstants.Tempo,
            PortfolioClassConstants.AgricultureForestryVehicle,
            PortfolioClassConstants.ElectricCommercialVehicle,
            PortfolioClassConstants.ElectricVehicleExtendedWarranty,
        };

    public CommercialVehiclePremiumCalculator(ApplicationDataContext db)
    {
        isMinRSMDAmount = false;
        _db = db;
    }
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();

        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);
        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

        var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

        switch (model.PortfolioAlias)
        {
            case PortfolioClassConstants.ElectricVehicle:
                return await ElectricVehicle(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.ElectricVehicleExtendedWarranty:
                return await ElectricVehicleExtendedWarranty(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.PassengerCarryingVehicle:
                return await PassengerCarryingVehicle(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.Taxi:
                return await Taxi(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.Ambulance:
                return await Ambulance(riskSetupModel, model);
            case PortfolioClassConstants.GoodsCarryingVehicle:
                return await GoodsCarryingVehicle(riskSetupModel, model);
            case PortfolioClassConstants.Tractor:
                return await Tractor(riskSetupModel, model);
            case PortfolioClassConstants.ConstructionEquipment:
                return await ConstructionEquipment(riskSetupModel, model);
            case PortfolioClassConstants.Tanker:
                return await Tanker(riskSetupModel, model);
            case PortfolioClassConstants.Tempo:
                return await Tempo(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.ElectricCommercialVehicle:
                return await EletricCommercialVechicle(riskSetupModel, globalriskSetupModel, model);
            case PortfolioClassConstants.AgricultureForestryVehicle:
                return await AgricultureForestryVehicle(riskSetupModel, model);
            default:
                break;
        }
        return null;
    }

    #region Ambulance
    private async Task<PremiumCalculationResultModel> Ambulance(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            var detailCalculationResult = new AmbulanceDetailCalculationResult();
            var res = new PremiumCalculationResultModel();
            var days = Convert.ToInt16(model.AmbulancePartial.CommonProperties.Days ?? "0");
            res.IsProRata = model.AmbulancePartial.CommonProperties.IsProRataOrShortScale;
            _isProRata = res.IsProRata;
            model.VehicleNumber = model.AmbulancePartial.CommonProperties.RegistrationNumber;

            shortScaleRate = 0;
            if (!res.IsProRata && model.AmbulancePartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }
            else
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.AmbulancePartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.AmbulancePartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.AmbulancePartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.AmbulancePartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.AmbulancePartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
            model.AmbulancePartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }
            detailCalculationResult.NumberOfSeats = model.AmbulancePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;

            int basicPremiumRateEnumValue = (int)AmbulanceConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)AmbulanceConfigType.LoadingRate;
            int asPerSeatCapacityEnumValue = (int)AmbulanceConfigType.AsPerSeatCapacity;
            int perBasicSeatCapacityEnumValue = (int)AmbulanceConfigType.PerBasicSeatCapacity;
            int paToPaidDriverEnumValue = (int)AmbulanceConfigType.PAToPaidDriver;
            int paToHelperEnumValue = (int)AmbulanceConfigType.PAToHelper;
            int paToPassengersEnumValue = (int)AmbulanceConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)AmbulanceConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)AmbulanceConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)AmbulanceConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)AmbulanceConfigType.RSMDTToHelperRate;
            int rsmdtToPassengersRateEnumValue = (int)AmbulanceConfigType.RSMDTToPassengersRate;
            int directDiscountPARateEnumValue = (int)AmbulanceConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)AmbulanceConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)AmbulanceConfigType.GovtLoadingRate;
                asPerSeatCapacityEnumValue = (int)AmbulanceConfigType.GovtAsPerSeatCapacity;
                perBasicSeatCapacityEnumValue = (int)AmbulanceConfigType.GovtPerBasicSeatCapacity;
                paToPaidDriverEnumValue = (int)AmbulanceConfigType.GovtPAToPaidDriver;
                paToHelperEnumValue = (int)AmbulanceConfigType.GovtPAToHelper;
                paToPassengersEnumValue = (int)AmbulanceConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)AmbulanceConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)AmbulanceConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)AmbulanceConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)AmbulanceConfigType.GovtRSMDTToHelperRate;
                rsmdtToPassengersRateEnumValue = (int)AmbulanceConfigType.GovtRSMDTToPassengersRate;
            }

            //discard agent for policies with third party only
            if (!model.AmbulancePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion

            #region CalculateSumInsured
            //price of motorcycle model
            decimal currentMarketPrice = Convert.ToDecimal(model.AmbulancePartial.CommonProperties.CurrentMarketPrice);  //get from db
                                                                                                                         // bool a = model.AmbulancePartial.CommonProperties.IsHelperInsured;
            if (!model.AmbulancePartial.CommonProperties.EnterSumInsured)
            {
                var ValueOfAccesories = model.AmbulancePartial.CommonProperties.ValueOfAccessories;
                var ValueWithoutAccesories = model.AmbulancePartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.AmbulancePartial.CommonProperties.SumInsuredAmount = ValueWithoutAccesories + Convert.ToDecimal(ValueOfAccesories);
            }
            else
            {
                model.AmbulancePartial.CommonProperties.SumInsuredAmount = model.AmbulancePartial.CommonProperties.ValueWithoutAccessories + (model.AmbulancePartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.AmbulancePartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.AmbulancePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                var basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicRate;
                var fullTotal = res.PrimaryBasicPremiumAmount = Convert.ToDecimal(basicRate * model.AmbulancePartial.CommonProperties.SumInsuredAmount);
                decimal basicAmount = res.PrimaryBasicPremiumAmount;
                if (!res.IsProRata)
                {
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;
                detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = basicAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };

                var amountPerSeatCapacity = ConstrantValueHelper.GetNewValue(riskSetupModel, asPerSeatCapacityEnumValue, Convert.ToDecimal(model.AmbulancePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver));
                res.AmountPerSeatCapacity = res.ActualAmountPerSeatCapacity = amountPerSeatCapacity;
                fullTotal -= amountPerSeatCapacity;
                if (!res.IsProRata)
                {
                    var amountPerSeatCapacityShortScale = GetProRataOrShortScaleAmount(amountPerSeatCapacity);
                    res.AmountPerSeatCapacity = amountPerSeatCapacityShortScale;
                }
                var total = res.PrimaryBasicPremiumAmount - res.AmountPerSeatCapacity;
                detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountPerSeatCapacity, ProRataOrShortScaleAmount = res.AmountPerSeatCapacity, Total = total };

                //for tailor
                decimal trailorAmount = 0;
                if (model.AmbulancePartial.CommonRSMDTModel.HasTailor && model.AmbulancePartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.ForTrailor);
                    res.TrailorRate = trailorRate;
                    trailorAmount = 200;
                    var trailorTotal = trailorRate * (model.AmbulancePartial.CommonRSMDTModel.ValueOfTailor ?? default(int)) - trailorAmount;
                    res.TrailorAmount = Math.Abs(trailorTotal);
                    var trailorTotalShortScale = trailorTotal;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.AmbulancePartial.CommonRSMDTModel.ValueOfTailor);
                    if (!_isProRata)
                    {
                        trailorTotalShortScale = GetProRataOrShortScaleAmount(trailorTotal);
                        res.TrailorAmount = trailorTotalShortScale;
                    }
                    //detailCalculationResult.ValueOfTrailorEntered = model.AmbulancePartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    total += trailorTotalShortScale;
                    fullTotal += trailorTotal;
                    res.PrimaryBasicPremiumAmount = total;
                    detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = trailorTotal, ProRataOrShortScaleAmount = trailorTotalShortScale, Total = total };
                }

                //loading for old vehicle
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.AmbulancePartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingRate = ageLoadingRate;
                var AgeLoading = Math.Abs(fullTotal) * ageLoadingRate;
                fullTotal += AgeLoading;
                res.AgeLoadingAmount = AgeLoading;
                if (!_isProRata)
                {
                    var ageLoadingShortScale = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = ageLoadingShortScale;
                }
                total += res.AgeLoadingAmount;
                detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = total };

                //voluntary excess
                if (model.AmbulancePartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.VoluntaryExcessRate, model.AmbulancePartial.CommonProperties.VoluntaryExcess);
                    var fullVEAmount = res.VoluntaryExcessAmount = Math.Abs(fullTotal) * res.VoluntaryExcessRate;
                    fullTotal -= res.VoluntaryExcessAmount;
                    if (!_isProRata)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                    }
                    total -= res.VoluntaryExcessAmount;
                    detailCalculationResult.SelectedVoluntaryExcess = Convert.ToDecimal(model.AmbulancePartial.CommonProperties.VoluntaryExcess);
                    detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = fullVEAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = total };
                }

                //   ncd
                if (model.AmbulancePartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.NCDRate, model.AmbulancePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullTotal) * res.NoClaimDiscountRate;
                    fullTotal -= ncdAmount;
                    if (!_isProRata)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(res.NoClaimDiscountAmount);
                    }
                    total -= res.NoClaimDiscountAmount;
                    detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = total };
                }


                if (model.AmbulancePartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    var PrivateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.PrivateUseDiscount);
                    var privateTaxiAmount = PrivateHireRate * Math.Abs(fullTotal);
                    res.PrivateHireAmount = privateTaxiAmount;
                    fullTotal -= privateTaxiAmount;
                    var privateTaxiAmountShortScale = privateTaxiAmount;
                    if (!_isProRata)
                    {
                        privateTaxiAmountShortScale = GetProRataOrShortScaleAmount(privateTaxiAmount);
                        res.PrivateHireAmount = privateTaxiAmountShortScale;
                    }
                    total -= privateTaxiAmountShortScale;
                    detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = PrivateHireRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = privateTaxiAmountShortScale, Total = total };
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.DirectDiscount);
                    res.DirectDiscountRate = DirectDiscountRate;
                    var DirectDiscountAmount = DirectDiscountRate * Math.Abs(fullTotal);
                    fullTotal -= DirectDiscountAmount;
                    res.DirectDiscountAmount = DirectDiscountAmount;
                    var DirectDiscountAmountShortScale = DirectDiscountAmount;
                    if (!_isProRata)
                    {
                        DirectDiscountAmountShortScale = GetProRataOrShortScaleAmount(DirectDiscountAmount);
                        res.DirectDiscountAmount = DirectDiscountAmountShortScale;
                    }
                    total -= DirectDiscountAmountShortScale;
                    detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = DirectDiscountRate, Amount = DirectDiscountAmount, ProRataOrShortScaleAmount = DirectDiscountAmountShortScale, Total = total };
                }

                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                //compare with minimum own premium amount
                res.CalculatedSubTotalA = total;
                res.SubTotalA = res.CalculatedSubTotalA;

                //var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.MinimumBasicPremiumAmount);
                //var minimumBasicpremiumShortScale = minimumBasicPremiumAmount;
                //res.MinbasicPremium = minimumBasicpremiumShortScale;
                //if (!res.IsProRata)
                //{
                //    minimumBasicpremiumShortScale = GetProRataOrShortScaleAmount(minimumBasicPremiumAmount);
                //    res.MinbasicPremium = minimumBasicpremiumShortScale;
                //}
                //// detailCalculationResult.MinimumBasicPremium.Amount = minimumBasicPremiumAmount;
                //res.SubTotalA = minimumBasicpremiumShortScale;
                //if (res.CalculatedSubTotalA > minimumBasicpremiumShortScale)

                //detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel() { Amount = minimumBasicPremiumAmount, ProRataOrShortScaleAmount = minimumBasicpremiumShortScale, Total = minimumBasicpremiumShortScale };

                if (model.AmbulancePartial.CommonProperties.ISRecoveryCharge)
                {

                    var recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;

                    var recoveryChargeShortScale = recoveryCharge;
                    if (!_isProRata)
                    {
                        recoveryChargeShortScale = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = recoveryChargeShortScale;
                    }
                    res.SubTotalA += recoveryChargeShortScale;
                    detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryChargeShortScale, Total = res.SubTotalA };
                }
                total = res.SubTotalA;
                if (model.AmbulancePartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                total = res.SubTotalA;
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.AmbulancePartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perBasicSeatCapacityEnumValue, Convert.ToDecimal(model.AmbulancePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver));
            var thirdPartyShortScale = thirdPartyAmount;

            if (!_isProRata)
            {
                thirdPartyShortScale = GetProRataOrShortScaleAmount(thirdPartyAmount);
            }
            res.TPLperCC = thirdPartyShortScale;
            res.SubTotalB = thirdPartyShortScale;
            res.ThirdPartyAmount = thirdPartyShortScale;
            detailCalculationResult.TPLAsPerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyShortScale, Total = res.SubTotalB };

            //   ncd
            if (model.AmbulancePartial.CommonProperties.IsComprehensive)
            {
                if (model.AmbulancePartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.NCDRate, model.AmbulancePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (!_isProRata)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.ActualSubTotalB = res.ThirdPartyAmount - res.ThirdPartyNoClaimDiscountAmount;
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                }
            }
            if (model.AmbulancePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;

            }
            #endregion

            #region Paid Driver and Passengers
            decimal paToDriverShortScale = 0;
            decimal paToHelperAmountShortScale = 0;
            decimal paToPassengersShortScale = 0;
            res.NumberofPassengers = model.AmbulancePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1;
            //driver
            decimal paToDriverAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            paToDriverShortScale = paToDriverAmount;
            if (!_isProRata)
            {
                paToDriverShortScale = GetProRataOrShortScaleAmount(paToDriverAmount);
            }
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paToDriverAmount);
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paToDriverAmount, ProRataOrShortScaleAmount = paToDriverShortScale, Total = paToDriverShortScale };

            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paToDriverAmount, ProRataOrShortScaleAmount = paToDriverShortScale, Total = paToDriverShortScale };

            }

            //direct discount for DRIVER
            if (model.AmbulancePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverShortScale = paToDriverAmount = paToDriverShortScale - res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }

            res.PAforDriver = paToDriverShortScale;
            if (model.AmbulancePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            decimal paToHelperAmount = 0;
            if (model.AmbulancePartial.CommonProperties.IsHelperInsured)
            {
                if (model.AmbulancePartial.CommonProperties.NumberOfHelpers != null && model.AmbulancePartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.AmbulancePartial.CommonProperties.NumberOfHelpers;
                    paToHelperAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue) * (model.AmbulancePartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paToHelperAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue);
                    res.NumberOfHelpers = 1;
                }
                paToHelperAmountShortScale = paToHelperAmount;
                if (!_isProRata)
                {
                    paToHelperAmountShortScale = GetProRataOrShortScaleAmount(paToHelperAmount);
                }
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paToHelperAmount);
                    detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel() { Amount = paToHelperAmount, ProRataOrShortScaleAmount = paToHelperAmountShortScale, Total = paToHelperAmountShortScale };

                }
                else
                {
                    detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel() { Amount = paToHelperAmount, ProRataOrShortScaleAmount = paToHelperAmountShortScale, Total = paToHelperAmountShortScale };

                }
                //direct discount for HELPER
                if (model.AmbulancePartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmountShortScale = paToHelperAmount = paToHelperAmountShortScale - res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
                res.PAforHelper = paToHelperAmountShortScale;
            }
            if (model.AmbulancePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                paToHelperAmount = res.PAforHelper;
                detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
            }
            res.NumberofPassengers -= (res.NumberOfHelpers ?? default(int));
            if (res.NumberofPassengers < 0)
            {
                res.NumberofPassengers = 0;
            }
            //passenger
            decimal paToPassengersAmount = res.NumberofPassengers *
                    ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengersShortScale = paToPassengersAmount;
            if (!_isProRata)
            {
                paToPassengersShortScale = GetProRataOrShortScaleAmount(paToPassengersAmount);
            }
            if (days != 365)
            {
                paToPassengersAmount = GetProRataOrShortScaleAmount(paToPassengersAmount);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel() { Amount = paToPassengersAmount, ProRataOrShortScaleAmount = paToPassengersShortScale, Total = paToPassengersShortScale };

            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel() { Amount = paToPassengersAmount, ProRataOrShortScaleAmount = paToPassengersShortScale, Total = paToPassengersShortScale };

            }
            //direct discount for PASSENGER
            if (model.AmbulancePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengersAmount * directDiscountPARate;
                paToPassengersShortScale = paToPassengersAmount = paToPassengersShortScale - res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengersShortScale;
            if (model.AmbulancePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengersAmount = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            //res.SubTotalC = (paToDriverShortScale + paToHelperAmountShortScale + paToPassengersShortScale).RoundToFourPrecisions();
            res.SubTotalC = (paToDriverAmount + paToHelperAmount + paToPassengersAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }

            //if (model.AmbulancePartial.CommonProperties.IsLayup)
            //{
            //    res.LayupDiscountPA = ((res.SubTotalC * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
            //    res.SubTotalC = res.SubTotalC - res.LayupDiscountPA;
            //    detailCalculationResult.LayupDiscountPA = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPA, ProRataOrShortScaleAmount = res.LayupDiscountPA, Total = res.SubTotalC };
            //}
            #endregion
            var RSMDTSumForPaidDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = RSMDTSumForPaidDriver;

            var RSMDTSumForHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.RSMDTSumInsuredAmountForHelper);
            res.RSMDTSuminsuredHelper = RSMDTSumForHelper;

            var RSMDTSumForPassengers = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = RSMDTSumForPassengers;

            #region RSMDT
            if (model.AmbulancePartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                //get the rate from db
                var RSMDRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = RSMDRate;
                var minimumRSMD = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.MinimumRSMDTAmount);
                var minimumRSMDShortScale = minimumRSMD;
                if (!_isProRata)
                {
                    minimumRSMDShortScale = GetProRataOrShortScaleAmount(minimumRSMD);
                }
                detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRSMD, ProRataOrShortScaleAmount = minimumRSMDShortScale, Total = minimumRSMDShortScale };

                var fullRSMDAmount = res.RiotAndStrikeAndMdAmount = model.AmbulancePartial.CommonProperties.SumInsuredAmount * RSMDRate;// + 0.05);//for riot &strike & md
                var RSMDShortScale = res.RiotAndStrikeAndMdAmount;
                if (!_isProRata)
                {
                    RSMDShortScale = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = RSMDShortScale;
                }
                detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = RSMDRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = RSMDShortScale, Total = RSMDShortScale };

                if (minimumRSMDShortScale > RSMDShortScale)
                {
                    RSMDShortScale = res.RSMDTAmount = minimumRSMDShortScale;
                    isMinRSMDAmount = true;
                }

                var TerroristRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = TerroristRate;
                var terrorismAmount = res.TerrorismAmount = model.AmbulancePartial.CommonProperties.SumInsuredAmount * TerroristRate;// + 0.05);//for terrorism
                var TerrorismShortScale = res.TerrorismAmount;
                if (!_isProRata)
                {
                    TerrorismShortScale = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    res.TerrorismAmount = TerrorismShortScale;
                }
                detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = TerroristRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = TerrorismShortScale, Total = TerrorismShortScale };

                decimal rsmdtDriverShortScale = 0;
                decimal rsmdtHelperShortScale = 0;
                decimal rsmdtPassengerShortScale = 0;

                ///// Need to add in configuration

                var RSMDTtoPaidDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                var rsmdtDriverAmount = RSMDTtoPaidDriver * RSMDTSumForPaidDriver;
                //PA amount must come from input field and rate from db
                rsmdtDriverShortScale = rsmdtDriverAmount;
                if (!_isProRata)
                {
                    rsmdtDriverShortScale = GetProRataOrShortScaleAmount(rsmdtDriverAmount);
                }
                res.RSMDTDriver = rsmdtDriverShortScale;
                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = RSMDTtoPaidDriver, Amount = rsmdtDriverAmount, ProRataOrShortScaleAmount = rsmdtDriverShortScale, Total = rsmdtDriverShortScale };
                if (model.AmbulancePartial.CommonProperties.IsHelperInsured)
                {
                    var RSMDTtoHelpers = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    decimal rsmdtHelperAmount = 0;
                    if (model.AmbulancePartial.CommonProperties.NumberOfHelpers != null && model.AmbulancePartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtHelperAmount = RSMDTSumForHelper * RSMDTtoHelpers * (model.AmbulancePartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtHelperAmount = RSMDTSumForHelper * RSMDTtoHelpers;
                    }
                    rsmdtHelperShortScale = rsmdtHelperAmount;

                    if (!_isProRata)
                    {
                        rsmdtHelperShortScale = GetProRataOrShortScaleAmount(rsmdtHelperAmount);
                    }
                    res.RSMDTHelper = rsmdtHelperShortScale;
                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = RSMDTtoHelpers, Amount = rsmdtHelperAmount, ProRataOrShortScaleAmount = rsmdtHelperShortScale, Total = rsmdtHelperShortScale };
                }
                //PA amount must come from input field and rate from db



                var RSMDTtoPassengers = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                var rsmdtPassengerAmount = RSMDTtoPassengers * res.NumberofPassengers * RSMDTSumForPassengers;
                rsmdtPassengerShortScale = rsmdtPassengerAmount;

                if (!_isProRata)
                {
                    rsmdtPassengerShortScale = GetProRataOrShortScaleAmount(rsmdtPassengerShortScale);
                }
                res.RSMDTPassenger = rsmdtPassengerShortScale;
                detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel() { Rate = RSMDTtoPassengers, Amount = rsmdtPassengerAmount, ProRataOrShortScaleAmount = rsmdtPassengerShortScale, Total = rsmdtPassengerShortScale };

                //PA amount must come from input field and rate from db

                res.SubTotalD = (RSMDShortScale + TerrorismShortScale + rsmdtDriverShortScale + rsmdtHelperShortScale + rsmdtPassengerShortScale);
                if (model.AmbulancePartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= RSMDShortScale;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.AmbulancePartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += RSMDShortScale;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;

                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.AmbulancePartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.AmbulancePartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AmbulanceConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.AmbulancePartial.CommonProperties.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.AmbulancePartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
                res.IsCoinsurance = model.IsCoinsurance;
                res.HGIShareRate = model.HGIShareRate;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            res.ThirdPartyAmount = res.SubTotalB;
            #endregion
            res.RiskDetails.AmbulanceDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }

    #endregion

    #region ElectricVehicle

    private async Task<PremiumCalculationResultModel> ElectricVehicle(List<CalculationConfigurationViewModel> riskSetupModel, List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            model.ElectricVehiclePartial.IsThirdParty = true;
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new ElectricVehicleDetailCalculationResult();
            model.VehicleNumber = model.ElectricVehiclePartial.RegistrationNumber;
            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.ElectricVehiclePartial.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ElectricVehiclePartial.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ElectricVehiclePartial.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.ElectricVehiclePartial.AgeforPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.ElectricVehiclePartial.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.KW = model.ElectricVehiclePartial.KiloWatt;
            detailCalculationResult.NumberOfSeats = model.ElectricVehiclePartial.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.ElectricVehiclePartial.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            model.ElectricVehiclePartial.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }

            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.ElectricVehiclePartial.Days);
            if (!model.ElectricVehiclePartial.IsProRataOrShortScale && model.ElectricVehiclePartial.IsComprehensive)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }
            else
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = 1;
            }
            #region GOVT
            int loadingRateEnumValue = (int)ElectricVehicleConfigType.LoadingRate;
            int paToPaidDriverEnumValue = (int)ElectricVehicleConfigType.PAToPaidDriver;
            int paToPassengersEnumValue = (int)ElectricVehicleConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)ElectricVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)ElectricVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToPassengersRateEnumValue = (int)ElectricVehicleConfigType.RSMDTToPassengersRate;

            if (model.ApplyGovtConfig)
            {
                loadingRateEnumValue = (int)ElectricVehicleConfigType.GovtLoadingRate;
                paToPaidDriverEnumValue = (int)ElectricVehicleConfigType.GovtPAToPaidDriver;
                paToPassengersEnumValue = (int)ElectricVehicleConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)ElectricVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)ElectricVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToPassengersRateEnumValue = (int)ElectricVehicleConfigType.GovtRSMDTToPassengersRate;
            }
            #endregion

            //discard agent for policies with third party only
            if (!model.ElectricVehiclePartial.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            int directDiscountPARateEnumValue = (int)ElectricVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            #endregion

            #region CalculateSumInsured
            //price of motorcycle model
            decimal currentMarketPrice = Convert.ToDecimal(model.ElectricVehiclePartial.CurrentMarketPrice);  //get from db
            if (!model.ElectricVehiclePartial.EnterSumInsured)
            {
                model.ElectricVehiclePartial.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.ElectricVehiclePartial.SumInsuredAmount = Math.Round(model.ElectricVehiclePartial.ValueWithoutAccessories + (model.ElectricVehiclePartial.ValueOfAccessories ?? default(decimal)), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.ElectricVehiclePartial.SumInsuredAmount = model.ElectricVehiclePartial.ValueWithoutAccessories + (model.ElectricVehiclePartial.ValueOfAccessories ?? default(decimal));
            }
            detailCalculationResult.SumInsured = model.ElectricVehiclePartial.SumInsuredAmount;
            #endregion
            var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.MinimumBasicPremiumAmount);

            if (model.ElectricVehiclePartial.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal thresholdAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.SumInsuredThresholdAmount);
                var basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.ForFirstThresholdAmount, model.ElectricVehiclePartial.KiloWatt);
                res.BasicPremiumRate = basicRate;
                decimal fullBasicPremium = 0;
                res.ThresholdAmount = thresholdAmount;
                res.SumInsuredToDisplay = model.ElectricVehiclePartial.SumInsuredAmount;

                if (model.ElectricVehiclePartial.SumInsuredAmount > thresholdAmount)
                {
                    decimal basicThresholdAmount = basicRate * thresholdAmount;
                    var Above20Lakh = res.ExcessAmount = model.ElectricVehiclePartial.SumInsuredAmount - thresholdAmount;
                    var rateAbove20Lakh = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.ForAboveThresholdAmount);
                    var premiumAbove20Lakh = rateAbove20Lakh * Above20Lakh;
                    res.SumInsuredToDisplay = res.ThresholdAmount;

                    if (days != 365)
                    {
                        res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(basicThresholdAmount);
                        decimal proRataExcessAmount = GetProRataOrShortScaleAmount(premiumAbove20Lakh);
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = basicThresholdAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremiumForAboveThreshold = new CalculationSubDetailAmountModel() { Rate = rateAbove20Lakh, Amount = premiumAbove20Lakh, ProRataOrShortScaleAmount = proRataExcessAmount, Total = proRataExcessAmount };
                        res.PrimaryBasicPremiumAmount += proRataExcessAmount;
                        fullBasicPremium = basicThresholdAmount + premiumAbove20Lakh;
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };

                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount = fullBasicPremium = basicThresholdAmount + premiumAbove20Lakh;
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = basicThresholdAmount, ProRataOrShortScaleAmount = basicThresholdAmount, Total = basicThresholdAmount };
                        detailCalculationResult.BasicPremiumForAboveThreshold = new CalculationSubDetailAmountModel() { Rate = rateAbove20Lakh, Amount = premiumAbove20Lakh, ProRataOrShortScaleAmount = premiumAbove20Lakh, Total = premiumAbove20Lakh };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }
                else
                {
                    if (days != 365)
                    {
                        fullBasicPremium = basicRate * model.ElectricVehiclePartial.SumInsuredAmount;
                        res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicPremium);
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount = fullBasicPremium = basicRate * model.ElectricVehiclePartial.SumInsuredAmount;
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                //add trailor
                decimal tailorAmount = 0;
                if (model.ElectricVehiclePartial.HasTailor && model.ElectricVehiclePartial.ValueOfTailor > 0)
                {
                    decimal tailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.ForTrailor);
                    res.TrailorRate = tailorRate;
                    tailorAmount = 300 + tailorRate * model.ElectricVehiclePartial.ValueOfTailor ?? default(int);
                    res.TrailorAmount = tailorAmount;
                    fullBasicPremium += tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = model.ElectricVehiclePartial.ValueOfTailor ?? default(int);

                    if (days != 365)
                    {
                        decimal proRataTailorAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataTailorAmount;
                        res.PrimaryBasicPremiumAmount += proRataTailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = tailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataTailorAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = tailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }

                //deduct tpl as per kw
                var tplPerKW = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.TPLPremiumAsPerKW, model.ElectricVehiclePartial.KiloWatt);
                res.BasicTPLPerKW = tplPerKW;
                fullBasicPremium -= tplPerKW;
                if (days != 365)
                {
                    decimal proRataTPLPerKW = GetProRataOrShortScaleAmount(tplPerKW);
                    res.BasicTPLPerKW = proRataTPLPerKW;
                    res.PrimaryBasicPremiumAmount -= proRataTPLPerKW;
                    detailCalculationResult.TPLAsPerKW = new CalculationSubDetailAmountModel { Amount = tplPerKW, ProRataOrShortScaleAmount = proRataTPLPerKW, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    res.PrimaryBasicPremiumAmount -= tplPerKW;
                    detailCalculationResult.TPLAsPerKW = new CalculationSubDetailAmountModel { Amount = tplPerKW, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }


                //add loading for old vehicle
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.ElectricVehiclePartial.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingAmount = Math.Abs(fullBasicPremium) * ageLoadingRate;
                res.AgeLoadingRate = ageLoadingRate;
                var basicTotal = fullBasicPremium + res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataAgeLoadingAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataAgeLoadingAmount;
                    decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataAgeLoadingAmount, Total = proRataTotal };
                }
                else
                {
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = basicTotal };
                }

                //add private taxi
                decimal privateTaxiAmount = 0;
                if (model.ElectricVehiclePartial.UseOfPrivateHire)
                {
                    var privateTaxiRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.PrivateTaxi);
                    privateTaxiAmount = privateTaxiRate * Math.Abs(basicTotal);
                    res.PrivateHireAmount = privateTaxiAmount;
                    basicTotal += privateTaxiAmount;
                    if (days != 365)
                    {
                        decimal proRataTaxiAmount = GetProRataOrShortScaleAmount(privateTaxiAmount);
                        res.PrivateHireAmount = proRataTaxiAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = proRataTaxiAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = privateTaxiAmount, Total = basicTotal };
                    }
                }

                //deduct voluntary excess
                if (model.ElectricVehiclePartial.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.VoluntaryExcessRate, model.ElectricVehiclePartial.VoluntaryExcess);
                    decimal totalVEAmount = res.VoluntaryExcessAmount = Math.Abs(basicTotal) * res.VoluntaryExcessRate;
                    basicTotal -= res.VoluntaryExcessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(totalVEAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = totalVEAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = totalVEAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = basicTotal };
                    }
                }

                //   ncd
                if (model.ElectricVehiclePartial.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.NCDRate, model.ElectricVehiclePartial.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(basicTotal) * res.NoClaimDiscountRate;
                    basicTotal -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = basicTotal };
                    }
                }

                //if all goes well check policy issue date. for the first year there is 0% discount

                int policyIssuedYear = model.ElectricVehiclePartial.PreviousPolicyIssuedYear; //get this data from db
                                                                                              // decimal ncdRate = ConstrantValueHelper.GetValue(riskSetupModel, "NCDRate", model.ElectricVehiclePartial.AgeOfVehicle ?? default(decimal));
                                                                                              //res.NoClaimDiscountAmount = veTotal * ncdRate;
                var amountAfterNcd = basicTotal;//- res.NoClaimDiscountAmount;
                res.CalculatedSubTotalA = amountAfterNcd;

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(res.CalculatedSubTotalA);
                    res.DirectDiscountAmount = directDiscountAmount;
                    res.CalculatedSubTotalA -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataDDAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        res.DirectDiscountAmount = proRataDDAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataDDAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = res.CalculatedSubTotalA };
                    }
                }

                if (days != 365)
                {
                    res.CalculatedSubTotalA = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                }

                if (model.ElectricVehiclePartial.IsLayup)
                {
                    if (model.ElectricVehiclePartial.IsRecoveryCharge)
                    {
                        var recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.RecoveryCharge);
                        res.ActualRecoveryCharge = recoveryCharge;
                        if (days != 365)
                        {
                            decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                            res.CalculatedSubTotalA += proRataRCAmount;
                            res.RecoveryCharge = proRataRCAmount;
                            detailCalculationResult.RecoveryChargeBeforeLayup = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = res.CalculatedSubTotalA };
                        }
                        else
                        {
                            res.CalculatedSubTotalA += recoveryCharge;
                            res.RecoveryCharge = recoveryCharge;
                            detailCalculationResult.RecoveryChargeBeforeLayup = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.CalculatedSubTotalA };
                        }
                    }
                    minimumBasicPremiumAmount += res.RecoveryCharge;
                    res.LayupDiscountOwnDamage = ((res.CalculatedSubTotalA * 2 / 3) / 365) * model.ElectricVehiclePartial.LayupDays;
                    res.CalculatedSubTotalA = res.CalculatedSubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.CalculatedSubTotalA };

                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.CalculatedSubTotalA, Total = res.CalculatedSubTotalA };
                //compare with minimum own premium amount
                res.MinbasicPremium = minimumBasicPremiumAmount;
                if (days != 365)
                {
                    var fullMinBasicPremium = minimumBasicPremiumAmount;
                    minimumBasicPremiumAmount = GetProRataOrShortScaleAmount(minimumBasicPremiumAmount);
                    res.MinbasicPremium = minimumBasicPremiumAmount;
                    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = fullMinBasicPremium, ProRataOrShortScaleAmount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                }

                res.SubTotalA = minimumBasicPremiumAmount;
                if (res.CalculatedSubTotalA > minimumBasicPremiumAmount) res.SubTotalA = res.CalculatedSubTotalA;

                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }

                if (model.ElectricVehiclePartial.IsRecoveryCharge)
                {
                    if (!model.ElectricVehiclePartial.IsLayup)
                    {
                        decimal recoveryAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.RecoveryCharge);
                        res.RecoveryCharge = recoveryAmount;
                        res.ActualRecoveryCharge = recoveryAmount;
                        if (days != 365)
                        {
                            decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryAmount);
                            res.RecoveryCharge = proRataRCAmount;
                            res.SubTotalA += proRataRCAmount;
                            detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryAmount, ProRataOrShortScaleAmount = proRataRCAmount, Total = res.SubTotalA };
                        }
                        else
                        {
                            res.SubTotalA += recoveryAmount;
                            detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryAmount, ProRataOrShortScaleAmount = recoveryAmount, Total = res.SubTotalA };
                        }
                    }
                }

                if (model.ElectricVehiclePartial.IsDifferentlyAble && res.CalculatedSubTotalA > minimumBasicPremiumAmount)
                {
                    res.SpecialDiscountRate = (model.ElectricVehiclePartial.SpecialDiscountRate ?? default(decimal)) / 100;
                    // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                    res.OwnDamageSpecialDiscountAmount = res.SubTotalA * res.SpecialDiscountRate;
                    res.ShortScaleOwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.OwnDamageSpecialDiscountAmount);
                    res.SubTotalA -= res.OwnDamageSpecialDiscountAmount;
                    detailCalculationResult.SpecialDiscount = new CalculationSubDetailAmountModel { Rate = res.SpecialDiscountRate, Amount = res.OwnDamageSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleOwnDamageSpecialDiscountAmount, Total = res.SubTotalA };

                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA.RoundToFourPrecisions() * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            //if (model.ElectricVehiclePartial.IsThirdParty)
            //{
            decimal totalTPL = 0;
            model.ElectricVehiclePartial.IsThirdParty = true;
            decimal thirdPartyAmount = res.TPLperKW = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.PerKWBasicAmount, model.ElectricVehiclePartial.KiloWatt);
            res.TPLperKW = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                res.TPLperKW = proRataOrShortScaleAmount;
                detailCalculationResult.BasicAsPerKW = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.BasicAsPerKW = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }

            //   ncd
            if (model.ElectricVehiclePartial.NCDYears > 0 && model.ElectricVehiclePartial.IsComprehensive)
            {
                res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.NCDRate, model.ElectricVehiclePartial.NCDYears);
                decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                if (days != 365)
                {
                    res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                    detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                }
                else
                {
                    res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                    detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                }
                res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
            }
            if (res.CalculatedSubTotalA > minimumBasicPremiumAmount)
            {
                if (model.ElectricVehiclePartial.IsDifferentlyAble && model.ElectricVehiclePartial.IsComprehensive)
                {
                    res.SpecialDiscountRate = (model.ElectricVehiclePartial.SpecialDiscountRate ?? default(decimal)) / 100;
                    // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                    res.TPLSpecialDiscountAmount = res.SubTotalB * res.SpecialDiscountRate;
                    res.ShortScaleTPLSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.TPLSpecialDiscountAmount);
                    res.SubTotalB -= res.TPLSpecialDiscountAmount;
                    detailCalculationResult.SpecialDiscountTPL = new CalculationSubDetailAmountModel { Rate = res.SpecialDiscountRate, Amount = res.TPLSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleTPLSpecialDiscountAmount, Total = res.SubTotalB };

                }
            }
            if (model.ElectricVehiclePartial.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.ElectricVehiclePartial.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB.RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver And Passengers
            res.NumberofPassengers = model.ElectricVehiclePartial.NumberofSeatsIncludingDriver - 1;

            decimal paToDriver = 0;
            decimal paToPassengers = 0;

            //driver
            paToDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);

            if (days != 365)
            {
                decimal fullPaToDriver = paToDriver;
                paToDriver = GetProRataOrShortScaleAmount(paToDriver);
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = paToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }

            //direct discount for DRIVER
            if (model.ElectricVehiclePartial.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriver * directDiscountPARate;
                paToDriver -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriver;
            if (model.ElectricVehiclePartial.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.ElectricVehiclePartial.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriver = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //passengers
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengers = paForIndividualPassenger * res.NumberofPassengers;
            if (days != 365)
            {
                decimal fullPaToPassengers = paToPassengers;
                paToPassengers = GetProRataOrShortScaleAmount(paToPassengers);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = paToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }

            //direct discount for PASSENGER
            if (model.ElectricVehiclePartial.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengers * directDiscountPARate;
                paToPassengers -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengers;
            if (model.ElectricVehiclePartial.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.ElectricVehiclePartial.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengers = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }

            res.SubTotalC = (paToDriver + paToPassengers).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = ((paToDriver + paToPassengers).RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            #endregion
            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;

            var rsmdtSumInsuredForPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassenger;
            #region RSMDT
            //check if rsmdt enabled
            if (model.ElectricVehiclePartial.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                //get the rate from db
                var minimumRsmdAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.MinimumRSMDTAmount);
                if (days != 365)
                {
                    decimal fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = rsmdRate;
                var rsmdAmount = model.ElectricVehiclePartial.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
                res.RSMDTAmount = rsmdAmount;
                if (days != 365)
                {
                    decimal fullRsmdAmount = rsmdAmount;
                    rsmdAmount = GetProRataOrShortScaleAmount(fullRsmdAmount);
                    res.RSMDTAmount = rsmdAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = fullRsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };

                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = rsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };
                }

                res.RiotAndStrikeAndMdAmount = rsmdAmount;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                res.TerrorismAmount = model.ElectricVehiclePartial.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
                if (days != 365)
                {
                    decimal fullTerrorismAmount = res.TerrorismAmount;
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = fullTerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = res.TerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtPassengerAmount = 0;

                var driverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                rsmdtDriverAmount = driverRate * rsmdtSumInsuredForDriver;//PA amount must come from input field and rate from db

                res.RSMDTforDriverRate = driverRate;

                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;
                if (days != 365)
                {
                    decimal fullPaToDriver = rsmdtDriverAmount;
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(fullPaToDriver);
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = rsmdtDriverAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };
                }
                res.RSMDTDriver = rsmdtDriverAmount;
                var passengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                rsmdtPassengerAmount = passengerRate * rsmdtSumInsuredForPassenger * res.NumberofPassengers;//PA amount must come from input field and rate from db     }
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassenger;

                res.RSMDTforPassengerRate = passengerRate;
                if (days != 365)
                {
                    decimal fullPaToPassengers = rsmdtPassengerAmount;
                    rsmdtPassengerAmount = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = rsmdtPassengerAmount, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };
                }
                res.RSMDTPassenger = rsmdtPassengerAmount;
                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtPassengerAmount;
                if (model.ElectricVehiclePartial.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.ElectricVehiclePartial.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD + res.RiotAndStrikeAndMdAmount };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD.RoundToFourPrecisions() * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.ElectricVehiclePartial.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.ElectricVehiclePartial.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount;
            var totalWithStamp = totalWithVat + res.StampDutyAmount;
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.ElectricVehiclePartial.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.ElectricVehiclePartial.SumInsuredAmount = res.SumInsuredAmount;
                res.IsCoinsurance = model.IsCoinsurance;
                res.HGIShareRate = model.HGIShareRate;

            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            res.ThirdPartyAmount = res.SubTotalB;
            #endregion

            res.RiskDetails.ElectricVehicleDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }

    #endregion


    #region ElectricVehicleExtendedWarranty

    private async Task<PremiumCalculationResultModel> ElectricVehicleExtendedWarranty(
        List<CalculationConfigurationViewModel> riskSetupModel,
        List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {


            var res = new PremiumCalculationResultModel();

            res.LimitOfLiabilityPerVehicle = model.ElectricVehiclePartial.LimitOfLiabilityPerVehicle;

            if (model.ElectricVehiclePartial.PremiumRate == null)
            {
                res.PremiumRate = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)EVExtendedWarrantyConfigType.PremiumRate);

            }
            else
            {
                res.PremiumRate = model.ElectricVehiclePartial.PremiumRate;
            }

            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;



            // sum insured
            model.ElectricVehiclePartial.SumInsuredAmount = res.SumInsuredAmount = model.FullSumInsured;

            // premium calculation

            res.SubTotalA = res.CalculatedSubTotalA = res.PremiumRate / 100
                                            * res.LimitOfLiabilityPerVehicle;

            res.TotalPremiumAmount = res.BasicPremium = res.SubTotalA;


            res.StampDutyAmount = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.StampDuty, res.SumInsuredAmount);

            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)EVExtendedWarrantyConfigType.VAT);

            res.VatPercent = VATPercent * 100;

            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();

            res.PremiumAfterStamp = (res.BasicPremium + res.VatAmount + res.StampDutyAmount).RoundToFourPrecisions();
            res.NetPremiumAmount = res.PremiumAfterStamp;



            return res;
        });
    }

    #endregion

    #region PassengerCarryingVehicle
    private async Task<PremiumCalculationResultModel> PassengerCarryingVehicle(List<CalculationConfigurationViewModel> riskSetupModel, List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new PassengerCarryingVehicleDetailCalculationResult();
            model.VehicleNumber = model.PassengerCarryingVehiclePartial.CommonProperties.RegistrationNumber;
            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.PassengerCarryingVehiclePartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.PassengerCarryingVehiclePartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.PassengerCarryingVehiclePartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.PassengerCarryingVehiclePartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.PassengerCarryingVehiclePartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.PassengerCarryingVehiclePartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }
            detailCalculationResult.NumberOfSeats = model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.PassengerCarryingVehiclePartial.CommonProperties.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.PassengerCarryingVehiclePartial.CommonProperties.Days);

            if (!model.PassengerCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                days = 365;
            }

            if (!model.PassengerCarryingVehiclePartial.CommonProperties.IsProRataOrShortScale)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }

            //discard agent for policies with third party only
            if (!model.PassengerCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion

            int basicPremiumRateEnumValue = (int)PassengerCarryingVehicleConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)PassengerCarryingVehicleConfigType.LoadingRate;
            int asPerSeatCapacityEnumValue = (int)PassengerCarryingVehicleConfigType.AsPerSeatCapacity;
            int perBasicSeatCapacityEnumValue = (int)PassengerCarryingVehicleConfigType.PerBasicSeatCapacity;
            int paToPaidDriverEnumValue = (int)PassengerCarryingVehicleConfigType.PAToPaidDriver;
            int paToHelperEnumValue = (int)PassengerCarryingVehicleConfigType.PAToHelper;
            int paToPassengersEnumValue = (int)PassengerCarryingVehicleConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)PassengerCarryingVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)PassengerCarryingVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)PassengerCarryingVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)PassengerCarryingVehicleConfigType.RSMDTToHelperRate;
            int rsmdtToPassengersRateEnumValue = (int)PassengerCarryingVehicleConfigType.RSMDTToPassengersRate;
            int directDiscountPARateEnumValue = (int)PassengerCarryingVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtLoadingRate;
                asPerSeatCapacityEnumValue = (int)PassengerCarryingVehicleConfigType.GovtAsPerSeatCapacity;
                perBasicSeatCapacityEnumValue = (int)PassengerCarryingVehicleConfigType.GovtPerBasicSeatCapacity;
                paToPaidDriverEnumValue = (int)PassengerCarryingVehicleConfigType.GovtPAToPaidDriver;
                paToHelperEnumValue = (int)PassengerCarryingVehicleConfigType.GovtPAToHelper;
                paToPassengersEnumValue = (int)PassengerCarryingVehicleConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtRSMDTToHelperRate;
                rsmdtToPassengersRateEnumValue = (int)PassengerCarryingVehicleConfigType.GovtRSMDTToPassengersRate;
            }
            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.PassengerCarryingVehiclePartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.PassengerCarryingVehiclePartial.CommonProperties.EnterSumInsured)
            {
                model.PassengerCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories = (currentMarketPrice - (currentMarketPrice * rateOfDepreciation));
                model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount = Math.Round(Convert.ToDecimal(model.PassengerCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories) + Convert.ToDecimal(model.PassengerCarryingVehiclePartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount = model.PassengerCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories + (model.PassengerCarryingVehiclePartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                var basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);

                res.BasicPremiumRate = basicPremiumRate;
                var fullBasicPremium = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
                decimal shortScaleIndividualProperty;
                decimal shortScaleTotal;
                if (days != 365)
                {
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicPremium);
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = basicPremiumRate, Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // deduct perseatcapacity
                var amountPerSeatCapacity = ConstrantValueHelper.GetNewValue(riskSetupModel, asPerSeatCapacityEnumValue, Convert.ToDecimal(model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver));
                res.AmountPerSeatCapacity = res.ActualAmountPerSeatCapacity = amountPerSeatCapacity;
                fullBasicPremium -= amountPerSeatCapacity;
                if (days != 365)
                {
                    shortScaleIndividualProperty = GetProRataOrShortScaleAmount(amountPerSeatCapacity);
                    res.AmountPerSeatCapacity = shortScaleIndividualProperty;
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicPremium);
                    detailCalculationResult.LessAsPerSeatsCapacity = new CalculationSubDetailAmountModel { Rate = model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver, Amount = amountPerSeatCapacity, ProRataOrShortScaleAmount = shortScaleIndividualProperty, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    res.PrimaryBasicPremiumAmount = fullBasicPremium;
                    detailCalculationResult.LessAsPerSeatsCapacity = new CalculationSubDetailAmountModel { Rate = model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver, Amount = amountPerSeatCapacity, ProRataOrShortScaleAmount = amountPerSeatCapacity, Total = res.PrimaryBasicPremiumAmount };
                }
                //add for tailor
                decimal tailorAmount = 0;
                if (model.PassengerCarryingVehiclePartial.CommonRSMDTModel.HasTailor && model.PassengerCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor > 0)
                {
                    var tailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.ForTrailor);
                    tailorAmount = tailorRate * (model.PassengerCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor ?? default(int)) - 200;
                    res.TrailorAmount = tailorAmount;
                    fullBasicPremium += tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.PassengerCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor);
                    res.TrailorRate = tailorRate;
                    if (days != 365)
                    {
                        shortScaleIndividualProperty = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = shortScaleIndividualProperty;
                        res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicPremium);
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel { Rate = tailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = shortScaleIndividualProperty, Total = res.PrimaryBasicPremiumAmount };
                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount = fullBasicPremium;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel { Rate = tailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }
                detailCalculationResult.TotalBasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };

                //loading for old vehicle
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.PassengerCarryingVehiclePartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingRate = ageLoadingRate;
                res.AgeLoadingAmount = Math.Abs(fullBasicPremium) * ageLoadingRate;
                var ageLoading = res.AgeLoadingAmount;
                var basicTotal = fullBasicPremium + res.AgeLoadingAmount;
                if (days != 365)
                {
                    shortScaleIndividualProperty = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = shortScaleIndividualProperty;
                    shortScaleTotal = GetProRataOrShortScaleAmount(basicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = ageLoading, ProRataOrShortScaleAmount = shortScaleIndividualProperty, Total = shortScaleTotal };
                }
                else
                {
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = basicTotal };
                }

                //voluntary excess
                if (model.PassengerCarryingVehiclePartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.VoluntaryExcessRate, model.PassengerCarryingVehiclePartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(basicTotal) * res.VoluntaryExcessRate;
                    basicTotal -= res.VoluntaryExcessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        shortScaleTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = shortScaleTotal };
                    }
                    else
                    {
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = basicTotal };
                    }
                }

                //   ncd
                if (model.PassengerCarryingVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.NCDRate, model.PassengerCarryingVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(basicTotal) * res.NoClaimDiscountRate;
                    basicTotal -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        var proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = basicTotal };
                    }
                }

                // TODO: add detailCalculationResult for NCD
                res.CalculatedSubTotalA = basicTotal;

                //private hire
                decimal privateTaxiAmount = 0;
                if (model.PassengerCarryingVehiclePartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    var privateTaxiRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.PrivateUseDiscount);
                    privateTaxiAmount = privateTaxiRate * Math.Abs(res.CalculatedSubTotalA);
                    res.PrivateHireAmount = privateTaxiAmount;
                    res.CalculatedSubTotalA -= privateTaxiAmount;
                    if (days != 365)
                    {
                        shortScaleIndividualProperty = GetProRataOrShortScaleAmount(privateTaxiAmount);
                        res.PrivateHireAmount = shortScaleIndividualProperty;
                        shortScaleTotal = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = shortScaleIndividualProperty, Total = shortScaleTotal };
                    }
                    else
                    {
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = privateTaxiAmount, Total = res.CalculatedSubTotalA };
                    }
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(res.CalculatedSubTotalA);
                    res.DirectDiscountAmount = directDiscountAmount;
                    res.DirectDiscountRate = directDiscountRate;
                    res.CalculatedSubTotalA -= directDiscountAmount;
                    if (days != 365)
                    {
                        shortScaleIndividualProperty = GetProRataOrShortScaleAmount(directDiscountAmount);
                        res.DirectDiscountAmount = shortScaleIndividualProperty;
                        shortScaleTotal = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = shortScaleIndividualProperty, Total = shortScaleTotal };
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = res.CalculatedSubTotalA };
                    }
                }

                if (model.PassengerCarryingVehiclePartial.CommonProperties.ISRecoveryCharge)
                {

                    var recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    res.CalculatedSubTotalA += res.RecoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataRCAmount;
                        decimal shortscaleSubtotalA = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = shortscaleSubtotalA };
                    }
                    else
                    {
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.CalculatedSubTotalA };
                    }
                }
                if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.CalculatedSubTotalA * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                    res.CalculatedSubTotalA = res.CalculatedSubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.CalculatedSubTotalA };
                }
                res.SubTotalA = res.CalculatedSubTotalA;

                if (days != 365)
                {
                    var fullCalculatedAmount = res.SubTotalA;
                    res.SubTotalA = GetProRataOrShortScaleAmount(res.SubTotalA);
                    detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, ProRataOrShortScaleAmount = res.SubTotalA, Total = res.SubTotalA };
                }
                else
                {
                    detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, ProRataOrShortScaleAmount = res.SubTotalA, Total = res.SubTotalA };
                }

                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.PassengerCarryingVehiclePartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perBasicSeatCapacityEnumValue, Convert.ToDecimal(model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver));

            if (days != 365)
            {
                res.TPLperCC = GetProRataOrShortScaleAmount(thirdPartyAmount);
                detailCalculationResult.BasicAsPerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.TPLperCC, Total = res.TPLperCC };
            }
            else
            {
                res.TPLperCC = thirdPartyAmount;
                detailCalculationResult.BasicAsPerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.TPLperCC, Total = res.TPLperCC };
            }

            //   ncd
            res.ThirdPartyNoClaimDiscountAmount = 0;
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                if (model.PassengerCarryingVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.NCDRate, model.PassengerCarryingVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }

                }
            }
            res.SubTotalB = res.TPLperCC - res.ThirdPartyNoClaimDiscountAmount;

            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver,Helper And Passengers
            decimal paDriverAmount = 0;
            decimal paHelperAmount = 0;
            decimal paPassengerAmount = 0;
            int numberOfPassengers = res.NumberofPassengers = model.PassengerCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1;

            paDriverAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            if (days != 365)
            {
                decimal fullPaToDriver = paDriverAmount;
                paDriverAmount = GetProRataOrShortScaleAmount(paDriverAmount);

                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paDriverAmount, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paDriverAmount, Total = paDriverAmount };
            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paDriverAmount, Amount = paDriverAmount, ProRataOrShortScaleAmount = paDriverAmount, Total = paDriverAmount };
            }

            //direct discount for DRIVER
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paDriverAmount * directDiscountPARate;
                paDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paDriverAmount;
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }

            //helper
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsHelperInsured)
            {
                if (model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers != null && model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers;
                    paHelperAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue) * (model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paHelperAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue);
                    res.NumberOfHelpers = 1;
                }
                if (days != 365)
                {
                    decimal fullPaToHelper = paHelperAmount;
                    paHelperAmount = GetProRataOrShortScaleAmount(paHelperAmount);
                    detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel { Rate = paHelperAmount, Amount = fullPaToHelper, ProRataOrShortScaleAmount = paHelperAmount, Total = paHelperAmount };
                }
                else
                {
                    detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel { Rate = paHelperAmount, Amount = paHelperAmount, ProRataOrShortScaleAmount = paHelperAmount, Total = paHelperAmount };
                }
                //direct discount for HELPER
                if (model.PassengerCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paHelperAmount * directDiscountPARate;
                    paHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = res.DirectDiscountRate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
            }
            res.PAforHelper = paHelperAmount;
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                paHelperAmount = res.PAforHelper;
                detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
            }
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            res.NumberofPassengers = numberOfPassengers;
            if (res.NumberofPassengers < 0)
            {
                res.NumberofPassengers = 0;
            }
            paPassengerAmount = res.NumberofPassengers * paForIndividualPassenger;
            if (days != 365)
            {
                decimal fullPaToPassengers = paPassengerAmount;
                paPassengerAmount = GetProRataOrShortScaleAmount(paPassengerAmount);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paPassengerAmount, Total = paPassengerAmount };
            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = paPassengerAmount, ProRataOrShortScaleAmount = paPassengerAmount, Total = paPassengerAmount };
            }
            //direct discount for PASSENGER
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paPassengerAmount * directDiscountPARate;
                paPassengerAmount -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paPassengerAmount;
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paPassengerAmount = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }

            res.SubTotalC = (paDriverAmount + paHelperAmount + paPassengerAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }

            #endregion
            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;

            var rsmdtSumInsuredForPassengers = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassengers;

            var rsmdtSumInsuredForHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.RSMDTSumInsuredAmountForHelper);
            res.RSMDTSuminsuredHelper = rsmdtSumInsuredForHelper;

            #region RSMDT
            if (model.PassengerCarryingVehiclePartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                var minimumRsmdAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.MinimumRSMDTAmount);
                if (days != 365)
                {
                    decimal fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }


                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = rsmdRate;
                var rsmdAmount = model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
                if (days != 365)
                {
                    decimal fullRsmdAmount = rsmdAmount;
                    rsmdAmount = GetProRataOrShortScaleAmount(fullRsmdAmount);
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = fullRsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };

                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = rsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };
                }

                res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = rsmdAmount;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount;
                    isMinRSMDAmount = true;
                }

                res.minRSMDTAmount = minimumRsmdAmount;
                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismAmount = model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
                res.TerrorismRate = terrorismRate;
                if (days != 365)
                {
                    decimal fullTerrorismAmount = res.TerrorismAmount;
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = fullTerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = res.TerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtPassengerAmount = 0;

                ///// Need to add in configuration

                var rsmdtDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                rsmdtDriverAmount = rsmdtSumInsuredForDriver * rsmdtDriverRate;//PA amount must come from input field and rate from db
                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;

                res.RSMDTforDriverRate = rsmdtDriverRate;
                if (days != 365)
                {
                    decimal fullPaToDriver = rsmdtDriverAmount;
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(fullPaToDriver);
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtDriverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtDriverRate, Amount = rsmdtDriverAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };
                }
                res.RSMDTDriver = rsmdtDriverAmount;

                //helper
                if (model.PassengerCarryingVehiclePartial.CommonProperties.IsHelperInsured)
                {
                    var rsmdtHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);

                    rsmdtHelperAmount = rsmdtSumInsuredForHelper * rsmdtHelperRate;//PA amount must come from input field and rate from db
                    detailCalculationResult.RSMDTSumInsuredForHelper = rsmdtSumInsuredForHelper;
                    res.RSMDTForHelperRate = rsmdtHelperRate;

                    if (model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers != null && model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtHelperAmount = rsmdtSumInsuredForHelper * rsmdtHelperRate * (model.PassengerCarryingVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtHelperAmount = rsmdtSumInsuredForHelper * rsmdtHelperRate;
                    }

                    if (days != 365)
                    {
                        decimal fullPaToHelper = rsmdtHelperAmount;
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(fullPaToHelper);
                        detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtHelperRate, Amount = fullPaToHelper, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                    }
                    else
                    {
                        detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtHelperRate, Amount = rsmdtHelperAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                    }
                }

                res.RSMDTHelper = rsmdtHelperAmount;
                var rsmdtPassengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                rsmdtPassengerAmount = rsmdtSumInsuredForPassengers *
                        res.NumberofPassengers * rsmdtPassengerRate;//PA amount must come from input field and rate from db
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassengers;

                res.RSMDTforPassengerRate = rsmdtPassengerRate;
                if (days != 365)
                {
                    decimal fullPaToPassengers = rsmdtPassengerAmount;
                    rsmdtPassengerAmount = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtPassengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = rsmdtPassengerRate, Amount = rsmdtPassengerAmount, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };
                }
                res.RSMDTPassenger = rsmdtPassengerAmount;
                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount + rsmdtPassengerAmount;
                if (model.PassengerCarryingVehiclePartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;

                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.PassengerCarryingVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };

                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat

            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.PassengerCarryingVehiclePartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PassengerCarryingVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.PassengerCarryingVehiclePartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
                res.IsCoinsurance = model.IsCoinsurance;
                res.HGIShareRate = model.HGIShareRate;

            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion
            res.RiskDetails.PassengerCarryingVehicleDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region Taxi
    private async Task<PremiumCalculationResultModel> Taxi(List<CalculationConfigurationViewModel> riskSetupModel, List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new TaxiDetailCalculationResult();
            res.VehicleCapacity = model.TaxiPartial.CommonProperties.CubicCapacity;
            model.VehicleNumber = model.TaxiPartial.CommonProperties.RegistrationNumber;

            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.TaxiPartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TaxiPartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TaxiPartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.TaxiPartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.TaxiPartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.CC = model.TaxiPartial.CommonProperties.CubicCapacity;
            detailCalculationResult.NumberOfSeats = model.TaxiPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.TaxiPartial.CommonProperties.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }

            model.TaxiPartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.TaxiPartial.CommonProperties.Days);
            if (!model.TaxiPartial.CommonProperties.IsComprehensive)
            {
                days = 365;
            }
            if (!model.TaxiPartial.CommonProperties.IsProRataOrShortScale)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }

            int basicPremiumRateEnumValue = (int)TaxiConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)TaxiConfigType.LoadingRate;
            int paToPaidDriverEnumValue = (int)TaxiConfigType.PAToPaidDriver;
            int paToPassengersEnumValue = (int)TaxiConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)TaxiConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)TaxiConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)TaxiConfigType.RSMDTToPaidDriverRate;
            int rsmdtToPassengersRateEnumValue = (int)TaxiConfigType.RSMDTToPassengersRate;
            int tplLoadingPerCCBasicAmount = (int)TaxiConfigType.PerCCBasicAmount;

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)TaxiConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)TaxiConfigType.GovtLoadingRate;
                paToPaidDriverEnumValue = (int)TaxiConfigType.GovtPAToPaidDriver;
                paToPassengersEnumValue = (int)TaxiConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)TaxiConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)TaxiConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)TaxiConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToPassengersRateEnumValue = (int)TaxiConfigType.GovtRSMDTToPassengersRate;
                tplLoadingPerCCBasicAmount = (int)TaxiConfigType.GovtPerCCBasicAmount;
            }

            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;
            var rsmdtSumInsuredForPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassenger;

            //discard agent for policies with third party only
            if (!model.TaxiPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)TaxiConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            #region CalculateSumInsured
            //price of taxi model
            decimal currentMarketPrice = Convert.ToDecimal(model.TaxiPartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.TaxiPartial.CommonProperties.EnterSumInsured)
            {
                model.TaxiPartial.CommonProperties.ValueWithoutAccessories = (currentMarketPrice - (currentMarketPrice * rateOfDepreciation));
                model.TaxiPartial.CommonProperties.SumInsuredAmount = Math.Round(model.TaxiPartial.CommonProperties.ValueWithoutAccessories + (model.TaxiPartial.CommonProperties.ValueOfAccessories ?? default(decimal)), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.TaxiPartial.CommonProperties.SumInsuredAmount = model.TaxiPartial.CommonProperties.ValueWithoutAccessories + (model.TaxiPartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.TaxiPartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.TaxiPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Own damage Premium
                var basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicRate;
                var fullBasicTotal = res.PrimaryBasicPremiumAmount = 1000 + model.TaxiPartial.CommonProperties.SumInsuredAmount * basicRate;

                if (days != 365)
                {
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                var ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.TaxiPartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingRate = ageLoadingRate;
                res.AgeLoadingAmount = ageLoadingRate * fullBasicTotal;
                fullBasicTotal += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataAgeLoadingAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataAgeLoadingAmount;
                    decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataAgeLoadingAmount, Total = proRataTotal };
                }
                else
                {
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = fullBasicTotal };
                }

                if (model.TaxiPartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.VoluntaryExcessRate, model.TaxiPartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = res.VoluntaryExcessRate * fullBasicTotal;
                    fullBasicTotal -= res.VoluntaryExcessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = fullBasicTotal };
                    }
                }

                //   ncd
                if (model.TaxiPartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.NCDRate, model.TaxiPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullBasicTotal) * res.NoClaimDiscountRate;
                    fullBasicTotal -= ncdAmount;
                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(res.NoClaimDiscountAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = fullBasicTotal };
                    }
                }

                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(fullBasicTotal);
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullBasicTotal -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataDDAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        res.DirectDiscountAmount = proRataDDAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataDDAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = fullBasicTotal };
                    }
                }

                //Recovery charge 
                if (model.TaxiPartial.CommonProperties.ISRecoveryCharge)
                {
                    var recoveryCharge = res.ActualRecoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.RecoveryCharge);
                    res.RecoveryCharge = recoveryCharge;
                    fullBasicTotal += recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataRCAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = proRataRCAmount };
                    }
                    else
                    {
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = fullBasicTotal };
                    }

                }
                if (days != 365)
                {
                    res.CalculatedSubTotalA = GetProRataOrShortScaleAmount(fullBasicTotal);
                }
                else
                {
                    res.CalculatedSubTotalA = fullBasicTotal;
                }
                res.SubTotalA = res.CalculatedSubTotalA;
                if (model.TaxiPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.TaxiPartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };

                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.TaxiPartial.CommonProperties.IsThirdParty = true;
            var BasicAsPerCubicCapacity = res.TPLperCC = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, tplLoadingPerCCBasicAmount, model.TaxiPartial.CommonProperties.CubicCapacity);
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(BasicAsPerCubicCapacity);
                res.SubTotalB = res.TPLperCC = proRataOrShortScaleAmount;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }
            else
            {
                res.SubTotalB = BasicAsPerCubicCapacity;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }

            //   ncd
            if (model.TaxiPartial.CommonProperties.IsComprehensive)
            {
                if (model.TaxiPartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.NCDRate, model.TaxiPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = res.SubTotalB - ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                }

            }
            if (model.TaxiPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.TaxiPartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver And Passengers
            res.NumberofPassengers = model.TaxiPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1;
            decimal paToDriver = 0;
            decimal paToPassengers = 0;


            //driver
            paToDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            if (days != 365)
            {
                decimal fullPaToDriver = paToDriver;
                paToDriver = GetProRataOrShortScaleAmount(paToDriver);
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = paToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            //direct discount for DRIVER
            if (model.TaxiPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriver * directDiscountPARate;
                paToDriver -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriver;
            if (model.TaxiPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.TaxiPartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriver = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //passenger
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengers = res.PAforPassenger = paForIndividualPassenger * (res.NumberofPassengers);
            if (days != 365)
            {
                decimal fullPaToPassengers = paToPassengers;
                paToPassengers = GetProRataOrShortScaleAmount(paToPassengers);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = paToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            //direct discount for PASSENGER
            if (model.TaxiPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengers * directDiscountPARate;
                paToPassengers -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengers;
            if (model.TaxiPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.TaxiPartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengers = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriver + paToPassengers).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            #endregion

            if (model.TaxiPartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                #region RSMDT
                //get the rate from db
                var minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.MinimumRSMDTAmount);
                if (days != 365)
                {
                    decimal fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = rsmdRate;
                var rsmdAmount = model.TaxiPartial.CommonProperties.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
                if (days != 365)
                {
                    decimal fullRsmdAmount = rsmdAmount;
                    rsmdAmount = GetProRataOrShortScaleAmount(fullRsmdAmount);
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = fullRsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };

                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = rsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };
                }

                res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = rsmdAmount;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                res.TerrorismAmount = model.TaxiPartial.CommonProperties.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
                if (days != 365)
                {
                    decimal fullTerrorismAmount = res.TerrorismAmount;
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = fullTerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = res.TerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriver = 0;
                decimal rsmdtPassenger = 0;


                var driverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                rsmdtDriver = rsmdtSumInsuredForDriver * driverRate;//PA amount must come from input field and rate from db
                res.RSMDTDriver = rsmdtDriver;
                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;

                if (days != 365)
                {
                    decimal fullPaToDriver = rsmdtDriver;
                    rsmdtDriver = GetProRataOrShortScaleAmount(fullPaToDriver);
                    res.RSMDTDriver = rsmdtDriver;
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = rsmdtDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }

                var passengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                rsmdtPassenger = rsmdtSumInsuredForPassenger *
                                    (model.TaxiPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1) * passengerRate;//PA amount must come from input field and rate from db
                res.RSMDTPassenger = rsmdtPassenger;
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassenger;

                if (days != 365)
                {
                    decimal fullPaToPassengers = rsmdtPassenger;
                    rsmdtPassenger = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    res.RSMDTPassenger = rsmdtPassenger;
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = rsmdtPassenger, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }

                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriver + rsmdtPassenger;
                if (model.TaxiPartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.TaxiPartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.TaxiPartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.TaxiPartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TaxiConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.TaxiPartial.CommonProperties.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.TaxiPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
                res.IsCoinsurance = model.IsCoinsurance;
                res.HGIShareRate = model.HGIShareRate;

            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            res.RiskDetails.TaxiDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region GoodsCarryingVehicle
    private async Task<PremiumCalculationResultModel> GoodsCarryingVehicle(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            if (model.AgricultureForestryVehiclePartial != null)
            {
                model.AgricultureForestryVehiclePartial = null;
            }
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new GoodsCarryingVehicleDetailCalculationResult();
            detailCalculationResult.IsProRateOrShortScaleAmount = model.GoodsCarryingVehiclePartial.CommonProperties.IsProRataOrShortScale;
            model.VehicleNumber = model.GoodsCarryingVehiclePartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity;
            detailCalculationResult.NumberOfSeats = model.GoodsCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            var days = Convert.ToInt16(model.GoodsCarryingVehiclePartial.CommonProperties.Days ?? "0");
            shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            if (!model.GoodsCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            DateViewModel dateViewModel = new DateViewModel();

            if (model.GoodsCarryingVehiclePartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.GoodsCarryingVehiclePartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.GoodsCarryingVehiclePartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.GoodsCarryingVehiclePartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.GoodsCarryingVehiclePartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.GoodsCarryingVehiclePartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            int basicPremiumRateEnumValue = (int)GoodsCarryingVehicleConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)GoodsCarryingVehicleConfigType.LoadingRate;
            int asPerTonEnumValue = (int)GoodsCarryingVehicleConfigType.AsPerTon;
            int perCarryingCapacityInTonEnumValue = (int)GoodsCarryingVehicleConfigType.PerCarryingCapacityInTon;
            int paToPaidDriverEnumValue = (int)GoodsCarryingVehicleConfigType.PAToPaidDriver;
            int paToHelperEnumValue = (int)GoodsCarryingVehicleConfigType.PAToHelper;
            int paToPassengersEnumValue = (int)GoodsCarryingVehicleConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)GoodsCarryingVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)GoodsCarryingVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)GoodsCarryingVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)GoodsCarryingVehicleConfigType.RSMDTToHelperRate;
            int rsmdtToPassengersRateEnumValue = (int)GoodsCarryingVehicleConfigType.RSMDTToPassengersRate;

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtLoadingRate;
                asPerTonEnumValue = (int)GoodsCarryingVehicleConfigType.GovtAsPerTon;
                perCarryingCapacityInTonEnumValue = (int)GoodsCarryingVehicleConfigType.GovtPerCarryingCapacityInTon;
                paToPaidDriverEnumValue = (int)GoodsCarryingVehicleConfigType.GovtPAToPaidDriver;
                paToHelperEnumValue = (int)GoodsCarryingVehicleConfigType.GovtPAToHelper;
                paToPassengersEnumValue = (int)GoodsCarryingVehicleConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtRSMDTToHelperRate;
                rsmdtToPassengersRateEnumValue = (int)GoodsCarryingVehicleConfigType.GovtRSMDTToPassengersRate;
            }

            //discard agent for policies with third party only
            if (!model.GoodsCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)GoodsCarryingVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.GoodsCarryingVehiclePartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.GoodsCarryingVehiclePartial.CommonProperties.EnterSumInsured)
            {
                model.GoodsCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount = Math.Round(model.GoodsCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories + Convert.ToDecimal(model.GoodsCarryingVehiclePartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount = model.GoodsCarryingVehiclePartial.CommonProperties.ValueWithoutAccessories + (model.GoodsCarryingVehiclePartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicPremiumRate;
                decimal amount = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
                decimal fullAmount = amount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                    amount = proRataOrShortScaleAmount;
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    res.PrimaryBasicPremiumAmount = amount;
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = amount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // excess more than 3 
                decimal excessMoreThan__TonsAmount = 0; //model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity - 3;
                decimal excessMoreThan__TonsMinimumValue = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.MinimumTonExcessValue);
                res.MinimumTon = excessMoreThan__TonsMinimumValue;
                if (model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity > excessMoreThan__TonsMinimumValue)
                {
                    decimal excessMorethanAmountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.MinimumTonExcessRateAmount);
                    decimal excessTon = model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity - excessMoreThan__TonsMinimumValue;
                    excessMoreThan__TonsAmount = excessMorethanAmountRate * excessTon;
                    res.AmountPerExcessTons = excessMoreThan__TonsAmount;

                    fullAmount += excessMoreThan__TonsAmount;

                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(excessMoreThan__TonsAmount);
                        res.AmountPerExcessTons = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.PrimaryBasicPremiumAmount = amount;
                    }
                    else
                    {
                        amount += excessMoreThan__TonsAmount;
                        res.PrimaryBasicPremiumAmount = amount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = excessMoreThan__TonsAmount, Total = amount };
                    }
                }

                // as per carrying capacity
                var amountAsPerCarryingCapacity = ConstrantValueHelper.GetNewValue(riskSetupModel, asPerTonEnumValue, model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity);
                res.AmountPerCarryingCapacity = amountAsPerCarryingCapacity;
                fullAmount -= amountAsPerCarryingCapacity;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(amountAsPerCarryingCapacity);
                    res.AmountPerCarryingCapacity = proRataOrShortScaleAmount;
                    amount -= proRataOrShortScaleAmount;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount -= amountAsPerCarryingCapacity;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = amountAsPerCarryingCapacity, Total = amount };
                }
                res.PrimaryBasicPremiumAmount = amount;

                // tailor
                if (model.GoodsCarryingVehiclePartial.CommonRSMDTModel.HasTailor && model.GoodsCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = res.TrailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.ForTrailor);
                    decimal trailorValue = model.GoodsCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    decimal tailorAmount = trailorRate * trailorValue - 200;
                    res.TrailorAmount = tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.GoodsCarryingVehiclePartial.CommonRSMDTModel.ValueOfTailor);
                    fullAmount += tailorAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = amount };
                    }
                    res.PrimaryBasicPremiumAmount = amount;
                }

                // age loading
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.GoodsCarryingVehiclePartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
                res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadingRate;
                decimal ageLoadingAmount = 0;
                ageLoadingAmount = res.AgeLoadingAmount;
                res.AgeLoadingRate = ageLoadingRate;
                fullAmount += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataOrShortScaleAmount;
                    amount += proRataOrShortScaleAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = ageLoadingAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount += res.AgeLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = amount };
                }

                // voluntary excess
                if (model.GoodsCarryingVehiclePartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.VoluntaryExcessRate, model.GoodsCarryingVehiclePartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                    detailCalculationResult.VoluntaryExcessValue = model.GoodsCarryingVehiclePartial.CommonProperties.VoluntaryExcess;
                    fullAmount -= res.VoluntaryExcessAmount;

                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                }

                //   ncd
                if (model.GoodsCarryingVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.NCDRate, model.GoodsCarryingVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                    fullAmount -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        amount -= res.NoClaimDiscountAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        amount -= ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                }

                //private hire
                if (model.GoodsCarryingVehiclePartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    decimal privateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.PrivateUseDiscount);
                    decimal privateHireAmount = privateHireRate * Math.Abs(fullAmount);
                    res.PrivateHireAmount = privateHireAmount;
                    fullAmount -= privateHireAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(privateHireAmount);
                        res.PrivateHireAmount = proRataOrShortScaleAmount;
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount -= privateHireAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = privateHireAmount, Total = amount };
                    }
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    decimal directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.DirectDiscount);
                    res.DirectDiscountRate = directDiscountRate;
                    decimal directDiscountAmount = Math.Abs(fullAmount) * directDiscountRate;
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullAmount -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.DirectDiscountAmount = proRataOrShortScaleAmount;
                    }
                    else
                    {
                        amount -= directDiscountAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = amount };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }
                res.CalculatedSubTotalA = amount;
                res.SubTotalA = res.CalculatedSubTotalA;


                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }

                if (model.GoodsCarryingVehiclePartial.CommonProperties.ISRecoveryCharge)
                {

                    decimal recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataOrShortScaleAmount;
                        res.SubTotalA += proRataOrShortScaleAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = res.SubTotalA };
                    }
                    else
                    {
                        res.SubTotalA += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.SubTotalA };
                    }
                }
                if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };
                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.HGIShareRate = model.HGIShareRate;
                    res.IsCoinsurance = model.IsCoinsurance;
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.GoodsCarryingVehiclePartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = res.TPLperTon = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perCarryingCapacityInTonEnumValue, Convert.ToDecimal(model.GoodsCarryingVehiclePartial.GoodsCarryingCapacity));
            detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, Total = thirdPartyAmount };
            res.SubTotalB = thirdPartyAmount;
            res.ActualSubTotalB = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = res.TPLperTon = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyAmount, Total = thirdPartyAmount };
            }

            //   ncd
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsComprehensive)
            {
                if (model.GoodsCarryingVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.NCDRate, model.GoodsCarryingVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.ThirdPartyNoClaimDiscountAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                    res.ActualSubTotalB -= ncdAmount;
                }
            }
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver and Passengers

            decimal paToDriverAmount = 0;
            decimal paToHelperAmount = 0;
            decimal paToPassengerAmount = 0;
            decimal paAmount = 0;

            //driver
            paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            res.PAforDriver = paAmount;
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforDriver = paToDriverAmount;
            }
            else
            {
                paToDriverAmount = paAmount;
            }
            detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToDriverAmount, Total = paToDriverAmount };
            //direct discount for DRIVER
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriverAmount;
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsHelperInsured)
            {
                if (model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers != null && model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers;
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue) * (model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue);
                    res.NumberOfHelpers = 1;
                }
                res.PAforHelper = paAmount;
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paAmount);
                    res.PAforHelper = paToHelperAmount;
                }
                else { paToHelperAmount = paAmount; }
                detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel()
                {
                    Amount = paAmount,
                    ProRataOrShortScaleAmount = paToHelperAmount,
                    Total = paToHelperAmount
                };
                //direct discount for HELPER
                if (model.GoodsCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
            }
            res.PAforHelper = paToHelperAmount;
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                paToHelperAmount = res.PAforHelper;
                detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
            }
            //passengers
            res.NumberofPassengers = model.GoodsCarryingVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1 - (res.NumberOfHelpers ?? default(int));
            if (res.NumberofPassengers < 0)
            {
                res.NumberofPassengers = 0;
            }
            paAmount = res.NumberofPassengers *
                    ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            res.PAforPassenger = paAmount;

            if (days != 365)
            {
                paToPassengerAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforPassenger = paToPassengerAmount;
            }
            else { paToPassengerAmount = paAmount; }
            detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToPassengerAmount, Total = paToPassengerAmount };

            //direct discount for PASSENGER
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengerAmount * directDiscountPARate;
                paToPassengerAmount -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengerAmount;
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengerAmount = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriverAmount + paToHelperAmount + paToPassengerAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            //if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
            //{
            //    res.LayupDiscountPA = ((res.SubTotalC * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
            //    res.SubTotalC = res.SubTotalC - res.LayupDiscountPA;
            //    detailCalculationResult.LayupDiscountPA = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPA, ProRataOrShortScaleAmount = res.LayupDiscountPA, Total = res.SubTotalC };
            //}
            #endregion

            detailCalculationResult.RSMDTSumInsuredAmountForDriver = res.RSMDTSuminsuredDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RSMDTSumInsuredAmountForPaidDriver);
            detailCalculationResult.RSMDTSumInsuredAmountForHelper = res.RSMDTSuminsuredHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RSMDTSumInsuredAmountForHelper);
            detailCalculationResult.RSMDTSumInsuredAmountForPassengers = res.RSMDTSuminsuredPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RSMDTSumInsuredAmountForPassengers);

            #region RSMDT
            if (model.GoodsCarryingVehiclePartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                decimal proRataOrShortScaleAmount = 0;

                decimal minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.MinimumRSMDTAmount);
                decimal rsmdRate = res.RSMDTRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RiotAndStrikeAndMdAmount = model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount * rsmdRate;
                if (days != 365)
                {
                    var fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                if (days != 365)
                {
                    var fullRSMDAmount = res.RiotAndStrikeAndMdAmount;
                    proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = proRataOrShortScaleAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = res.RiotAndStrikeAndMdAmount, ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount, Total = res.RiotAndStrikeAndMdAmount };
                }

                res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
                if (minimumRsmdAmount > res.RiotAndStrikeAndMdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                decimal terrorismRate = res.TerrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                decimal terrorismAmount = model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount * terrorismRate;
                res.TerrorismAmount = terrorismAmount;
                if (days != 365)
                {
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(terrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    res.TerrorismAmount = terrorismAmount;
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtPassengerAmount = 0;
                decimal rsmdtAmount = 0;
                ///// Need to add in configuration

                decimal rsmdtToDriverRate = res.RSMDTforDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForDriver * rsmdtToDriverRate;
                res.RSMDTDriver = rsmdtAmount;

                if (days != 365)
                {
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTDriver = rsmdtDriverAmount;
                }
                else { rsmdtDriverAmount = rsmdtAmount; }

                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToDriverRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };

                //helper
                if (model.GoodsCarryingVehiclePartial.CommonProperties.IsHelperInsured)
                {
                    decimal rsmdtToHelperRate = res.RSMDTForHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    if (model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers != null && model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate * (model.GoodsCarryingVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate;
                    }
                    res.RSMDTHelper = rsmdtAmount;

                    if (days != 365)
                    {
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                        res.RSMDTHelper = rsmdtHelperAmount;
                    }
                    else { rsmdtHelperAmount = rsmdtAmount; }

                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToHelperRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                }
                decimal rsmdtToPassengersRate = res.RSMDTforPassengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForPassengers * res.NumberofPassengers * rsmdtToPassengersRate;
                res.RSMDTPassenger = rsmdtAmount;
                if (days != 365)
                {
                    rsmdtPassengerAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTPassenger = rsmdtPassengerAmount;
                }
                else { rsmdtPassengerAmount = rsmdtAmount; }

                detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToPassengersRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };
                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount + rsmdtPassengerAmount;

                if (model.GoodsCarryingVehiclePartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.GoodsCarryingVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.GoodsCarryingVehiclePartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.SumInsuredAmount = res.FullSumInsured = model.FullSumInsured = model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.GoodsCarryingVehiclePartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
            }

            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddDays(days);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            detailCalculationResult.Days = Convert.ToInt32(model.GoodsCarryingVehiclePartial.CommonProperties.Days);
            res.RiskDetails.GoodsCarryingVehicleDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region ConstructionEquipment
    private async Task<PremiumCalculationResultModel> ConstructionEquipment(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new ConstructionEquipmentDetailCalculationResult();
            model.VehicleNumber = model.ConstructionEquipmentPartial.CommonProperties.RegistrationNumber;

            detailCalculationResult.IsProRateOrShortScaleAmount = model.ConstructionEquipmentPartial.CommonProperties.IsProRataOrShortScale;
            res.VehicleCapacity = model.ConstructionEquipmentPartial.GoodsCarryingCapacity;
            detailCalculationResult.NumberOfSeats = model.ConstructionEquipmentPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            var days = Convert.ToInt16(model.ConstructionEquipmentPartial.CommonProperties.Days ?? "0");

            shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            if (!model.ConstructionEquipmentPartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            DateViewModel dateViewModel = new DateViewModel();

            if (model.ConstructionEquipmentPartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ConstructionEquipmentPartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ConstructionEquipmentPartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.ConstructionEquipmentPartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.ConstructionEquipmentPartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.ConstructionEquipmentPartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            int basicPremiumRateEnumValue = (int)ConstructionEquipmentConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)ConstructionEquipmentConfigType.LoadingRate;
            int asPerSeatCapacityEnumValue = (int)ConstructionEquipmentConfigType.AsPerSeatCapacity;
            int perBasicSeatCapacityEnumValue = (int)ConstructionEquipmentConfigType.PerCarryingCapacityInTon;
            int paToPaidDriverEnumValue = (int)ConstructionEquipmentConfigType.PAToPaidDriver;
            int paToHelpersEnumValue = (int)ConstructionEquipmentConfigType.PAToHelper;
            int paToPassengersEnumValue = (int)ConstructionEquipmentConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)ConstructionEquipmentConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)ConstructionEquipmentConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)ConstructionEquipmentConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)ConstructionEquipmentConfigType.RSMDTToHelperRate;
            int rsmdtToPassengersRateEnumValue = (int)ConstructionEquipmentConfigType.RSMDTToPassengersRate;
            int tplHeavyEquipment = (int)ConstructionEquipmentConfigType.TPLForHeavyEquipments;
            int asPerSeatCapacityHeavyEquipment = (int)ConstructionEquipmentConfigType.AsPerSeatCapacityForHeavyEquipments;

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)ConstructionEquipmentConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)ConstructionEquipmentConfigType.GovtLoadingRate;
                asPerSeatCapacityEnumValue = (int)ConstructionEquipmentConfigType.GovtAsPerSeatCapacity;
                perBasicSeatCapacityEnumValue = (int)ConstructionEquipmentConfigType.GovtPerCarryingCapacityInTon;
                paToPaidDriverEnumValue = (int)ConstructionEquipmentConfigType.GovtPAToPaidDriver;
                paToHelpersEnumValue = (int)ConstructionEquipmentConfigType.GovtPAToHelper;
                paToPassengersEnumValue = (int)ConstructionEquipmentConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)ConstructionEquipmentConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)ConstructionEquipmentConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)ConstructionEquipmentConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)ConstructionEquipmentConfigType.GovtRSMDTToHelperRate;
                rsmdtToPassengersRateEnumValue = (int)ConstructionEquipmentConfigType.GovtRSMDTToPassengersRate;
                tplHeavyEquipment = (int)ConstructionEquipmentConfigType.GovtTPLForHeavyEquipments;
                asPerSeatCapacityHeavyEquipment = (int)ConstructionEquipmentConfigType.GovtAsPerSeatCapacityForHeavyEquipments;
            }

            //discard agent for policies with third party only
            if (!model.ConstructionEquipmentPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)ConstructionEquipmentConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);

            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.ConstructionEquipmentPartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.ConstructionEquipmentPartial.CommonProperties.EnterSumInsured)
            {
                model.ConstructionEquipmentPartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount = Math.Round(model.ConstructionEquipmentPartial.CommonProperties.ValueWithoutAccessories + Convert.ToDecimal(model.ConstructionEquipmentPartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount = model.ConstructionEquipmentPartial.CommonProperties.ValueWithoutAccessories + (model.ConstructionEquipmentPartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.ConstructionEquipmentPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicPremiumRate;
                decimal amount = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount;
                decimal fullAmount = amount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                    amount = proRataOrShortScaleAmount;
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    res.PrimaryBasicPremiumAmount = amount;
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = amount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // excess more than 3 
                decimal excessMoreThan__TonsAmount = 0; //model.ConstructionEquipmentPartial.GoodsCarryingCapacity - 3;
                decimal excessMoreThan__TonsMinimumValue = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.MinimumTonExcessValue);
                res.MinimumTon = excessMoreThan__TonsMinimumValue;
                if (model.ConstructionEquipmentPartial.GoodsCarryingCapacity > excessMoreThan__TonsMinimumValue)
                {
                    decimal excessMorethanAmountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.MinimumTonExcessRateAmount);
                    decimal excessTon = model.ConstructionEquipmentPartial.GoodsCarryingCapacity - excessMoreThan__TonsMinimumValue;
                    excessMoreThan__TonsAmount = excessMorethanAmountRate * excessTon;
                    res.AmountPerExcessTons = excessMoreThan__TonsAmount;

                    fullAmount += excessMoreThan__TonsAmount;

                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(excessMoreThan__TonsAmount);
                        res.AmountPerExcessTons = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.PrimaryBasicPremiumAmount = amount;
                    }
                    else
                    {
                        amount += excessMoreThan__TonsAmount;
                        res.PrimaryBasicPremiumAmount = amount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = excessMoreThan__TonsAmount, Total = amount };
                    }
                }

                // as per carrying capacity
                var amountAsPerCarryingCapacity = model.ConstructionEquipmentPartial.IsHeavyEquipment ?
                                                ConstrantValueHelper.GetNewValue(riskSetupModel, asPerSeatCapacityHeavyEquipment) :
                                                ConstrantValueHelper.GetNewValue(riskSetupModel, asPerSeatCapacityEnumValue, model.ConstructionEquipmentPartial.GoodsCarryingCapacity);
                res.AmountPerCarryingCapacity = amountAsPerCarryingCapacity;
                fullAmount -= amountAsPerCarryingCapacity;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(amountAsPerCarryingCapacity);
                    res.AmountPerCarryingCapacity = proRataOrShortScaleAmount;
                    amount -= proRataOrShortScaleAmount;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount -= amountAsPerCarryingCapacity;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = amountAsPerCarryingCapacity, Total = amount };
                }
                res.PrimaryBasicPremiumAmount = amount;

                // tailor
                if (model.ConstructionEquipmentPartial.CommonRSMDTModel.HasTailor && model.ConstructionEquipmentPartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = res.TrailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.ForTrailor);
                    decimal trailorValue = model.ConstructionEquipmentPartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    decimal tailorAmount = trailorRate * trailorValue - 200;
                    res.TrailorAmount = tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.ConstructionEquipmentPartial.CommonRSMDTModel.ValueOfTailor);
                    fullAmount += tailorAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = amount };
                    }
                    res.PrimaryBasicPremiumAmount = amount;
                }

                // age loading
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.ConstructionEquipmentPartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
                res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadingRate;
                res.AgeLoadingRate = ageLoadingRate;
                fullAmount += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataOrShortScaleAmount;
                    amount += proRataOrShortScaleAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount += res.AgeLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = amount };
                }

                // voluntary excess
                if (model.ConstructionEquipmentPartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.VoluntaryExcessRate, model.ConstructionEquipmentPartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                    detailCalculationResult.VoluntaryExcessValue = model.ConstructionEquipmentPartial.CommonProperties.VoluntaryExcess;
                    fullAmount -= res.VoluntaryExcessAmount;

                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                }

                //   ncd
                if (model.ConstructionEquipmentPartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.NCDRate, model.ConstructionEquipmentPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                    fullAmount -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        amount -= res.NoClaimDiscountAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        amount -= ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                }

                //private hire
                if (model.ConstructionEquipmentPartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    decimal privateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.PrivateUseDiscount);
                    decimal privateHireAmount = privateHireRate * Math.Abs(fullAmount);
                    res.PrivateHireAmount = privateHireAmount;
                    fullAmount -= privateHireAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(privateHireAmount);
                        res.PrivateHireAmount = proRataOrShortScaleAmount;
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount -= privateHireAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = privateHireAmount, Total = amount };
                    }
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    decimal directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.DirectDiscount);
                    res.DirectDiscountRate = directDiscountRate;
                    decimal directDiscountAmount = Math.Abs(fullAmount) * directDiscountRate;
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullAmount -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.DirectDiscountAmount = proRataOrShortScaleAmount;
                    }
                    else
                    {
                        amount -= directDiscountAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = amount };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }
                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                res.CalculatedSubTotalA = amount;

                //detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.CalculatedSubTotalA, Total = res.CalculatedSubTotalA };
                //detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };

                res.SubTotalA = res.CalculatedSubTotalA;
                if (model.ConstructionEquipmentPartial.CommonProperties.ISRecoveryCharge)
                {

                    decimal recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataOrShortScaleAmount;
                        res.SubTotalA += proRataOrShortScaleAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = res.SubTotalA };
                    }
                    else
                    {
                        res.SubTotalA += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.SubTotalA };
                    }
                }
                if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };

                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.ConstructionEquipmentPartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = 0;
            if (model.ConstructionEquipmentPartial.IsHeavyEquipment)
            {
                thirdPartyAmount = res.TPLperTon = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, tplHeavyEquipment);
            }
            else
            {
                thirdPartyAmount = res.TPLperTon = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perBasicSeatCapacityEnumValue, Convert.ToDecimal(model.ConstructionEquipmentPartial.GoodsCarryingCapacity));
            }
            detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, Total = thirdPartyAmount };
            res.SubTotalB = thirdPartyAmount;
            res.ActualSubTotalB = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = res.TPLperTon = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyAmount, Total = thirdPartyAmount };
            }

            //   ncd
            if (model.ConstructionEquipmentPartial.CommonProperties.IsComprehensive)
            {
                if (model.ConstructionEquipmentPartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.NCDRate, model.ConstructionEquipmentPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.ThirdPartyNoClaimDiscountAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                    res.ActualSubTotalB -= ncdAmount;
                }
            }
            if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };
            }

            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB.RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            #endregion

            #region Paid Driver and Passengers

            decimal paToDriverAmount = 0;
            decimal paToHelperAmount = 0;
            decimal paToPassengerAmount = 0;
            decimal paAmount = 0;

            //driver
            paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            res.PAforDriver = paAmount;
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforDriver = paToDriverAmount;
            }
            else
            {
                paToDriverAmount = paAmount;
            }
            detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToDriverAmount, Total = paToDriverAmount };

            //direct discount for DRIVER
            if (model.ConstructionEquipmentPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriverAmount;
            if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            if (model.ConstructionEquipmentPartial.CommonProperties.IsHelperInsured)
            {
                if (model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers != null && model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers;
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelpersEnumValue) * (model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelpersEnumValue);
                    res.NumberOfHelpers = 1;
                }
                res.PAforHelper = paAmount;
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paAmount);
                    res.PAforHelper = paToHelperAmount;
                }
                else { paToHelperAmount = paAmount; }
                detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel()
                {
                    Amount = paAmount,
                    ProRataOrShortScaleAmount = paToHelperAmount,
                    Total = paToHelperAmount
                };
                //direct discount for HELPER
                if (model.ConstructionEquipmentPartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
            }
            res.PAforHelper = paToHelperAmount;
            if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                paToHelperAmount = res.PAforHelper;
                detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
            }
            //passengers
            res.NumberofPassengers = model.ConstructionEquipmentPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1 - (res.NumberOfHelpers ?? default(int));
            if (res.NumberofPassengers < 0)
            {
                res.NumberofPassengers = 0;
            }
            paAmount = res.NumberofPassengers *
                    ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            res.PAforPassenger = paAmount;

            if (days != 365)
            {
                paToPassengerAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforPassenger = paToPassengerAmount;
            }
            else { paToPassengerAmount = paAmount; }

            detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel()
            {
                Amount = paAmount,
                ProRataOrShortScaleAmount = paToPassengerAmount,
                Total = paToPassengerAmount
            };
            //direct discount for PASSENGER
            if (model.ConstructionEquipmentPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengerAmount * directDiscountPARate;
                paToPassengerAmount -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengerAmount;
            if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengerAmount = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }

            res.SubTotalC = (paToDriverAmount + paToHelperAmount + paToPassengerAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = ((paToDriverAmount + paToHelperAmount + paToPassengerAmount).RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            //if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
            //{
            //    res.LayupDiscountPA = ((res.SubTotalC * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
            //    res.SubTotalC = res.SubTotalC - res.LayupDiscountPA;
            //    detailCalculationResult.LayupDiscountPA = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPA, ProRataOrShortScaleAmount = res.LayupDiscountPA, Total = res.SubTotalC };

            //}
            #endregion

            detailCalculationResult.RSMDTSumInsuredAmountForDriver = res.RSMDTSuminsuredDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.RSMDTSumInsuredAmountForPaidDriver);
            detailCalculationResult.RSMDTSumInsuredAmountForHelper = res.RSMDTSuminsuredHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.RSMDTSumInsuredAmountForHelper);
            detailCalculationResult.RSMDTSumInsuredAmountForPassengers = res.RSMDTSuminsuredPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.RSMDTSumInsuredAmountForPassengers);

            #region RSMDT
            if (model.ConstructionEquipmentPartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                decimal proRataOrShortScaleAmount = 0;

                decimal minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.MinimumRSMDTAmount);
                decimal rsmdRate = res.RSMDTRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RiotAndStrikeAndMdAmount = model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount * rsmdRate;
                if (days != 365)
                {
                    var fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                if (days != 365)
                {
                    var fullRSMDAmount = res.RiotAndStrikeAndMdAmount;
                    proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = proRataOrShortScaleAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = res.RiotAndStrikeAndMdAmount, ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount, Total = res.RiotAndStrikeAndMdAmount };
                }

                res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
                if (minimumRsmdAmount > res.RiotAndStrikeAndMdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                decimal terrorismRate = res.TerrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                decimal terrorismAmount = model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount * terrorismRate;
                res.TerrorismAmount = terrorismAmount;
                if (days != 365)
                {
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(terrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    res.TerrorismAmount = terrorismAmount;
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtPassengerAmount = 0;
                decimal rsmdtAmount = 0;
                ///// Need to add in configuration

                decimal rsmdtToDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForDriver * rsmdtToDriverRate;
                res.RSMDTDriver = rsmdtAmount;

                if (days != 365)
                {
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTDriver = rsmdtDriverAmount;
                }
                else { rsmdtDriverAmount = rsmdtAmount; }

                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToDriverRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };

                //helper
                if (model.ConstructionEquipmentPartial.CommonProperties.IsHelperInsured)
                {
                    decimal rsmdtToHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    if (model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers != null && model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate * (model.ConstructionEquipmentPartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate;
                    }
                    res.RSMDTHelper = rsmdtAmount;
                    if (days != 365)
                    {
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                        res.RSMDTHelper = rsmdtHelperAmount;
                    }
                    else { rsmdtHelperAmount = rsmdtAmount; }

                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToHelperRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                }

                //passengers
                decimal rsmdtToPassengersRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForPassengers * res.NumberofPassengers * rsmdtToPassengersRate;
                res.RSMDTPassenger = rsmdtAmount;
                if (days != 365)
                {
                    rsmdtPassengerAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTPassenger = rsmdtPassengerAmount;
                }
                else { rsmdtPassengerAmount = rsmdtAmount; }

                detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToPassengersRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };

                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount + rsmdtPassengerAmount;
                if (model.ConstructionEquipmentPartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.ConstructionEquipmentPartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }


                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD.RoundToFourPrecisions() * model.HGIShareRate) / 100;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.ConstructionEquipmentPartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ConstructionEquipmentConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.ConstructionEquipmentPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
                res.HGIShareRate = model.HGIShareRate;
                res.IsCoinsurance = model.IsCoinsurance;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddDays(days);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            detailCalculationResult.Days = Convert.ToInt32(model.ConstructionEquipmentPartial.CommonProperties.Days);
            res.RiskDetails.ConstructionEquipmentDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
#endregion

    #region Tractor
    private async Task<PremiumCalculationResultModel> Tractor(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new TractorDetailCalculationResult();

            detailCalculationResult.IsProRateOrShortScaleAmount = model.TractorPartial.CommonProperties.IsProRataOrShortScale;
            model.VehicleNumber = model.TractorPartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.TractorPartial.HorsePower;

            var days = Convert.ToInt16(model.TractorPartial.CommonProperties.Days ?? "0");
            shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);

            if (!model.TractorPartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            DateViewModel dateViewModel = new DateViewModel();

            if (model.TractorPartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TractorPartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TractorPartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.TractorPartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.TractorPartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.TractorPartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            int basicPremiumRateEnumValue = (int)TractorConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)TractorConfigType.LoadingForOldVehicle;
            int asPerHorsePower = (int)TractorConfigType.TPLPremiumAsPerHorsePower;
            int perBasicHorsePowerEnumValue = (int)TractorConfigType.PerHorsePowerBasicAmount;
            int paToPaidDriverEnumValue = (int)TractorConfigType.PAToPaidDriver;
            int paToHelpersEnumValue = (int)TractorConfigType.PAToHelper;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)TractorConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)TractorConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)TractorConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)TractorConfigType.RSMDTToHelperRate;
            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)TractorConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)TractorConfigType.GovtLoadingForOldVehicle;
                asPerHorsePower = (int)TractorConfigType.GovtTPLPremiumAsPerHorsePower;
                perBasicHorsePowerEnumValue = (int)TractorConfigType.GovtPerHorsePowerBasicAmount;
                paToPaidDriverEnumValue = (int)TractorConfigType.GovtPAToPaidDriver;
                paToHelpersEnumValue = (int)TractorConfigType.GovtPAToHelper;
                riotAndStrikeAndMDAmountRateEnumValue = (int)TractorConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)TractorConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)TractorConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)TractorConfigType.GovtRSMDTToHelperRate;
            }

            //discard agent for policies with third party only
            if (!model.TractorPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)TractorConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.TractorPartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.TractorPartial.CommonProperties.EnterSumInsured)
            {
                model.TractorPartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.TractorPartial.CommonProperties.SumInsuredAmount = Math.Round(model.TractorPartial.CommonProperties.ValueWithoutAccessories + Convert.ToDecimal(model.TractorPartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.TractorPartial.CommonProperties.SumInsuredAmount = model.TractorPartial.CommonProperties.ValueWithoutAccessories + (model.TractorPartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.TractorPartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.TractorPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicPremiumRate;
                decimal amount = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.TractorPartial.CommonProperties.SumInsuredAmount;
                decimal fullAmount = amount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                    amount = proRataOrShortScaleAmount;
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    res.PrimaryBasicPremiumAmount = amount;
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = amount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // tailor
                if (model.TractorPartial.CommonRSMDTModel.HasTailor && model.TractorPartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = res.TrailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.ForTrailor);
                    decimal trailorValue = model.TractorPartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    decimal tailorAmount = trailorRate * trailorValue - 200;
                    res.TrailorAmount = tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.TractorPartial.CommonRSMDTModel.ValueOfTailor);
                    fullAmount += tailorAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = amount };
                    }
                    res.PrimaryBasicPremiumAmount = amount;
                }

                // age loading
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.TractorPartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
                res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadingRate;
                res.AgeLoadingRate = ageLoadingRate;
                fullAmount += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataOrShortScaleAmount;
                    amount += proRataOrShortScaleAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount += res.AgeLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = amount };
                }

                // voluntary excess
                if (model.TractorPartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.VoluntaryExcessRate, model.TractorPartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                    detailCalculationResult.VoluntaryExcessValue = model.TractorPartial.CommonProperties.VoluntaryExcess;
                    fullAmount -= res.VoluntaryExcessAmount;

                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                }

                //   ncd
                if (model.TractorPartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.NCDRate, model.TractorPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                    fullAmount -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        amount -= res.NoClaimDiscountAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        amount -= ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                }

                if (model.TractorPartial.IsOwnUse)
                {
                    res.OwnUseDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.OwnUseDiscount);
                    decimal ownUseDiscountAmount = res.OwnUseDiscountAmount = Math.Abs(fullAmount) * res.OwnUseDiscountRate;
                    fullAmount -= ownUseDiscountAmount;
                    if (days != 365)
                    {
                        res.OwnUseDiscountAmount = GetProRataOrShortScaleAmount(ownUseDiscountAmount);
                        amount -= res.OwnUseDiscountAmount;
                        detailCalculationResult.OwnUseDiscount = new CalculationSubDetailAmountModel() { Rate = res.OwnUseDiscountRate, Amount = ownUseDiscountAmount, ProRataOrShortScaleAmount = res.OwnUseDiscountAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.OwnUseDiscountAmount;
                        detailCalculationResult.OwnUseDiscount = new CalculationSubDetailAmountModel() { Rate = res.OwnUseDiscountRate, Amount = ownUseDiscountAmount, ProRataOrShortScaleAmount = res.OwnUseDiscountAmount, Total = amount };
                    }
                }



                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    decimal directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.DirectDiscount);
                    res.DirectDiscountRate = directDiscountRate;
                    decimal directDiscountAmount = Math.Abs(fullAmount) * directDiscountRate;
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullAmount -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.DirectDiscountAmount = proRataOrShortScaleAmount;
                    }
                    else
                    {
                        amount -= directDiscountAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = amount };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }

                if (model.TractorPartial.CommonProperties.ISRecoveryCharge)
                {
                    decimal recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    fullAmount += recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = amount };
                    }
                }

                res.CalculatedSubTotalA = amount;
                res.SubTotalA = res.CalculatedSubTotalA;

                if (model.TractorPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.TractorPartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };
                }
                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.HGIShareRate = model.HGIShareRate;
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.IsCoinsurance = model.IsCoinsurance;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion

            }

            #region Third Party
            decimal totalTPL = 0;
            decimal thirdPartyAmount = 0;
            model.TractorPartial.CommonProperties.IsThirdParty = true;

            thirdPartyAmount = res.TPLperHorsePower = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perBasicHorsePowerEnumValue, model.TractorPartial.HorsePower);

            if (model.TractorPartial.CommonRSMDTModel == null)
                model.TractorPartial.CommonRSMDTModel = new CommonRSMDTModel();

            if (model.TractorPartial.CommonRSMDTModel.HasTailor && model.TractorPartial.CommonRSMDTModel.ValueOfTailor >= 0)
            {
                thirdPartyAmount += 1000;
                res.TPLperHorsePower = totalTPL = thirdPartyAmount;
                res.TPLperCC = totalTPL = thirdPartyAmount;
            }
            detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, Total = thirdPartyAmount };
            res.SubTotalB = thirdPartyAmount;
            res.ActualSubTotalB = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = res.TPLperHorsePower = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyAmount, Total = thirdPartyAmount };
            }

            //   ncd
            if (model.TractorPartial.CommonProperties.IsComprehensive)
            {
                if (model.TractorPartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.NCDRate, model.TractorPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.ThirdPartyNoClaimDiscountAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                    res.ActualSubTotalB -= ncdAmount;
                }
            }
            if (model.TractorPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.TractorPartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver and Passengers

            decimal paToDriverAmount = 0;
            decimal paToHelperAmount = 0;
            decimal paAmount = 0;

            //driver
            paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            res.PAforDriver = paAmount;
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforDriver = paToDriverAmount;
            }
            else
            {
                paToDriverAmount = paAmount;
            }
            detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToDriverAmount, Total = paToDriverAmount };

            //direct discount for DRIVER
            if (model.TractorPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriverAmount;
            if (model.TractorPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.TractorPartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            if (model.TractorPartial.CommonProperties.IsHelperInsured)
            {
                if (model.TractorPartial.CommonProperties.NumberOfHelpers != null && model.TractorPartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.TractorPartial.CommonProperties.NumberOfHelpers;
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelpersEnumValue) * (model.TractorPartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelpersEnumValue);
                    res.NumberOfHelpers = 1;
                }

                res.PAforHelper = paAmount;
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paAmount);
                    res.PAforHelper = paToHelperAmount;
                }
                else { paToHelperAmount = paAmount; }
                detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToHelperAmount, Total = paToHelperAmount };

                //direct discount for HELPER
                if (model.TractorPartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
                res.PAforHelper = paToHelperAmount;
                if (model.TractorPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.TractorPartial.CommonProperties.LayupDays;
                    res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                    paToHelperAmount = res.PAforHelper;
                    detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
                }
            }

            res.SubTotalC = (paToDriverAmount + paToHelperAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            #endregion

            detailCalculationResult.RSMDTSumInsuredAmountForDriver = res.RSMDTSuminsuredDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.RSMDTSumInsuredAmountForDriver);
            detailCalculationResult.RSMDTSumInsuredAmountForHelper = res.RSMDTSuminsuredHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.RSMDTSumInsuredAmountForHelper);

            #region RSMDT
            if (model.TractorPartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                decimal proRataOrShortScaleAmount = 0;

                decimal minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.MinimumRSMDTAmount);
                decimal rsmdRate = res.RSMDTRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.SumInsuredWithTrailerForRSMDTTractor = model.TractorPartial.CommonProperties.SumInsuredAmount;
                if (model.TractorPartial.CommonRSMDTModel.HasTailor && model.TractorPartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    res.SumInsuredWithTrailerForRSMDTTractor += (model.TractorPartial.CommonRSMDTModel.ValueOfTailor ?? default(decimal));
                }
                res.RiotAndStrikeAndMdAmount = res.SumInsuredWithTrailerForRSMDTTractor * rsmdRate;
                if (days != 365)
                {
                    var fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                if (days != 365)
                {
                    var fullRSMDAmount = res.RiotAndStrikeAndMdAmount;
                    proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = proRataOrShortScaleAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = res.RiotAndStrikeAndMdAmount, ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount, Total = res.RiotAndStrikeAndMdAmount };
                }

                res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
                if (minimumRsmdAmount > res.RiotAndStrikeAndMdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount;
                    isMinRSMDAmount = true;
                }

                decimal terrorismRate = res.TerrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                decimal terrorismAmount = res.SumInsuredWithTrailerForRSMDTTractor * terrorismRate;
                res.TerrorismAmount = terrorismAmount;
                if (days != 365)
                {
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(terrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    res.TerrorismAmount = terrorismAmount;
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtAmount = 0;
                ///// Need to add in configuration

                decimal rsmdtToDriverRate = res.RSMDTforDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForDriver * rsmdtToDriverRate;
                res.RSMDTDriver = rsmdtAmount;

                if (days != 365)
                {
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTDriver = rsmdtDriverAmount;
                }
                else { rsmdtDriverAmount = rsmdtAmount; }

                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToDriverRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };

                //helper
                if (model.TractorPartial.CommonProperties.IsHelperInsured)
                {
                    decimal rsmdtToHelperRate = res.RSMDTForHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    if (model.TractorPartial.CommonProperties.NumberOfHelpers != null && model.TractorPartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate * (model.TractorPartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate;
                    }
                    res.RSMDTHelper = rsmdtAmount;

                    if (days != 365)
                    {
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                        res.RSMDTHelper = rsmdtHelperAmount;
                    }
                    else { rsmdtHelperAmount = rsmdtAmount; }

                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToHelperRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                }
                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount;
                if (model.TractorPartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.TractorPartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;

                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.TractorPartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.TractorPartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TractorConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.SumInsuredAmount = model.TractorPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount + (model.TractorPartial.CommonRSMDTModel?.ValueOfTailor ?? default(decimal));
            res.SumInsuredAmount = res.FullSumInsured = model.FullSumInsured = model.TractorPartial.CommonProperties.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.TractorPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddDays(days);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            detailCalculationResult.Days = Convert.ToInt32(model.TractorPartial.CommonProperties.Days);
            res.RiskDetails.TractorDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region Tanker
    private async Task<PremiumCalculationResultModel> Tanker(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new TankerDetailCalculationResult();
            detailCalculationResult.IsProRateOrShortScaleAmount = model.TankerPartial.CommonProperties.IsProRataOrShortScale;
            model.VehicleNumber = model.TankerPartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.TankerPartial.GoodsCarryingCapacity;
            detailCalculationResult.NumberOfSeats = model.TankerPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            var days = Convert.ToInt16(model.TankerPartial.CommonProperties.Days ?? "0");
            shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            if (!model.TankerPartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            DateViewModel dateViewModel = new DateViewModel();

            if (model.TankerPartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TankerPartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TankerPartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.TankerPartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.TankerPartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.TankerPartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            int basicPremiumRateEnumValue = (int)TankerConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)TankerConfigType.LoadingRate;
            int asPerSeatCapacityEnumValue = (int)TankerConfigType.AsPerSeatCapacity;
            int perBasicSeatCapacityEnumValue = (int)TankerConfigType.PerCarryingCapacityInTon;
            int paToPaidDriverEnumValue = (int)TankerConfigType.PAToPaidDriver;
            int paToHelperEnumValue = (int)TankerConfigType.PAToHelper;
            int paToPassengersEnumValue = (int)TankerConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)TankerConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)TankerConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)TankerConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)TankerConfigType.RSMDTToHelperRate;
            int rsmdtToPassengersRateEnumValue = (int)TankerConfigType.RSMDTToPassengersRate;

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)TankerConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)TankerConfigType.GovtLoadingRate;
                asPerSeatCapacityEnumValue = (int)TankerConfigType.GovtAsPerSeatCapacity;
                perBasicSeatCapacityEnumValue = (int)TankerConfigType.GovtPerCarryingCapacityInTon;
                paToPaidDriverEnumValue = (int)TankerConfigType.GovtPAToPaidDriver;
                paToHelperEnumValue = (int)TankerConfigType.GovtPAToHelper;
                paToPassengersEnumValue = (int)TankerConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)TankerConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)TankerConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)TankerConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)TankerConfigType.GovtRSMDTToHelperRate;
                rsmdtToPassengersRateEnumValue = (int)TankerConfigType.GovtRSMDTToPassengersRate;
            }

            //discard agent for policies with third party only
            if (!model.TankerPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)TankerConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);

            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.TankerPartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.TankerPartial.CommonProperties.EnterSumInsured)
            {
                model.TankerPartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.TankerPartial.CommonProperties.SumInsuredAmount = Math.Round(model.TankerPartial.CommonProperties.ValueWithoutAccessories + Convert.ToDecimal(model.TankerPartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.TankerPartial.CommonProperties.SumInsuredAmount = model.TankerPartial.CommonProperties.ValueWithoutAccessories + (model.TankerPartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.TankerPartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.TankerPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicPremiumRate;
                decimal amount = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.TankerPartial.CommonProperties.SumInsuredAmount;
                decimal fullAmount = amount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                    amount = proRataOrShortScaleAmount;
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    res.PrimaryBasicPremiumAmount = amount;
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = amount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // excess more than 3 
                decimal excessMoreThan__TonsAmount = 0; //model.TankerPartial.GoodsCarryingCapacity - 3;
                decimal excessMoreThan__TonsMinimumValue = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.MinimumTonExcessValue);
                res.MinimumTon = excessMoreThan__TonsMinimumValue;
                if (model.TankerPartial.GoodsCarryingCapacity > excessMoreThan__TonsMinimumValue)
                {
                    decimal excessMorethanAmountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.MinimumTonExcessRateAmount);
                    decimal excessTon = model.TankerPartial.GoodsCarryingCapacity - excessMoreThan__TonsMinimumValue;
                    excessMoreThan__TonsAmount = excessMorethanAmountRate * excessTon;
                    res.AmountPerExcessTons = excessMoreThan__TonsAmount;

                    fullAmount += excessMoreThan__TonsAmount;

                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(excessMoreThan__TonsAmount);
                        res.AmountPerExcessTons = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.PrimaryBasicPremiumAmount = amount;
                    }
                    else
                    {
                        amount += excessMoreThan__TonsAmount;
                        res.PrimaryBasicPremiumAmount = amount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = excessMoreThan__TonsAmount, Total = amount };
                    }
                }

                // as per carrying capacity
                var amountAsPerCarryingCapacity = ConstrantValueHelper.GetNewValue(riskSetupModel, asPerSeatCapacityEnumValue, model.TankerPartial.GoodsCarryingCapacity);
                res.AmountPerCarryingCapacity = amountAsPerCarryingCapacity;
                fullAmount -= amountAsPerCarryingCapacity;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(amountAsPerCarryingCapacity);
                    res.AmountPerCarryingCapacity = proRataOrShortScaleAmount;
                    amount -= proRataOrShortScaleAmount;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount -= amountAsPerCarryingCapacity;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = amountAsPerCarryingCapacity, Total = amount };
                }
                res.PrimaryBasicPremiumAmount = amount;

                // tailor
                if (model.TankerPartial.CommonRSMDTModel.HasTailor && model.TankerPartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = res.TrailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.ForTrailor);
                    decimal trailorValue = model.TankerPartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    decimal tailorAmount = trailorRate * trailorValue - 200;
                    res.TrailorAmount = tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.TankerPartial.CommonRSMDTModel.ValueOfTailor);
                    fullAmount += tailorAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = amount };
                    }
                    res.PrimaryBasicPremiumAmount = amount;
                }

                // age loading
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.TankerPartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
                res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadingRate;
                res.AgeLoadingRate = ageLoadingRate;
                fullAmount += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataOrShortScaleAmount;
                    amount += proRataOrShortScaleAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount += res.AgeLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = amount };
                }

                // voluntary excess
                if (model.TankerPartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.VoluntaryExcessRate, model.TankerPartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                    detailCalculationResult.VoluntaryExcessValue = model.TankerPartial.CommonProperties.VoluntaryExcess;
                    fullAmount -= res.VoluntaryExcessAmount;

                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                }

                //   ncd
                if (model.TankerPartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.NCDRate, model.TankerPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                    fullAmount -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        amount -= res.NoClaimDiscountAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        amount -= ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                }

                //private hire
                if (model.TankerPartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    decimal privateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.PrivateUseDiscount);
                    decimal privateHireAmount = privateHireRate * Math.Abs(fullAmount);
                    res.PrivateHireAmount = privateHireAmount;
                    fullAmount -= privateHireAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(privateHireAmount);
                        res.PrivateHireAmount = proRataOrShortScaleAmount;
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount -= privateHireAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = privateHireAmount, Total = amount };
                    }
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    decimal directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.DirectDiscount);
                    res.DirectDiscountRate = directDiscountRate;
                    decimal directDiscountAmount = Math.Abs(fullAmount) * directDiscountRate;
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullAmount -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.DirectDiscountAmount = proRataOrShortScaleAmount;
                    }
                    else
                    {
                        amount -= directDiscountAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = amount };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }

                if (model.TankerPartial.CommonProperties.ISRecoveryCharge)
                {

                    decimal recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    fullAmount += recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = amount };
                    }
                }
                res.CalculatedSubTotalA = amount;
                res.SubTotalA = res.CalculatedSubTotalA;
                if (model.TankerPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }

                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.HGIShareRate = model.HGIShareRate;
                    res.IsCoinsurance = model.IsCoinsurance;
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.TankerPartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = res.TPLperTon = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perBasicSeatCapacityEnumValue, Convert.ToDecimal(model.TankerPartial.GoodsCarryingCapacity));
            detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, Total = thirdPartyAmount };
            res.SubTotalB = thirdPartyAmount;
            res.ActualSubTotalB = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = res.TPLperTon = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyAmount, Total = thirdPartyAmount };
            }

            //   ncd
            if (model.TankerPartial.CommonProperties.IsComprehensive)
            {
                if (model.TankerPartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.NCDRate, model.TankerPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.ThirdPartyNoClaimDiscountAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                    res.ActualSubTotalB -= ncdAmount;
                }
            }
            if (model.TankerPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver and Passengers

            decimal paToDriverAmount = 0;
            decimal paToHelperAmount = 0;
            decimal paToPassengerAmount = 0;
            decimal paAmount = 0;

            //driver
            paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            res.PAforDriver = paAmount;
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforDriver = paToDriverAmount;
            }
            else
            {
                paToDriverAmount = paAmount;
            }
            detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToDriverAmount, Total = paToDriverAmount };

            //direct discount for DRIVER
            if (model.TankerPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriverAmount;
            if (model.TankerPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            if (model.TankerPartial.CommonProperties.IsHelperInsured)
            {
                if (model.TankerPartial.CommonProperties.NumberOfHelpers != null && model.TankerPartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.TankerPartial.CommonProperties.NumberOfHelpers;
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue) * (model.TankerPartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue);
                    res.NumberOfHelpers = 1;
                }
                res.PAforHelper = paAmount;
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paAmount);
                    res.PAforHelper = paToHelperAmount;
                }
                else { paToHelperAmount = paAmount; }
                detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToHelperAmount, Total = paToHelperAmount };
                //direct discount for HELPER
                if (model.TankerPartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
                res.PAforHelper = paToHelperAmount;
                if (model.TankerPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                    res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                    paToHelperAmount = res.PAforHelper;
                    detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
                }
            }
            //passengers
            res.NumberofPassengers = model.TankerPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1 - (res.NumberOfHelpers ?? default(int));
            if (res.NumberofPassengers < 0)
            {
                res.NumberofPassengers = 0;
            }
            paAmount = res.NumberofPassengers *
                    ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            res.PAforPassenger = paAmount;

            if (days != 365)
            {
                paToPassengerAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforPassenger = paToPassengerAmount;
            }
            else { paToPassengerAmount = paAmount; }
            detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToPassengerAmount, Total = paToPassengerAmount };

            //direct discount for PASSENGER
            if (model.TankerPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengerAmount * directDiscountPARate;
                paToPassengerAmount -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengerAmount;
            if (model.TankerPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengerAmount = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriverAmount + paToHelperAmount + paToPassengerAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            #endregion

            detailCalculationResult.RSMDTSumInsuredAmountForDriver = res.RSMDTSuminsuredDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.RSMDTSumInsuredAmountForPaidDriver);
            detailCalculationResult.RSMDTSumInsuredAmountForHelper = res.RSMDTSuminsuredHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.RSMDTSumInsuredAmountForHelper);
            detailCalculationResult.RSMDTSumInsuredAmountForPassengers = res.RSMDTSuminsuredPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.RSMDTSumInsuredAmountForPassengers);

            #region RSMDT
            if (model.TankerPartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                decimal proRataOrShortScaleAmount = 0;

                decimal minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.MinimumRSMDTAmount);
                decimal rsmdRate = res.RSMDTRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RiotAndStrikeAndMdAmount = model.TankerPartial.CommonProperties.SumInsuredAmount * rsmdRate;
                if (days != 365)
                {
                    var fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                if (days != 365)
                {
                    var fullRSMDAmount = res.RiotAndStrikeAndMdAmount;
                    proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = proRataOrShortScaleAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = res.RiotAndStrikeAndMdAmount, ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount, Total = res.RiotAndStrikeAndMdAmount };
                }

                res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
                if (minimumRsmdAmount > res.RiotAndStrikeAndMdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                }

                decimal terrorismRate = res.TerrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                decimal terrorismAmount = model.TankerPartial.CommonProperties.SumInsuredAmount * terrorismRate;
                res.TerrorismAmount = terrorismAmount;
                if (days != 365)
                {
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(terrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    res.TerrorismAmount = terrorismAmount;
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtPassengerAmount = 0;
                decimal rsmdtAmount = 0;
                ///// Need to add in configuration

                decimal rsmdtToDriverRate = res.RSMDTforDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForDriver * rsmdtToDriverRate;
                res.RSMDTDriver = rsmdtAmount;

                if (days != 365)
                {
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTDriver = rsmdtDriverAmount;
                }
                else { rsmdtDriverAmount = rsmdtAmount; }

                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToDriverRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };

                //helper
                if (model.TankerPartial.CommonProperties.IsHelperInsured)
                {
                    decimal rsmdtToHelperRate = res.RSMDTForHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    if (model.TankerPartial.CommonProperties.NumberOfHelpers != null && model.TankerPartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate * (model.TankerPartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate;
                    }
                    res.RSMDTHelper = rsmdtAmount;

                    if (days != 365)
                    {
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                        res.RSMDTHelper = rsmdtHelperAmount;
                    }
                    else { rsmdtHelperAmount = rsmdtAmount; }

                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToHelperRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                }
                decimal rsmdtToPassengersRate = res.RSMDTforPassengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForPassengers * res.NumberofPassengers * rsmdtToPassengersRate;
                res.RSMDTPassenger = rsmdtAmount;
                if (days != 365)
                {
                    rsmdtPassengerAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTPassenger = rsmdtPassengerAmount;
                }
                else { rsmdtPassengerAmount = rsmdtAmount; }

                detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToPassengersRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtPassengerAmount, Total = rsmdtPassengerAmount };


                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount + rsmdtPassengerAmount;
                if (model.TankerPartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.TankerPartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.TankerPartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.TankerPartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TankerConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.SumInsuredAmount = res.FullSumInsured = model.FullSumInsured = model.TankerPartial.CommonProperties.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.TankerPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddDays(days);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            detailCalculationResult.Days = Convert.ToInt32(model.TankerPartial.CommonProperties.Days);
            res.RiskDetails.TankerDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region Tempo
    private async Task<PremiumCalculationResultModel> Tempo(List<CalculationConfigurationViewModel> riskSetupModel, List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new TempoDetailCalculationResult();
            model.VehicleNumber = model.TempoPartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.TempoPartial.CommonProperties.CubicCapacity;
            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.TempoPartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TempoPartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.TempoPartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.TempoPartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.TempoPartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.CC = model.TempoPartial.CommonProperties.CubicCapacity;
            detailCalculationResult.NumberOfSeats = model.TempoPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.TempoPartial.CommonProperties.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }

            model.TempoPartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.TempoPartial.CommonProperties.Days);
            if (!model.TempoPartial.CommonProperties.IsComprehensive)
            {
                days = 365;
            }
            if (!model.TempoPartial.CommonProperties.IsProRataOrShortScale)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }

            int basicPremiumRateEnumValue = (int)TempoConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)TempoConfigType.LoadingRate;
            int paToPaidDriverEnumValue = (int)TempoConfigType.PAToPaidDriver;
            int paToPassengersEnumValue = (int)TempoConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)TempoConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)TempoConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)TempoConfigType.RSMDTToPaidDriverRate;
            int rsmdtToPassengersRateEnumValue = (int)TempoConfigType.RSMDTToPassengersRate;
            int tplLoadingPerCCBasicAmountEnumValue = (int)TaxiConfigType.PerCCBasicAmount;

            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)TempoConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)TempoConfigType.GovtLoadingRate;
                paToPaidDriverEnumValue = (int)TempoConfigType.GovtPAToPaidDriver;
                paToPassengersEnumValue = (int)TempoConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)TempoConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)TempoConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)TempoConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToPassengersRateEnumValue = (int)TempoConfigType.GovtRSMDTToPassengersRate;
                tplLoadingPerCCBasicAmountEnumValue = (int)TempoConfigType.GovtPerCCBasicAmount;
            }

            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;
            var rsmdtSumInsuredForPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassenger;
            //discard agent for policies with third party only
            if (!model.TempoPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)TempoConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            #region CalculateSumInsured
            //price of Tempo model
            decimal currentMarketPrice = Convert.ToDecimal(model.TempoPartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.TempoPartial.CommonProperties.EnterSumInsured)
            {
                model.TempoPartial.CommonProperties.ValueWithoutAccessories = (currentMarketPrice - (currentMarketPrice * rateOfDepreciation));
                model.TempoPartial.CommonProperties.SumInsuredAmount = Math.Round(model.TempoPartial.CommonProperties.ValueWithoutAccessories + (model.TempoPartial.CommonProperties.ValueOfAccessories ?? default(decimal)), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.TempoPartial.CommonProperties.SumInsuredAmount = model.TempoPartial.CommonProperties.ValueWithoutAccessories + (model.TempoPartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.TempoPartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.TempoPartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Own damage Premium
                var basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicRate;
                var fullBasicTotal = res.PrimaryBasicPremiumAmount = model.TempoPartial.CommonProperties.SumInsuredAmount * basicRate;

                if (days != 365)
                {
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                var ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.TempoPartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingRate = ageLoadingRate;
                res.AgeLoadingAmount = ageLoadingRate * fullBasicTotal;
                fullBasicTotal += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataAgeLoadingAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataAgeLoadingAmount;
                    decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataAgeLoadingAmount, Total = proRataTotal };
                }
                else
                {
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = fullBasicTotal };
                }

                if (model.TempoPartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.VoluntaryExcessRate, model.TempoPartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = res.VoluntaryExcessRate * fullBasicTotal;
                    fullBasicTotal -= res.VoluntaryExcessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = fullBasicTotal };
                    }
                }

                //   ncd
                if (model.TempoPartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.NCDRate, model.TempoPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullBasicTotal) * res.NoClaimDiscountRate;
                    fullBasicTotal -= ncdAmount;
                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(res.NoClaimDiscountAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = fullBasicTotal };
                    }
                }

                if (model.TempoPartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    var PrivateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.PrivateUseDiscount);
                    var privateTaxiAmount = PrivateHireRate * fullBasicTotal;
                    res.PrivateHireAmount = privateTaxiAmount;
                    fullBasicTotal -= privateTaxiAmount;
                    if (days != 365)
                    {
                        res.PrivateHireAmount = GetProRataOrShortScaleAmount(res.PrivateHireAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = PrivateHireRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = res.PrivateHireAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = PrivateHireRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = res.PrivateHireAmount, Total = fullBasicTotal };
                    }
                }
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(fullBasicTotal);
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullBasicTotal -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataDDAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        res.DirectDiscountAmount = proRataDDAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataDDAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = fullBasicTotal };
                    }
                }

                //Recovery charge 
                if (model.TempoPartial.CommonProperties.ISRecoveryCharge)
                {
                    var recoveryCharge = res.ActualRecoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.RecoveryCharge);
                    res.RecoveryCharge = recoveryCharge;
                    fullBasicTotal += recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataRCAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = fullBasicTotal };
                    }
                }

                if (days != 365)
                {
                    res.CalculatedSubTotalA = GetProRataOrShortScaleAmount(fullBasicTotal);
                }
                else
                {
                    res.CalculatedSubTotalA = fullBasicTotal;
                }

                res.SubTotalA = res.CalculatedSubTotalA;

                if (model.TempoPartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.TempoPartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA.RoundToFourPrecisions() * model.HGIShareRate) / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }

                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.TempoPartial.CommonProperties.IsThirdParty = true;
            var BasicAsPerCubicCapacity = res.TPLperCC = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, tplLoadingPerCCBasicAmountEnumValue, model.TempoPartial.CommonProperties.CubicCapacity);
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(BasicAsPerCubicCapacity);
                res.SubTotalB = res.TPLperCC = proRataOrShortScaleAmount;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }
            else
            {
                res.SubTotalB = BasicAsPerCubicCapacity;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }

            //   ncd
            if (model.TempoPartial.CommonProperties.IsComprehensive)
            {
                if (model.TempoPartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.NCDRate, model.TempoPartial.CommonProperties.NCDYears);
                    decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;

                }
            }
            if (model.TempoPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.TempoPartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB.RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            #endregion

            #region Paid Driver And Passengers
            res.NumberofPassengers = model.TempoPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1;
            decimal paToDriver = 0;
            decimal paToPassengers = 0;


            //driver
            paToDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            if (days != 365)
            {
                decimal fullPaToDriver = paToDriver;
                paToDriver = GetProRataOrShortScaleAmount(paToDriver);
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = paToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            //direct discount for DRIVER
            if (model.TempoPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriver * directDiscountPARate;
                paToDriver -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriver;
            if (model.TempoPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.TempoPartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriver = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //passenger
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengers = res.PAforPassenger = paForIndividualPassenger * (res.NumberofPassengers);
            if (days != 365)
            {
                decimal fullPaToPassengers = paToPassengers;
                paToPassengers = GetProRataOrShortScaleAmount(paToPassengers);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = paToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            //direct discount for PASSENGER
            if (model.TempoPartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengers * directDiscountPARate;
                paToPassengers -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengers;
            if (model.TempoPartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.TempoPartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengers = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriver + paToPassengers).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = ((paToDriver + paToPassengers).RoundToFourPrecisions() * model.HGIShareRate) / 100;
            }
            #endregion

            if (model.TempoPartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                #region RSMDT
                //get the rate from db
                var minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.MinimumRSMDTAmount);
                if (days != 365)
                {
                    decimal fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = rsmdRate;
                var rsmdAmount = model.TempoPartial.CommonProperties.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
                if (days != 365)
                {
                    decimal fullRsmdAmount = rsmdAmount;
                    rsmdAmount = GetProRataOrShortScaleAmount(fullRsmdAmount);
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = fullRsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };

                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = rsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };
                }

                res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = rsmdAmount;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount;
                    isMinRSMDAmount = true;
                }

                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                res.TerrorismAmount = model.TempoPartial.CommonProperties.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
                if (days != 365)
                {
                    decimal fullTerrorismAmount = res.TerrorismAmount;
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = fullTerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = res.TerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriver = 0;
                decimal rsmdtPassenger = 0;


                var driverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                rsmdtDriver = rsmdtSumInsuredForDriver * driverRate;//PA amount must come from input field and rate from db
                res.RSMDTDriver = rsmdtDriver;
                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;

                if (days != 365)
                {
                    decimal fullPaToDriver = rsmdtDriver;
                    rsmdtDriver = GetProRataOrShortScaleAmount(fullPaToDriver);
                    res.RSMDTDriver = rsmdtDriver;
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = rsmdtDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }

                var passengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                rsmdtPassenger = rsmdtSumInsuredForPassenger *
                                    (model.TempoPartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1) * passengerRate;//PA amount must come from input field and rate from db
                res.RSMDTPassenger = rsmdtPassenger;
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassenger;

                if (days != 365)
                {
                    decimal fullPaToPassengers = rsmdtPassenger;
                    rsmdtPassenger = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    res.RSMDTPassenger = rsmdtPassenger;
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = rsmdtPassenger, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }

                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriver + rsmdtPassenger;
                if (model.TempoPartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.TempoPartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;

                }
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD.RoundToFourPrecisions() * model.HGIShareRate) / 100;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                #endregion
            }

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;

            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.TempoPartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.TempoPartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)TempoConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.FullSumInsured = model.FullSumInsured = model.TempoPartial.CommonProperties.SumInsuredAmount;
            res.SumInsuredAmount = model.FullSumInsured;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.TempoPartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
                res.HGIShareRate = model.HGIShareRate;
                res.IsCoinsurance = model.IsCoinsurance;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            res.RiskDetails.TempoDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region ElectricCommercialVehicle
    private async Task<PremiumCalculationResultModel> EletricCommercialVechicle(List<CalculationConfigurationViewModel> riskSetupModel, List<GlobalConfigurationViewModel> globalriskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new ElectricCommercialVehicleCalculationResult();
            model.VehicleNumber = model.ElectricCommercialVehiclePartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.ElectricCommercialVehiclePartial.CommonProperties.CubicCapacity;
            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.ElectricCommercialVehiclePartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ElectricCommercialVehiclePartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ElectricCommercialVehiclePartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.ElectricCommercialVehiclePartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.ElectricCommercialVehiclePartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.CC = model.ElectricCommercialVehiclePartial.CommonProperties.CubicCapacity;
            detailCalculationResult.NumberOfSeats = model.ElectricCommercialVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.ElectricCommercialVehiclePartial.CommonProperties.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }

            model.ElectricCommercialVehiclePartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.ElectricCommercialVehiclePartial.CommonProperties.Days);
            if (!model.ElectricCommercialVehiclePartial.CommonProperties.IsComprehensive)
            {
                days = 365;
            }
            if (!model.ElectricCommercialVehiclePartial.CommonProperties.IsProRataOrShortScale)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            }

            int basicPremiumRateEnumValue = (int)ElectricCommercialVehicleConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)ElectricCommercialVehicleConfigType.LoadingRate;
            int paToPaidDriverEnumValue = (int)ElectricCommercialVehicleConfigType.PAToPaidDriver;
            int paToPassengersEnumValue = (int)ElectricCommercialVehicleConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricCommercialVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)ElectricCommercialVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)ElectricCommercialVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToPassengersRateEnumValue = (int)ElectricCommercialVehicleConfigType.RSMDTToPassengersRate;
            int tplLoadingPerCCBasicAmountEnumValue = (int)TaxiConfigType.PerCCBasicAmount;
            int directDiscountPARateEnumValue = (int)ElectricCommercialVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtLoadingRate;
                paToPaidDriverEnumValue = (int)ElectricCommercialVehicleConfigType.GovtPAToPaidDriver;
                paToPassengersEnumValue = (int)ElectricCommercialVehicleConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToPassengersRateEnumValue = (int)ElectricCommercialVehicleConfigType.GovtRSMDTToPassengersRate;
                tplLoadingPerCCBasicAmountEnumValue = (int)ElectricCommercialVehicleConfigType.GovtPerCCBasicAmount;
            }

            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.RSMDTSumInsuredAmountForPaidDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;
            var rsmdtSumInsuredForPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassenger;

            //discard agent for policies with third party only
            if (!model.ElectricCommercialVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion

            #region CalculateSumInsured
            //price of Tempo model
            decimal currentMarketPrice = Convert.ToDecimal(model.ElectricCommercialVehiclePartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.ElectricCommercialVehiclePartial.CommonProperties.EnterSumInsured)
            {
                model.ElectricCommercialVehiclePartial.CommonProperties.ValueWithoutAccessories = (currentMarketPrice - (currentMarketPrice * rateOfDepreciation));
                model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount = Math.Round(model.ElectricCommercialVehiclePartial.CommonProperties.ValueWithoutAccessories + (model.ElectricCommercialVehiclePartial.CommonProperties.ValueOfAccessories ?? default(decimal)), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount = model.ElectricCommercialVehiclePartial.CommonProperties.ValueWithoutAccessories + (model.ElectricCommercialVehiclePartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Own damage Premium
                var basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicRate;
                var fullBasicTotal = res.PrimaryBasicPremiumAmount = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount * basicRate;

                if (days != 365)
                {
                    res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Rate = res.BasicPremiumRate, Amount = fullBasicTotal, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                var ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.ElectricCommercialVehiclePartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                res.AgeLoadingRate = ageLoadingRate;
                res.AgeLoadingAmount = ageLoadingRate * fullBasicTotal;
                fullBasicTotal += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataAgeLoadingAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataAgeLoadingAmount;
                    decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = proRataAgeLoadingAmount, Total = proRataTotal };
                }
                else
                {
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = fullBasicTotal };
                }

                if (model.ElectricCommercialVehiclePartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.VoluntaryExcessRate, model.ElectricCommercialVehiclePartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = res.VoluntaryExcessRate * fullBasicTotal;
                    fullBasicTotal -= res.VoluntaryExcessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = fullBasicTotal };
                    }
                }

                //   ncd
                if (model.ElectricCommercialVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.NCDRate, model.ElectricCommercialVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullBasicTotal) * res.NoClaimDiscountRate;
                    fullBasicTotal -= ncdAmount;
                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(res.NoClaimDiscountAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = fullBasicTotal };
                    }
                }

                if (model.ElectricCommercialVehiclePartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    decimal privateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.PrivateUseDiscount);
                    decimal privateHireAmount = privateHireRate * Math.Abs(fullBasicTotal);
                    res.PrivateHireAmount = privateHireAmount;
                    fullBasicTotal -= privateHireAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(privateHireAmount);
                        res.PrivateHireAmount = proRataOrShortScaleAmount;
                        fullBasicTotal -= proRataOrShortScaleAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = fullBasicTotal };
                    }
                    else
                    {
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = privateHireAmount, Total = fullBasicTotal };
                    }
                }

                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(fullBasicTotal);
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullBasicTotal -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataDDAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        res.DirectDiscountAmount = proRataDDAmount;
                        decimal proRataTotal = GetProRataOrShortScaleAmount(fullBasicTotal);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataDDAmount, Total = proRataTotal };
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = fullBasicTotal };
                    }
                }

                if (days != 365)
                {
                    res.CalculatedSubTotalA = GetProRataOrShortScaleAmount(fullBasicTotal);
                }
                else
                {
                    res.CalculatedSubTotalA = fullBasicTotal;
                }

                //compare with minimum own premium amount
                //var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.MinimumBasicPremiumAmount);
                //res.MinbasicPremium = minimumBasicPremiumAmount;
                //if (days != 365)
                //{
                //    var fullMinBasicPremium = minimumBasicPremiumAmount;
                //    minimumBasicPremiumAmount = GetProRataOrShortScaleAmount(minimumBasicPremiumAmount);
                //    res.MinbasicPremium = minimumBasicPremiumAmount;
                //    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = fullMinBasicPremium, ProRataOrShortScaleAmount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                //}
                //else
                //{
                //    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                //}

                //res.SubTotalA = minimumBasicPremiumAmount;
                //if (res.CalculatedSubTotalA > minimumBasicPremiumAmount)
                res.SubTotalA = res.CalculatedSubTotalA;


                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }

                //Recovery charge 
                if (model.ElectricCommercialVehiclePartial.CommonProperties.ISRecoveryCharge)
                {
                    var recoveryCharge = res.ActualRecoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.RecoveryCharge);
                    res.RecoveryCharge = recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataRCAmount;
                        res.SubTotalA += proRataRCAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = res.SubTotalA };
                    }
                    else
                    {
                        res.SubTotalA += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.SubTotalA };
                    }
                }
                if (model.ElectricCommercialVehiclePartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.ElectricCommercialVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };

                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();

                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.ElectricCommercialVehiclePartial.CommonProperties.IsThirdParty = true;
            var BasicAsPerCubicCapacity = res.TPLperCC = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, tplLoadingPerCCBasicAmountEnumValue, model.ElectricCommercialVehiclePartial.CommonProperties.CubicCapacity);
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(BasicAsPerCubicCapacity);
                res.SubTotalB = res.TPLperCC = proRataOrShortScaleAmount;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }
            else
            {
                res.SubTotalB = BasicAsPerCubicCapacity;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = BasicAsPerCubicCapacity, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }

            //   ncd
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsComprehensive)
            {
                if (model.ElectricCommercialVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.NCDRate, model.ElectricCommercialVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;


                }
            }
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.ElectricCommercialVehiclePartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            #endregion

            #region Paid Driver And Passengers
            res.NumberofPassengers = model.ElectricCommercialVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1;
            decimal paToDriver = 0;
            decimal paToPassengers = 0;


            //driver
            paToDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);

            if (days != 365)
            {
                decimal fullPaToDriver = paToDriver;
                paToDriver = GetProRataOrShortScaleAmount(paToDriver);
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            else
            {
                detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel { Rate = paToDriver, Amount = paToDriver, ProRataOrShortScaleAmount = paToDriver, Total = paToDriver };
            }
            //direct discount for DRIVER
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriver * directDiscountPARate;
                paToDriver -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriver;
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.ElectricCommercialVehiclePartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriver = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //passenger
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengers = res.PAforPassenger = paForIndividualPassenger * (res.NumberofPassengers);
            if (days != 365)
            {
                decimal fullPaToPassengers = paToPassengers;
                paToPassengers = GetProRataOrShortScaleAmount(paToPassengers);
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            else
            {
                detailCalculationResult.PAForPassenger = new CalculationSubDetailAmountModel { Rate = paForIndividualPassenger, Amount = paToPassengers, ProRataOrShortScaleAmount = paToPassengers, Total = paToPassengers };
            }
            //direct discount for PASSENGER
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPAPassenger = paToPassengers * directDiscountPARate;
                paToPassengers -= res.DirectDiscountAmountPAPassenger;
                detailCalculationResult.DirectDiscountForPAPassenger = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPAPassenger,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPAPassenger,
                    Total = res.DirectDiscountAmountPAPassenger
                };
            }
            res.PAforPassenger = paToPassengers;
            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAPassenger = ((res.PAforPassenger * 2 / 3) / 365) * model.ElectricCommercialVehiclePartial.CommonProperties.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengers = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriver + paToPassengers).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = ((res.SubTotalC * model.HGIShareRate) / 100).RoundToFourPrecisions();
            }

            #endregion

            if (model.ElectricCommercialVehiclePartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                #region RSMDT
                //get the rate from db
                var minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.MinimumRSMDTAmount);
                if (days != 365)
                {
                    decimal fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RSMDTRate = rsmdRate;
                var rsmdAmount = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
                if (days != 365)
                {
                    decimal fullRsmdAmount = rsmdAmount;
                    rsmdAmount = GetProRataOrShortScaleAmount(fullRsmdAmount);
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = fullRsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };

                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel { Rate = rsmdRate, Amount = rsmdAmount, ProRataOrShortScaleAmount = rsmdAmount, Total = rsmdAmount };
                }

                res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = rsmdAmount;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                res.TerrorismAmount = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
                if (days != 365)
                {
                    decimal fullTerrorismAmount = res.TerrorismAmount;
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = fullTerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel { Rate = terrorismRate, Amount = res.TerrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriver = 0;
                decimal rsmdtPassenger = 0;


                var driverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);

                rsmdtDriver = rsmdtSumInsuredForDriver * driverRate;//PA amount must come from input field and rate from db
                res.RSMDTDriver = rsmdtDriver;
                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;

                if (days != 365)
                {
                    decimal fullPaToDriver = rsmdtDriver;
                    rsmdtDriver = GetProRataOrShortScaleAmount(fullPaToDriver);
                    res.RSMDTDriver = rsmdtDriver;
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = rsmdtDriver, ProRataOrShortScaleAmount = rsmdtDriver, Total = rsmdtDriver };
                }

                var passengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);

                rsmdtPassenger = rsmdtSumInsuredForPassenger *
                                    (model.ElectricCommercialVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver - 1) * passengerRate;//PA amount must come from input field and rate from db
                res.RSMDTPassenger = rsmdtPassenger;
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassenger;

                if (days != 365)
                {
                    decimal fullPaToPassengers = rsmdtPassenger;
                    rsmdtPassenger = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    res.RSMDTPassenger = rsmdtPassenger;
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = rsmdtPassenger, ProRataOrShortScaleAmount = rsmdtPassenger, Total = rsmdtPassenger };
                }

                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriver + rsmdtPassenger;
                if (model.ElectricCommercialVehiclePartial.CommonProperties.IsLayup)
                {
                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.ElectricCommercialVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                #endregion
            }

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

            globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.ElectricCommercialVehiclePartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricCommercialVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.SumInsuredAmount = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount;
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            res.RiskDetails.ElectricCommercialVehicleCalculationResult = detailCalculationResult;
            model.FullSumInsured = res.FullSumInsured = res.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = model.ElectricCommercialVehiclePartial.CommonProperties.SumInsuredAmount = (res.SumInsuredAmount * model.HGIShareRate) / 100;
                res.HGIShareRate = model.HGIShareRate;
                res.IsCoinsurance = model.IsCoinsurance;
            }
            return res;
        });
    }
    #endregion

    #region Agriculture Forestry Vehicle
    private async Task<PremiumCalculationResultModel> AgricultureForestryVehicle(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            if (model.GoodsCarryingVehiclePartial != null)
            {
                model.GoodsCarryingVehiclePartial = null;
            }var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            var res = new PremiumCalculationResultModel();
            var detailCalculationResult = new AgricultureForestryVehicleDetailCalculationResult();
            detailCalculationResult.IsProRateOrShortScaleAmount = model.AgricultureForestryVehiclePartial.CommonProperties.IsProRataOrShortScale;
            model.VehicleNumber = model.AgricultureForestryVehiclePartial.CommonProperties.RegistrationNumber;
            res.VehicleCapacity = model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity;
            detailCalculationResult.NumberOfSeats = model.AgricultureForestryVehiclePartial.CommonRSMDTModel.NumberofSeatsIncludingDriver;
            var days = Convert.ToInt16(model.AgricultureForestryVehiclePartial.CommonProperties.Days ?? "0");
            shortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.CommercialVehicleShortScaleRate, days);
            if (!model.AgricultureForestryVehiclePartial.CommonProperties.IsComprehensive)
            {
                shortScaleRate = 1;
            }
            res.Days = days;
            res.ShortScaleRate = shortScaleRate;

            DateViewModel dateViewModel = new DateViewModel();

            if (model.AgricultureForestryVehiclePartial.CommonProperties.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.AgricultureForestryVehiclePartial.CommonProperties.DateOfPurchase ?? default(DateTime));
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.AgricultureForestryVehiclePartial.CommonProperties.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.AgricultureForestryVehiclePartial.CommonProperties.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.AgricultureForestryVehiclePartial.CommonProperties.AgeForPrintEnglish = ageStringEnglish;
            model.AgricultureForestryVehiclePartial.CommonProperties.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            int basicPremiumRateEnumValue = (int)AgricultureForestryVehicleConfigType.BasicPremiumRate;
            int loadingRateEnumValue = (int)AgricultureForestryVehicleConfigType.LoadingRate;
            int asPerTonEnumValue = (int)AgricultureForestryVehicleConfigType.AsPerTon;
            int perCarryingCapacityInTonEnumValue = (int)AgricultureForestryVehicleConfigType.PerCarryingCapacityInTon;
            int paToPaidDriverEnumValue = (int)AgricultureForestryVehicleConfigType.PAToPaidDriver;
            int paToHelperEnumValue = (int)AgricultureForestryVehicleConfigType.PAToHelper;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)AgricultureForestryVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)AgricultureForestryVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)AgricultureForestryVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToHelperRateEnumValue = (int)AgricultureForestryVehicleConfigType.RSMDTToHelperRate;
            if (model.ApplyGovtConfig)
            {
                basicPremiumRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtBasicPremiumRate;
                loadingRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtLoadingRate;
                asPerTonEnumValue = (int)AgricultureForestryVehicleConfigType.GovtAsPerTon;
                perCarryingCapacityInTonEnumValue = (int)AgricultureForestryVehicleConfigType.GovtPerCarryingCapacityInTon;
                paToPaidDriverEnumValue = (int)AgricultureForestryVehicleConfigType.GovtPAToPaidDriver;
                paToHelperEnumValue = (int)AgricultureForestryVehicleConfigType.GovtPAToHelper;
                riotAndStrikeAndMDAmountRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToHelperRateEnumValue = (int)AgricultureForestryVehicleConfigType.GovtRSMDTToHelperRate;
            }

            //discard agent for policies with third party only
            if (!model.AgricultureForestryVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            #endregion
            int directDiscountPARateEnumValue = (int)AgricultureForestryVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);

            #region CalculateSumInsured
            decimal currentMarketPrice = Convert.ToDecimal(model.AgricultureForestryVehiclePartial.CommonProperties.CurrentMarketPrice);  //get from db
            if (!model.AgricultureForestryVehiclePartial.CommonProperties.EnterSumInsured)
            {
                model.AgricultureForestryVehiclePartial.CommonProperties.ValueWithoutAccessories = currentMarketPrice - (currentMarketPrice * rateOfDepreciation);
                model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount = Math.Round(model.AgricultureForestryVehiclePartial.CommonProperties.ValueWithoutAccessories + Convert.ToDecimal(model.AgricultureForestryVehiclePartial.CommonProperties.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount = model.AgricultureForestryVehiclePartial.CommonProperties.ValueWithoutAccessories + (model.AgricultureForestryVehiclePartial.CommonProperties.ValueOfAccessories ?? default(decimal));
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount;
            #endregion

            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premium
                decimal basicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
                res.BasicPremiumRate = basicPremiumRate;
                decimal amount = res.PrimaryBasicPremiumAmount = basicPremiumRate * model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount;
                decimal fullAmount = amount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                    amount = proRataOrShortScaleAmount;
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    res.PrimaryBasicPremiumAmount = amount;
                }
                else
                {
                    detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = basicPremiumRate, Amount = res.PrimaryBasicPremiumAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = amount };
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                // excess more than 3 
                decimal excessMoreThan__TonsAmount = 0; //model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity - 3;
                decimal excessMoreThan__TonsMinimumValue = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.MinimumTonExcessValue);
                res.MinimumTon = excessMoreThan__TonsMinimumValue;
                if (model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity > excessMoreThan__TonsMinimumValue)
                {
                    decimal excessMorethanAmountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.MinimumTonExcessRateAmount);
                    decimal excessTon = model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity - excessMoreThan__TonsMinimumValue;
                    excessMoreThan__TonsAmount = excessMorethanAmountRate * excessTon;
                    res.AmountPerExcessTons = excessMoreThan__TonsAmount;

                    fullAmount += excessMoreThan__TonsAmount;

                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(excessMoreThan__TonsAmount);
                        res.AmountPerExcessTons = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.PrimaryBasicPremiumAmount = amount;
                    }
                    else
                    {
                        amount += excessMoreThan__TonsAmount;
                        res.PrimaryBasicPremiumAmount = amount;
                        detailCalculationResult.ExcessMoreThan = new CalculationSubDetailAmountModel() { Rate = excessMorethanAmountRate, Amount = excessMoreThan__TonsAmount, ProRataOrShortScaleAmount = excessMoreThan__TonsAmount, Total = amount };
                    }
                }

                // as per carrying capacity
                var amountAsPerCarryingCapacity = ConstrantValueHelper.GetNewValue(riskSetupModel, asPerTonEnumValue, model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity);
                res.AmountPerCarryingCapacity = amountAsPerCarryingCapacity;
                fullAmount -= amountAsPerCarryingCapacity;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(amountAsPerCarryingCapacity);
                    res.AmountPerCarryingCapacity = proRataOrShortScaleAmount;
                    amount -= proRataOrShortScaleAmount;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount -= amountAsPerCarryingCapacity;
                    detailCalculationResult.PerSeatCapacity = new CalculationSubDetailAmountModel() { Amount = amountAsPerCarryingCapacity, ProRataOrShortScaleAmount = amountAsPerCarryingCapacity, Total = amount };
                }
                res.PrimaryBasicPremiumAmount = amount;

                // tailor
                if (model.AgricultureForestryVehiclePartial.CommonRSMDTModel.HasTailor && model.AgricultureForestryVehiclePartial.CommonRSMDTModel.ValueOfTailor >= 1)
                {
                    decimal trailorRate = res.TrailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.ForTrailor);
                    decimal trailorValue = model.AgricultureForestryVehiclePartial.CommonRSMDTModel.ValueOfTailor ?? default(int);
                    decimal tailorAmount = trailorRate * trailorValue - 200;
                    res.TrailorAmount = tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = Convert.ToInt32(model.AgricultureForestryVehiclePartial.CommonRSMDTModel.ValueOfTailor);
                    fullAmount += tailorAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(tailorAmount);
                        res.TrailorAmount = proRataOrShortScaleAmount;
                        amount += proRataOrShortScaleAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount += tailorAmount;
                        detailCalculationResult.Trailor = new CalculationSubDetailAmountModel() { Rate = trailorRate, Amount = tailorAmount, ProRataOrShortScaleAmount = tailorAmount, Total = amount };
                    }
                    res.PrimaryBasicPremiumAmount = amount;
                }

                // age loading
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.AgricultureForestryVehiclePartial.CommonProperties.AgeOfVehicle ?? default(decimal));
                detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);
                res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadingRate;
                decimal ageLoadingAmount = 0;
                ageLoadingAmount = res.AgeLoadingAmount;
                res.AgeLoadingRate = ageLoadingRate;
                fullAmount += res.AgeLoadingAmount;
                if (days != 365)
                {
                    decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
                    res.AgeLoadingAmount = proRataOrShortScaleAmount;
                    amount += proRataOrShortScaleAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = ageLoadingAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                }
                else
                {
                    amount += res.AgeLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadingRate, Amount = res.AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = amount };
                }

                // voluntary excess
                if (model.AgricultureForestryVehiclePartial.CommonProperties.VoluntaryExcess != 0)
                {
                    res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.VoluntaryExcessRate, model.AgricultureForestryVehiclePartial.CommonProperties.VoluntaryExcess);
                    decimal voluntaryExcessAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                    detailCalculationResult.VoluntaryExcessValue = model.AgricultureForestryVehiclePartial.CommonProperties.VoluntaryExcess;
                    fullAmount -= res.VoluntaryExcessAmount;

                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                    else
                    {
                        amount -= res.VoluntaryExcessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = voluntaryExcessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = amount };
                    }
                }

                //   ncd
                if (model.AgricultureForestryVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.NCDRate, model.AgricultureForestryVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                    fullAmount -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        amount -= res.NoClaimDiscountAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        amount -= ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = amount };
                    }
                }

                //private hire
                if (model.AgricultureForestryVehiclePartial.CommonRSMDTModel.UseOfPrivateHire)
                {
                    decimal privateHireRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.PrivateUseDiscount);
                    decimal privateHireAmount = privateHireRate * Math.Abs(fullAmount);
                    res.PrivateHireAmount = privateHireAmount;
                    fullAmount -= privateHireAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(privateHireAmount);
                        res.PrivateHireAmount = proRataOrShortScaleAmount;
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                    }
                    else
                    {
                        amount -= privateHireAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel() { Rate = privateHireRate, Amount = privateHireAmount, ProRataOrShortScaleAmount = privateHireAmount, Total = amount };
                    }
                }

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    decimal directDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.DirectDiscount);
                    res.DirectDiscountRate = directDiscountRate;
                    decimal directDiscountAmount = Math.Abs(fullAmount) * directDiscountRate;
                    res.DirectDiscountAmount = directDiscountAmount;
                    fullAmount -= directDiscountAmount;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        amount -= proRataOrShortScaleAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = amount };
                        res.DirectDiscountAmount = proRataOrShortScaleAmount;
                    }
                    else
                    {
                        amount -= directDiscountAmount;
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = amount };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }
                res.CalculatedSubTotalA = amount;
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.CalculatedSubTotalA, Total = res.CalculatedSubTotalA };
                res.SubTotalA = res.CalculatedSubTotalA;

                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }
                if (model.AgricultureForestryVehiclePartial.CommonProperties.ISRecoveryCharge)
                {

                    decimal recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.RecoveryCharge);
                    res.ActualRecoveryCharge = recoveryCharge;
                    res.RecoveryCharge = recoveryCharge;
                    if (days != 365)
                    {
                        decimal proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                        res.RecoveryCharge = proRataOrShortScaleAmount;
                        res.SubTotalA += proRataOrShortScaleAmount;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = res.SubTotalA };
                    }
                    else
                    {
                        res.SubTotalA += recoveryCharge;
                        detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel() { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.SubTotalA };
                    }
                }
                if (model.AgricultureForestryVehiclePartial.CommonProperties.IsLayup)
                {
                    res.LayupDiscountOwnDamage = ((res.SubTotalA * 2 / 3) / 365) * model.AgricultureForestryVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalA = res.SubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.SubTotalA };
                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.SubTotalA, Total = res.SubTotalA };
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.HGIShareRate = model.HGIShareRate;
                    res.IsCoinsurance = model.IsCoinsurance;
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
                }
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            model.AgricultureForestryVehiclePartial.CommonProperties.IsThirdParty = true;
            decimal thirdPartyAmount = res.TPLperTon = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perCarryingCapacityInTonEnumValue, Convert.ToDecimal(model.AgricultureForestryVehiclePartial.GoodsCarryingCapacity));
            detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, Total = thirdPartyAmount };
            res.SubTotalB = thirdPartyAmount;
            res.ActualSubTotalB = thirdPartyAmount;
            if (days != 365)
            {
                decimal proRataOrShortScaleAmount = res.TPLperTon = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.SubTotalB = proRataOrShortScaleAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyAmount, Total = thirdPartyAmount };
            }

            //   ncd
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsComprehensive)
            {
                if (model.AgricultureForestryVehiclePartial.CommonProperties.NCDYears > 0)
                {
                    res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.NCDRate, model.AgricultureForestryVehiclePartial.CommonProperties.NCDYears);
                    decimal ncdAmount = res.ThirdPartyNoClaimDiscountAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

                    if (days != 365)
                    {
                        res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount, Total = res.ThirdPartyNoClaimDiscountAmount };
                    }
                    else
                    {
                        res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.TPLNCD = new CalculationSubDetailAmountModel() { Rate = res.ThirdPartyNoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = ncdAmount };
                    }
                    res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
                    res.ActualSubTotalB -= ncdAmount;
                }
            }
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.AgricultureForestryVehiclePartial.CommonProperties.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = (res.SubTotalB * model.HGIShareRate) / 100;
            }
            #endregion

            #region Paid Driver and Helpers

            decimal paToDriverAmount = 0;
            decimal paToHelperAmount = 0;
            decimal paAmount = 0;

            //driver
            paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
            res.PAforDriver = paAmount;
            if (days != 365)
            {
                paToDriverAmount = GetProRataOrShortScaleAmount(paAmount);
                res.PAforDriver = paToDriverAmount;
            }
            else
            {
                paToDriverAmount = paAmount;
            }
            detailCalculationResult.PAForPaidDriver = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToDriverAmount, Total = paToDriverAmount };

            //direct discount for DRIVER
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsDirectDiscountPA)
            {
                res.DirectDiscountAmountPA = paToDriverAmount * directDiscountPARate;
                paToDriverAmount -= res.DirectDiscountAmountPA;
                detailCalculationResult.DirectDiscountForPADriver = new CalculationSubDetailAmountModel()
                {
                    Rate = directDiscountPARate,
                    Amount = res.DirectDiscountAmountPA,
                    ProRataOrShortScaleAmount = res.DirectDiscountAmountPA,
                    Total = res.DirectDiscountAmountPA
                };
            }
            res.PAforDriver = paToDriverAmount;
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPADriver = ((res.PAforDriver * 2 / 3) / 365) * model.AgricultureForestryVehiclePartial.CommonProperties.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriverAmount = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //helper
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsHelperInsured)
            {
                if (model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers != null && model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                {
                    res.NumberOfHelpers = model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers;
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue) * (model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                }
                else
                {
                    paAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, paToHelperEnumValue);
                    res.NumberOfHelpers = 1;
                }
                res.PAforHelper = paAmount;
                if (days != 365)
                {
                    paToHelperAmount = GetProRataOrShortScaleAmount(paAmount);
                    res.PAforHelper = paToHelperAmount;
                }
                else { paToHelperAmount = paAmount; }

                detailCalculationResult.PAForHelper = new CalculationSubDetailAmountModel() { Amount = paAmount, ProRataOrShortScaleAmount = paToHelperAmount, Total = paToHelperAmount };

                //direct discount for HELPER
                if (model.AgricultureForestryVehiclePartial.CommonProperties.IsDirectDiscountPA)
                {
                    res.DirectDiscountAmountPAHelper = paToHelperAmount * directDiscountPARate;
                    paToHelperAmount -= res.DirectDiscountAmountPAHelper;
                    detailCalculationResult.DirectDiscountForPAHelper = new CalculationSubDetailAmountModel()
                    {
                        Rate = directDiscountPARate,
                        Amount = res.DirectDiscountAmountPAHelper,
                        ProRataOrShortScaleAmount = res.DirectDiscountAmountPAHelper,
                        Total = res.DirectDiscountAmountPAHelper
                    };
                }
                res.PAforHelper = paToHelperAmount;
            }
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsLayup)
            {
                res.LayupDiscountPAHelper = ((res.PAforHelper * 2 / 3) / 365) * model.AgricultureForestryVehiclePartial.CommonProperties.LayupDays;
                res.PAforHelper = res.PAforHelper - res.LayupDiscountPAHelper;
                paToHelperAmount = res.PAforHelper;
                detailCalculationResult.LayupDiscountPAHelper = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAHelper, ProRataOrShortScaleAmount = res.LayupDiscountPAHelper, Total = res.LayupDiscountPAHelper };
            }
            res.SubTotalC = (paToDriverAmount + paToHelperAmount).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            #endregion

            detailCalculationResult.RSMDTSumInsuredAmountForDriver = res.RSMDTSuminsuredDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RSMDTSumInsuredAmountForPaidDriver);
            detailCalculationResult.RSMDTSumInsuredAmountForHelper = res.RSMDTSuminsuredHelper = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)GoodsCarryingVehicleConfigType.RSMDTSumInsuredAmountForHelper);

            #region RSMDT
            if (model.AgricultureForestryVehiclePartial.CommonProperties.IsRiotStrike)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                decimal proRataOrShortScaleAmount = 0;

                decimal minimumRsmdAmount = res.minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.MinimumRSMDTAmount);
                decimal rsmdRate = res.RSMDTRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                res.RiotAndStrikeAndMdAmount = model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount * rsmdRate;
                if (days != 365)
                {
                    var fullMinRsmdAmount = minimumRsmdAmount;
                    minimumRsmdAmount = res.minRSMDTAmount = GetProRataOrShortScaleAmount(fullMinRsmdAmount);
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel { Amount = fullMinRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minimumRsmdAmount, ProRataOrShortScaleAmount = minimumRsmdAmount, Total = minimumRsmdAmount };
                }

                if (days != 365)
                {
                    var fullRSMDAmount = res.RiotAndStrikeAndMdAmount;
                    proRataOrShortScaleAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                    res.RiotAndStrikeAndMdAmount = proRataOrShortScaleAmount;
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = fullRSMDAmount, ProRataOrShortScaleAmount = proRataOrShortScaleAmount, Total = proRataOrShortScaleAmount };
                }
                else
                {
                    detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = rsmdRate, Amount = res.RiotAndStrikeAndMdAmount, ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount, Total = res.RiotAndStrikeAndMdAmount };
                }

                res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
                if (minimumRsmdAmount > res.RiotAndStrikeAndMdAmount)
                {
                    res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDAmount = true;
                }

                decimal terrorismRate = res.TerrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismRate = terrorismRate;
                decimal terrorismAmount = model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount * terrorismRate;
                res.TerrorismAmount = terrorismAmount;
                if (days != 365)
                {
                    res.TerrorismAmount = GetProRataOrShortScaleAmount(terrorismAmount);
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }
                else
                {
                    res.TerrorismAmount = terrorismAmount;
                    detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate, Amount = terrorismAmount, ProRataOrShortScaleAmount = res.TerrorismAmount, Total = res.TerrorismAmount };
                }

                decimal rsmdtDriverAmount = 0;
                decimal rsmdtHelperAmount = 0;
                decimal rsmdtAmount = 0;
                ///// Need to add in configuration

                decimal rsmdtToDriverRate = res.RSMDTforDriverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForDriver * rsmdtToDriverRate;
                res.RSMDTDriver = rsmdtAmount;

                if (days != 365)
                {
                    rsmdtDriverAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                    res.RSMDTDriver = rsmdtDriverAmount;
                }
                else { rsmdtDriverAmount = rsmdtAmount; }

                detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToDriverRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtDriverAmount, Total = rsmdtDriverAmount };

                //helper
                if (model.AgricultureForestryVehiclePartial.CommonProperties.IsHelperInsured)
                {
                    decimal rsmdtToHelperRate = res.RSMDTForHelperRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToHelperRateEnumValue);
                    if (model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers != null && model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers > 0)
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate * (model.AgricultureForestryVehiclePartial.CommonProperties.NumberOfHelpers ?? default(int));
                    }
                    else
                    {
                        rsmdtAmount = detailCalculationResult.RSMDTSumInsuredAmountForHelper * rsmdtToHelperRate;
                    }
                    res.RSMDTHelper = rsmdtAmount;

                    if (days != 365)
                    {
                        rsmdtHelperAmount = GetProRataOrShortScaleAmount(rsmdtAmount);
                        res.RSMDTHelper = rsmdtHelperAmount;
                    }
                    else { rsmdtHelperAmount = rsmdtAmount; }

                    detailCalculationResult.HelperRSMDT = new CalculationSubDetailAmountModel() { Rate = rsmdtToHelperRate, Amount = rsmdtAmount, ProRataOrShortScaleAmount = rsmdtHelperAmount, Total = rsmdtHelperAmount };
                }
                res.SubTotalD = res.RSMDTAmount + res.TerrorismAmount + rsmdtDriverAmount + rsmdtHelperAmount;
                if (model.AgricultureForestryVehiclePartial.CommonProperties.IsLayup)
                {

                    if (isMinRSMDAmount) res.SubTotalD -= res.RSMDTAmount;
                    res.LayupDiscountRSMDT = ((res.SubTotalD * 2 / 3) / 365) * model.AgricultureForestryVehiclePartial.CommonProperties.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD };
                    if (isMinRSMDAmount) res.SubTotalD += res.RSMDTAmount;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = (res.SubTotalD * model.HGIShareRate) / 100;
                }
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                res.StampDutyAmount = 0;
            }
            else
            {
                if (model.AgricultureForestryVehiclePartial.CommonProperties.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)AgricultureForestryVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount;
            var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.VatPercent = VATPercent * 100;
            res.SumInsuredAmount = res.FullSumInsured = model.FullSumInsured = model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = (model.FullSumInsured * model.HGIShareRate) / 100;
                model.AgricultureForestryVehiclePartial.CommonProperties.SumInsuredAmount = res.SumInsuredAmount;
            }
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ThirdPartyAmount = res.SubTotalB;
            res.ExpiryDate = DateTime.UtcNow.AddDays(days);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            #endregion

            detailCalculationResult.Days = Convert.ToInt32(model.AgricultureForestryVehiclePartial.CommonProperties.Days);
            res.RiskDetails.AgricultureForestryVehicleDetailCalculationResult = detailCalculationResult;
            return res;
        });
    }
    #endregion

    #region Helper
    private decimal GetProRataOrShortScaleAmount(decimal orgAmount)
    {
        return orgAmount * shortScaleRate;
        //if (isProRata)
        //{
        //    return (orgAmount * days) / 365M;
        //}
        //else
        //{
        //    return orgAmount * shortScaleRate;
        //}
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }
    #endregion
}
