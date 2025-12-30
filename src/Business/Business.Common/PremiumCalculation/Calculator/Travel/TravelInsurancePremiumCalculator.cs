using Business.Common.Helper;
using Data.Context;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Constant.Policy;
using SharedKernel.Helper;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public class TravelInsurancePremiumCalculator : IPolicyPremiumCalculator
{
    private readonly ApplicationDataContext _db;
    private readonly ICurrencyExchangeRateConfigurationService _currencyExchangeRateConfigurationService;

    int days;
    public TravelInsurancePremiumCalculator(ApplicationDataContext db, ICurrencyExchangeRateConfigurationService currencyExchangeRateConfigurationService)
    {
        days = 0;
        _db = db;
        _currencyExchangeRateConfigurationService = currencyExchangeRateConfigurationService;
    }
    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.TravelInsurance
        };
    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }

    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {
        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();

        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);
        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();

        var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

        //_riskSetupModel = _mapper.Map<List<CalculationConfigurationViewModel>>(
        // _calculationConfigurationRepository.GetAll(x => x.PortfolioAlias == PortfolioClassConstants.TravelInsurance).ToList());
        //_globalriskSetupModel = _mapper.Map<List<GlobalConfigurationViewModel>>(
        //_globalConfigurationRepository.GetAll().ToList());

        return await Task.Run(() =>
        {
            if (model.TravelInsurancePartial == null)
            {
                throw new ArgumentNullException(nameof(model.TravelInsurancePartial));
            }
            var premiumCalculationResultModel = new PremiumCalculationResultModel();

            premiumCalculationResultModel.BasicPremiumA = model.TravelInsurancePartial.PremiumAmount.RoundToFourPrecisions();
            premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;


            var selectedCurrency = premiumCalculationResultModel.SelectedCurrency = model.TravelInsurancePartial.SelectedCurrency;

            var targetSellRate = model.TravelInsurancePartial.ExchangeRate =
            (decimal)_currencyExchangeRateConfigurationService.GetLatestCurrencyExchangeRate(selectedCurrency, "NPR").TargetSell;

            if (model.TravelInsurancePartial.TravellingCountry == TravelInsuranceConstants.PlanL)
                targetSellRate = model.TravelInsurancePartial.ExchangeRate =
                   (decimal)_currencyExchangeRateConfigurationService.GetLatestCurrencyExchangeRate("EUR", "NPR").TargetSell;

            premiumCalculationResultModel.BasicPremium = (model.TravelInsurancePartial.PremiumAmount * targetSellRate).RoundToFourPrecisions();
            
            if (model.AgentId != null && !model.TravelInsurancePartial.IncludeDirectDiscount)
            {
                premiumCalculationResultModel.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
                      globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                premiumCalculationResultModel.AgentCommissonAmount = (premiumCalculationResultModel.BasicPremium *
                                                               premiumCalculationResultModel.AgentCommissonRate / 100).RoundToFourPrecisions();

                premiumCalculationResultModel.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
                    globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                premiumCalculationResultModel.NepalAgentTDSAmount = (premiumCalculationResultModel.AgentCommissonAmount * premiumCalculationResultModel.NepalAgentTDSRate / 100).RoundToFourPrecisions();
            }
            else if (model.TravelInsurancePartial.IncludeDirectDiscount)
            {
                var specialAgency = TravelInsuranceSpecialAgencyDiscount.SpecialAgentDiscountDetails.Where(x => x.AgentCode == model.TravelInsurancePartial.SpecialAgencyCode).SingleOrDefault();
                premiumCalculationResultModel.TravelDirectDiscount = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)TravelInsuranceConfigType.TIDirectDiscount);

                premiumCalculationResultModel.BasicPremiumUSDBeforeSpecialDiscount = premiumCalculationResultModel.BasicPremiumA;
                premiumCalculationResultModel.BasicPremiumBeforeSpecialDiscount = premiumCalculationResultModel.BasicPremium;

                var discountAmount = (premiumCalculationResultModel.TravelDirectDiscount / 100) * premiumCalculationResultModel.BasicPremiumA;
                premiumCalculationResultModel.BasicPremiumA -= discountAmount;
                model.TravelInsurancePartial.PremiumAmount = premiumCalculationResultModel.BasicPremiumA;
                premiumCalculationResultModel.BasicPremium = (premiumCalculationResultModel.BasicPremiumA * targetSellRate).RoundToFourPrecisions();

                model.AgentId = null;
            }

            days = premiumCalculationResultModel.Days = model.TravelInsurancePartial.PolicyPeriodInDays;

            premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)TravelInsuranceConfigType.StampDuty, premiumCalculationResultModel.BasicPremium);
            premiumCalculationResultModel.TotalPremiumAmount = premiumCalculationResultModel.BasicPremium;

            if (TravelPolicyVATHelper.CheckVatApplicable(model.PortfolioAlias, model.TravelInsurancePartial.TravellingCountry, model.TravelInsurancePartial.InsuranceType))
            {
                premiumCalculationResultModel.VatPercent = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)TravelInsuranceConfigType.VATRate);
            }
            else
            {
                premiumCalculationResultModel.VatPercent = 0;
            }

            premiumCalculationResultModel.VatAmount = (premiumCalculationResultModel.BasicPremium * premiumCalculationResultModel.VatPercent / 100).RoundToFourPrecisions();
            premiumCalculationResultModel.PremiumAfterStamp = premiumCalculationResultModel.BasicPremium +
                                                              premiumCalculationResultModel.VatAmount +
                                                              premiumCalculationResultModel.StampDutyAmount;

            premiumCalculationResultModel.NetPremiumAmount = premiumCalculationResultModel.PremiumAfterStamp;

            return premiumCalculationResultModel;
        });
    }
}
