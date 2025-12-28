using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedKernel.Constant;
public static class RiskTypeSelected
{
    public const string BasicAndThirdParty = "Basic and Third Party";
    public const string ThirdParty = "Third Party";
    public const string BasicOnly = "Basic";
    public const string AllRisks = "Basic,Third Party and RSMDT";
    public const string BasicAndRSMDT = "Basic and RSMDT";
    public const string RSMDTOnly = "RSMDT";
    public const string ThirdPartyOld = "Third Party-Old";
    public const string ThirdParty2 = "Third Party 2";


    //branch list for thirdparty2
    public static readonly string[] branchList = new[] { "" };//{ "PKR", "BTL", "BRTM", "BRT1", "THK" };

    public static List<SelectListItem> GetRiskTypeList()
    {
        List<SelectListItem> riskTypeList = new List<SelectListItem>();
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.BasicOnly, Text = RiskTypeSelected.BasicOnly });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.BasicAndThirdParty, Text = RiskTypeSelected.BasicAndThirdParty });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.BasicAndRSMDT, Text = RiskTypeSelected.BasicAndRSMDT });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.ThirdParty, Text = RiskTypeSelected.ThirdParty });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.ThirdPartyOld, Text = RiskTypeSelected.ThirdPartyOld });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.ThirdParty2, Text = RiskTypeSelected.ThirdParty2 });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.AllRisks, Text = RiskTypeSelected.AllRisks });
        riskTypeList.Add(new SelectListItem { Value = RiskTypeSelected.RSMDTOnly, Text = RiskTypeSelected.RSMDTOnly });
        return riskTypeList;
    }

    public static List<SelectListItem> GetRiskTypeListWithIndividualRiskTypes()
    {
        return new List<SelectListItem>
            {
                new SelectListItem{Text=RiskTypeSelected.BasicOnly,Value = "0"},
                new SelectListItem{Text=RiskTypeSelected.RSMDTOnly,Value = "1"},
                new SelectListItem{Text=RiskTypeSelected.ThirdParty,Value = "2"}
            };
    }

}
