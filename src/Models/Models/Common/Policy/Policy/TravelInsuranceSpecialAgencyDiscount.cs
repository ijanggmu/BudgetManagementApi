using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Policy;
public static class TravelInsuranceSpecialAgencyDiscount
{
    static TravelInsuranceSpecialAgencyDiscount()
    {
        SpecialAgentDiscountDetails = new List<TravelInsuranceDiscountModel>
            {
                new TravelInsuranceDiscountModel
                {
                    Id = new Guid().ToString(),
                    AgentName = "Machhapuchchhre Bank Ltd",
                    AgentCode = "MBL",
                    DiscountPercentage = 10,
                    EffectiveFrom = new DateTime(2020, 02, 25),
                    IsActive = true
                },
                new TravelInsuranceDiscountModel
                {
                    Id = new Guid().ToString(),
                    AgentName = "Nepal Bank",
                    AgentCode = "NB",
                    DiscountPercentage = 15,
                    EffectiveFrom = new DateTime(2020, 02, 25),
                    IsActive = false
                }
            };
    }

    public static List<TravelInsuranceDiscountModel> SpecialAgentDiscountDetails;
}

public class TravelInsuranceDiscountModel
{
    public string Id { get; set; }
    public string AgentName { get; set; }
    public string AgentCode { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public bool IsActive { get; set; }
}
