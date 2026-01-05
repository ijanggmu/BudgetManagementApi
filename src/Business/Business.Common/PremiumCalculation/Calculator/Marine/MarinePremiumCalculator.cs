using Business.Common.Helper;
using Business.Common.PremiumCalculation.Calculator.Travel;
using Models.Common.Policy.Calculation.Marine;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Enum;
using Models.Common.Policy.Policy;
using Newtonsoft.Json;
using SharedKernel.Constant;
using UnderwritingService.Calculation.PremiumCalculation.Abstract;
using Data.Context;
using Models.Common.Policy.Policy.Marine;
using SharedKernel.Helper;
using SharedKernel.Constant.Permission;

namespace Business.Common.PremiumCalculation.Calculator.Marine;
public class MarinePremiumCalculator : IPolicyPremiumCalculator
{
    private readonly ICurrencyExchangeRateConfigurationService _currencyExchangeRateConfigurationService;
    private readonly IMarineTariffScheduleService _marineTariffScheduleService;
    private const string NPR = "NPR";
    private readonly decimal proRate;
    private readonly ApplicationDataContext _db;

    public MarinePremiumCalculator(
        ICurrencyExchangeRateConfigurationService currencyExchangeRateConfigurationService,
    IMarineTariffScheduleService marineTariffScheduleService,
    ApplicationDataContext db)
    {
        _currencyExchangeRateConfigurationService = currencyExchangeRateConfigurationService;
        _marineTariffScheduleService = marineTariffScheduleService;
        proRate = 0;
        _db = db;
    }

    public IReadOnlyCollection<string> SupportedPortfolioAliases =>
        new[]
        {
            PortfolioClassConstants.Marine
        };
    public async Task<PremiumCalculationResultModel> CalculatePremium(CreatePolicyViewModel model)
    {

        var calculationConfigData = _db.CalculationConfigurations.Where(x => x.PortfolioAlias == model.PortfolioAlias && !x.IsDeleted).ToList();
        var riskSetupModel = CalculationConfigMapper.MapToCalculationConfigurationViewModel(calculationConfigData);
        var globalConfigData = _db.GlobalConfigurations.Where(x => !x.IsDeleted).ToList();
        var globalriskSetupModel = CalculationConfigMapper.MapToGlobalConfigurationViewModel(globalConfigData);

        if (model.MarinePartial.MaterialsOfInsurance == null)
        {
            throw new ArgumentNullException(nameof(model.MarinePartial.MaterialsOfInsurance));
        }

        var marineExtensionRisk = ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel, (int)MarineConfigType.MarineExtensionRisk);


        if (model.MarinePartial.InvoiceDetails != null)
        {
            var InvoiceArray = string.Join(",", model.MarinePartial.InvoiceDetails.Where(x => !string.IsNullOrEmpty(x.InvoiceNo)).Select(x => x.InvoiceNo).ToArray());

            model.MarinePartial.ProFormaInvoice = InvoiceArray;

            var InvoiceDateArray = string.Join(",", model.MarinePartial.InvoiceDetails.Select(x => x.InvoiceDate).ToArray());

            model.MarinePartial.ProFormaDate = InvoiceDateArray;
        }

        var premiumCalculationResultModel = new PremiumCalculationResultModel();

        if (model.MarinePartial.IncrementalCostRate != null)
        {
            premiumCalculationResultModel.IncrementalCostRate = model.MarinePartial.IncrementalCostRate.Value;
        }
        if (model.MarinePartial.DutyRate != null)
        {
            premiumCalculationResultModel.DutyRate = model.MarinePartial.DutyRate.Value;
        }
        if (model.MarinePartial.ToleranceRate != null)
        {
            premiumCalculationResultModel.ToleranceRate = model.MarinePartial.ToleranceRate.Value;
        }
        if (model.IsDeclaration)
        {
            premiumCalculationResultModel.Days = Convert.ToInt32(model.MarinePartial.Days);
            premiumCalculationResultModel.ExpiryDate = model.ExpiryDate;

        }
        premiumCalculationResultModel.IsCoinsurance = model.IsCoinsurance;
        premiumCalculationResultModel.HGIShareRate = model.HGIShareRate;

