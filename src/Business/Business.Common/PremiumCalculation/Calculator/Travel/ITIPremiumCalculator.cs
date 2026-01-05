using Business.Common.Helper;
using Data.Context;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Configuration.CalculationConfiguration;
using Models.Common.Policy.Configuration.GlobalConfiguration;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using Models.WebApi.Customer.Policy;
using SharedKernel.Constant;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;

namespace Business.Common.PremiumCalculation.Calculator.Travel;
public class ITIPremiumCalculator : IPolicyPremiumCalculator
{
    private readonly ApplicationDataContext _db;
    private readonly ICurrencyExchangeRateConfigurationService _currencyExchangeRateConfiguration;

    int days;
    public ITIPremiumCalculator(ICurrencyExchangeRateConfigurationService currencyExchangeRateConfiguration, ApplicationDataContext db)
    {
        _currencyExchangeRateConfiguration = currencyExchangeRateConfiguration;
        days = 0;
        _db = db;
    }
    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.InternationalTravelInsurance
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

        return await Task.Run(() =>
        {
            if (model.InternationalTravelInsurancePartial == null)
            {
                throw new ArgumentNullException(nameof(model.InternationalTravelInsurancePartial));
            }
            var premiumCalculationResultModel = new PremiumCalculationResultModel();
            premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;

            if (model.InternationalTravelInsurancePartial.IsCovid19Coverage)
            {
                var covidRate = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel,
                    (int)InternationalTravelInsuranceConfigType.COVIDCoverage) / 100;

                model.InternationalTravelInsurancePartial.PremiumAmount += covidRate * model.InternationalTravelInsurancePartial.PremiumAmount;
            }

            premiumCalculationResultModel.BasicPremiumA = model.InternationalTravelInsurancePartial.PremiumAmount.RoundToFourPrecisions();

            var selectedCurrency = premiumCalculationResultModel.SelectedCurrency = model.InternationalTravelInsurancePartial.SelectedCurrency;

            var targetSellRate = model.IsPolicyFromThirdParty == true ? model.InternationalTravelInsurancePartial.ExchangeRate :
                (decimal)_currencyExchangeRateConfiguration.GetLatestCurrencyExchangeRate(selectedCurrency, "NPR").TargetSell;

            model.InternationalTravelInsurancePartial.ExchangeRate = targetSellRate;

            premiumCalculationResultModel.BasicPremium = (model.InternationalTravelInsurancePartial.PremiumAmount * targetSellRate).RoundToFourPrecisions();

            if (model.InternationalTravelInsurancePartial.TravelPlanType == InternationalTravelInsuranceConfigType.PlanOne)
            {
                premiumCalculationResultModel.SumInsuredAmount = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)InternationalTravelInsuranceConfigType.PlanOne);
            }
            else
            {
                premiumCalculationResultModel.SumInsuredAmount = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)InternationalTravelInsuranceConfigType.PlanTwo);
            }

            if (model.AgentId != null)
            {
                premiumCalculationResultModel.AgentCommissonRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
                      globalriskSetupModel, (int)GlobalConfigType.AgentCommissionRate);
                premiumCalculationResultModel.AgentCommissonAmount = (premiumCalculationResultModel.BasicPremium *
                                                               premiumCalculationResultModel.AgentCommissonRate / 100).RoundToFourPrecisions();

                premiumCalculationResultModel.NepalAgentTDSRate = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
                    globalriskSetupModel, (int)GlobalConfigType.NepalAgentTDSRate);
                premiumCalculationResultModel.NepalAgentTDSAmount = (premiumCalculationResultModel.AgentCommissonAmount * premiumCalculationResultModel.NepalAgentTDSRate / 100).RoundToFourPrecisions();
            }

            days = premiumCalculationResultModel.Days = model.InternationalTravelInsurancePartial.PolicyPeriodInDays;

            premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)InternationalTravelInsuranceConfigType.StampDuty, premiumCalculationResultModel.BasicPremium);
            premiumCalculationResultModel.TotalPremiumAmount = premiumCalculationResultModel.BasicPremium;
            premiumCalculationResultModel.VatPercent = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)InternationalTravelInsuranceConfigType.VATRate);
            premiumCalculationResultModel.VatAmount = (premiumCalculationResultModel.BasicPremium * premiumCalculationResultModel.VatPercent / 100).RoundToFourPrecisions();
            premiumCalculationResultModel.PremiumAfterStamp = premiumCalculationResultModel.BasicPremium +
                                                              premiumCalculationResultModel.VatAmount +
                                                              premiumCalculationResultModel.StampDutyAmount;

            premiumCalculationResultModel.NetPremiumAmount = premiumCalculationResultModel.PremiumAfterStamp;
            return premiumCalculationResultModel;
        });
    }
}
