using Business.Common.Helper;
using Models.Common.Policy.Calculation.Fire;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy.Fire;
using Models.Common.Policy.Policy;
using Newtonsoft.Json;
using SharedKernel.Constant.Permission;
using SharedKernel.Constant;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;
using Business.Common.PremiumCalculation.Service;
using SharedKernel.Helper;
using Data.Context;

namespace Business.Common.PremiumCalculation.Calculator.Fire;
public class FirePremiumCalculator : IPolicyPremiumCalculator
{
    private IShortScaleService _shortScaleService;
    private GlobalConfigType _shortScaleConfigType;
    private List<GlobalConfigurationViewModel> _globalriskSetupModel;
    private IPropertySubsidySILimitService _propertySubsidySILimitService;
    private IPropertyRiskConfigurationService _propertyRiskConfiguration;
    private readonly ApplicationDataContext _db;

    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.Household,
            PortfolioClassConstants.Property,
            PortfolioClassConstants.LossOfProfit,
        };

    public FirePremiumCalculator(
        IShortScaleService shortScaleService,
        IPropertySubsidySILimitService propertySubsidySILimitService,
        IPropertyRiskConfigurationService propertyRiskConfiguration
,
        ApplicationDataContext db)
    {
        _shortScaleService = shortScaleService;
        _propertySubsidySILimitService = propertySubsidySILimitService;
        _propertyRiskConfiguration = propertyRiskConfiguration;
        _db = db;
    }
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();

        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);
        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

        _globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);
        switch (model.PortfolioAlias)
        {
            case PortfolioClassConstants.Household:
                _shortScaleConfigType = GlobalConfigType.HouseHoldShortScaleRate;
                return await CalculateHouseholdPremium(riskSetupModel, model);
            case PortfolioClassConstants.Property:
                _shortScaleConfigType = GlobalConfigType.PropertyShortScaleRate;
                return await CalculatePropertyPremium(riskSetupModel, model);
            case PortfolioClassConstants.LossOfProfit:
                return await CalculateLOPPremium(riskSetupModel, model);
            default:
                break;
        }
        return null;
    }


    #region LOP Calculator
    private async Task<PremiumCalculationResultModel> CalculateLOPPremium(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        return await Task.Run(() =>
        {
            PremiumCalculationResultModel premiumCalculationResultModel = new PremiumCalculationResultModel();

            premiumCalculationResultModel.AnnualRevenue = model.LOPPartial.AnnualRevenue;
            premiumCalculationResultModel.InsuredAmount = model.LOPPartial.InsuredAmount;
            premiumCalculationResultModel.SumInsuredAmount = premiumCalculationResultModel.AnnualRevenue;
            premiumCalculationResultModel.BasicPremium = model.LOPPartial.AnnualPremium.RoundToFourPrecisions();
            premiumCalculationResultModel.RiskTypeSelected = RiskTypeSelected.BasicOnly;
            if (model.LOPPartial != null)
            {
                model.LOPPartial.EffectiveDate = model.EffectiveDate;
                model.LOPPartial.ExpiryDate = model.ExpiryDate;
                model.LOPPartial.ProposedDate = model.ProposedDate;
            }
            if (model.LOPPartial.IsRSMDTSelected)
            {
                premiumCalculationResultModel.PoolPremiumAmount = premiumCalculationResultModel.RSMDTAmount = (model.LOPPartial.RSMDTPremium ?? default(decimal)).RoundToFourPrecisions();
                premiumCalculationResultModel.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;
            }

            model.LOPPartial.FullAnnualPremium = premiumCalculationResultModel.BasicPremium;
            model.LOPPartial.FullAnnualRevenue = premiumCalculationResultModel.AnnualRevenue;
            model.LOPPartial.FullRSMDTPremium = premiumCalculationResultModel.PoolPremiumAmount;
            model.LOPPartial.FullInsuredAmount = premiumCalculationResultModel.InsuredAmount;

            if (model.IsCoinsurance)
            {
                premiumCalculationResultModel.AnnualRevenue = model.LOPPartial.AnnualRevenue = (model.LOPPartial.AnnualRevenue *= model.HGIShareRate / 100);
                model.LOPPartial.AnnualPremium *= model.HGIShareRate / 100;
                model.LOPPartial.InsuredAmount *= model.HGIShareRate / 100;
                premiumCalculationResultModel.IsCoinsurance = model.IsCoinsurance;
                premiumCalculationResultModel.HGIShareRate = model.HGIShareRate;
                premiumCalculationResultModel.FullSumInsured = model.FullSumInsured = premiumCalculationResultModel.SumInsuredAmount;
                premiumCalculationResultModel.SumInsuredAmount = CoinsurancePremiumCalculator.GetCoinsurancePremium(premiumCalculationResultModel.SumInsuredAmount, model.HGIShareRate);
                premiumCalculationResultModel.BasicPremiumWithoutCoinsuranceRate = premiumCalculationResultModel.BasicPremium;
                premiumCalculationResultModel.BasicPremium = CoinsurancePremiumCalculator.GetCoinsurancePremium(premiumCalculationResultModel.BasicPremium, model.HGIShareRate);
                if (model.LOPPartial.IsRSMDTSelected)
                {
                    model.LOPPartial.RSMDTPremium *= model.HGIShareRate / 100;
                    premiumCalculationResultModel.RSMDTPremiumWithoutCoinsuranceRate = premiumCalculationResultModel.PoolPremiumAmount;
                    premiumCalculationResultModel.PoolPremiumAmount = CoinsurancePremiumCalculator.GetCoinsurancePremium(premiumCalculationResultModel.PoolPremiumAmount, model.HGIShareRate);
                }
            }
            premiumCalculationResultModel.PolicyPeriod = model.LOPPartial.PolicyPeriodInDays;
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                premiumCalculationResultModel.StampDutyAmount = 0;
            }
            else
            {
                premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
               _globalriskSetupModel,
               (int)GlobalConfigType.StampDuty,
               premiumCalculationResultModel.SumInsuredAmount);
            }

            premiumCalculationResultModel.TotalPremiumAmount = premiumCalculationResultModel.BasicPremium + premiumCalculationResultModel.PoolPremiumAmount;

            premiumCalculationResultModel.VatPercent =
                ConstrantValueHelper.GetGlobalValue(_globalriskSetupModel, (int)GlobalConfigType.VATRate);
            premiumCalculationResultModel.VatAmount = premiumCalculationResultModel.TotalPremiumAmount * premiumCalculationResultModel.VatPercent;
            var totalWithVat = premiumCalculationResultModel.TotalPremiumAmount + premiumCalculationResultModel.VatAmount;
            var totalWithStamp = totalWithVat + premiumCalculationResultModel.StampDutyAmount;
            premiumCalculationResultModel.SubTotalA = totalWithStamp;
            premiumCalculationResultModel.NetPremiumAmount = premiumCalculationResultModel.SubTotalA;
            premiumCalculationResultModel.VatPercent *= Convert.ToDecimal(100);
            return premiumCalculationResultModel;
        });
    }
   #endregion

    #region Household Calculator
    private async Task<PremiumCalculationResultModel> CalculateHouseholdPremium(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        List<FireRiskConfiguration> householdConfigList = ReadConfiguration("Household");
        return await Task.Run(() =>
        {
            var test = JsonConvert.SerializeObject(model);
            PremiumCalculationResultModel premiumCalculationResultModel = new PremiumCalculationResultModel();
            model.FirePartial.PeriodOfInsuranceFrom = model.EffectiveDate;
            model.FirePartial.PeriodOfInsuranceTo = model.ExpiryDate;

            if (model.AgentId == null)
            {
                premiumCalculationResultModel.DirectDiscountRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)HouseholdConfigType.DirectDiscount : (int)HouseholdConfigType.DirectDiscount);
            }

            premiumCalculationResultModel.IsCoinsurance = model.IsCoinsurance;
            premiumCalculationResultModel.HGIShareRate = model.HGIShareRate;
            premiumCalculationResultModel.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(
                _globalriskSetupModel, (int)_shortScaleConfigType, (decimal)model.PolicyPeriodInDays);
            premiumCalculationResultModel.Days = model.PolicyPeriodInDays.Value;
            _shortScaleService.Initialize(premiumCalculationResultModel.ShortScaleRate);

            foreach (var asset in model.FirePartial.SubjectMatterOfInsurance)
            {
                if (asset.Id == null)
                {
                    Guid value = Guid.NewGuid();
                    asset.Id = value.ToString();
                }

                var firePremiumCalculationModel = new FirePremiumCalculationModel(asset.Id);
                #region SumInsuredAmount
                if (model.IsCoinsurance)
                {
                    if (asset.LoadContentsFromFile)
                    {
                        if (asset.HouseholdContentExcelData != null)
                        {
                            foreach (var item in asset.HouseholdContentExcelData)
                            {
                                item.Rate *= (model.HGIShareRate / 100);
                                item.Total *= (model.HGIShareRate / 100);
                            }
                        }
                    }
                }

                firePremiumCalculationModel.TotalValueOfContents =
                    asset.Equipment +
                    asset.RawMaterials +
                    asset.WorkInProgress +
                    asset.FinishedGoods +
                    asset.SemiFinishedGoods +
                    asset.MoneyAndJewellery +
                    asset.Art +
                    asset.FurnitureFixtureOrFitting +
                    asset.OtherItems;
                asset.SumInsuredAmount = (firePremiumCalculationModel.TotalValueOfContents + asset.ValueOfBuilding).RoundToFourPrecisions();
                decimal eachSumInsured = asset.SumInsuredAmount.RoundToFourPrecisions();
                var totalSumInsured = 0m;
                totalSumInsured += eachSumInsured;

                firePremiumCalculationModel.SumInsuredAmount = asset.SumInsuredAmount.RoundToFourPrecisions();
                premiumCalculationResultModel.SumInsuredAmount += firePremiumCalculationModel.SumInsuredAmount.RoundToFourPrecisions();

                decimal fullSumInsuredAmount = totalSumInsured;
                model.FullSumInsured = premiumCalculationResultModel.FullSumInsured = premiumCalculationResultModel.SumInsuredAmount;
                #endregion

                #region ODP
                premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;
                if (string.IsNullOrEmpty(asset.RiskCode))
                {
                    throw new Exception($"Please select Risk Types for all assets.");
                }
                firePremiumCalculationModel.RiskDetails =
                    householdConfigList.FirstOrDefault(c => c.RiskCode == asset.RiskCode);
                if (firePremiumCalculationModel.RiskDetails == null)
                {
                    throw new Exception($"Please add configuration for Risk Code {asset.RiskCode}" +
                        " in Household Configurations JSON file");
                }

                firePremiumCalculationModel.BasicPremiumRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel,
                    model.PortfolioAlias == PortfolioClassConstants.Household ?
                        (int)HouseholdConfigType.BasicPremiumRate : (int)HouseholdConfigType.BasicPremiumRate,
                    asset.SumInsuredAmount);
                asset.BuildingCompositionDescription = BuildingComposition.FirstClass.ToString();

                if (asset.LoadingMultiplier != 0 && asset.BuildingComposition == ((int)BuildingComposition.SecondClass).ToString())
                {
                    asset.BuildingCompositionDescription = BuildingComposition.SecondClass.ToString();

                    firePremiumCalculationModel.BasicPremium =
                            firePremiumCalculationModel.BasicPremiumRate * fullSumInsuredAmount / 100 * asset.LoadingMultiplier;
                    firePremiumCalculationModel.ShortScaleBasicPremium =
                        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.BasicPremium);
                    premiumCalculationResultModel.InitialBasicpremium +=
                        firePremiumCalculationModel.ShortScaleBasicPremium;
                }
                else
                {

                    firePremiumCalculationModel.BasicPremium =
                            firePremiumCalculationModel.BasicPremiumRate * fullSumInsuredAmount / 100;
                    firePremiumCalculationModel.ShortScaleBasicPremium =
                        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.BasicPremium);
                    premiumCalculationResultModel.InitialBasicpremium +=
                        firePremiumCalculationModel.ShortScaleBasicPremium;
                }


                // Direct Discount
                if (model.AgentId == null)
                {
                    firePremiumCalculationModel.DirectDiscountAmount =
                        premiumCalculationResultModel.DirectDiscountRate * firePremiumCalculationModel.BasicPremium;
                    firePremiumCalculationModel.ShortScaleDirectDiscountAmount =
                        _shortScaleService.CalculateShortScaleAmount(
                            firePremiumCalculationModel.DirectDiscountAmount);
                    premiumCalculationResultModel.DirectDiscountAmount +=
                        firePremiumCalculationModel.ShortScaleDirectDiscountAmount;
                }
                firePremiumCalculationModel.PremiumAfterDirectDiscount =
                    firePremiumCalculationModel.BasicPremium - firePremiumCalculationModel.DirectDiscountAmount;
                firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount =
                    firePremiumCalculationModel.ShortScaleBasicPremium -
                    firePremiumCalculationModel.ShortScaleDirectDiscountAmount;

                premiumCalculationResultModel.PremiumAfterDirectDiscount +=
                    firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;


                // minimum basic premium
                firePremiumCalculationModel.MinimumBasicPremiumAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)HouseholdConfigType.MinimunBasicPremiumAmount);
                firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)HouseholdConfigType.MinimunBasicPremiumAmount);
                //firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount =
                //        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.MinimumBasicPremiumAmount);
                if (firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount < firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount)
                {
                    premiumCalculationResultModel.BasicPremium += firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount;
                    firePremiumCalculationModel.BasicPremiumToDisplay = firePremiumCalculationModel.MinimumBasicPremiumAmount;
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount;
                }
                else
                {
                    premiumCalculationResultModel.BasicPremium +=
                   firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;
                    firePremiumCalculationModel.BasicPremiumToDisplay = firePremiumCalculationModel.PremiumAfterDirectDiscount;
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;
                }

                #endregion

                #region RSMDT
                if (model.FirePartial.IsRSMDT)
                {
                    premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;

                    firePremiumCalculationModel.IsRSMDTSelected = model.FirePartial.IsRSMDT;
                    firePremiumCalculationModel.RSMDRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)HouseholdConfigType.RSMDRate : (int)HouseholdConfigType.RSMDRate,
                        asset.SumInsuredAmount);
                    firePremiumCalculationModel.RSMDAmount =
                        firePremiumCalculationModel.RSMDRate * fullSumInsuredAmount;
                    firePremiumCalculationModel.ShortScaleRSMDAmount =
                        _shortScaleService.CalculateShortScaleAmount(
                            firePremiumCalculationModel.RSMDAmount);

                    //terrorism
                    firePremiumCalculationModel.TerrorismRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)HouseholdConfigType.TerrorismRate : (int)HouseholdConfigType.TerrorismRate,
                        asset.SumInsuredAmount);

                    firePremiumCalculationModel.TerrorismAmount =
                        firePremiumCalculationModel.TerrorismRate * fullSumInsuredAmount;
                    firePremiumCalculationModel.ShortScaleTerrorismAmount =
                     _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.TerrorismAmount);

                    firePremiumCalculationModel.RSMDTAmount = firePremiumCalculationModel.RSMDAmount +
                        firePremiumCalculationModel.TerrorismAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmount =
                        firePremiumCalculationModel.ShortScaleRSMDAmount +
                        firePremiumCalculationModel.ShortScaleTerrorismAmount;
                }

                // Direct Discount



                if (model.FirePartial.IsRSMDT)
                {
                    if (model.AgentId == null)
                    {
                        firePremiumCalculationModel.RSMDTDirectDiscountAmount =
                            premiumCalculationResultModel.DirectDiscountRate * firePremiumCalculationModel.RSMDTAmount;
                        firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount =
                            _shortScaleService.CalculateShortScaleAmount(
                                firePremiumCalculationModel.RSMDTDirectDiscountAmount);
                        premiumCalculationResultModel.RSMDTDirectDiscountAmount +=
                            firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount;
                    }
                    firePremiumCalculationModel.PremiumAfterRSMDTDirectDiscount =
                        firePremiumCalculationModel.RSMDTAmount - firePremiumCalculationModel.RSMDTDirectDiscountAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount =
                        firePremiumCalculationModel.ShortScaleRSMDTAmount -
                        firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount;

                    premiumCalculationResultModel.RSMDTAfterDirectDiscount +=
                        firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount;
                }


                // minimum basic premium
                firePremiumCalculationModel.MinimumRSMDTAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)HouseholdConfigType.MinimunRSMDTAmount);
                firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)HouseholdConfigType.MinimunRSMDTAmount);
                //firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount =
                //        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.MinimumRSMDTAmount);
                if (firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount < firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount)
                {
                    firePremiumCalculationModel.RSMDTAmountToDisplay = firePremiumCalculationModel.MinimumRSMDTAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount;
                }
                else
                {
                    firePremiumCalculationModel.RSMDTAmountToDisplay = firePremiumCalculationModel.PremiumAfterRSMDTDirectDiscount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount;
                }
                #endregion

                if (model.IsCoinsurance)
                {
                    firePremiumCalculationModel.BasicPremiumToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.BasicPremiumToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.PremiumAfterDirectDiscount = CalculateCoInsuredAmount(firePremiumCalculationModel.PremiumAfterDirectDiscount, model.HGIShareRate);
                    firePremiumCalculationModel.RSMDTAmountToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.RSMDTAmountToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.RSMDTAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.RSMDTAmount, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleRSMDTAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleRSMDTAmount, model.HGIShareRate);
                    firePremiumCalculationModel.SumInsuredAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.SumInsuredAmount, model.HGIShareRate);
                    asset.SumInsuredAmount = firePremiumCalculationModel.SumInsuredAmount;
                }
                firePremiumCalculationModel.GrossPremium =
                    (firePremiumCalculationModel.BasicPremiumToDisplay + firePremiumCalculationModel.RSMDTAmountToDisplay).RoundToFourPrecisions();
                firePremiumCalculationModel.ShortScaleGrossPremium =
                        (firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay + firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay).RoundToFourPrecisions();


                premiumCalculationResultModel.FirePremiumCalculationModels.Add(firePremiumCalculationModel);
                if (!model.FirePartial.IsRSMDT)
                {
                    firePremiumCalculationModel.RSMDTActualDisplay = firePremiumCalculationModel.ShortScaleRSMDTAmount;
                }
                asset.BasicPremium = firePremiumCalculationModel.BasicPremiumToDisplay;
                asset.RsmdtPremium = firePremiumCalculationModel.RSMDTAmountToDisplay;
                asset.TransactionSumInsuredAmount = asset.SumInsuredAmount;
            }

            if (model.FirePartial.IsRSMDT)
            {
                premiumCalculationResultModel.PoolPremiumAmount = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleRSMDTAmountToDisplay);
            }
            else
            {
                premiumCalculationResultModel.FireUnselectedRSMDTAmount = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleRSMDTAmountToDisplay);
            }

            decimal totalBasicPremium = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleBasicPremiumToDisplay);
            if (model.FirePartial.ProvideSubsidy)
            {
                premiumCalculationResultModel.GovernmentSubsidyRate = model.FirePartial.SubsidyRate;
                premiumCalculationResultModel.BasicGovernmentSubsidyAmount = (totalBasicPremium *
                                                                        premiumCalculationResultModel.GovernmentSubsidyRate / 100).RoundToFourPrecisions();
                premiumCalculationResultModel.BasicBalanceAmount = totalBasicPremium - premiumCalculationResultModel.BasicGovernmentSubsidyAmount;
                premiumCalculationResultModel.PoolGovernmentSubsidyAmount = (premiumCalculationResultModel.PoolPremiumAmount *
                                                                        premiumCalculationResultModel.GovernmentSubsidyRate / 100).RoundToFourPrecisions();
                premiumCalculationResultModel.PoolBalanceAmount = premiumCalculationResultModel.PoolPremiumAmount - premiumCalculationResultModel.PoolGovernmentSubsidyAmount;
                premiumCalculationResultModel.GovernmentSubsidyAmount = premiumCalculationResultModel.BasicGovernmentSubsidyAmount + premiumCalculationResultModel.PoolGovernmentSubsidyAmount;
                premiumCalculationResultModel.BalanceAmount = premiumCalculationResultModel.BasicBalanceAmount + premiumCalculationResultModel.PoolBalanceAmount;
            }

            if (model.IsCoinsurance)
            {
                premiumCalculationResultModel.BasicPremium = CalculateCoInsuredAmount(premiumCalculationResultModel.BasicPremium, model.HGIShareRate);
                premiumCalculationResultModel.SumInsuredAmount = CalculateCoInsuredAmount(premiumCalculationResultModel.SumInsuredAmount, model.HGIShareRate);
            }
            premiumCalculationResultModel.TotalPremiumAmount = (premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleBasicPremiumToDisplay) + premiumCalculationResultModel.PoolPremiumAmount).RoundToFourPrecisions();


            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

            if (model.IsCoinsurance && !model.IsHGILead)
            {
                premiumCalculationResultModel.StampDutyAmount = 0;
            }
            else
            {
                premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(
                                   globalriskSetupModel,
                                   (int)GlobalConfigType.StampDuty,
                                   premiumCalculationResultModel.FirePremiumCalculationModels.Sum(m => m.SumInsuredAmount));
            }
            premiumCalculationResultModel.VatPercent = ConstrantValueHelper.GetNewValue(riskSetupModel, (int)HouseholdConfigType.VAT);
            premiumCalculationResultModel.VatAmount = (premiumCalculationResultModel.TotalPremiumAmount * premiumCalculationResultModel.VatPercent).RoundToFourPrecisions();
            var totalWithVat = premiumCalculationResultModel.TotalPremiumAmount + premiumCalculationResultModel.VatAmount;
            var totalWithStamp = totalWithVat + premiumCalculationResultModel.StampDutyAmount;
            premiumCalculationResultModel.PremiumAfterStamp = totalWithStamp;
            premiumCalculationResultModel.NetPremiumAmount = totalWithStamp;
            premiumCalculationResultModel.VatPercent *= Convert.ToDecimal(100);
            premiumCalculationResultModel.ExpiryDate = model.FirePartial.PeriodOfInsuranceTo;

            return premiumCalculationResultModel;
        });
    }
  #endregion

    #region Property Calculator
    private async Task<PremiumCalculationResultModel> CalculatePropertyPremium(List<CalculationConfigurationViewModel> riskSetupModel, CreatePolicyViewModel model)
    {
        List<FireRiskConfiguration> propertyConfigList = await _propertyRiskConfiguration.GetAllPropertyRiskConfig();//ReadConfiguration("Property");
        if (model.FirePartial.SubsidyClassID != null)
        {
            if (model.FirePartial.SubsidyClassID != "Select Subsidy SI Limit")
            {
                if (!string.IsNullOrEmpty(model.FirePartial.SubsidyClassID))
                {
                    var subsidySILImit = await _propertySubsidySILimitService.GetSingleSubsidySILimit(model.FirePartial.SubsidyClassID);
                    if (subsidySILImit != null)
                    {
                        model.FirePartial.SubsidyClass = subsidySILImit.SILabel;
                        model.FirePartial.SubsidyLimit = subsidySILImit.SILimit;
                    }
                }
            }

        }

        return await Task.Run(() =>
        {
            var test = JsonConvert.SerializeObject(model);
            PremiumCalculationResultModel premiumCalculationResultModel = new PremiumCalculationResultModel();
            model.FirePartial.PeriodOfInsuranceFrom = model.EffectiveDate;
            model.FirePartial.PeriodOfInsuranceTo = model.ExpiryDate;
            if (model.AgentId == null)
            {
                premiumCalculationResultModel.DirectDiscountRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)PropertyConfigType.DirectDiscount : (int)PropertyConfigType.DirectDiscount);
            }

            premiumCalculationResultModel.IsCoinsurance = model.IsCoinsurance;
            premiumCalculationResultModel.HGIShareRate = model.HGIShareRate;
            premiumCalculationResultModel.ShortScaleRate = model.ShortScale != null ? (model.ShortScale.Value / 100) : ConstrantValueHelper.GetGlobalValue(
                _globalriskSetupModel, (int)_shortScaleConfigType, (decimal)model.PolicyPeriodInDays);
            premiumCalculationResultModel.Days = model.PolicyPeriodInDays.Value;
            _shortScaleService.Initialize(premiumCalculationResultModel.ShortScaleRate);



            foreach (var asset in model.FirePartial.SubjectMatterOfInsurance)
            {
                if (asset.Id == null)
                {
                    Guid value = Guid.NewGuid();
                    asset.Id = value.ToString();
                }

                var firePremiumCalculationModel = new FirePremiumCalculationModel(asset.Id);

                #region SumInsuredAmount
                if (model.IsCoinsurance)
                {
                    if (asset.LoadContentsFromFile)
                    {
                        if (asset.HouseholdContentExcelData != null)
                        {
                            foreach (var item in asset.HouseholdContentExcelData)
                            {
                                item.Rate *= (model.HGIShareRate / 100);
                                item.Total *= (model.HGIShareRate / 100);
                            }
                        }
                    }
                }
                //if (asset.LoadContentsFromFile)
                //{
                //    if (asset.HouseholdContentExcelData != null)
                //    {
                //        firePremiumCalculationModel.TotalValueOfContents =
                //            asset.HouseholdContentExcelData.Sum(s => s.Total);
                //    }
                //}
                //else
                //{


                //}

                firePremiumCalculationModel.TotalValueOfContents =
                asset.Equipment +
                asset.RawMaterials +
                asset.WorkInProgress +
                asset.FinishedGoods +
                asset.SemiFinishedGoods +
                asset.MoneyAndJewellery +
                asset.Art +
                asset.FurnitureFixtureOrFitting +
                asset.OtherItems;
                asset.SumInsuredAmount = (firePremiumCalculationModel.TotalValueOfContents + asset.ValueOfBuilding).RoundToFourPrecisions();
                decimal eachSumInsured = asset.SumInsuredAmount.RoundToFourPrecisions();
                var totalSumInsured = 0m;
                totalSumInsured += eachSumInsured;
                firePremiumCalculationModel.SumInsuredAmount = asset.SumInsuredAmount.RoundToFourPrecisions();
                premiumCalculationResultModel.SumInsuredAmount += firePremiumCalculationModel.SumInsuredAmount.RoundToFourPrecisions();

                decimal fullSumInsuredAmount = totalSumInsured;
                model.FullSumInsured = premiumCalculationResultModel.FullSumInsured = premiumCalculationResultModel.SumInsuredAmount;
                #endregion

                #region compare SI with Subsidy SILImit
                if (model.FirePartial.ProvideSubsidy)
                {
                    if (model.FirePartial.SubsidyClassID == "Select Subsidy SI Limit" || model.FirePartial.SubsidyClassID == null)
                    {
                        throw new Exception($"Please select subsidy category.");
                    }
                }
                if (model.FirePartial.ProvideSubsidy && (model.FirePartial.SubsidyClassID != "Select Subsidy SI Limit"))
                {
                    if (model.FirePartial.SubsidyClassID != null)
                    {
                        if (premiumCalculationResultModel.SumInsuredAmount > model.FirePartial.SubsidyLimit)
                        {
                            throw new Exception($" Total Sum insured should be less than or equal to  {model.FirePartial.SubsidyLimit}");
                        }
                    }
                }

                #endregion

                #region ODP
                premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;
                if (string.IsNullOrEmpty(asset.RiskCode))
                {
                    throw new Exception($"Please select Risk Types for all assets.");
                }
                firePremiumCalculationModel.RiskDetails =
                    propertyConfigList.FirstOrDefault(c => c.RiskCode == asset.RiskCode);
                if (firePremiumCalculationModel.RiskDetails == null)
                {
                    throw new Exception($"Please add configuration for Risk Code {asset.RiskCode}" +
                        " in Property Configurations JSON file");
                }

                firePremiumCalculationModel.BasicPremiumRate = firePremiumCalculationModel.RiskDetails.Rate / 100;
                asset.BuildingCompositionDescription = BuildingComposition.FirstClass.ToString();

                if (asset.LoadingMultiplier != 0 && asset.BuildingComposition == ((int)BuildingComposition.SecondClass).ToString())
                {
                    asset.BuildingCompositionDescription = BuildingComposition.SecondClass.ToString();

                    firePremiumCalculationModel.BasicPremium =
                       firePremiumCalculationModel.BasicPremiumRate * firePremiumCalculationModel.SumInsuredAmount * asset.LoadingMultiplier;
                    firePremiumCalculationModel.ShortScaleBasicPremium =
                        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.BasicPremium);
                    premiumCalculationResultModel.InitialBasicpremium +=
                        firePremiumCalculationModel.ShortScaleBasicPremium;
                }
                else
                {
                    firePremiumCalculationModel.BasicPremium =
                       firePremiumCalculationModel.BasicPremiumRate * firePremiumCalculationModel.SumInsuredAmount;
                    firePremiumCalculationModel.ShortScaleBasicPremium =
                        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.BasicPremium);
                    premiumCalculationResultModel.InitialBasicpremium +=
                        firePremiumCalculationModel.ShortScaleBasicPremium;
                }
                if (model.FirePartial.ProvideLockdownDiscount)
                {
                    firePremiumCalculationModel.LockdownBasicDiscountAmount = model.FirePartial.LockdownDiscountDetail.FirstOrDefault(x => x.Identifier == asset.Identifier).BasicLockdownDiscountAmount;
                    firePremiumCalculationModel.ShortScaleLockdownBasicDiscountAmount = firePremiumCalculationModel.LockdownBasicDiscountAmount;
                    premiumCalculationResultModel.BasicDiscountAmount +=
                        firePremiumCalculationModel.ShortScaleLockdownBasicDiscountAmount;
                }
                firePremiumCalculationModel.PremiumAfterLockdownBasicDiscount =
                    firePremiumCalculationModel.BasicPremium - firePremiumCalculationModel.LockdownBasicDiscountAmount;
                firePremiumCalculationModel.ShortScalePremiumAfterLockdownBasicDiscount =
                    firePremiumCalculationModel.ShortScaleBasicPremium - firePremiumCalculationModel.ShortScaleLockdownBasicDiscountAmount;

                premiumCalculationResultModel.PremiumAfterLockdownDiscount +=
                    firePremiumCalculationModel.ShortScalePremiumAfterLockdownBasicDiscount;

                // Direct Discount
                if (model.AgentId == null)
                {
                    firePremiumCalculationModel.DirectDiscountAmount =
                        premiumCalculationResultModel.DirectDiscountRate * firePremiumCalculationModel.PremiumAfterLockdownBasicDiscount;
                    firePremiumCalculationModel.ShortScaleDirectDiscountAmount =
                        _shortScaleService.CalculateShortScaleAmount(
                            firePremiumCalculationModel.DirectDiscountAmount);
                    premiumCalculationResultModel.DirectDiscountAmount +=
                        firePremiumCalculationModel.ShortScaleDirectDiscountAmount;
                }
                firePremiumCalculationModel.PremiumAfterDirectDiscount =
                    firePremiumCalculationModel.PremiumAfterLockdownBasicDiscount - firePremiumCalculationModel.DirectDiscountAmount;
                firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount =
                    firePremiumCalculationModel.ShortScalePremiumAfterLockdownBasicDiscount -
                    firePremiumCalculationModel.ShortScaleDirectDiscountAmount;

                premiumCalculationResultModel.PremiumAfterDirectDiscount +=
                    firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;


                // minimum basic premium
                firePremiumCalculationModel.MinimumBasicPremiumAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)PropertyConfigType.MinimunBasicPremiumAmount);
                firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)PropertyConfigType.MinimunBasicPremiumAmount);
                //firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount =
                //        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.MinimumBasicPremiumAmount);
                if (firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount < firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount)
                {
                    premiumCalculationResultModel.BasicPremium += firePremiumCalculationModel.ShortScaleMinimumBasicPremiumAmount;
                    firePremiumCalculationModel.BasicPremiumToDisplay = firePremiumCalculationModel.MinimumBasicPremiumAmount;
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = firePremiumCalculationModel.MinimumBasicPremiumAmount;
                }
                else
                {
                    premiumCalculationResultModel.BasicPremium +=
                   firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;
                    firePremiumCalculationModel.BasicPremiumToDisplay = firePremiumCalculationModel.PremiumAfterDirectDiscount;
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount;
                }
                #endregion

                #region RSMDT
                if (model.FirePartial.IsRSMDT)
                {
                    premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;

                    firePremiumCalculationModel.IsRSMDTSelected = model.FirePartial.IsRSMDT;
                    firePremiumCalculationModel.RSMDRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)PropertyConfigType.RSMDRate : (int)PropertyConfigType.RSMDRate,
                        asset.SumInsuredAmount);

                    firePremiumCalculationModel.RSMDAmount =
                        firePremiumCalculationModel.RSMDRate * firePremiumCalculationModel.SumInsuredAmount;
                    firePremiumCalculationModel.ShortScaleRSMDAmount =
                     _shortScaleService.CalculateShortScaleAmount(
                         firePremiumCalculationModel.RSMDAmount);

                    firePremiumCalculationModel.TerrorismRate = ConstrantValueHelper.GetNewValue(
                        riskSetupModel,
                        model.PortfolioAlias == PortfolioClassConstants.Household ?
                            (int)PropertyConfigType.TerrorismRate : (int)PropertyConfigType.TerrorismRate,
                        asset.SumInsuredAmount);

                    firePremiumCalculationModel.TerrorismAmount =
                        firePremiumCalculationModel.TerrorismRate * firePremiumCalculationModel.SumInsuredAmount;
                    firePremiumCalculationModel.ShortScaleTerrorismAmount =
                     _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.TerrorismAmount);

                    firePremiumCalculationModel.RSMDTAmount = firePremiumCalculationModel.RSMDAmount +
                        firePremiumCalculationModel.TerrorismAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmount =
                        firePremiumCalculationModel.ShortScaleRSMDAmount +
                        firePremiumCalculationModel.ShortScaleTerrorismAmount;

                    if (model.FirePartial.ProvideLockdownDiscount && model.FirePartial.IsRSMDT)
                    {
                        firePremiumCalculationModel.LockdownRSMDTDiscountAmount = model.FirePartial.LockdownDiscountDetail.FirstOrDefault(x => x.Identifier == asset.Identifier).RSMDTLockdownDiscountAmount;
                        firePremiumCalculationModel.ShortScaleLockdownRSMDTDiscountAmount = firePremiumCalculationModel.LockdownRSMDTDiscountAmount;
                        premiumCalculationResultModel.PoolDiscountAmount +=
                            firePremiumCalculationModel.ShortScaleLockdownRSMDTDiscountAmount;
                    }
                    firePremiumCalculationModel.PremiumAfterLockdownRSMDTDiscount =
                        firePremiumCalculationModel.RSMDTAmount - firePremiumCalculationModel.LockdownRSMDTDiscountAmount;
                    firePremiumCalculationModel.ShortScalePremiumAfterLockdownRSMDTDiscount =
                        firePremiumCalculationModel.ShortScaleRSMDTAmount -
                        firePremiumCalculationModel.ShortScaleLockdownRSMDTDiscountAmount;

                    premiumCalculationResultModel.PremiumAfterLockdownRSMDTDiscount +=
                        firePremiumCalculationModel.ShortScalePremiumAfterLockdownRSMDTDiscount;
                }


                //Direct Dicount
                if (model.FirePartial.IsRSMDT)
                {
                    if (model.AgentId == null)
                    {
                        firePremiumCalculationModel.RSMDTDirectDiscountAmount =
                            premiumCalculationResultModel.DirectDiscountRate * firePremiumCalculationModel.PremiumAfterLockdownRSMDTDiscount;
                        firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount =
                            _shortScaleService.CalculateShortScaleAmount(
                                firePremiumCalculationModel.RSMDTDirectDiscountAmount);
                        premiumCalculationResultModel.RSMDTDirectDiscountAmount +=
                            firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount;
                    }
                    firePremiumCalculationModel.PremiumAfterRSMDTDirectDiscount =
                        firePremiumCalculationModel.PremiumAfterLockdownRSMDTDiscount - firePremiumCalculationModel.RSMDTDirectDiscountAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount =
                        firePremiumCalculationModel.ShortScalePremiumAfterLockdownRSMDTDiscount -
                        firePremiumCalculationModel.RSMDTShortScaleDirectDiscountAmount;

                    premiumCalculationResultModel.RSMDTAfterDirectDiscount +=
                        firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount;
                }


                // minimum basic premium
                firePremiumCalculationModel.MinimumRSMDTAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)PropertyConfigType.MinimunRSMDTAmount);
                firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)PropertyConfigType.MinimunRSMDTAmount);
                //firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount =
                //        _shortScaleService.CalculateShortScaleAmount(firePremiumCalculationModel.MinimumRSMDTAmount);
                if (firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount < firePremiumCalculationModel.ShortScaleMinimumRSMDTAmount)
                {
                    firePremiumCalculationModel.RSMDTAmountToDisplay = firePremiumCalculationModel.MinimumRSMDTAmount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = firePremiumCalculationModel.MinimumRSMDTAmount;
                }
                else
                {
                    firePremiumCalculationModel.RSMDTAmountToDisplay = firePremiumCalculationModel.PremiumAfterRSMDTDirectDiscount;
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = firePremiumCalculationModel.ShortScaleRSMDTAfterDirectDiscount;
                }
                // firePremiumCalculationModel.ShortScaleRSMDTAmount = firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay;
                #endregion

                if (model.IsCoinsurance)
                {
                    firePremiumCalculationModel.BasicPremiumToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.BasicPremiumToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.PremiumAfterDirectDiscount = CalculateCoInsuredAmount(firePremiumCalculationModel.PremiumAfterDirectDiscount, model.HGIShareRate);
                    firePremiumCalculationModel.RSMDTAmountToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.RSMDTAmountToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.RSMDTAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.RSMDTAmount, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScalePremiumAfterDirectDiscount, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay, model.HGIShareRate);
                    firePremiumCalculationModel.ShortScaleRSMDTAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.ShortScaleRSMDTAmount, model.HGIShareRate);
                    firePremiumCalculationModel.SumInsuredAmount = CalculateCoInsuredAmount(firePremiumCalculationModel.SumInsuredAmount, model.HGIShareRate);
                    asset.SumInsuredAmount = firePremiumCalculationModel.SumInsuredAmount;
                }

                firePremiumCalculationModel.GrossPremium =
                    (firePremiumCalculationModel.BasicPremiumToDisplay + firePremiumCalculationModel.RSMDTAmountToDisplay).RoundToFourPrecisions();
                firePremiumCalculationModel.ShortScaleGrossPremium =
                       (firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay + firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay).RoundToFourPrecisions();

                premiumCalculationResultModel.FirePremiumCalculationModels.Add(firePremiumCalculationModel);
                if (!model.FirePartial.IsRSMDT)
                {
                    firePremiumCalculationModel.RSMDTActualDisplay = firePremiumCalculationModel.ShortScaleRSMDTAmount;
                }
                //if (model.IsCoinsurance) asset.SumInsuredAmount = asset.SumInsuredAmount;
                asset.BasicPremium = firePremiumCalculationModel.BasicPremiumToDisplay;
                asset.RsmdtPremium = firePremiumCalculationModel.RSMDTAmountToDisplay;
                asset.TransactionSumInsuredAmount = asset.SumInsuredAmount;

                premiumCalculationResultModel.BasicPremiumWithoutCoinsuranceRate = firePremiumCalculationModel.ShortScaleBasicPremiumToDisplay;
                premiumCalculationResultModel.RSMDTPremiumWithoutCoinsuranceRate = firePremiumCalculationModel.ShortScaleRSMDTAmountToDisplay;

            }

            if (model.FirePartial.IsRSMDT)
            {
                premiumCalculationResultModel.PoolPremiumAmount = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleRSMDTAmountToDisplay);
            }
            else
            {
                premiumCalculationResultModel.FireUnselectedRSMDTAmount = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleRSMDTAmountToDisplay);
            }
            decimal totalBasicPremium = premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleBasicPremiumToDisplay);
            if (model.FirePartial.ProvideSubsidy)
            {
                premiumCalculationResultModel.GovernmentSubsidyRate = model.FirePartial.SubsidyRate;
                premiumCalculationResultModel.BasicGovernmentSubsidyAmount = (premiumCalculationResultModel.BasicPremium *
                                                                        premiumCalculationResultModel.GovernmentSubsidyRate / 100).RoundToFourPrecisions();
                premiumCalculationResultModel.BasicBalanceAmount = premiumCalculationResultModel.BasicPremium - premiumCalculationResultModel.BasicGovernmentSubsidyAmount;
                premiumCalculationResultModel.PoolGovernmentSubsidyAmount = (premiumCalculationResultModel.PoolPremiumAmount *
                                                                        premiumCalculationResultModel.GovernmentSubsidyRate / 100).RoundToFourPrecisions();
                premiumCalculationResultModel.PoolBalanceAmount = premiumCalculationResultModel.PoolPremiumAmount - premiumCalculationResultModel.PoolGovernmentSubsidyAmount;
                premiumCalculationResultModel.GovernmentSubsidyAmount = premiumCalculationResultModel.BasicGovernmentSubsidyAmount + premiumCalculationResultModel.PoolGovernmentSubsidyAmount;
                premiumCalculationResultModel.BalanceAmount = premiumCalculationResultModel.BasicBalanceAmount + premiumCalculationResultModel.PoolBalanceAmount;
            }
            if (model.IsCoinsurance)
            {
                premiumCalculationResultModel.BasicPremium = CalculateCoInsuredAmount(premiumCalculationResultModel.BasicPremium, model.HGIShareRate);
                premiumCalculationResultModel.SumInsuredAmount = CalculateCoInsuredAmount(premiumCalculationResultModel.SumInsuredAmount, model.HGIShareRate);
            }
            premiumCalculationResultModel.TotalPremiumAmount = (premiumCalculationResultModel.FirePremiumCalculationModels.Sum(x => x.ShortScaleBasicPremiumToDisplay) + premiumCalculationResultModel.PoolPremiumAmount).RoundToFourPrecisions();
            premiumCalculationResultModel.GrossDiscountAmount = premiumCalculationResultModel.BasicDiscountAmount + premiumCalculationResultModel.PoolDiscountAmount;
            var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

            var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);
            if (model.IsCoinsurance && !model.IsHGILead)
            {
                premiumCalculationResultModel.StampDutyAmount = 0;
            }
            else
            {
                premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetGlobalValue(
                globalriskSetupModel,
                (int)GlobalConfigType.StampDuty,
                premiumCalculationResultModel.FirePremiumCalculationModels.Sum(m => m.SumInsuredAmount));
            }
            if (model.FirePartial.ProvideSubsidy)
            {
                premiumCalculationResultModel.VatPercent = 0;
            }
            else
            {
                premiumCalculationResultModel.VatPercent =
                    ConstrantValueHelper.GetNewValue(riskSetupModel, (int)PropertyConfigType.VAT);
            }


            premiumCalculationResultModel.VatAmount = (premiumCalculationResultModel.TotalPremiumAmount * premiumCalculationResultModel.VatPercent).RoundToFourPrecisions();
            var totalWithVat = premiumCalculationResultModel.TotalPremiumAmount + premiumCalculationResultModel.VatAmount;
            var totalWithStamp = totalWithVat + premiumCalculationResultModel.StampDutyAmount;
            premiumCalculationResultModel.PremiumAfterStamp = totalWithStamp;
            premiumCalculationResultModel.NetPremiumAmount = totalWithStamp;
            premiumCalculationResultModel.VatPercent *= Convert.ToDecimal(100);
            premiumCalculationResultModel.ExpiryDate = model.FirePartial.PeriodOfInsuranceTo;
            return premiumCalculationResultModel;
        });
    }
  #endregion

    private List<FireRiskConfiguration> ReadConfiguration(string portFolio)
    {
        //var filename = portFolio == "Property" ? Path.Join(_fileDirectory, _propertyConfigurationFileName) :
        //    Path.Join(_fileDirectory, _householdConfigurationFileName);
        //if (!File.Exists(filename))
        //{
        //    throw new Exception($"Can not find {portFolio} Configurations JSON file.");
        //}

        var configurationList = new List<FireRiskConfiguration>();
        //using (StreamReader file = File.OpenText(filename))
        //{
        //    try
        //    {
        //        configurationList = new JsonSerializer().Deserialize(file, typeof(List<FireRiskConfiguration>))
        //            as List<FireRiskConfiguration>;
        //    }
        //    catch
        //    {
        //        throw new Exception($"Invalid {portFolio} Configurations JSON file.");
        //    }
        //}

        //if (configurationList.Count == 0)
        //{
        //    throw new Exception($"{portFolio} Configurations JSON file has no configuration.");
        //}

        return configurationList;
    }

    private decimal CalculateCoInsuredAmount(decimal amountToCoinsure, decimal shareRate)
    {
        return amountToCoinsure * shareRate / 100;
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }
}