        if (model.MarinePartial.Clauses != null)
        {
            List<KeyValuePair<int, string>> clausesList = new List<KeyValuePair<int, string>>();
            string[] clausesValue = new string[model.MarinePartial.Clauses.Length];
            for (var i = 0; i < model.MarinePartial.Clauses.Length; i++)
            {
                var clausesData = ClausesDictionary.marineClauses().Where(x => x.Key == Convert.ToInt32(model.MarinePartial.Clauses[i])).FirstOrDefault();
                clausesValue[i] = clausesData.Value;
                clausesList.Add(clausesData);
            }
            model.MarinePartial.ClausesJson = JsonConvert.SerializeObject(clausesList);
            model.MarinePartial.Clauses = clausesValue;
        }

        //if (model.MarinePartial.Surveyor != null)
        //{
        //    if (model.MarinePartial.Surveyor.SurveyorId != "Select a Surveyor")
        //    {
        //        if (!string.IsNullOrEmpty(model.MarinePartial.Surveyor.SurveyorId))
        //        {
        //            var surveyorList = await _surveyorLIstService.GetSingleSurveyorList(model.MarinePartial.Surveyor.SurveyorId);
        //            if (surveyorList != null)
        //            {
        //                model.MarinePartial.Surveyor.ContactPerson = surveyorList.ContactPerson;
        //                model.MarinePartial.Surveyor.Address = surveyorList.Address;
        //                model.MarinePartial.Surveyor.CityStateCountry = surveyorList.StreetAddress;
        //                model.MarinePartial.Surveyor.PhoneNumber = surveyorList.TelephoneNumber;
        //                model.MarinePartial.Surveyor.Fax = surveyorList.Fax;
        //                model.MarinePartial.Surveyor.Email = surveyorList.Email;
        //                model.MarinePartial.Surveyor.Name = surveyorList.SurveyorName;
        //            }
        //        }
        //    }

        //}

        #region Retrieve Rates for Calculation
        premiumCalculationResultModel.CurrencyOfValue = model.MarinePartial.CurrencyOfValue;
        if (model.MarinePartial.CurrencyOfValue == NPR)
        {
            premiumCalculationResultModel.PerUnitNRSValue = 1;
        }
        else
        {
            var exchangeRate = _currencyExchangeRateConfigurationService.GetLatestCurrencyExchangeRate(
                model.MarinePartial.CurrencyOfValue, NPR);

            if (exchangeRate == null)
            {
                throw new ArgumentException(
                    $"Exchange rate for currency {model.MarinePartial.CurrencyOfValue} is unavailable.",
                    nameof(model.MarinePartial.CurrencyOfValue));
            }

            if (model.MarinePartial.IsManualExchangeRate &&
                model.MarinePartial.ExchangeRate != null &&
                model.MarinePartial.ExchangeRate != 0)
            {
                premiumCalculationResultModel.PerUnitNRSValue =
                    model.MarinePartial.ExchangeRate.Value / exchangeRate.BaseValue.Value;
            }
            else
            {
                premiumCalculationResultModel.PerUnitNRSValue =
                   exchangeRate.TargetSell.Value / exchangeRate.BaseValue.Value;
            }
        }

