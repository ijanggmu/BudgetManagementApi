using Models.Common.Policy.Calculation;
using Models.Common.Policy.ThirdPartyApi.e2e;
using SharedKernel.Constant.Permission;
using SharedKernel.Helper;

namespace Business.Common.PremiumCalculation.Service;
public static class PremiumCalculationJsonService
{
    public static ICalculationPremiumJson GetCalculationJson(string portfolio,
    PremiumCalculationResultModel premiumCalculation)
    {
        ICalculationPremiumJson data = new CalculationPremium();


        switch (portfolio)
        {
            case PortfolioClassConstants.Motorcycle:
                data.BasicPremium = premiumCalculation.SubTotalA.RoundToFour();
                data.ThirdPartyPremium = premiumCalculation.SubTotalB.RoundToFour();
                data.RSMDTPremium = premiumCalculation.SubTotalC.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.TotalPremiumAmount.RoundToFour();
                data.StampDuty = premiumCalculation.StampDutyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium = premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.ElectricMotorcycle:
                data.BasicPremium =
                    premiumCalculation.SubTotalA.RoundToFour();
                data.ThirdPartyPremium =
                    premiumCalculation.SubTotalB.RoundToFour();
                data.RSMDTPremium =
                    premiumCalculation.SubTotalC.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.TotalPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.PrivateVehicle:
            case PortfolioClassConstants.ElectricVehicle:
                data.BasicPremium =
                    premiumCalculation.SubTotalA.RoundToFour();
                data.ThirdPartyPremium =
                    premiumCalculation.SubTotalB.RoundToFour();
                data.RSMDTPremium =
                    premiumCalculation.SubTotalD.RoundToFour();
                data.PersonalAccidentPremium =
                    premiumCalculation.SubTotalC.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.TotalPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.PersonalHealthInsurance:
                data.BasicPremium =
                    premiumCalculation.BasicPremium.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.PersonalAccident:
                data.BasicPremium =
                    premiumCalculation.BasicPremium.RoundToFour();
                data.RSMDTPremium =
                    premiumCalculation.RSMDTAmount.RoundToFour();
                data.GrossPremium =
                    premiumCalculation.GrossPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.TravelInsurance:
            case PortfolioClassConstants.InternationalTravelInsurance:
                data.BasicPremium =
                    premiumCalculation.BasicPremium.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                break;
            case PortfolioClassConstants.Cattle:
            case PortfolioClassConstants.Fish:
            case PortfolioClassConstants.Goat:
            case PortfolioClassConstants.Cow:
            case PortfolioClassConstants.Buffalo:
            case PortfolioClassConstants.Sheep:
            case PortfolioClassConstants.Pig:
            case PortfolioClassConstants.Donkey:
            case PortfolioClassConstants.Horse:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.BasicPremium = (premiumCalculation.GrossPremiumAmount - premiumCalculation.PAofInsured)
                    .RoundToFour();
                data.GrossPremium =
                    premiumCalculation.GrossPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.GovernmentSubsidyAmount = premiumCalculation.GovernmentSubsidyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.PersonalAccidentPremium = premiumCalculation.PAofInsured.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                data.PayableAmount = data.NetPremium - data.GovernmentSubsidyAmount;
                break;
            case PortfolioClassConstants.Vegetables:
            case PortfolioClassConstants.Turmeric:
            case PortfolioClassConstants.Sugarcane:
            case PortfolioClassConstants.Ginger:
            case PortfolioClassConstants.Paddy:
            case PortfolioClassConstants.Chaityadhan:
            case PortfolioClassConstants.Fruit:
            case PortfolioClassConstants.Orange:
            case PortfolioClassConstants.Junar:
            case PortfolioClassConstants.Lemon:
            case PortfolioClassConstants.Kiwi:
            case PortfolioClassConstants.DragonFruit:
            case PortfolioClassConstants.Mushroom:
            case PortfolioClassConstants.Bee:
            case PortfolioClassConstants.Cardamom:
            case PortfolioClassConstants.Tea:
            case PortfolioClassConstants.Coffee:
            case PortfolioClassConstants.Seed:
            case PortfolioClassConstants.Grass:
            case PortfolioClassConstants.Banana:
            case PortfolioClassConstants.Potato:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.BasicPremium = (premiumCalculation.GrossPremiumAmount - premiumCalculation.PAofInsured)
                    .RoundToFour();
                data.GrossPremium =
                    premiumCalculation.GrossPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.GovernmentSubsidyAmount = premiumCalculation.GovernmentSubsidyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.PersonalAccidentPremium = premiumCalculation.PAofInsured.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                data.PayableAmount = data.NetPremium - data.GovernmentSubsidyAmount;

                break;
            case PortfolioClassConstants.Poultry:
            case PortfolioClassConstants.Ostrich:
            case PortfolioClassConstants.Kalij:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.BasicPremium = (premiumCalculation.GrossPremiumAmount - premiumCalculation.PAofInsured)
                    .RoundToFour();
                data.GrossPremium =
                    premiumCalculation.GrossPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.GovernmentSubsidyAmount = premiumCalculation.GovernmentSubsidyAmount.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.PersonalAccidentPremium = premiumCalculation.PAofInsured.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                data.PayableAmount = data.NetPremium - data.GovernmentSubsidyAmount;

                break;
            case PortfolioClassConstants.CerealPaddy:
            case PortfolioClassConstants.Wheat:
            case PortfolioClassConstants.Maize:
            case PortfolioClassConstants.Barley:
            case PortfolioClassConstants.Millet:
            case PortfolioClassConstants.Cereal:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.BasicPremium = (premiumCalculation.GrossPremiumAmount - premiumCalculation.PAofInsured)
                    .RoundToFour();
                data.GrossPremium =
                    premiumCalculation.GrossPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.GovernmentSubsidyAmount = premiumCalculation.GovernmentSubsidyAmount.RoundToFour();
                data.PersonalAccidentPremium = premiumCalculation.PAofInsured.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();
                data.PayableAmount = data.NetPremium - data.GovernmentSubsidyAmount;

                break;

            case PortfolioClassConstants.Household:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.GrossPremium =
                    premiumCalculation.TotalPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();

                break;
            case PortfolioClassConstants.Property:
                data.SumInsured =
                    premiumCalculation.SumInsuredAmount.RoundToFour();
                data.GrossPremium =
                    premiumCalculation.TotalPremiumAmount.RoundToFour();
                data.StampDuty =
                    premiumCalculation.StampDutyAmount.RoundToFour();
                data.TotalPremium =
                    premiumCalculation.PremiumAfterStamp.RoundToFour();
                data.VatAmount = premiumCalculation.VatAmount.RoundToFour();
                data.NetPremium =
                    premiumCalculation.NetPremiumAmount.RoundToFour();

                break;
        }

        return data;
    }
}
