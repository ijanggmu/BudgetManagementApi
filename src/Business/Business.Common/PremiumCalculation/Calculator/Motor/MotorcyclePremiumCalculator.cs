using Models.Common.Policy.Calculation;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Risk;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;
using Business.Common.Helper;
using Models.Common.Policy.Enum;
using SharedKernel.Constant;
using SharedKernel.Helper;
using Data.Context;
using SharedKernel.Constant.Permission;

namespace Business.Common.PremiumCalculation.Calculator.Motor;
public class MotorcyclePremiumCalculator : IPolicyPremiumCalculator
{
    private readonly ApplicationDataContext _db;
    private decimal _shortScaleRate;
    private readonly bool isProRata;
    private int days;
    private bool isMinRSMDTAmount;

    public MotorcyclePremiumCalculator(ApplicationDataContext db)
    {
        _shortScaleRate = 0;
        isProRata = false;
        days = 0;
        isMinRSMDTAmount = false;
        _db = db;
    }
    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.Motorcycle
        };
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        #region Initialization

        model.MotorPartial.IsThirdParty = true;
        var res = new PremiumCalculationResultModel();
        model.VehicleNumber = model.MotorPartial.RegistrationNumber;
        res.VehicleCapacity = model.MotorPartial.CubicCapacity ?? 0;
        res.MotorRiskViewModel = new MotorRiskViewModel();
        DateViewModel dateViewModel = new DateViewModel();
        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();

        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);
        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
        
        var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

        var ageAndDepreciationTuple = new Tuple<DateViewModel, string>(null, "");
        //calculate period values
        if (model.MotorPartial.PurchasedNewOld) //// true for new and false for old
        {
            ageAndDepreciationTuple =
                await GetVehicleAgeAndDepreciation(model.MotorPartial.DateOfPurchase ?? default,
                    riskSetupModel);
            dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now,
                model.MotorPartial.DateOfPurchase ?? default);

        }
        else
        {
            ageAndDepreciationTuple =
                await GetVehicleAgeAndDepreciation(
                    Convert.ToDateTime(model.MotorPartial.YearsFromRegistrationDateYears), riskSetupModel);
            dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now,
                Convert.ToDateTime(model.MotorPartial.YearsFromRegistrationDateYears));
        }

        model.MotorPartial.AgeOfVehicle = dateViewModel.PeriodDifference;
        //this is done for Print Page
        var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
        var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
        model.MotorPartial.AgeForPrintEnglish = ageStringEnglish;
        model.MotorPartial.AgeForPrint = ageStringNepali;
        //proRata or ShortScale
        days = res.Days = Convert.ToInt16(model.MotorPartial.Days);
        //isProRata =res.IsProRata= model.MotorPartial.IsProRataOrShortScale;
        if (!isProRata && model.MotorPartial.IsComprehensive)
        {
            _shortScaleRate = res.ShortScaleRate = model.ShortScale != null
                ? model.ShortScale.Value / 100
                : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel,
                    (int)GlobalConfigType.MotorCycleShortScaleRate, days);
        }
        else
        {
            _shortScaleRate = res.ShortScaleRate = 1;
        }

        int basicPremiumRateEnumValue = (int)MotorCycleConfigType.BasicPremiumRate;
        int perCCBasicAmountEnumValue = (int)MotorCycleConfigType.PerCCBasicAmount;
        int loadingRateEnumValue = (int)MotorCycleConfigType.LoadingRate;
        int riotAndStrikeAndMDAmountRateEnumValue = (int)MotorCycleConfigType.RiotAndStrikeAndMDAmountRate;
        int terrorismAmountRateEnumValue = (int)MotorCycleConfigType.TerrorismAmountRate;
        int paToRiderandonePillionRiderRateEnumValue = (int)MotorCycleConfigType.PAToRiderandonePillionRiderRate;

        if (model.ApplyGovtConfig)
        {
            basicPremiumRateEnumValue = (int)MotorCycleConfigType.GovtBasicPremiumRate;
            perCCBasicAmountEnumValue = (int)MotorCycleConfigType.GovtPerCCBasicAmount;
            loadingRateEnumValue = (int)MotorCycleConfigType.GovtLoadingRate;
            riotAndStrikeAndMDAmountRateEnumValue = (int)MotorCycleConfigType.GovtRiotAndStrikeAndMDAmountRate;
            terrorismAmountRateEnumValue = (int)MotorCycleConfigType.GovtTerrorismAmountRate;
            paToRiderandonePillionRiderRateEnumValue =
                (int)MotorCycleConfigType.GovtPAToRiderandonePillionRiderRate;
        }

        //discard agent for policies with third party only
        if (!model.MotorPartial.IsComprehensive)
        {
            res.RiskTypeSelected =
                //_branchConfigRepository.Count(x => x.BranchCode == model.BranchCode) > 0
                //? model.RiskTypeSelected = RiskTypeSelected.ThirdParty2
                //:
                model.RiskTypeSelected = RiskTypeSelected.ThirdParty;
            model.Agent = null;
            model.AgentName = null;
            model.AgentId = null;
        }
        if (!model.IsCoinsurance)
        {
            model.MinimumBasicPremium = false;
            model.MinimumRSMDT = false;
        }
        #endregion

        #region CalculateSumInsured

        //price of motorcycle model
        decimal currentMarketPrice = Convert.ToDecimal(model.MotorPartial.CurrentMarketPrice); //get from db
        if (model.MotorPartial.EnterSumInsured)
        {
            model.MotorPartial.SumInsuredAmount = res.SumInsuredAmount =
                Convert.ToDecimal(model.MotorPartial.ValueWithoutAccessories) +
                Convert.ToDecimal(model.MotorPartial.ValueOfAccessories);
        }
        else
        {
            model.MotorPartial.ValueWithoutAccessories = (currentMarketPrice -
                                                          currentMarketPrice *
                                                           Convert.ToDecimal(ageAndDepreciationTuple.Item2))
                .ToString();
            model.MotorPartial.SumInsuredAmount =
                Math.Round(
                    Convert.ToDecimal(model.MotorPartial.ValueWithoutAccessories) +
                    Convert.ToDecimal(model.MotorPartial.ValueOfAccessories), MidpointRounding.AwayFromZero);
            res.SumInsuredAmount = model.MotorPartial.SumInsuredAmount;
        }

        #endregion

        #region Basic Premium

        var minimumBasicPremiumAmount =
            ConstrantValueHelper.GetNewValue(riskSetupModel, (int)MotorCycleConfigType.MinimumBasicPremiumAmount);

        if (model.IsCoinsurance && model.MinimumBasicPremium)
        {
            minimumBasicPremiumAmount = model.MinimumBasicPremiumAmount;
        }

        if (model.MotorPartial.IsComprehensive)
        {
            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
            var BasicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
            res.BasicPremiumRate = BasicPremiumRate * 100;
            var fullBasicPremium = res.PrimaryBasicPremiumAmount = BasicPremiumRate * res.SumInsuredAmount;
            //if(res.BasicPremium < minimumBasicPremiumAmount)
            //{
            //    res.BasicPremium = minimumBasicPremiumAmount;
            //}
            if (days != 365)
            {
                res.PrimaryBasicPremiumAmount = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Basic",
                    Title =
                        $"Basic Premium  (SumInsured: {res.SumInsuredAmount.RoundWithoutDecimalNepaliFormat()})",
                    Rate = BasicPremiumRate,
                    Amount = fullBasicPremium,
                    ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount,
                    Total = res.PrimaryBasicPremiumAmount
                });
            }
            else
            {
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Basic",
                    Title =
                        $"Basic Premium  (SumInsured: {res.SumInsuredAmount.RoundWithoutDecimalNepaliFormat()})",
                    Rate = BasicPremiumRate,
                    Amount = res.PrimaryBasicPremiumAmount,
                    ProRataOrShortScaleAmount = res.PrimaryBasicPremiumAmount,
                    Total = res.PrimaryBasicPremiumAmount
                });
            }

            //ageLoading
            decimal ageLoadiingRate = res.LoadingRate = ConstrantValueHelper.GetNewValue(riskSetupModel,
                loadingRateEnumValue, model.MotorPartial.AgeOfVehicle ?? default);

            res.AgeLoadingAmount = fullBasicPremium * ageLoadiingRate;
            res.AgeLoadingRate = ageLoadiingRate * 100;
            var basicTotal = fullBasicPremium + res.AgeLoadingAmount;
            decimal proRataOrShortScaleBasicTotal = 0;
            if (days != 365)
            {
                var fullAgeLoadingAmount = res.AgeLoadingAmount;
                res.AgeLoadingAmount = GetProRataOrShortScaleAmount(fullAgeLoadingAmount);
                proRataOrShortScaleBasicTotal = res.AgeLoadingAmount + res.PrimaryBasicPremiumAmount;
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Age",
                    Title =
                        $"Add: Loading as per age ({DateHelper.GetYearsMonthDayString(ageAndDepreciationTuple.Item1)})",
                    Rate = ageLoadiingRate,
                    Amount = fullAgeLoadingAmount,
                    ProRataOrShortScaleAmount = res.AgeLoadingAmount,
                    Total = proRataOrShortScaleBasicTotal
                });
            }
            else
            {
                proRataOrShortScaleBasicTotal = res.AgeLoadingAmount + res.PrimaryBasicPremiumAmount;
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Age",
                    Title =
                        $"Add: Loading as per age ({DateHelper.GetYearsMonthDayString(ageAndDepreciationTuple.Item1)})",
                    Rate = ageLoadiingRate,
                    Amount = res.AgeLoadingAmount,
                    ProRataOrShortScaleAmount = res.AgeLoadingAmount,
                    Total = basicTotal
                });
            }

            //deduct voluntary excess

            if (model.MotorPartial.VoluntaryExcess != 0)
            {
                decimal veRate = ConstrantValueHelper.GetNewValue(riskSetupModel,
                    (int)MotorCycleConfigType.VoluntaryExcessRate, model.MotorPartial.VoluntaryExcess);
                res.VoluntaryExcessRate = veRate * 100;
                res.VoluntaryExcessAmount = basicTotal * veRate;
                basicTotal -= res.VoluntaryExcessAmount;
                if (days != 365)
                {
                    var fullVEAmount = res.VoluntaryExcessAmount;
                    res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(fullVEAmount);

                    proRataOrShortScaleBasicTotal -= res.VoluntaryExcessAmount;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "VE",
                        Title = $"Less: Voluntary Excess percentage ({model.MotorPartial.VoluntaryExcess})",
                        Rate = veRate,
                        Amount = fullVEAmount,
                        ProRataOrShortScaleAmount = res.VoluntaryExcessAmount,
                        Total = proRataOrShortScaleBasicTotal
                    });
                }
                else
                {
                    proRataOrShortScaleBasicTotal -= res.VoluntaryExcessAmount;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "VE",
                        Title = $"Less: Voluntary Excess percentage ({model.MotorPartial.VoluntaryExcess})",
                        Rate = veRate,
                        Amount = res.VoluntaryExcessAmount,
                        ProRataOrShortScaleAmount = res.VoluntaryExcessAmount,
                        Total = basicTotal
                    });
                }
            }


            //if all goes well check policy issue date. for the first year there is 0% discount

            int policyIssuedYear =
                model.MotorPartial.PreviousPolicyIssuedYear ?? default; //get this data from db

            //   ncd
            if (model.MotorPartial.NCDYears > 0)
            {
                res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel,
                    (int)MotorCycleConfigType.NCDRate, model.MotorPartial.NCDYears);
                decimal ncdAmount = Math.Abs(basicTotal) * res.NoClaimDiscountRate;
                basicTotal -= ncdAmount;

                if (days != 365)
                {
                    res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                    proRataOrShortScaleBasicTotal -= res.NoClaimDiscountAmount;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "NCD",
                        Title = $"Less: NCD ",
                        Rate = res.NoClaimDiscountRate,
                        Amount = ncdAmount,
                        ProRataOrShortScaleAmount = res.NoClaimDiscountAmount,
                        Total = proRataOrShortScaleBasicTotal
                    });
                }
                else
                {
                    res.NoClaimDiscountAmount = ncdAmount;
                    proRataOrShortScaleBasicTotal -= res.NoClaimDiscountAmount;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "NCD",
                        Title = $"Less: NCD ",
                        Rate = res.NoClaimDiscountRate,
                        Amount = ncdAmount,
                        ProRataOrShortScaleAmount = ncdAmount,
                        Total = basicTotal
                    });
                }
            }

            res.CalculatedSubTotalA = proRataOrShortScaleBasicTotal;
            //check if policy is issued through agent. if yes  direct discount is not applicable
            if (model.AgentId == null && !model.ApplyGovtConfig)
            {
                var ddRate = res.DirectDiscountRate =
                    ConstrantValueHelper.GetNewValue(riskSetupModel, (int)MotorCycleConfigType.DirectDiscount);
                var ddAmount = res.DirectDiscountAmount = ddRate * Math.Abs(basicTotal);
                basicTotal -= ddAmount;
                if (days != 365)
                {
                    var proRataOrShortScaleDDAmount = GetProRataOrShortScaleAmount(res.DirectDiscountAmount);
                    res.DirectDiscountAmount = proRataOrShortScaleDDAmount;
                    proRataOrShortScaleBasicTotal -= proRataOrShortScaleDDAmount;
                    res.CalculatedSubTotalA = proRataOrShortScaleBasicTotal;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "Direct Discount",
                        Title = "Less: Direct Discount",
                        Rate = ddRate,
                        Amount = ddAmount,
                        ProRataOrShortScaleAmount = proRataOrShortScaleDDAmount,
                        Total = res.CalculatedSubTotalA
                    });
                }
                else
                {
                    res.CalculatedSubTotalA = basicTotal;
                    res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                    {
                        GroupName = "Direct Discount",
                        Title = "Less: Direct Discount",
                        Rate = ddRate,
                        Amount = ddAmount,
                        ProRataOrShortScaleAmount = ddAmount,
                        Total = res.CalculatedSubTotalA
                    });
                }

            }
            else
            {
                res.AgentCommissonRate =
                    ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel,
                        (int)GlobalConfigType.AgentCommissionRate);
                res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(basicTotal) / 100;
                if (days != 365) res.AgentCommissonAmount = GetProRataOrShortScaleAmount(res.AgentCommissonAmount);

                res.NepalAgentTDSRate =
                    ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel,
                        (int)GlobalConfigType.NepalAgentTDSRate);
                res.NepalAgentTDSAmount = res.NepalAgentTDSRate * res.AgentCommissonAmount / 100;
            }

            if (model.MotorPartial.IsLayup)
            {
                res.LayupDiscountOwnDamage = basicTotal * 2 / 3 / 365 * model.MotorPartial.LayupDays;
                res.CalculatedSubTotalA = basicTotal - res.LayupDiscountOwnDamage;
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Layup Discount",
                    Title = "Less: Lay Discount",
                    Rate = 0.66m,
                    Amount = res.LayupDiscountOwnDamage,
                    ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage,
                    Total = res.CalculatedSubTotalA
                });

            }

            // var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)MotorCycleConfigType.MinimumBasicPremiumAmount);
            decimal fullMinBasicPremiumAmount = minimumBasicPremiumAmount;
            // if (model.MinimumBasicPremium || !model.IsCoinsurance)
            // {
            if (days != 365)
            {
                minimumBasicPremiumAmount = GetProRataOrShortScaleAmount(fullMinBasicPremiumAmount);
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Minimum Own Damage Premium",
                    Title = "Compare: Minimum Own Damage Premium",
                    Amount = fullMinBasicPremiumAmount,
                    ProRataOrShortScaleAmount = minimumBasicPremiumAmount,
                    Total = minimumBasicPremiumAmount
                });
            }
            else
            {
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Minimum Own Damage Premium",
                    Title = "Compare: Minimum Own Damage Premium",
                    Amount = minimumBasicPremiumAmount,
                    ProRataOrShortScaleAmount = minimumBasicPremiumAmount,
                    Total = minimumBasicPremiumAmount
                });
            }
            res.MinbasicPremium = minimumBasicPremiumAmount;
            res.SubTotalA = minimumBasicPremiumAmount;


            if (res.CalculatedSubTotalA > minimumBasicPremiumAmount)
            {
                res.SubTotalA = res.CalculatedSubTotalA;
            }
            else
            {
                res.SubTotalA = minimumBasicPremiumAmount;
            }



            if (model.MotorPartial.IsDifferentlyAble && res.CalculatedSubTotalA > minimumBasicPremiumAmount)
            {
                res.SpecialDiscountRate = (model.MotorPartial.SpecialDiscountRate ?? default) / 100;
                // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                res.OwnDamageSpecialDiscountAmount = res.SubTotalA * res.SpecialDiscountRate;
                res.ShortScaleOwnDamageSpecialDiscountAmount =
                    GetProRataOrShortScaleAmount(res.OwnDamageSpecialDiscountAmount);
                res.SubTotalA -= res.OwnDamageSpecialDiscountAmount;
                res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel()
                {
                    GroupName = "Differently Abled People",
                    Title = $"Special Discount ",
                    Rate = res.SpecialDiscountRate,
                    Amount = res.OwnDamageSpecialDiscountAmount,
                    ProRataOrShortScaleAmount = res.ShortScaleOwnDamageSpecialDiscountAmount,
                    Total = res.SubTotalA
                });

            }

            if (model.IsCoinsurance)
            {
                res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                res.SubTotalA = res.SubTotalA * model.HGIShareRate / 100;
            }

            res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
            res.TypeOfPolicy = "Own Damage Premium";
        }


        #endregion

        #region Third Party

        if (model.MotorPartial.IsThirdParty)
        {
            decimal totalTPL = 0;
            decimal thirdPartyAmount = totalTPL = res.ThirdPartyAmountForPrint =
                ConstrantValueHelper.GetNewValue(riskSetupModel, perCCBasicAmountEnumValue,
                    model.MotorPartial.CubicCapacity ?? 0);
            if (days != 365)
            {
                res.SubTotalB = GetProRataOrShortScaleAmount(thirdPartyAmount);
                res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                {
                    GroupName = "TPL",
                    Title = $"Basic as per c.c ({model.MotorPartial.CubicCapacity})",
                    Rate = null,
                    Amount = thirdPartyAmount,
                    ProRataOrShortScaleAmount = res.SubTotalB,
                    Total = res.SubTotalB
                });
            }
            else
            {
                res.SubTotalB = thirdPartyAmount;
                res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                {
                    GroupName = "TPL",
                    Title = $"Basic as per c.c ({model.MotorPartial.CubicCapacity})",
                    Rate = null,
                    Amount = thirdPartyAmount,
                    ProRataOrShortScaleAmount = res.SubTotalB,
                    Total = thirdPartyAmount
                });
            }

            //   ncd
            if (model.MotorPartial.IsComprehensive && model.MotorPartial.NCDYears > 0)
            {
                res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel,
                    (int)MotorCycleConfigType.NCDRate, model.MotorPartial.NCDYears);
                decimal ncdAmount = totalTPL * res.NoClaimDiscountRate;

                if (days != 365)
                {
                    res.ThirdPartyNoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                    res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                    {
                        GroupName = "TPL",
                        Title = $"Less: NCD ",
                        Rate = res.NoClaimDiscountRate,
                        Amount = ncdAmount,
                        ProRataOrShortScaleAmount = res.ThirdPartyNoClaimDiscountAmount,
                        Total = res.SubTotalB - res.ThirdPartyNoClaimDiscountAmount
                    });
                }
                else
                {
                    res.ThirdPartyNoClaimDiscountAmount = ncdAmount;
                    res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                    {
                        GroupName = "TPL",
                        Title = $"Less: NCD ",
                        Rate = res.NoClaimDiscountRate,
                        Amount = ncdAmount,
                        ProRataOrShortScaleAmount = ncdAmount,
                        Total = res.SubTotalB - ncdAmount
                    });
                }

                res.SubTotalB -= res.ThirdPartyNoClaimDiscountAmount;
            }

            if (model.MotorPartial.IsLayup)
            {
                res.LayupDiscountThirdParty = res.SubTotalB * 2 / 3 / 365 * model.MotorPartial.LayupDays;
                res.SubTotalB = res.SubTotalB - res.LayupDiscountThirdParty;
                res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                {
                    GroupName = "Layup Discount",
                    Title = "Less: Lay Discount",
                    Rate = 0.66m,
                    Amount = res.LayupDiscountThirdParty,
                    ProRataOrShortScaleAmount = res.LayupDiscountThirdParty,
                    Total = res.SubTotalB
                });

            }

            if (res.CalculatedSubTotalA > minimumBasicPremiumAmount)
            {
                if (model.MotorPartial.IsComprehensive && model.MotorPartial.IsDifferentlyAble)
                {
                    res.SpecialDiscountRate =
                        (model.MotorPartial.SpecialDiscountRate ?? default) / 100;
                    res.TPLSpecialDiscountAmount = res.SubTotalB * res.SpecialDiscountRate;
                    res.ShortScaleTPLSpecialDiscountAmount =
                        GetProRataOrShortScaleAmount(res.TPLSpecialDiscountAmount);
                    res.SubTotalB -= res.TPLSpecialDiscountAmount;
                    res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel()
                    {
                        GroupName = "",
                        Title = $"Special Discount ",
                        Rate = res.SpecialDiscountRate,
                        Amount = res.TPLSpecialDiscountAmount,
                        ProRataOrShortScaleAmount = res.ShortScaleTPLSpecialDiscountAmount,
                        Total = res.SubTotalB
                    });

                }
            }

            if (model.IsCoinsurance)
            {
                res.TPLPremiumWithoutCoinsuranceRate = res.SubTotalB;
                res.SubTotalB = res.SubTotalB * model.HGIShareRate / 100;
            }

            res.SubTotalB = res.SubTotalB.RoundToFourPrecisions();
        }

        #endregion

        #region RSMDT

        //check if rsmdt enabled
        if (model.MotorPartial.RiotStrike)
        {
            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
            //get the rate from db
            var riotAndStrikeAndMdRate =
                ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
            res.RSMDTRate = riotAndStrikeAndMdRate;

            res.RiotAndStrikeAndMdAmount = model.MotorPartial.SumInsuredAmount * riotAndStrikeAndMdRate;
            if (days != 365)
            {
                var fullRSMDTAmount = res.RiotAndStrikeAndMdAmount;
                res.RiotAndStrikeAndMdAmount = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = "Riot & Strike and MD ",
                    Rate = riotAndStrikeAndMdRate,
                    Amount = fullRSMDTAmount,
                    ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount,
                    Total = res.RiotAndStrikeAndMdAmount
                });
            }
            else
            {
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = "Riot & Strike and MD ",
                    Rate = riotAndStrikeAndMdRate,
                    Amount = res.RiotAndStrikeAndMdAmount,
                    ProRataOrShortScaleAmount = res.RiotAndStrikeAndMdAmount,
                    Total = res.RiotAndStrikeAndMdAmount
                });
            }



            var minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel,
                (int)MotorCycleConfigType.MinimumRSMDTAmount);
            res.minRSMDTAmount = minRSMDTAmount;

            if (model.MinimumRSMDT || !model.IsCoinsurance)
            {
                if (days != 365)
                {
                    var fullMinRSMDAmount = minRSMDTAmount;
                    minRSMDTAmount = GetProRataOrShortScaleAmount(minRSMDTAmount);
                    res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                    {
                        GroupName = "RSMDT",
                        Title = "Compare: Minimun RSMD Amount ",
                        Amount = fullMinRSMDAmount,
                        ProRataOrShortScaleAmount = minRSMDTAmount,
                        Total = minRSMDTAmount
                    });
                }
                else
                {
                    res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                    {
                        GroupName = "RSMDT",
                        Title = "Compare: Minimun RSMD Amount ",
                        Amount = minRSMDTAmount,
                        ProRataOrShortScaleAmount = minRSMDTAmount,
                        Total = minRSMDTAmount
                    });
                }
            }

            if (model.MinimumRSMDT || !model.IsCoinsurance)
            {
                if (res.RiotAndStrikeAndMdAmount < minRSMDTAmount)
                {
                    res.RiotAndStrikeAndMdAmount = minRSMDTAmount;
                    isMinRSMDTAmount = true;
                }
            }

            var terrorismRate = res.TerrorismRate =
                ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
            res.TerrorismAmount =
                model.MotorPartial.SumInsuredAmount * terrorismRate; // + 0.05);//for terrorism
            if (days != 365)
            {
                var fullTerrorismAmount = res.TerrorismAmount;
                res.TerrorismAmount = GetProRataOrShortScaleAmount(fullTerrorismAmount);
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = "Terrorism ",
                    Rate = terrorismRate,
                    Amount = fullTerrorismAmount,
                    ProRataOrShortScaleAmount = res.TerrorismAmount,
                    Total = res.TerrorismAmount
                });
            }
            else
            {
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = "Terrorism ",
                    Rate = terrorismRate,
                    Amount = res.TerrorismAmount,
                    ProRataOrShortScaleAmount = res.TerrorismAmount,
                    Total = res.TerrorismAmount
                });
            }

            decimal paToRiderAndOnePillionRiderAmount = res.PayToRiderAndOnePillionSumInsured =
                ConstrantValueHelper.GetNewValue(riskSetupModel,
                    (int)MotorCycleConfigType.PAToRiderandonePillionRiderAmount);
            decimal paToRiderAndOnePillionRiderRate = res.PAToDriverAndPillionRiderRate =
                ConstrantValueHelper.GetNewValue(riskSetupModel, paToRiderandonePillionRiderRateEnumValue);
            decimal rsmdtRiderAndOnePillionRiderAmount = res.PayToRiderAndOnePillionRiderAmount =
                paToRiderAndOnePillionRiderAmount * paToRiderAndOnePillionRiderRate;
            if (days != 365)
            {
                var fullRiderAndPillionAmount = rsmdtRiderAndOnePillionRiderAmount;
                rsmdtRiderAndOnePillionRiderAmount =
                    GetProRataOrShortScaleAmount(rsmdtRiderAndOnePillionRiderAmount);
                res.PayToRiderAndOnePillionRiderAmount = rsmdtRiderAndOnePillionRiderAmount;
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = $"PA to Rider and one pillion rider ({paToRiderAndOnePillionRiderAmount})",
                    Rate = paToRiderAndOnePillionRiderRate,
                    Amount = fullRiderAndPillionAmount,
                    ProRataOrShortScaleAmount = rsmdtRiderAndOnePillionRiderAmount,
                    Total = rsmdtRiderAndOnePillionRiderAmount
                });
            }
            else
            {
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "RSMDT",
                    Title = $"PA to Rider and one pillion rider ({paToRiderAndOnePillionRiderAmount})",
                    Rate = paToRiderAndOnePillionRiderRate,
                    Amount = rsmdtRiderAndOnePillionRiderAmount,
                    ProRataOrShortScaleAmount = rsmdtRiderAndOnePillionRiderAmount,
                    Total = rsmdtRiderAndOnePillionRiderAmount
                });
            }

            res.SubTotalC = res.RiotAndStrikeAndMdAmount + res.TerrorismAmount +
                            rsmdtRiderAndOnePillionRiderAmount;
            if (model.MotorPartial.IsLayup)
            {
                if (isMinRSMDTAmount) res.SubTotalC -= res.RiotAndStrikeAndMdAmount;
                res.LayupDiscountRSMDT = res.SubTotalC * 2 / 3 / 365 * model.MotorPartial.LayupDays;
                res.SubTotalC = res.SubTotalC - res.LayupDiscountRSMDT;
                res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel()
                {
                    GroupName = "Layup Discount",
                    Title = "Less: Lay Discount",
                    Rate = 0.66m,
                    Amount = res.LayupDiscountRSMDT,
                    ProRataOrShortScaleAmount = res.LayupDiscountRSMDT,
                    Total = res.SubTotalC
                });
                if (isMinRSMDTAmount) res.SubTotalC += res.RiotAndStrikeAndMdAmount;
            }

            if (model.IsCoinsurance)
            {
                res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = res.SubTotalC * model.HGIShareRate / 100;
            }

            res.SubTotalC = res.SubTotalC.RoundToFourPrecisions();
        }

        #endregion

        #region Total, Stamp and Vat

        res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC;
        if (model.IsCoinsurance)
        {
            res.StampDutyAmount = model.StampDuty;
        }
        else
        {
            if (model.MotorPartial.IsComprehensive)
            {
                res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel,
                    (int)GlobalConfigType.StampDuty, model.MotorPartial.SumInsuredAmount);
            }
            else
            {
                res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel,
                    (int)GlobalConfigType.TPLStampPrice);
            }
        }

        decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)MotorCycleConfigType.VAT);
        res.VatPercent = VATPercent * Convert.ToDecimal(100);
        res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
        var totalWithVat = res.TotalPremiumAmount + res.VatAmount; //
        var totalWithStamp = totalWithVat + res.StampDutyAmount; //for stamp duty
        res.PremiumAfterStamp = totalWithStamp;
        res.NetPremiumAmount = totalWithStamp;

        res.SumInsuredAmount = model.MotorPartial.SumInsuredAmount;

        #endregion

        #region Additional Calculation

        res.BasicPremium = res.SubTotalA;
        res.PoolPremiumAmount = res.SubTotalC;
        res.ThirdPartyAmount = res.ThirdPartyAmountNet = res.SubTotalB;
        res.RSMDTAmount = res.RiotAndStrikeAndMdAmount;
        res.ExpiryDate = DateTime.UtcNow.AddDays(365);

        #endregion

        model.FullSumInsured = res.FullSumInsured = res.SumInsuredAmount;
        if (model.IsCoinsurance)
        {
            res.SumInsuredAmount = model.MotorPartial.SumInsuredAmount =
                res.SumInsuredAmount * model.HGIShareRate / 100;
            res.HGIShareRate = model.HGIShareRate;
            res.IsCoinsurance = model.IsCoinsurance;
        }

        return res;
    }


    public async Task<Tuple<DateViewModel, string>> GetVehicleAgeAndDepreciation(DateTime registrationDate,
            List<CalculationConfigurationViewModel> riskSetup)
    {
        return await Task.Run(() =>
        {
            DateViewModel dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, registrationDate);
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += 12 * dateViewModel.Years;
            }

            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(riskSetup,
                (int)MotorCycleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(riskSetup,
                    (int)MotorCycleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
            }

            return new Tuple<DateViewModel, string>
            (
                dateViewModel,
                rateOfDepreciation.ToString() as string
            );
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
            return orgAmount * _shortScaleRate;
        }
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }

    #endregion
}
