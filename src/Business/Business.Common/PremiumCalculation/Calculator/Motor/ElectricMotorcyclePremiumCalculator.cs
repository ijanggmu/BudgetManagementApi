using Business.Common.Helper;
using Data.Context;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Risk;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator.Motor;
public class ElectricMotorcyclePremiumCalculator : IPolicyPremiumCalculator
{
    private readonly ApplicationDataContext _db;
    private bool isProRata;
    private decimal shortScaleRate;
    private int days;
    private bool isMinRSMDTAmount;

    public ElectricMotorcyclePremiumCalculator(ApplicationDataContext db)
    {
        shortScaleRate = 0;
        isProRata = false;
        days = 0;
        isMinRSMDTAmount = false;
        _db = db;
    }
    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.ElectricMotorcycle
        };
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        #region Initialization
        model.ElectricMotorcyclePartial.IsThirdParty = true;
        var res = new PremiumCalculationResultModel();
        var detailCalculationResult = new ElectricMotorcycleDetailCalculationResult();
        DateViewModel dateViewModel = new DateViewModel();
        model.VehicleNumber = model.ElectricMotorcyclePartial.RegistrationNumber;
        res.VehicleCapacity = model.ElectricMotorcyclePartial.KiloWatt ?? 0;
        res.ElectricMotorcycleRiskViewModel = new ElectricMotorcycleRiskViewModel();

        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();
        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);

        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
        var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

        //var riskSetupModel = _mapper.Map<List<CalculationConfigurationViewModel>>(_calculationConfigurationRepository.GetAll(x => x.PortfolioAlias == model.PortfolioAlias).ToList());
        //var globalriskSetupModel = _mapper.Map<List<GlobalConfigurationViewModel>>(_globalConfigurationRepository.GetAll().ToList());
        res.Days = Convert.ToInt16(model.ElectricMotorcyclePartial.Days);

        bool IsShortScale = false;
        if (res.Days != 365)
        {
            IsShortScale = true;
        }

        if (IsShortScale && model.ElectricMotorcyclePartial.IsComprehensive)
        {
            shortScaleRate = res.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.ElectricMotorcycleShortScaleRate, res.Days);
        }
        else
        {
            shortScaleRate = res.ShortScaleRate = 1;
        }

        var ageAndDepreciationTuple = new Tuple<DateViewModel, string>(null, "");
        //calculate period values
        if (model.ElectricMotorcyclePartial.PurchasedNewOld)   //// true for new and false for old
        {
            ageAndDepreciationTuple = await GetVehicleAgeAndDepreciation(model.ElectricMotorcyclePartial.DateOfPurchase ?? default(DateTime), riskSetupModel);
            dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, model.ElectricMotorcyclePartial.DateOfPurchase ?? default(DateTime));
        }
        else
        {
            ageAndDepreciationTuple = await GetVehicleAgeAndDepreciation(Convert.ToDateTime(model.ElectricMotorcyclePartial.YearsFromRegistrationDateYears), riskSetupModel);
            dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, Convert.ToDateTime(model.ElectricMotorcyclePartial.YearsFromRegistrationDateYears));

        }
        var ageStringNepali = DateHelper.GetNepaliYearsMonthDayString(dateViewModel);
        model.ElectricMotorcyclePartial.AgeForPrint = ageStringNepali;
        var ageStringEnglish = DateHelper.GetEnglishYearsMonthDayString(dateViewModel);
        model.ElectricMotorcyclePartial.AgeForPrintEnglish = ageStringEnglish;
        model.ElectricMotorcyclePartial.AgeOfVehicle = Convert.ToDecimal(ageAndDepreciationTuple.Item1.PeriodDifference);
        detailCalculationResult.AgeDifference = DateHelper.GetYearsMonthDayString(dateViewModel);

        //discard agent for policies with third party only
        if (!model.ElectricMotorcyclePartial.IsComprehensive)
        {
            //res.RiskTypeSelected = _branchConfigRepository.Count(x => x.BranchCode == model.BranchCode) > 0 ? model.RiskTypeSelected = RiskTypeSelected.ThirdParty2 : model.RiskTypeSelected = RiskTypeSelected.ThirdParty;
            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.ThirdParty2;
            model.Agent = null;
            model.AgentName = null;
            model.AgentId = null;
        }
        #endregion

        #region CalculateSumInsured
        //price of motorcycle model
        decimal currentMarketPrice = Convert.ToDecimal(model.ElectricMotorcyclePartial.CurrentMarketPrice);//get from db
        if (!model.ElectricMotorcyclePartial.EnterSumInsured)
        {
            model.ElectricMotorcyclePartial.ValueWithoutAccessories = (currentMarketPrice - (currentMarketPrice * Convert.ToDecimal(ageAndDepreciationTuple.Item2))).ToString();
            model.ElectricMotorcyclePartial.SumInsuredAmount = Math.Round(Convert.ToDecimal(model.ElectricMotorcyclePartial.ValueWithoutAccessories) + Convert.ToDecimal(model.ElectricMotorcyclePartial.ValueOfAccessories), MidpointRounding.AwayFromZero);
        }
        else
        {
            model.ElectricMotorcyclePartial.SumInsuredAmount = Convert.ToDecimal(model.ElectricMotorcyclePartial.ValueWithoutAccessories) + Convert.ToDecimal(model.ElectricMotorcyclePartial.ValueOfAccessories);
        }

        res.SumInsuredAmount = model.ElectricMotorcyclePartial.SumInsuredAmount;

        #region GOVT
        int basicPremiumRateEnumValue = (int)ElectricMotorcycleConfigType.BasicPremiumRate;
        int loadingRateEnumValue = (int)ElectricMotorcycleConfigType.LoadingRate;
        int paToRiderandonePillionRiderRateEnumValue = (int)ElectricMotorcycleConfigType.PAToRiderandonePillionRiderRate;
        int riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricMotorcycleConfigType.RiotAndStrikeAndMDAmountRate;
        int terrorismAmountRateEnumValue = (int)ElectricMotorcycleConfigType.TerrorismAmountRate;
        int ecoFriendlyDiscountRateEnumValue = (int)ElectricMotorcycleConfigType.EcoFriendlyDiscountRate;

        if (model.ApplyGovtConfig)
        {
            basicPremiumRateEnumValue = (int)ElectricMotorcycleConfigType.GovtBasicPremiumRate;
            loadingRateEnumValue = (int)ElectricMotorcycleConfigType.GovtLoadingRate;
            paToRiderandonePillionRiderRateEnumValue = (int)ElectricMotorcycleConfigType.GovtPAToRiderandonePillionRiderRate;
            riotAndStrikeAndMDAmountRateEnumValue = (int)ElectricMotorcycleConfigType.GovtRiotAndStrikeAndMDAmountRate;
            terrorismAmountRateEnumValue = (int)ElectricMotorcycleConfigType.GovtTerrorismAmountRate;
            ecoFriendlyDiscountRateEnumValue = (int)ElectricMotorcycleConfigType.GovtEcoFriendlyDiscountRate;
        }
        #endregion

        #endregion
        var minimumBasicPremiumAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.MinimumBasicPremiumAmount);

        #region Basic Premium
        if (model.ElectricMotorcyclePartial.IsComprehensive)
        {
            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndThirdParty;
            var BasicPremiumRate = ConstrantValueHelper.GetNewValue(riskSetupModel, basicPremiumRateEnumValue);
            res.BasicPremiumRate = BasicPremiumRate * 100;
            var fullAmount = res.PrimaryBasicPremiumAmount = BasicPremiumRate * model.ElectricMotorcyclePartial.SumInsuredAmount;

            var BasicPremiumShortScale = res.PrimaryBasicPremiumAmount;
            if (IsShortScale)
            {
                BasicPremiumShortScale = GetProRataOrShortScaleAmount(res.PrimaryBasicPremiumAmount);
                res.PrimaryBasicPremiumAmount = BasicPremiumShortScale;
            }
            detailCalculationResult.BasicPremium = new CalculationSubDetailAmountModel() { Rate = res.BasicPremiumRate, Amount = fullAmount, ProRataOrShortScaleAmount = BasicPremiumShortScale, Total = BasicPremiumShortScale };

            //eco friendly discount
            decimal ecoFriendlyDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, ecoFriendlyDiscountRateEnumValue);
            var EcoFriendlyDiscountAmount = res.EcoFriendlyDiscountAmount = ecoFriendlyDiscountRate * fullAmount;
            fullAmount -= res.EcoFriendlyDiscountAmount;

            if (IsShortScale)
            {
                res.EcoFriendlyDiscountAmount = GetProRataOrShortScaleAmount(res.EcoFriendlyDiscountAmount);
            }
            var EcofriendlyDiscountedAmount = BasicPremiumShortScale - res.EcoFriendlyDiscountAmount;
            detailCalculationResult.EcoFriendlyDiscount = new CalculationSubDetailAmountModel() { Rate = ecoFriendlyDiscountRate * 100, Amount = EcoFriendlyDiscountAmount, ProRataOrShortScaleAmount = res.EcoFriendlyDiscountAmount, Total = EcofriendlyDiscountedAmount };

            //age loading
            decimal ageLoadiingRate = ConstrantValueHelper.GetNewValue(riskSetupModel, loadingRateEnumValue, model.ElectricMotorcyclePartial.AgeOfVehicle ?? default(decimal));
            var AgeLoadingAmount = res.AgeLoadingAmount = Math.Abs(fullAmount) * ageLoadiingRate;
            fullAmount += res.AgeLoadingAmount;

            if (IsShortScale)
            {
                res.AgeLoadingAmount = GetProRataOrShortScaleAmount(res.AgeLoadingAmount);
            }
            var basicTotal = EcofriendlyDiscountedAmount + res.AgeLoadingAmount;
            detailCalculationResult.LoadingForOldVehicle = new CalculationSubDetailAmountModel() { Rate = ageLoadiingRate * 100, Amount = AgeLoadingAmount, ProRataOrShortScaleAmount = res.AgeLoadingAmount, Total = basicTotal };

            if (model.ElectricMotorcyclePartial.VoluntaryExcess != 0)
            {
                res.VoluntaryExcessRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.VoluntaryExcessRate, model.ElectricMotorcyclePartial.VoluntaryExcess);
                var veAmount = res.VoluntaryExcessAmount = Math.Abs(fullAmount) * res.VoluntaryExcessRate;
                fullAmount -= res.VoluntaryExcessAmount;
                if (IsShortScale)
                {
                    res.VoluntaryExcessAmount = GetProRataOrShortScaleAmount(res.VoluntaryExcessAmount);
                }

                basicTotal -= res.VoluntaryExcessAmount;
                detailCalculationResult.SelectedVoluntaryExcess = model.ElectricMotorcyclePartial.VoluntaryExcess;
                detailCalculationResult.VoluntaryExcess = new CalculationSubDetailAmountModel() { Rate = res.VoluntaryExcessRate, Amount = veAmount, ProRataOrShortScaleAmount = res.VoluntaryExcessAmount, Total = basicTotal };
            }

            //   ncd
            if (model.ElectricMotorcyclePartial.NCDYears > 0)
            {
                res.NoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.NCDRate, model.ElectricMotorcyclePartial.NCDYears);
                var ncdAmount = res.NoClaimDiscountAmount = Math.Abs(fullAmount) * res.NoClaimDiscountRate;
                fullAmount -= res.NoClaimDiscountAmount;
                if (IsShortScale)
                {
                    res.NoClaimDiscountAmount = GetProRataOrShortScaleAmount(ncdAmount);
                }

                basicTotal -= res.NoClaimDiscountAmount;
                detailCalculationResult.NCD = new CalculationSubDetailAmountModel() { Rate = res.NoClaimDiscountRate, Amount = ncdAmount, ProRataOrShortScaleAmount = res.NoClaimDiscountAmount, Total = basicTotal };
            }

            //if all goes well check policy issue date. for the first year there is 0% discount

            int policyIssuedYear = model.ElectricMotorcyclePartial.PreviousPolicyIssuedYear ?? default(int); //get this data from db
            var amountAfterNcd = basicTotal;
            res.CalculatedSubTotalA = amountAfterNcd;
            //check if policy is issued through agent. if yes  direct discount is not applicable
            if (model.AgentId == null && !model.ApplyGovtConfig)
            {
                var ddRate = res.DirectDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.DirectDiscount);
                var ddAmount = res.DirectDiscountAmount = ddRate * Math.Abs(fullAmount);
                fullAmount -= ddAmount;

                if (IsShortScale)
                {
                    res.DirectDiscountAmount = GetProRataOrShortScaleAmount(res.DirectDiscountAmount);
                }
                res.CalculatedSubTotalA = res.CalculatedSubTotalA - res.DirectDiscountAmount;
                detailCalculationResult.DirectDiscount = new CalculationSubDetailAmountModel() { Rate = ddRate * 100, Amount = ddAmount, ProRataOrShortScaleAmount = res.DirectDiscountAmount, Total = res.CalculatedSubTotalA };

                //  res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel() { GroupName = "Direct Discount", Title = "Less: Direct Discount", Rate = ddRate, Amount = ddAmount, Total = res.CalculatedSubTotalA });
            }
            if (model.ElectricMotorcyclePartial.IsLayup)
            {
                res.LayupDiscountOwnDamage = ((res.CalculatedSubTotalA * 2 / 3) / 365) * model.ElectricMotorcyclePartial.LayupDays;
                res.CalculatedSubTotalA = res.CalculatedSubTotalA - res.LayupDiscountOwnDamage;
                detailCalculationResult.LayupDiscountOwnDamage = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountOwnDamage, ProRataOrShortScaleAmount = res.LayupDiscountOwnDamage, Total = res.CalculatedSubTotalA };

            }
            res.SubTotalA = res.MinbasicPremium = minimumBasicPremiumAmount;
            var MinimumBasicShortScale = minimumBasicPremiumAmount;
            if (IsShortScale)
            {
                res.SubTotalA = res.MinbasicPremium = MinimumBasicShortScale = GetProRataOrShortScaleAmount(minimumBasicPremiumAmount);
            }
            detailCalculationResult.MinimumBasicPremium = new CalculationSubDetailAmountModel() { Amount = minimumBasicPremiumAmount, ProRataOrShortScaleAmount = MinimumBasicShortScale, Total = MinimumBasicShortScale };

            if (res.CalculatedSubTotalA > MinimumBasicShortScale) res.SubTotalA = res.CalculatedSubTotalA;
            res.TypeOfPolicy = "Own Damage Premium";

            if (model.AgentId != null)
            {
                res.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                res.AgentCommissonAmount = res.AgentCommissonRate * Math.Abs(res.SubTotalA) / 100;

                res.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                res.NepalAgentTDSAmount = res.AgentCommissonAmount * res.NepalAgentTDSRate / 100;
            }

            if (model.ElectricMotorcyclePartial.IsDifferentlyAble && res.CalculatedSubTotalA > minimumBasicPremiumAmount)
            {
                res.SpecialDiscountRate = (model.ElectricMotorcyclePartial.SpecialDiscountRate ?? default(decimal)) / 100;
                // res.OwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(basicTotal * res.SpecialDiscountRate);
                res.OwnDamageSpecialDiscountAmount = res.SubTotalA * res.SpecialDiscountRate;
                res.ShortScaleOwnDamageSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.OwnDamageSpecialDiscountAmount);
                res.SubTotalA -= res.OwnDamageSpecialDiscountAmount;
                detailCalculationResult.SpecialDiscount = new CalculationSubDetailAmountModel() { Rate = res.SpecialDiscountRate, Amount = res.OwnDamageSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleOwnDamageSpecialDiscountAmount, Total = res.SubTotalA };

                // res.MotorRiskViewModel.Owndamagepremium.Add(new RiskBaseViewModel() { GroupName = "Differently Abled People", Title = $"Special Discount ", Rate = res.SpecialDiscountRate, Amount = res.OwnDamageSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleOwnDamageSpecialDiscountAmount, Total = res.SubTotalA });

            }
            if (model.IsCoinsurance)
            {
                res.BasicPremiumWithoutCoinsuranceRate = res.SubTotalA;
                res.SubTotalA = (res.SubTotalA * model.HGIShareRate) / 100;
            }
            res.SubTotalA = res.SubTotalA.RoundToFourPrecisions();
        }
        #endregion

        #region Third Party
        //if (model.ElectricMotorcyclePartial.IsThirdParty)
        //{
        decimal totalTPL = 0;
        decimal thirdPartyAmount = totalTPL = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.PerKWBasicAmount, model.ElectricMotorcyclePartial.KiloWatt ?? 0);

        //  res.MotorRiskViewModel.ThirdPary.Add(new RiskBaseViewModel() { GroupName = "TPL", Title = $"Basic as per c.c ({model.ElectricMotorcyclePartial.KiloWatt})", Rate = null, Amount = thirdPartyAmount, Total = thirdPartyAmount });

        // res.ThirdPartyNoClaimDiscountAmount = thirdPartyAmount * ConstrantValueHelper.GetValue(riskSetupModel, "NCDRate", model.ElectricMotorcyclePartial.AgeOfVehicle ?? default(decimal));

        // res.SubTotalB = thirdPartyAmount - res.ThirdPartyNoClaimDiscountAmount;
        var thirdPartyShortScale = thirdPartyAmount;
        if (IsShortScale)
        {
            thirdPartyShortScale = GetProRataOrShortScaleAmount(thirdPartyAmount);
        }
        res.SubTotalB = res.TPLperKW = thirdPartyShortScale;
        detailCalculationResult.TPLAsPerKW = new CalculationSubDetailAmountModel() { Amount = thirdPartyAmount, ProRataOrShortScaleAmount = thirdPartyShortScale, Total = res.SubTotalB };
        //}

        //   ncd
        if (model.ElectricMotorcyclePartial.NCDYears > 0 && model.ElectricMotorcyclePartial.IsComprehensive)
        {
            res.ThirdPartyNoClaimDiscountRate = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.NCDRate, model.ElectricMotorcyclePartial.NCDYears);
            decimal ncdAmount = totalTPL * res.ThirdPartyNoClaimDiscountRate;

            if (IsShortScale)
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
            if (model.ElectricMotorcyclePartial.IsComprehensive && model.ElectricMotorcyclePartial.IsDifferentlyAble)
            {
                res.SpecialDiscountRate = (model.ElectricMotorcyclePartial.SpecialDiscountRate ?? default(decimal)) / 100;
                res.TPLSpecialDiscountAmount = res.SubTotalB * res.SpecialDiscountRate;
                res.ShortScaleTPLSpecialDiscountAmount = GetProRataOrShortScaleAmount(res.TPLSpecialDiscountAmount);
                res.SubTotalB -= res.TPLSpecialDiscountAmount;
                detailCalculationResult.SpecialDiscountTPL = new CalculationSubDetailAmountModel() { Rate = res.SpecialDiscountRate, Amount = res.TPLSpecialDiscountAmount, ProRataOrShortScaleAmount = res.ShortScaleTPLSpecialDiscountAmount, Total = res.SubTotalB };
            }
        }
        if (model.ElectricMotorcyclePartial.IsLayup)
        {
            res.LayupDiscountThirdParty = ((res.SubTotalB * 2 / 3) / 365) * model.ElectricMotorcyclePartial.LayupDays;
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

        #region RSMDT
        //check if rsmdt enabled
        if (model.ElectricMotorcyclePartial.RiotStrike)
        {
            res.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.AllRisks;
            //get the rate from db
            var riotAndStrikeAndMdRate = ConstrantValueHelper.GetNewValue(riskSetupModel, riotAndStrikeAndMDAmountRateEnumValue);
            var minRSMDTAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.MinimumRSMDTAmount);
            var minRSMDShortScale = minRSMDTAmount;
            if (IsShortScale)
            {
                minRSMDShortScale = GetProRataOrShortScaleAmount(minRSMDTAmount);
            }
            detailCalculationResult.MinimumRSMDTAmount = new CalculationSubDetailAmountModel() { Amount = minRSMDTAmount, ProRataOrShortScaleAmount = minRSMDShortScale, Total = minRSMDShortScale };


            var totalRSMD = res.RiotAndStrikeAndMdAmount = model.ElectricMotorcyclePartial.SumInsuredAmount * riotAndStrikeAndMdRate;
            res.RSMDTRate = riotAndStrikeAndMdRate;
            var RSMDShortScale = res.RiotAndStrikeAndMdAmount;
            if (IsShortScale)
            {
                RSMDShortScale = GetProRataOrShortScaleAmount(res.RiotAndStrikeAndMdAmount);
                res.RiotAndStrikeAndMdAmount = RSMDShortScale;
            }
            detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = riotAndStrikeAndMdRate * 100, Amount = totalRSMD, ProRataOrShortScaleAmount = RSMDShortScale, Total = RSMDShortScale };

            if (RSMDShortScale < minRSMDShortScale)
            {
                RSMDShortScale = minRSMDShortScale;
                res.RiotAndStrikeAndMdAmount = RSMDShortScale;
                detailCalculationResult.RiotAndStrikeMD = new CalculationSubDetailAmountModel() { Rate = riotAndStrikeAndMdRate * 100, Amount = totalRSMD, ProRataOrShortScaleAmount = RSMDShortScale, Total = RSMDShortScale };
                isMinRSMDTAmount = true;
            }

            // res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel() { GroupName = "RSMDT", Title = "Riot & Strike and MD ", Rate = riotAndStrikeAndMdRate, Amount = model.ElectricMotorcyclePartial.SumInsuredAmount, Total = res.RiotAndStrikeAndMdAmount });
            var terrorismRate = ConstrantValueHelper.GetNewValue(riskSetupModel, terrorismAmountRateEnumValue);
            var totalTerrorism = res.TerrorismAmount = model.ElectricMotorcyclePartial.SumInsuredAmount * terrorismRate;
            var TerrorismShortScale = res.TerrorismAmount;
            if (IsShortScale)
            {
                TerrorismShortScale = GetProRataOrShortScaleAmount(res.TerrorismAmount);
                res.TerrorismAmount = TerrorismShortScale;
            }
            detailCalculationResult.Terrorism = new CalculationSubDetailAmountModel() { Rate = terrorismRate * 100, Amount = totalTerrorism, ProRataOrShortScaleAmount = TerrorismShortScale, Total = TerrorismShortScale };
            res.TerrorismRate = terrorismRate;
            // + 0.05);//for terrorism
            //  res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel() { GroupName = "RSMDT", Title = "Terrorism ", Rate = terrorismRate, Amount = model.ElectricMotorcyclePartial.SumInsuredAmount, Total = res.TerrorismAmount });
            decimal paToRiderAndOnePillionRiderAmount = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.PAToRiderandonePillionRiderAmount);
            decimal paToRiderAndOnePillionRiderRate = ConstrantValueHelper.GetNewValue(riskSetupModel, paToRiderandonePillionRiderRateEnumValue);
            res.PAToDriverAndPillionRiderRate = paToRiderAndOnePillionRiderRate;

            decimal rsmdtRiderAndOnePillionRiderAmount = paToRiderAndOnePillionRiderAmount * paToRiderAndOnePillionRiderRate;
            var RSMDTToPillionShortScale = res.PayToRiderAndOnePillionRiderAmount = rsmdtRiderAndOnePillionRiderAmount;
            if (IsShortScale)
            {
                RSMDTToPillionShortScale = GetProRataOrShortScaleAmount(rsmdtRiderAndOnePillionRiderAmount);
                res.PayToRiderAndOnePillionRiderAmount = RSMDTToPillionShortScale;
            }
            detailCalculationResult.RSMDtoPillionRider = new CalculationSubDetailAmountModel() { Rate = DecimalHelper.RoundToThree(paToRiderAndOnePillionRiderRate * 100), Amount = rsmdtRiderAndOnePillionRiderAmount, ProRataOrShortScaleAmount = RSMDTToPillionShortScale, Total = RSMDTToPillionShortScale };
            //res.MotorRiskViewModel.RSMDT.Add(new RiskBaseViewModel() { GroupName = "RSMDT", Title = $"PA to Rider and one pillion rider ({paToRiderAndOnePillionRiderAmount})", Rate = paToRiderAndOnePillionRiderRate, Amount = (paToRiderAndOnePillionRiderAmount), Total = rsmdtRiderAndOnePillionRiderAmount });
            res.SubTotalC = (res.RiotAndStrikeAndMdAmount + TerrorismShortScale + RSMDTToPillionShortScale);
            if (model.ElectricMotorcyclePartial.IsLayup)
            {
                if (isMinRSMDTAmount) res.SubTotalC -= res.RiotAndStrikeAndMdAmount;
                res.LayupDiscountRSMDT = ((res.SubTotalC * 2 / 3) / 365) * model.ElectricMotorcyclePartial.LayupDays;
                res.SubTotalC = res.SubTotalC - res.LayupDiscountRSMDT;
                detailCalculationResult.LayupDiscountRSMDT = new CalculationSubDetailAmountModel() { Rate = 0.66m, Amount = res.LayupDiscountRSMDT, ProRataOrShortScaleAmount = res.LayupDiscountRSMDT, Total = res.SubTotalC };
                if (isMinRSMDTAmount) res.SubTotalC += res.RiotAndStrikeAndMdAmount;
            }
            if (model.IsCoinsurance)
            {
                res.RSMDTPremiumWithoutCoinsuranceRate = res.SubTotalC;
                res.SubTotalC = (res.SubTotalC * model.HGIShareRate) / 100;
            }
            res.SubTotalC = res.SubTotalC.RoundToFourPrecisions();
        }

        #endregion

        #region Total, Stamp and Vat
        res.TotalPremiumAmount = res.SubTotalA + res.SubTotalB + res.SubTotalC;
        if (model.IsCoinsurance && !model.IsHGILead)
        {
            res.StampDutyAmount = 0;
        }
        else
        {
            if (model.ElectricMotorcyclePartial.IsComprehensive)
            {
                res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.StampDuty, Math.Round(model.ElectricMotorcyclePartial.SumInsuredAmount));
            }
            else
            {
                res.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(globalriskSetupModel, (int)GlobalConfigType.TPLStampPrice);
            }
        }
        decimal VATPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)ElectricMotorcycleConfigType.VAT);
        res.VatPercent = VATPercent * 100;
        res.VatAmount = (VATPercent * res.TotalPremiumAmount).RoundToFourPrecisions();
        var totalWithVat = res.TotalPremiumAmount + res.VatAmount;
        var totalWithStamp = totalWithVat + res.StampDutyAmount;
        res.PremiumAfterStamp = totalWithStamp;
        res.NetPremiumAmount = totalWithStamp;

        res.SumInsuredAmount = detailCalculationResult.SumInsured = model.ElectricMotorcyclePartial.SumInsuredAmount;
        #endregion

        #region Additional Calculation
        res.PoolPremiumAmount = res.SubTotalC;
        res.ExpiryDate = DateTime.UtcNow.AddDays(365);
        res.ThirdPartyAmount = res.SubTotalB;
        res.BasicPremium = res.SubTotalA;
        #endregion

        res.RiskDetails.ElectricMotorcycleDetailCalculationResult = detailCalculationResult;
        model.FullSumInsured = res.FullSumInsured = res.SumInsuredAmount;
        if (model.IsCoinsurance)
        {
            res.SumInsuredAmount = model.ElectricMotorcyclePartial.SumInsuredAmount = (res.SumInsuredAmount * model.HGIShareRate) / 100;
            res.HGIShareRate = model.HGIShareRate;
            res.IsCoinsurance = model.IsCoinsurance;
        }
        return res;
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }
    private async Task<Tuple<DateViewModel, string>> GetVehicleAgeAndDepreciation(
        DateTime registrationDate, List<CalculationConfigurationViewModel> riskSetup)
    {
        return await Task.Run(() =>
        {
            DateViewModel dateViewModel = DateHelper.GetPeriodDifference(DateTime.Now, registrationDate);
            int months = dateViewModel.Months;
            if (dateViewModel.Years > 0)
            {
                months += (12 * dateViewModel.Years);
            }
            decimal minPeriodForDepreciation = ConstrantValueHelper.GetNewValue(
                riskSetup, (int)ElectricMotorcycleConfigType.MinimumPeriodForDepreciation, months);
            decimal rateOfDepreciation = 0;
            if (minPeriodForDepreciation <= months)
            {
                rateOfDepreciation = ConstrantValueHelper.GetNewValue(
                    riskSetup, (int)ElectricMotorcycleConfigType.DepreciationRate, dateViewModel.PeriodDifference);
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


    #endregion
}
