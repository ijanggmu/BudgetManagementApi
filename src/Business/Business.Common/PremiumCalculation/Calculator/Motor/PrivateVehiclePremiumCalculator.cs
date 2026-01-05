using Business.Common.Helper;
using Data.Context;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator.Motor;
public class PrivateVehiclePremiumCalculator : IPolicyPremiumCalculator
{
    private readonly bool isProRata;
    private int days;
    private bool isMinRSMDTAmount;
    private decimal shortScaleRate;
    private readonly ApplicationDataContext _db;

    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.PrivateVehicle
        };

    public PrivateVehiclePremiumCalculator(ApplicationDataContext db)
    {
        shortScaleRate = 0;
        isProRata = false;
        days = 0;
        isMinRSMDTAmount = false;
        _db = db;
    }
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            #region Initialization
            var detailCalculationResult = new PrivateVehicleDetailCalculationResult();
            var res = new PremiumCalculationResultModel();

            var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();
            var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);

            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            model.PrivateVehiclePartial.IsThirdParty = true;
            model.VehicleNumber = model.PrivateVehiclePartial.RegistrationNumber;

            //calculate age in year
            DateViewModel dateViewModel = new DateViewModel();
            if (model.PrivateVehiclePartial.PurchasedNewOld)   //// Is New
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.PrivateVehiclePartial.DateOfPurchase ?? default);
            }
            else
            {
                dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.PrivateVehiclePartial.YearsFromRegistrationDateYears);
            }
            var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
            model.PrivateVehiclePartial.AgeForPrint = ageStringNepali;
            var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
            model.PrivateVehiclePartial.AgeForPrintEnglish = ageStringEnglish;
            detailCalculationResult.CC = model.PrivateVehiclePartial.CubicCapacity;
            detailCalculationResult.NumberOfSeats = model.PrivateVehiclePartial.NumberofSeatsIncludingDriver;
            detailCalculationResult.SelectedVoluntaryExcess = model.PrivateVehiclePartial.VoluntaryExcess;
            detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

            model.PrivateVehiclePartial.AgeOfVehicle = dateViewModel.PeriodDifference;
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += 12 * dateViewModel.Years;
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            //proRata and ShortScale
            days = detailCalculationResult.Days = Convert.ToInt32(model.PrivateVehiclePartial.Days);
            if (!model.PrivateVehiclePartial.IsProRataOrShortScale && model.PrivateVehiclePartial.IsComprehensive)
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = model.ShortScale != null ? model.ShortScale.Value / 100 : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.PrivateVehicleShortScaleRate, days);
            }
            else
            {
                shortScaleRate = detailCalculationResult.ShortScaleRate = 1;
            }

            //pa
            var rsmdtSumInsuredForDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.RSMDTSumInsuredAmountForDriver);
            res.RSMDTSuminsuredDriver = rsmdtSumInsuredForDriver;
            var rsmdtSumInsuredForPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.RSMDTSumInsuredAmountForPassengers);
            res.RSMDTSuminsuredPassenger = rsmdtSumInsuredForPassenger;

            //discard agent for policies with third party only
            if (!model.PrivateVehiclePartial.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.ThirdParty;
                model.Agent = null;
                model.AgentName = null;
                model.AgentId = null;
            }
            int directDiscountPARateEnumValue = (int)PrivateVehicleConfigType.DirectDiscountPA;
            decimal directDiscountPARate = ConstrantValueHelper.GetNewValue(riskSetupModel, directDiscountPARateEnumValue);
            if (!model.IsCoinsurance)
            {
                model.MinimumBasicPremium = false;
                model.MinimumRSMDT = false;
            }

            #endregion

            #region CalculateSumInsured
            //price of motorcycle model
            decimal currentMarketPrice = Convert.ToDecimal(model.PrivateVehiclePartial.CurrentMarketPrice);  //get from db
            if (!model.PrivateVehiclePartial.EnterSumInsured)
            {
                model.PrivateVehiclePartial.ValueWithoutAccessories = (currentMarketPrice - currentMarketPrice * rateOfDepreciation).ToString();
                model.PrivateVehiclePartial.SumInsuredAmount = Math.Round(Convert.ToDecimal(model.PrivateVehiclePartial.ValueWithoutAccessories) + Convert.ToDecimal(model.PrivateVehiclePartial.ValueOfAccessories), MidpointRounding.AwayFromZero);
            }
            else
            {
                model.PrivateVehiclePartial.SumInsuredAmount = Convert.ToDecimal(model.PrivateVehiclePartial.ValueWithoutAccessories) + Convert.ToDecimal(model.PrivateVehiclePartial.ValueOfAccessories);
            }
            res.SumInsuredAmount = detailCalculationResult.SumInsured = model.PrivateVehiclePartial.SumInsuredAmount;

            int loadingRateEnumValue = (int)PrivateVehicleConfigType.LoadingForOldVehicle;
            int paToPaidDriverEnumValue = (int)PrivateVehicleConfigType.PAToPaidDriver;
            int paToPassengersEnumValue = (int)PrivateVehicleConfigType.PAToPassengers;
            int riotAndStrikeAndMDAmountRateEnumValue = (int)PrivateVehicleConfigType.RiotAndStrikeAndMDAmountRate;
            int terrorismAmountRateEnumValue = (int)PrivateVehicleConfigType.TerrorismAmountRate;
            int rsmdtToPaidDriverRateEnumValue = (int)PrivateVehicleConfigType.RSMDTToPaidDriverRate;
            int rsmdtToPassengersRateEnumValue = (int)PrivateVehicleConfigType.RSMDTToPassengersRate;
            int tPLPremiumAsPerCCEnumValue = (int)PrivateVehicleConfigType.TPLPremiumAsPerCC;
            int perCCBasicAmountEnumValue = (int)PrivateVehicleConfigType.PerCCBasicAmount;
            int forFirstThresholdAmountEnumValue = (int)PrivateVehicleConfigType.ForFirstThresholdAmount;
            int forAboveThresholdAmountEnumValue = (int)PrivateVehicleConfigType.ForAboveThresholdAmount;

            if (model.ApplyGovtConfig)
            {
                loadingRateEnumValue = (int)PrivateVehicleConfigType.GovtLoadingForOldVehicle;
                paToPaidDriverEnumValue = (int)PrivateVehicleConfigType.GovtPAToPaidDriver;
                paToPassengersEnumValue = (int)PrivateVehicleConfigType.GovtPAToPassengers;
                riotAndStrikeAndMDAmountRateEnumValue = (int)PrivateVehicleConfigType.GovtRiotAndStrikeAndMDAmountRate;
                terrorismAmountRateEnumValue = (int)PrivateVehicleConfigType.GovtTerrorismAmountRate;
                rsmdtToPaidDriverRateEnumValue = (int)PrivateVehicleConfigType.GovtRSMDTToPaidDriverRate;
                rsmdtToPassengersRateEnumValue = (int)PrivateVehicleConfigType.GovtRSMDTToPassengersRate;
                tPLPremiumAsPerCCEnumValue = (int)PrivateVehicleConfigType.GovtTPLPremiumAsPerCC;
                perCCBasicAmountEnumValue = (int)PrivateVehicleConfigType.GovtPerCCBasicAmount;
                forFirstThresholdAmountEnumValue = (int)PrivateVehicleConfigType.GovtForFirstThresholdAmount;
                forAboveThresholdAmountEnumValue = (int)PrivateVehicleConfigType.GovtForAboveThresholdAmount;
            }

            #endregion

            var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.MinimumBasicPremiumAmount);
            if (model.IsCoinsurance && model.MinimumBasicPremium)
            {
                minimumBasicPremiumAmount = model.MinimumBasicPremiumAmount;
            }
            if (model.PrivateVehiclePartial.IsComprehensive)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
                #region Basic Premiums

                decimal thresholdAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.SumInsuredThresholdAmount);
                decimal basicRate = ConstrantValueHelper.GetNewValue(riskSetupModel, forFirstThresholdAmountEnumValue, model.PrivateVehiclePartial.CubicCapacity);
                res.BasicPremiumRate = basicRate;
                decimal basicThresholdAmount = basicRate * thresholdAmount;
                res.ThresholdAmount = thresholdAmount;
                res.SumInsuredToDisplay = res.SumInsuredAmount;
                decimal fullBasicPremium = 0;
                if (model.PrivateVehiclePartial.SumInsuredAmount > thresholdAmount)
                {
                    decimal excessSumInsured = res.ExcessAmount = model.PrivateVehiclePartial.SumInsuredAmount - thresholdAmount;
                    decimal excessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, forAboveThresholdAmountEnumValue);
                    decimal excessAmount = excessRate * excessSumInsured;
                    res.SumInsuredToDisplay = res.ThresholdAmount;

                    if (days != 365)
                    {
                        res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(basicThresholdAmount);
                        decimal proRataExcessAmount = GetProRataOrShortScaleAmount(excessAmount);
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = basicThresholdAmount, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremiumForAboveThreshold = new CalculationSubDetailAmountModel() { Rate = excessRate, Amount = excessAmount, ProRataOrShortScaleAmount = proRataExcessAmount, Total = proRataExcessAmount };
                        res.PrimaryBasicPremiumAmount += proRataExcessAmount;
                        fullBasicPremium = basicThresholdAmount + excessAmount;
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };

                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount = fullBasicPremium = basicThresholdAmount + excessAmount;
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = basicThresholdAmount, ProRataOrShortScaleAmount = basicThresholdAmount, Total = basicThresholdAmount };
                        detailCalculationResult.BasicPremiumForAboveThreshold = new CalculationSubDetailAmountModel() { Rate = excessRate, Amount = excessAmount, ProRataOrShortScaleAmount = excessAmount, Total = excessAmount };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }
                else
                {
                    if (days != 365)
                    {
                        fullBasicPremium = basicRate * model.PrivateVehiclePartial.SumInsuredAmount;
                        res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(fullBasicPremium);
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                    else
                    {
                        res.PrimaryBasicPremiumAmount = fullBasicPremium = basicRate * model.PrivateVehiclePartial.SumInsuredAmount;
                        detailCalculationResult.BasicPremiumForFirstThreshold = new CalculationSubDetailAmountModel() { Rate = basicRate, Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                        detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel { Amount = fullBasicPremium, ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount, Total = res.PrimaryBasicPremiumAmount };
                    }
                }
                res.BasicPremiumForPrint = res.PrimaryBasicPremiumAmount;

                //add tailorAmount
                if (model.PrivateVehiclePartial.HasTailor && model.PrivateVehiclePartial.ValueOfTailor > 0)
                {
                    decimal tailorRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.ForTrailor);
                    res.TrailorRate = tailorRate;
                    decimal tailorAmount = 300 + tailorRate * model.PrivateVehiclePartial.ValueOfTailor ?? default(int);
                    res.TrailorAmount = tailorAmount;
                    fullBasicPremium += tailorAmount;
                    detailCalculationResult.ValueOfTrailorEntered = model.PrivateVehiclePartial.ValueOfTailor ?? default;

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

                //deduct tpl premium
                var tplPerCC = ConstrantValueHelper.GetNewValue(riskSetupModel, tPLPremiumAsPerCCEnumValue, model.PrivateVehiclePartial.CubicCapacity);
                fullBasicPremium -= tplPerCC;
                if (days != 365)
                {
                    res.TPLperCC = GetProRataOrShortScaleAmount(tplPerCC);
                    res.PrimaryBasicPremiumAmount -= res.TPLperCC;
                    detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel { Amount = tplPerCC, ProRataOrShortScaleAmount = res.TPLperCC, Total = res.PrimaryBasicPremiumAmount };
                }
                else
                {
                    res.TPLperCC = tplPerCC;
                    res.PrimaryBasicPremiumAmount -= tplPerCC;
                    detailCalculationResult.TPLAsPerCC = new CalculationSubDetailAmountModel { Amount = tplPerCC, ProRataOrShortScaleAmount = tplPerCC, Total = res.PrimaryBasicPremiumAmount };
                }

                //add loading for old vehicle
                decimal ageLoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.PrivateVehiclePartial.AgeOfVehicle ?? default);
                decimal ageLoadingAmount = res.AgeLoadingAmount = Math.Abs(fullBasicPremium) * ageLoadingRate;
                var basicTotal = fullBasicPremium + res.AgeLoadingAmount;
                res.AgeLoadingRate = ageLoadingRate;
                if (days != 365)
                {
                    res.AgeLoadingAmount = GetProRataOrShortScaleAmount(ageLoadingAmount);
                    decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = ageLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = proRataTotal };
                }
                else
                {
                    res.AgeLoadingAmount = ageLoadingAmount;
                    detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel { Rate = ageLoadingRate, Amount = ageLoadingAmount, ProRataOrShortScaleAmount = ageLoadingAmount, Total = basicTotal };
                }

                //add private taxi
                decimal privateTaxiAmount = 0;
                if (model.PrivateVehiclePartial.UseOfPrivateHire)
                {
                    var privateTaxiRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.PrivateTaxi);
                    privateTaxiAmount = privateTaxiRate * Math.Abs(basicTotal);
                    basicTotal += privateTaxiAmount;
                    if (days != 365)
                    {
                        res.PrivateHireAmount = GetProRataOrShortScaleAmount(privateTaxiAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = res.PrivateHireAmount, Total = proRataTotal };
                    }
                    else
                    {
                        res.PrivateHireAmount = privateTaxiAmount;
                        detailCalculationResult.PrivateHire = new CalculationSubDetailAmountModel { Rate = privateTaxiRate, Amount = privateTaxiAmount, ProRataOrShortScaleAmount = privateTaxiAmount, Total = basicTotal };
                    }
                }

                //less voluntary excess
                if (model.PrivateVehiclePartial.VoluntaryExcess != 0)
                {
                    decimal veRate = res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.VoluntaryExcessRate, model.PrivateVehiclePartial.VoluntaryExcess);
                    decimal voluntaryAccessAmount = Math.Abs(basicTotal) * veRate;
                    basicTotal -= voluntaryAccessAmount;
                    if (days != 365)
                    {
                        res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(voluntaryAccessAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = veRate, Amount = voluntaryAccessAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = proRataTotal };
                    }
                    else
                    {
                        res.VoluntaryExcessAmount = voluntaryAccessAmount;
                        detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel { Rate = veRate, Amount = voluntaryAccessAmount, ProRataOrShortScaleAmount = voluntaryAccessAmount, Total = basicTotal };
                    }
                }

                //   ncd
                if (model.PrivateVehiclePartial.NCDYears > 0)
                {
                    res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.NCDRate, model.PrivateVehiclePartial.NCDYears);
                    decimal ncdAmount = Math.Abs(basicTotal) * res.NoClaimDiscountRate;
                    basicTotal -= ncdAmount;

                    if (days != 365)
                    {
                        res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(basicTotal);
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = proRataTotal };
                    }
                    else
                    {
                        res.NoClaimDiscountAmount = ncdAmount;
                        detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = ncdAmount, Total = basicTotal };
                    }
                }

                //if all goes well check policy issue date. for the first year there is 0% discount
                int policyIssuedYear = model.PrivateVehiclePartial.PreviousPolicyIssuedYear; //get this data from db

                var amountAfterNcd = basicTotal;
                res.CalculatedSubTotalA = basicTotal;

                //check if policy is issued through agent. if yes  direct discount is not applicable
                if (model.AgentId == null && !model.ApplyGovtConfig)
                {
                    var directDiscountRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.DirectDiscount);
                    var directDiscountAmount = directDiscountRate * Math.Abs(res.CalculatedSubTotalA);
                    res.CalculatedSubTotalA = res.CalculatedSubTotalA - directDiscountAmount;

                    if (days != 365)
                    {
                        decimal proRataDDAmount = GetProRataOrShortScaleAmount(directDiscountAmount);
                        decimal proRataTotal = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = proRataDDAmount, Total = proRataTotal };
                        res.DirectDiscountAmount = proRataDDAmount;
                    }
                    else
                    {
                        detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel { Rate = directDiscountRate, Amount = directDiscountAmount, ProRataOrShortScaleAmount = directDiscountAmount, Total = res.CalculatedSubTotalA };
                        res.DirectDiscountAmount = directDiscountAmount;
                    }
                }

                if (days != 365)
                {
                    res.CalculatedSubTotalA = GetProRataOrShortScaleAmount(res.CalculatedSubTotalA);
                }

                if (model.PrivateVehiclePartial.IsLayup)
                {
                    if (model.PrivateVehiclePartial.ISRecoveryCharge)
                    {
                        var recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.RecoveryCharge);
                        res.ActualRecoveryCharge = recoveryCharge;
                        if (days != 365)
                        {
                            decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                            if (!model.MinimumBasicPremium)
                            {
                                res.CalculatedSubTotalA += proRataRCAmount;
                            }
                            res.RecoveryCharge = proRataRCAmount;
                            detailCalculationResult.RecoveryChargeBeforeLayup = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = res.CalculatedSubTotalA };
                        }
                        else
                        {
                            if (!model.MinimumBasicPremium)
                            {
                                res.CalculatedSubTotalA += recoveryCharge;
                            }
                            res.RecoveryCharge = recoveryCharge;
                            detailCalculationResult.RecoveryChargeBeforeLayup = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.CalculatedSubTotalA };
                        }
                    }
                    minimumBasicPremiumAmount += res.RecoveryCharge;
                    res.LayupDiscountOwnDamage = res.CalculatedSubTotalA * 2 / 3 / 365 * model.PrivateVehiclePartial.LayupDays;
                    res.CalculatedSubTotalA = res.CalculatedSubTotalA - res.LayupDiscountOwnDamage;
                    detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.CalculatedSubTotalA };
                }
                detailCalculationResult.CalculatedOwnDamagePremium = new CalculationSubDetailAmountModel { Amount = res.CalculatedSubTotalA, Total = res.CalculatedSubTotalA };

                //compare with minimum own premium amount
                //var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.MinimumBasicPremiumAmount);////// Shifted before tpl to use minimumBasicPremiumAmount in tpl part

                if (days != 365)
                {
                    var fullMinBasicPremium = minimumBasicPremiumAmount;
                    minimumBasicPremiumAmount = GetProRataOrShortScaleAmount(minimumBasicPremiumAmount);
                    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = fullMinBasicPremium, ProRataOrShortScaleAmount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                }
                else
                {
                    detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel { Amount = minimumBasicPremiumAmount, Total = minimumBasicPremiumAmount };
                }



                res.SubTotalA = minimumBasicPremiumAmount;
                if (res.CalculatedSubTotalA > minimumBasicPremiumAmount) res.SubTotalA = res.CalculatedSubTotalA;


                res.MinbasicPremium = minimumBasicPremiumAmount;


                if (model.AgentId != null)
                {
                    res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                    res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                    res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                    res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
                }

                //recovery charge
                if (model.PrivateVehiclePartial.ISRecoveryCharge)
                {
                    if (!model.PrivateVehiclePartial.IsLayup)
                    {
                        var recoveryCharge = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.RecoveryCharge);
                        res.ActualRecoveryCharge = recoveryCharge;
                        if (days != 365)
                        {
                            decimal proRataRCAmount = GetProRataOrShortScaleAmount(recoveryCharge);
                            if (!model.MinimumBasicPremium)
                            {
                                res.SubTotalA += proRataRCAmount;
                            }

                            res.RecoveryCharge = proRataRCAmount;
                            detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = proRataRCAmount, Total = res.SubTotalA };
                        }
                        else
                        {
                            if (!model.MinimumBasicPremium)
                            {
                                res.SubTotalA += recoveryCharge;
                            }
                            res.RecoveryCharge = recoveryCharge;
                            detailCalculationResult.RecoveryCharge = new CalculationSubDetailAmountModel { Amount = recoveryCharge, ProRataOrShortScaleAmount = recoveryCharge, Total = res.SubTotalA };
                        }
                    }
                }


                if (model.PrivateVehiclePartial.IsDifferentlyAble && res.CalculatedSubTotalA > minimumBasicPremiumAmount)
                {
                    res.SpecialDiscountRate = (model.PrivateVehiclePartial.SpecialDiscountRate ?? default) / 100;
                    // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                    res.OwnDamageSpecialDiscountAmount = res.SubTotalA * res.SpecialDiscountRate;
                    res.ShortScaleOwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.OwnDamageSpecialDiscountAmount);
                    res.SubTotalA -= res.OwnDamageSpecialDiscountAmount;
                    detailCalculationResult.SpecialDiscount = new CalculationSubDetailAmountModel { Rate = res.SpecialDiscountRate, Amount = res.OwnDamageSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleOwnDamageSpecialDiscountAmount, Total = res.SubTotalA };

                }
                if (model.IsCoinsurance)
                {
                    res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                    res.SubTotalA = res.SubTotalA * model.HGIShareRate / 100;
                }
                res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
                #endregion
            }

            #region Third Party
            decimal totalTPL = 0;
            decimal thirdPartyAmount = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, perCCBasicAmountEnumValue, model.PrivateVehiclePartial.CubicCapacity);
            //res.ThirdPartyNoClaimDiscountAmount = thirdPartyAmount * ConstrantValueHelper.GetNewValue(riskSetupModel, "NCDRate", model.PrivateVehiclePartial.AgeOfVehicle ?? default(decimal));
            //res.SubTotalB = thirdPartyAmount - res.ThirdPartyNoClaimDiscountAmount;
            if (days != 365)
            {
                res.SubTotalB = GetProRataOrShortScaleAmount(thirdPartyAmount);
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                detailCalculationResult.BasicAsPerCC = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = res.SubTotalB, Total = res.SubTotalB };
            }

            //   ncd
            if (model.PrivateVehiclePartial.NCDYears > 0 && model.PrivateVehiclePartial.IsComprehensive)
            {
                res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.NCDRate, model.PrivateVehiclePartial.NCDYears);
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
                if (model.PrivateVehiclePartial.IsDifferentlyAble && model.PrivateVehiclePartial.IsComprehensive)
                {
                    res.SpecialDiscountRate = (model.PrivateVehiclePartial.SpecialDiscountRate ?? default) / 100;
                    // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                    res.TPLSpecialDiscountAmount = res.SubTotalB * res.SpecialDiscountRate;
                    res.ShortScaleTPLSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.TPLSpecialDiscountAmount);
                    res.SubTotalB -= res.TPLSpecialDiscountAmount;
                    detailCalculationResult.SpecialDiscountTPL = new CalculationSubDetailAmountModel { Rate = res.SpecialDiscountRate, Amount = res.TPLSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleTPLSpecialDiscountAmount, Total = res.SubTotalB };

                }
            }
            if (model.PrivateVehiclePartial.IsLayup)
            {
                res.LayupDiscountThirdParty = res.SubTotalB * 2 / 3 / 365 * model.PrivateVehiclePartial.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                detailCalculationResult.LayupDiscountTPL = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountThirdParty, ProRataOrShortScaleAmount = res.LayupDiscountThirdParty, Total = res.SubTotalB };

            }
            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = res.SubTotalB * model.HGIShareRate / 100;
            }
            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
            #endregion

            #region Paid Driver And Passengers

            res.NumberofPassengers = model.PrivateVehiclePartial.NumberofSeatsIncludingDriver - 1;

            decimal paToDriver = 0;
            decimal paToPassengers = 0;
            //driver
            paToDriver = res.PAforDriver = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPaidDriverEnumValue);
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
            if (model.PrivateVehiclePartial.IsDirectDiscountPA)
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
            if (model.PrivateVehiclePartial.IsLayup)
            {
                res.LayupDiscountPADriver = res.PAforDriver * 2 / 3 / 365 * model.PrivateVehiclePartial.LayupDays;
                res.PAforDriver = res.PAforDriver - res.LayupDiscountPADriver;
                paToDriver = res.PAforDriver;
                detailCalculationResult.LayupDiscountPADriver = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPADriver, ProRataOrShortScaleAmount = res.LayupDiscountPADriver, Total = res.LayupDiscountPADriver };
            }
            //passenger
            var paForIndividualPassenger = ConstrantValueHelper.GetNewValue(riskSetupModel, paToPassengersEnumValue);
            paToPassengers = res.PAforPassenger = paForIndividualPassenger * (model.PrivateVehiclePartial.NumberofSeatsIncludingDriver - 1);
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
            if (model.PrivateVehiclePartial.IsDirectDiscountPA)
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
            if (model.PrivateVehiclePartial.IsLayup)
            {
                res.LayupDiscountPAPassenger = res.PAforPassenger * 2 / 3 / 365 * model.PrivateVehiclePartial.LayupDays;
                res.PAforPassenger = res.PAforPassenger - res.LayupDiscountPAPassenger;
                paToPassengers = res.PAforPassenger;
                detailCalculationResult.LayupDiscountPAPassenger = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountPAPassenger, ProRataOrShortScaleAmount = res.LayupDiscountPAPassenger, Total = res.LayupDiscountPAPassenger };
            }
            res.SubTotalC = (paToDriver + paToPassengers).RoundToFourPrecisions();
            if (model.IsCoinsurance)
            {
                res.PaidDriverAndPassengersPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate / 100).RoundToFourPrecisions();
            }
            #endregion

            #region RSMDT
            //check if rsmdt enabled
            if (model.PrivateVehiclePartial.RiotStrikeAndTerrorism)
            {
                res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
                var minimumRsmdAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.MinimumRSMDTAmount);
                if (model.MinimumRSMDT || !model.IsCoinsurance)
                {
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
                }
                else
                {
                    detailCalculationResult.MinimumRSMDTAmount = null;
                }

                if (!model.MinimumRSMDT && model.IsCoinsurance)
                {
                    minimumRsmdAmount = 0;
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = minimumRsmdAmount;
                }


                var rsmdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
                var rsmdAmount = model.PrivateVehiclePartial.SumInsuredAmount * rsmdRate;// + 0.05);//for riot &strike & md
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

                res.RiotAndStrikeAndMdAmount = rsmdAmount;
                res.RSMDTRate = rsmdRate;
                if (minimumRsmdAmount > rsmdAmount)
                {
                    res.RiotAndStrikeAndMdAmount = res.RSMDTAmount = minimumRsmdAmount;
                    isMinRSMDTAmount = true;
                }

                var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
                res.TerrorismAmount = model.PrivateVehiclePartial.SumInsuredAmount * terrorismRate;// + 0.05);//for terrorism
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
                res.TerrorismRate = terrorismRate;
                decimal paToPillionDriverAmount = 0;
                decimal paToPillionPassengerAmount = 0;

                var driverRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPaidDriverRateEnumValue);
                paToPillionDriverAmount = rsmdtSumInsuredForDriver * driverRate;//PA amount must come from input field and rate from db
                detailCalculationResult.RSMDTSumInsuredForDriver = rsmdtSumInsuredForDriver;
                res.RSMDTforDriverRate = driverRate;
                if (days != 365)
                {
                    decimal fullPaToDriver = paToPillionDriverAmount;
                    paToPillionDriverAmount = GetProRataOrShortScaleAmount(fullPaToDriver);
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = fullPaToDriver, ProRataOrShortScaleAmount = paToPillionDriverAmount, Total = paToPillionDriverAmount };
                }
                else
                {
                    detailCalculationResult.DriverRSMDT = new CalculationSubDetailAmountModel { Rate = driverRate, Amount = paToPillionDriverAmount, ProRataOrShortScaleAmount = paToPillionDriverAmount, Total = paToPillionDriverAmount };
                }
                res.PaforDriverAmount = paToPillionDriverAmount;
                var passengerRate = ConstrantValueHelper.GetNewValue(riskSetupModel, rsmdtToPassengersRateEnumValue);
                paToPillionPassengerAmount = rsmdtSumInsuredForPassenger *
                                    (model.PrivateVehiclePartial.NumberofSeatsIncludingDriver - 1) * passengerRate;//PA amount must come from input field and rate from db
                detailCalculationResult.RSMDTSumInsuredForPassengers = rsmdtSumInsuredForPassenger;
                res.RSMDTforPassengerRate = passengerRate;
                if (days != 365)
                {
                    decimal fullPaToPassengers = paToPillionPassengerAmount;
                    paToPillionPassengerAmount = GetProRataOrShortScaleAmount(fullPaToPassengers);
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = fullPaToPassengers, ProRataOrShortScaleAmount = paToPillionPassengerAmount, Total = paToPillionPassengerAmount };
                }
                else
                {
                    detailCalculationResult.PassengerRSMDT = new CalculationSubDetailAmountModel { Rate = passengerRate, Amount = paToPillionPassengerAmount, ProRataOrShortScaleAmount = paToPillionPassengerAmount, Total = paToPillionPassengerAmount };
                }
                res.PaforPassengerAmount = paToPillionPassengerAmount;
                res.SubTotalD = res.RiotAndStrikeAndMdAmount + res.TerrorismAmount + paToPillionDriverAmount + paToPillionPassengerAmount;
                res.RSMDTDriver = paToPillionDriverAmount;
                res.RSMDTPassenger = paToPillionPassengerAmount;
                if (model.PrivateVehiclePartial.IsLayup)
                {
                    if (isMinRSMDTAmount) res.SubTotalD -= res.RiotAndStrikeAndMdAmount;
                    res.LayupDiscountRSMDT = res.SubTotalD * 2 / 3 / 365 * model.PrivateVehiclePartial.LayupDays;
                    res.SubTotalD = res.SubTotalD - res.LayupDiscountRSMDT;
                    detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalD + res.RiotAndStrikeAndMdAmount };
                    if (isMinRSMDTAmount) res.SubTotalD += res.RiotAndStrikeAndMdAmount;
                }
                if (model.IsCoinsurance)
                {
                    res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalD;
                    res.SubTotalD = res.SubTotalD * model.HGIShareRate / 100;
                }
                res.SubTotalD = res.SubTotalD.RoundToFourPrecisions();
            }
            #endregion

            #region Total, Stamp and Vat
            res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC + res.SubTotalD;
            if (model.IsCoinsurance)
            {
                res.StampDutyAmount = model.StampDuty;
            }
            else
            {
                if (model.PrivateVehiclePartial.IsComprehensive)
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, model.PrivateVehiclePartial.SumInsuredAmount);
                }
                else
                {
                    res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
                }
            }
            decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PrivateVehicleConfigType.VAT);
            res.VatPercent = VATPercent * 100;
            res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
            var totalWithVat = res.TotalPremiumAmount + res.VatAmount;
            var totalWithStamp = totalWithVat + res.StampDutyAmount;
            res.PremiumAfterStamp = totalWithStamp;
            res.NetPremiumAmount = totalWithStamp;
            res.SumInsuredAmount = model.PrivateVehiclePartial.SumInsuredAmount;
            #endregion

            #region Additional Calculation
            res.PoolPremiumAmount = res.SubTotalD;
            res.ExpiryDate = DateTime.UtcNow.AddYears(1);
            res.BasicPremium = res.SubTotalA + res.SubTotalC;
            res.ThirdPartyAmount = res.SubTotalB;
            #endregion

            res.RiskDetails.PrivateVehicleDetailCalculationResult = detailCalculationResult;
            model.FullSumInsured = res.FullSumInsured = res.SumInsuredAmount;
            if (model.IsCoinsurance)
            {
                res.SumInsuredAmount = model.PrivateVehiclePartial.SumInsuredAmount = res.SumInsuredAmount * model.HGIShareRate / 100;
                res.HGIShareRate = model.HGIShareRate;
                res.IsCoinsurance = model.IsCoinsurance;
            }
            return res;
        });
    }
    #region Helper
    private decimal GetProRataOrShortScaleAmount(decimal orgAmount)
    {
        if (isProRata)
        {
            return orgAmount * days / 365M;
        }
        else
        {
            return orgAmount * shortScaleRate;
        }
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }
    #endregion
}