        switch (model.MarinePartial.ModeOfTransit)
        {
            case ModeOftransit.AirCargo:
                premiumCalculationResultModel.ModeOfTransitDiscountRate =
                    ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)MarineConfigType.DiscountOnAirTransit);
                break;
            case ModeOftransit.GoodsInTransit:
                if (!model.IsTPPolicy)
                {
                    if (model.MarinePartial.InlandTransitType == InlandTransitType.WithinNepal)
                    {
                        premiumCalculationResultModel.ModeOfTransitDiscountRate =
                            ConstrantValueHelper.GetValueWithoutRateConversion(
                                riskSetupModel,
                                (int)MarineConfigType.DiscountOnInlandTransitWithinNepal,
                                model.MarinePartial.InlandTransitDistance.Value);
                    }
                    else
                    {
                        premiumCalculationResultModel.ModeOfTransitDiscountRate =
                            ConstrantValueHelper.GetValueWithoutRateConversion(
                                riskSetupModel, (int)MarineConfigType.DiscountOnInlandTransitOutsideNepal);
                    }
                }

                break;
        }

        if (model.MarinePartial.IsContainerUsed)
        {
            premiumCalculationResultModel.ContainerDisocuntRate =
                ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel, (int)MarineConfigType.ContainerDiscount);
        }

        if (!model.MarinePartial.IsAllRiskSelected && !model.MarinePartial.IsMinimumRiskSelected)
        {
            if (model.MarinePartial.IsWaterDamageSelected)
            {
                premiumCalculationResultModel.WaterDamagePremiumRate =
                    ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)MarineConfigType.WaterDamage);
            }

            if (model.MarinePartial.IsTPNDSelected)
            {
                premiumCalculationResultModel.TPNDPremiumRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel, (int)MarineConfigType.TPND);
            }
            else if (model.MarinePartial.IsNonDeliverySelected)
            {
                premiumCalculationResultModel.NonDeliveryPremiumRate =
                    ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)MarineConfigType.NonDelivery);
            }
        }

        if (model.MarinePartial.IsSRCCSelected)
        {
            switch (model.MarinePartial.ModeOfTransit)
            {
                case ModeOftransit.AirCargo:
                    premiumCalculationResultModel.SRCCRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel, (int)MarineConfigType.SRCCByAir);
                    break;
                case ModeOftransit.GoodsInTransit:
                    premiumCalculationResultModel.SRCCRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel, (int)MarineConfigType.SRCCForInlandOnly);
                    break;
                case ModeOftransit.MarineCargo:
                    premiumCalculationResultModel.SRCCRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                    riskSetupModel, (int)MarineConfigType.SRCCIncludingByShip);
                    break;
            }
        }

        if (model.AgentId == null && model.IsTPPolicy == false)
        {
            premiumCalculationResultModel.DirectDiscountRate = ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel, (int)MarineConfigType.DirectDiscount);
        }

        #endregion

        #region Sum Insured and Initial Basic Premium Calculation
        premiumCalculationResultModel.MarineIndividualCalculations = new List<MarineIndividualCalculation>();
        foreach (var materialOfInsurance in model.MarinePartial.MaterialsOfInsurance)
        {
            materialOfInsurance.PrimaryId = Guid.NewGuid().ToString();
            materialOfInsurance.FullUnitValue = materialOfInsurance.UnitValue;
            var individualCalculation = new MarineIndividualCalculation();
            individualCalculation.PrimaryId = materialOfInsurance.PrimaryId;
            individualCalculation.ProductDescription = materialOfInsurance.Description;
            individualCalculation.InvoiceAmount = materialOfInsurance.Quantity * materialOfInsurance.UnitValue;
            individualCalculation.ToleranceAmount =
                individualCalculation.InvoiceAmount * premiumCalculationResultModel.ToleranceRate / 100;
            individualCalculation.AmountWithTolerance =
                individualCalculation.InvoiceAmount + individualCalculation.ToleranceAmount;

            individualCalculation.IncrementalCostAmount =
                individualCalculation.AmountWithTolerance * premiumCalculationResultModel.IncrementalCostRate / 100;
            individualCalculation.AmountWithIncrementalCost =
                individualCalculation.AmountWithTolerance + individualCalculation.IncrementalCostAmount;
            individualCalculation.DutyAmount =
                individualCalculation.AmountWithIncrementalCost * premiumCalculationResultModel.DutyRate / 100;


            individualCalculation.SumInsuredInCurrencyOfValue =
                (individualCalculation.AmountWithIncrementalCost + individualCalculation.DutyAmount).RoundToFourPrecisions();
            individualCalculation.SumInsuredInNRS =
                (individualCalculation.SumInsuredInCurrencyOfValue * premiumCalculationResultModel.PerUnitNRSValue).RoundToFourPrecisions();



            premiumCalculationResultModel.TotalSumInsuredAmount += individualCalculation.InvoiceAmount.RoundToFourPrecisions();


            premiumCalculationResultModel.SumInsuredAmount += individualCalculation.SumInsuredInNRS;

            premiumCalculationResultModel.TotalSumInsuredInCurrencyOfValue +=
                individualCalculation.SumInsuredInCurrencyOfValue;

            var tariffSchedule = await _marineTariffScheduleService.GetSingleMarineTariffScheduleByCode(
                materialOfInsurance.ProductTypeCode);

            if (tariffSchedule == null)
            {
                throw new ArgumentException(
                    $"Tarriff schedule for product code {materialOfInsurance.ProductTypeCode} is unavailable.",
                    nameof(materialOfInsurance.ProductTypeCode));
            }

            if (model.MarinePartial.IsAllRiskSelected)
            {
                individualCalculation.TariffSchedulePremiumRate = tariffSchedule.AllRiskValue;
            }
            else if (model.MarinePartial.IsMinimumRiskSelected)
            {
                if (model.IsTPPolicy)
                {
                    individualCalculation.TariffSchedulePremiumRate = marineExtensionRisk;
                }
                else
                {
                    individualCalculation.TariffSchedulePremiumRate = tariffSchedule.MinimumRiskValue;
                }
            }
            else
            {
                individualCalculation.TariffSchedulePremiumRate = tariffSchedule.BasicRiskValue;
            }

            individualCalculation.TariffSchedulePremiumAmount =
                individualCalculation.SumInsuredInNRS * individualCalculation.TariffSchedulePremiumRate / 100;

            premiumCalculationResultModel.MarineIndividualCalculations.Add(individualCalculation);

            premiumCalculationResultModel.InitialBasicpremium += individualCalculation.TariffSchedulePremiumAmount;
        }
        #endregion

        #region Discount Rate Calculation Based on Letter of Credit
        if (model.MarinePartial.LettersOfCredit != null)
        {
            if (model.MarinePartial.LettersOfCredit.Count == 1)
            {
                premiumCalculationResultModel.LCorDPDiscountRate =
                    ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel,
                        (int)MarineConfigType.DiscountOnLetterOfCredit,
                        model.MarinePartial.LettersOfCredit.FirstOrDefault().LCAmount);
            }
            else if (model.MarinePartial.LettersOfCredit.Count > 1)
            {
                premiumCalculationResultModel.LCorDPDiscountRate =
                    ConstrantValueHelper.GetValueWithoutRateConversion(
                        riskSetupModel,
                        (int)MarineConfigType.DiscountByDeclaredPolicy,
                        model.MarinePartial.LettersOfCredit.Sum(lc => lc.LCAmount));
            }
        }
        #endregion

        #region Basic Premium Calculation
        premiumCalculationResultModel.ModeOfTransitDiscountAmount =
               premiumCalculationResultModel.InitialBasicpremium *
               premiumCalculationResultModel.ModeOfTransitDiscountRate / 100;
        premiumCalculationResultModel.PremiumAfterModeOfTransitDiscount =
            premiumCalculationResultModel.InitialBasicpremium -
            premiumCalculationResultModel.ModeOfTransitDiscountAmount;
        premiumCalculationResultModel.ContainerDiscountAmount =
            premiumCalculationResultModel.PremiumAfterModeOfTransitDiscount *
            premiumCalculationResultModel.ContainerDisocuntRate / 100;
        premiumCalculationResultModel.PremiumAfterContainerDiscount =
            premiumCalculationResultModel.PremiumAfterModeOfTransitDiscount -
            premiumCalculationResultModel.ContainerDiscountAmount;

        if (!model.MarinePartial.IsAllRiskSelected && !model.MarinePartial.IsMinimumRiskSelected)
        {
            if (model.MarinePartial.IsWaterDamageSelected)
            {
                premiumCalculationResultModel.WaterDamagePremiumAmount =
                    premiumCalculationResultModel.InitialBasicpremium *
                    premiumCalculationResultModel.WaterDamagePremiumRate / 100;
            }
            if (model.MarinePartial.IsTPNDSelected)
            {
                premiumCalculationResultModel.TPNDPremiumAmount =
                    premiumCalculationResultModel.InitialBasicpremium *
                    premiumCalculationResultModel.TPNDPremiumRate / 100;
            }
            else if (model.MarinePartial.IsNonDeliverySelected)
            {
                premiumCalculationResultModel.NonDeliveryPremiumAmount =
                    premiumCalculationResultModel.InitialBasicpremium *
                    premiumCalculationResultModel.NonDeliveryPremiumRate / 100;
            }
        }


        premiumCalculationResultModel.PremiumWithWaterDamage =
            premiumCalculationResultModel.PremiumAfterContainerDiscount +
            premiumCalculationResultModel.WaterDamagePremiumAmount;
        premiumCalculationResultModel.PremiumWithNonDelivery =
            premiumCalculationResultModel.PremiumWithWaterDamage +
            premiumCalculationResultModel.NonDeliveryPremiumAmount;
        premiumCalculationResultModel.PremiumWithTPND =
            premiumCalculationResultModel.PremiumWithNonDelivery + premiumCalculationResultModel.TPNDPremiumAmount;
        premiumCalculationResultModel.DirectDiscountOnBasic =
            premiumCalculationResultModel.PremiumWithTPND * premiumCalculationResultModel.DirectDiscountRate / 100;
        premiumCalculationResultModel.PremiumAfterDirectDiscountOnBasic =
            premiumCalculationResultModel.PremiumWithTPND - premiumCalculationResultModel.DirectDiscountOnBasic;
        premiumCalculationResultModel.SpecialLCorDPDiscountOnBasic =
            premiumCalculationResultModel.PremiumAfterDirectDiscountOnBasic *
            premiumCalculationResultModel.LCorDPDiscountRate / 100;
        premiumCalculationResultModel.BasicPremium =
            (premiumCalculationResultModel.PremiumAfterDirectDiscountOnBasic -
            premiumCalculationResultModel.SpecialLCorDPDiscountOnBasic).RoundToFourPrecisions();
        if (model.IsCoinsurance)
        {
            premiumCalculationResultModel.BasicPremiumWithoutCoinsuranceRate = premiumCalculationResultModel.BasicPremium;
            premiumCalculationResultModel.BasicPremium = CoinsurancePremiumCalculator.GetCoinsurancePremium(premiumCalculationResultModel.BasicPremium, model.HGIShareRate);
        }
        if (model.MarinePartial.IsInstallmentPayment)
        {
            premiumCalculationResultModel.IsInstallmentPremium = model.MarinePartial.IsInstallmentPayment;
            model.MarinePartial.InvoiceText = "First Installment Payment.";
            premiumCalculationResultModel.FullBasicPremiumWithoutInstallment = premiumCalculationResultModel.BasicPremium;
            premiumCalculationResultModel.BasicPremium = model.MarinePartial.BasicPremiumInstallment;
        }
        premiumCalculationResultModel.TotalBasicPremium = premiumCalculationResultModel.BasicPremium.RoundToFourPrecisions();
        #endregion

        #region SRCC Premium Calculation
        premiumCalculationResultModel.SRCCPremiumAmount =
            premiumCalculationResultModel.SumInsuredAmount * premiumCalculationResultModel.SRCCRate / 100;
        premiumCalculationResultModel.DirectDiscountOnSRCC =
            premiumCalculationResultModel.SRCCPremiumAmount *
            premiumCalculationResultModel.DirectDiscountRate / 100;
        premiumCalculationResultModel.PremiumAfterDirectDiscountOnSRCC =
            premiumCalculationResultModel.SRCCPremiumAmount - premiumCalculationResultModel.DirectDiscountOnSRCC;
        premiumCalculationResultModel.SpecialLCorDPDiscountOnSrcc =
            premiumCalculationResultModel.PremiumAfterDirectDiscountOnSRCC *
            premiumCalculationResultModel.LCorDPDiscountRate / 100;
        premiumCalculationResultModel.TotalSRCCPremiumAmount =
            (premiumCalculationResultModel.PremiumAfterDirectDiscountOnSRCC -
            premiumCalculationResultModel.SpecialLCorDPDiscountOnSrcc).RoundToFourPrecisions();
        if (model.IsCoinsurance)
        {
            premiumCalculationResultModel.RSMDTPremiumWithoutCoinsuranceRate = premiumCalculationResultModel.TotalSRCCPremiumAmount;
            premiumCalculationResultModel.TotalSRCCPremiumAmount = CoinsurancePremiumCalculator.GetCoinsurancePremium(premiumCalculationResultModel.TotalSRCCPremiumAmount, model.HGIShareRate);
        }
        if (model.MarinePartial.IsInstallmentPayment)
        {
            premiumCalculationResultModel.IsInstallmentPremium = model.MarinePartial.IsInstallmentPayment;

            decimal marinelimit =
                ConstrantValueHelper.GetValueWithoutRateConversion(riskSetupModel,
                    (int)MarineConfigType.InstallmentLimit);

            if (premiumCalculationResultModel.SumInsuredAmount <= marinelimit)
            {
                throw new Exception($"Installment not allowed for SI less than or equals to {marinelimit}.");
            }

            model.MarinePartial.InvoiceText = "First Installment Payment.";
            premiumCalculationResultModel.FullSRCCPremiumWithoutInstallment = premiumCalculationResultModel.TotalSRCCPremiumAmount;
            premiumCalculationResultModel.TotalSRCCPremiumAmount = model.MarinePartial.SRCCPremiumInstallment;
        }
        premiumCalculationResultModel.PoolPremiumAmount = premiumCalculationResultModel.TotalSRCCPremiumAmount;
        #endregion

        #region Stamp and VAT Calculation
        premiumCalculationResultModel.MinimumGrossPremium = (ConstrantValueHelper.GetValueWithoutRateConversion(
                riskSetupModel, (int)MarineConfigType.MinimumGrossPremium)).RoundToFourPrecisions();
        premiumCalculationResultModel.CalculatedGrossPremium =
            (premiumCalculationResultModel.BasicPremium + premiumCalculationResultModel.TotalSRCCPremiumAmount).RoundToFourPrecisions();
        if (premiumCalculationResultModel.CalculatedGrossPremium < premiumCalculationResultModel.MinimumGrossPremium)
        {
            premiumCalculationResultModel.GrossPremiumAmount = premiumCalculationResultModel.MinimumGrossPremium;
            premiumCalculationResultModel.BasicPremium = premiumCalculationResultModel.MinimumGrossPremium - premiumCalculationResultModel.TotalSRCCPremiumAmount;
        }
        else
        {
            premiumCalculationResultModel.GrossPremiumAmount = premiumCalculationResultModel.CalculatedGrossPremium;
        }
        premiumCalculationResultModel.TotalPremiumAmount = premiumCalculationResultModel.GrossPremiumAmount.RoundToFourPrecisions();
        if (model.IsCoinsurance && !model.IsHGILead)
        {
            premiumCalculationResultModel.StampDutyAmount = 0;
        }
        else
        {
            premiumCalculationResultModel.StampDutyAmount = ConstrantValueHelper.GetGlobalValueWithoutRateConversion(
               globalriskSetupModel,
               (int)GlobalConfigType.StampDuty,
               premiumCalculationResultModel.SumInsuredAmount);
        }

        premiumCalculationResultModel.VatPercent = ConstrantValueHelper.GetValueWithoutRateConversion(
              riskSetupModel, (int)MarineConfigType.VAT);
        premiumCalculationResultModel.VatAmount = (premiumCalculationResultModel.TotalPremiumAmount * premiumCalculationResultModel.VatPercent / 100).RoundToFourPrecisions();
        premiumCalculationResultModel.PremiumAfterStamp = premiumCalculationResultModel.VatAmount +
                                                          premiumCalculationResultModel.TotalPremiumAmount +
                                                          premiumCalculationResultModel.StampDutyAmount;
        premiumCalculationResultModel.NetPremiumAmount = premiumCalculationResultModel.PremiumAfterStamp;

        //SI incase of coinsurance 
        if (model.IsCoinsurance)
        {
            premiumCalculationResultModel.MarineIndividualCalculations = new List<MarineIndividualCalculation>();
            premiumCalculationResultModel.SumInsuredAmount = 0;
            premiumCalculationResultModel.TotalSumInsuredInCurrencyOfValue = 0;
            premiumCalculationResultModel.TotalSumInsuredAmount = 0;
            foreach (var materialOfInsurance in model.MarinePartial.MaterialsOfInsurance)
            {
                materialOfInsurance.PrimaryId = Guid.NewGuid().ToString();
                var individualCalculation = new MarineIndividualCalculation();
                individualCalculation.PrimaryId = materialOfInsurance.PrimaryId;
                individualCalculation.ProductDescription = materialOfInsurance.Description;
                if (!model.IsDraft)
                    materialOfInsurance.FullUnitValue = materialOfInsurance.UnitValue;

                materialOfInsurance.UnitValue = CoinsurancePremiumCalculator.GetCoinsurancePremium(materialOfInsurance.FullUnitValue, model.HGIShareRate);
                individualCalculation.InvoiceAmount = materialOfInsurance.Quantity * materialOfInsurance.FullUnitValue;
                individualCalculation.ToleranceAmount =
                    individualCalculation.InvoiceAmount * premiumCalculationResultModel.ToleranceRate / 100;
                individualCalculation.AmountWithTolerance =
                    individualCalculation.InvoiceAmount + individualCalculation.ToleranceAmount;
                individualCalculation.IncrementalCostAmount =
                    individualCalculation.AmountWithTolerance * premiumCalculationResultModel.IncrementalCostRate / 100;
                individualCalculation.AmountWithIncrementalCost =
                    individualCalculation.AmountWithTolerance + individualCalculation.IncrementalCostAmount;
                individualCalculation.DutyAmount =
                    individualCalculation.AmountWithIncrementalCost * premiumCalculationResultModel.DutyRate / 100;


                individualCalculation.SumInsuredInCurrencyOfValue =
                    (individualCalculation.AmountWithIncrementalCost + individualCalculation.DutyAmount).RoundToFourPrecisions();
                individualCalculation.SumInsuredInNRS =
                    (individualCalculation.SumInsuredInCurrencyOfValue * premiumCalculationResultModel.PerUnitNRSValue).RoundToFourPrecisions();



                premiumCalculationResultModel.TotalSumInsuredAmount += individualCalculation.InvoiceAmount.RoundToFourPrecisions();

                if (model.IsCoinsurance)
                {
                    CalculateCoinsuranceAmount(individualCalculation, model.HGIShareRate);
                    premiumCalculationResultModel.FullSumInsured += individualCalculation.FullSumInsuredInNRS;
                    model.FullSumInsured = premiumCalculationResultModel.FullSumInsured;
                    premiumCalculationResultModel.TotalSumInsuredAmount += (individualCalculation.InvoiceAmount.RoundToFourPrecisions() * model.HGIShareRate) / 100;

                }


                individualCalculation.SumInsuredInCurrencyOfValue = (individualCalculation.SumInsuredInCurrencyOfValue * model.HGIShareRate) / 100;


                premiumCalculationResultModel.SumInsuredAmount += individualCalculation.SumInsuredInNRS;

                premiumCalculationResultModel.TotalSumInsuredInCurrencyOfValue +=
                    individualCalculation.SumInsuredInCurrencyOfValue;

                var tariffSchedule = await _marineTariffScheduleService.GetSingleMarineTariffScheduleByCode(
                    materialOfInsurance.ProductTypeCode);

                if (tariffSchedule == null)
                {
                    throw new ArgumentException(
                        $"Tarriff schedule for product code {materialOfInsurance.ProductTypeCode} is unavailable.",
                        nameof(materialOfInsurance.ProductTypeCode));
                }

                if (model.MarinePartial.IsAllRiskSelected)
                {
                    individualCalculation.TariffSchedulePremiumRate = tariffSchedule.AllRiskValue;
                }
                else if (model.MarinePartial.IsMinimumRiskSelected)
                {
                    individualCalculation.TariffSchedulePremiumRate = tariffSchedule.MinimumRiskValue;
                }
                else
                {
                    individualCalculation.TariffSchedulePremiumRate = tariffSchedule.BasicRiskValue;
                }

                individualCalculation.TariffSchedulePremiumAmount =
                    individualCalculation.SumInsuredInNRS * individualCalculation.TariffSchedulePremiumRate / 100;

                premiumCalculationResultModel.MarineIndividualCalculations.Add(individualCalculation);
            }
        }

        if (model.MarinePartial.IsAllRiskSelected && model.MarinePartial.IsSRCCSelected) premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;
        else if (model.MarinePartial.IsAllRiskSelected) premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;
        else if (!model.MarinePartial.IsAllRiskSelected && model.MarinePartial.IsSRCCSelected) premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;
        else if (!model.MarinePartial.IsMinimumRiskSelected && model.MarinePartial.IsSRCCSelected) premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicAndRSMDT;
        else premiumCalculationResultModel.RiskTypeSelected = model.RiskTypeSelected = RiskTypeSelected.BasicOnly;


        #endregion



        return premiumCalculationResultModel;
    }
    #region helpers
    public decimal GetProRateAmount(decimal amount)
    {
        return (proRate * amount);
    }

    public void CalculateCoinsuranceAmount(MarineIndividualCalculation marineIndividualCalculation, decimal rate)
    {
        marineIndividualCalculation.FullSumInsuredInNRS = marineIndividualCalculation.SumInsuredInNRS;
        marineIndividualCalculation.SumInsuredInNRS = CoinsurancePremiumCalculator.GetCoinsurancePremium(marineIndividualCalculation.SumInsuredInNRS, rate);
    }

    public Task<PremiumCalculationResultModel> CalculateEndorsementPremium(EndorsementViewModel endorsementVM, PremiumCalculationResultModel premiumCalculation)
    {
        throw new NotImplementedException();
    }
    #endregion
}
