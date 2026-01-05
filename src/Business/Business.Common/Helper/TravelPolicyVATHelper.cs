using SharedKernel.Constant.Permission;
using SharedKernel.Constant.Policy;

namespace Business.Common.Helper;
public class TravelPolicyVATHelper
{
    public static bool CheckVatApplicable(string classID, string planType, string benefitType)
    {
        switch (classID)
        {
            case PortfolioClassConstants.TravelInsurance:
                //studentPlan
                if (planType == TravelInsuranceConstants.PlanE || planType == TravelInsuranceConstants.PlanF || planType == TravelInsuranceConstants.PlanG || planType == TravelInsuranceConstants.PlanH)
                {
                    if (benefitType == TravelInsuranceConstants.StandardBasicAC || benefitType == TravelInsuranceConstants.StandardBasicAB
                        || benefitType == TravelInsuranceConstants.GoldBasicAC || benefitType == TravelInsuranceConstants.GoldBasicAB
                        || benefitType == TravelInsuranceConstants.PremiumBasicAC || benefitType == TravelInsuranceConstants.PremiumBasicAB
                        || benefitType == TravelInsuranceConstants.PlatinumBasicAC || benefitType == TravelInsuranceConstants.PlatinumBasicAB
                        )
                    {
                        return false;
                    }
                }
                //other than student
                else
                {
                    if (benefitType == TravelInsuranceConstants.StandardBasicAC || benefitType == TravelInsuranceConstants.StandardBasicAB
                        || benefitType == TravelInsuranceConstants.GoldBasicAC || benefitType == TravelInsuranceConstants.GoldBasicAB
                        || benefitType == TravelInsuranceConstants.PremiumBasicAC || benefitType == TravelInsuranceConstants.PremiumBasicAB
                        || benefitType == TravelInsuranceConstants.PlatinumBasicAC || benefitType == TravelInsuranceConstants.PlatinumBasicAB
                        )
                    {
                        return false;
                    }
                }
                return true;
            case PortfolioClassConstants.BUPA:
                var bupaBenefit = benefitType.Split(',');

                if (bupaBenefit.Length > 1)
                {
                    return true;
                }
                return false;
            case PortfolioClassConstants.InternationalTravelInsurance:
                return true;
            case PortfolioClassConstants.ITIMP:
                return true;
            default:
                return true;
        }
    }
}
