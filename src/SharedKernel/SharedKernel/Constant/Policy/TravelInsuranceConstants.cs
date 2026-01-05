using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Constant.Policy;
public class TravelInsuranceConstants
{
    #region Plans

    public const string PlanA = "Worldwide excluding USA and Canada";
    public const string PlanB = "Worldwide including USA and Canada";
    public const string PlanC = "Asian countries excluding SAARC countries";
    public const string PlanD = "SAARC countries";
    public const string PlanE = "Student- worldwide excluding USA and Canada";
    public const string PlanF = "Student- worldwide including USA and Canada";
    public const string PlanG = "Student- SAARC countries";
    public const string PlanH = "Student";
    public const string PlanL = "Schengen Countries - EURO";

    //for ITI
    public const string PlanI = "Asia Pacific";

    public const string PlanJ = "Worldwide 1 -(Excluding USA and Canada)";
    public const string PlanK = "Worldwide 2 -(Including USA and Canada)";

    #endregion Plans

    #region insurance type

    public const string StandardBasicAC = "Standard Basic (A-C)";
    public const string StandardBasicAB = "Standard Basic (A-B)";
    public const string StandardFullCoverageAN = "Standard Full Coverage (A-N)";
    public const string StandardFullCoverageAI = "Standard Full Coverage (A-I)";

    public const string BudgetFullCoverageAG = "Budget Plan (A-G)";

    public const string SchengenAG = "Schengen Plan (A-G)";

    public const string GoldBasicAC = "Gold Basic (A-C)";
    public const string GoldBasicAB = "Gold Basic (A-B)";
    public const string GoldFullCoverageAN = "Gold Full Coverage (A-N)";
    public const string GoldFullCoverageAI = "Gold Full Coverage (A-I)";

    public const string PremiumBasicAC = "Premium Basic (A-C)";
    public const string PremiumBasicAB = "Premium Basic (A-B)";
    public const string PremiumFullCoverageAN = "Premium Full Coverage (A-N)";
    public const string PremiumFullCoverageAI = "Premium Full Coverage (A-I)";

    public const string PlatinumBasicAC = "Platinum Basic (A-C)";
    public const string PlatinumBasicAB = "Platinum Basic (A-B)";
    public const string PlatinumFullCoverageAN = "Platinum Full Coverage (A-N)";
    public const string PlatinumFullCoverageAI = "Platinum Full Coverage (A-I)";

    public const string Gold = "Gold";
    public const string Platinum = "Platinum";
    public const string Silver = "Silver";


    #endregion insurance type

    #region Plans BUPA

    public const string BUPAPlanA = "Business Travel";
    public const string BUPAPlanB = "Single Trip";
    public const string BUPAPlanC = "Annual Trip";

    public const string BUPACoverPlanA = "Medical Cover";
    public const string BUPACoverPlanB = "Non-Medical Option";
    public const string BUPACoverPlanC = "Trip Cancellation";

    #endregion Plans BUPA

    #region TripType TI

    public const string TripTypeA = "Annual Multi Trip";

    #endregion TripType TI

    #region Id Types

    //Passport No, Citizenship No , ID Card No.

    public static List<KeyValuePair<string, int>> getIdTypes()
    {
        var idTypes = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("Passport No", 1),
            new KeyValuePair<string, int>("Citizenship No", 2),
            new KeyValuePair<string, int>("ID Card No", 3),
            new KeyValuePair<string, int>("NationalID No", 4)
        };
        return idTypes;
    }

    #endregion Id Types

    // #region ITI schedule  footer constant claim and Assistance Company details
    //
    // public const string ITIFooterConstantOld = "Claim and Assistance company\n" +
    //                                  "For 24 hours Emergency Assistance call\n" +
    //                                  "Asia Assistance Network (M) SDN.BHD\n" +
    //                                  "AA One NO. 1, BLOCK N, JAYA ONE,\n" +
    //                                  "72A, Jalan University, 46200 Petaling Jaya\n" +
    //                                  "Selangor Darul Ehsan\n" +
    //                                  "Malaysia\n" +
    //                                  "Hot Line No.:+603 7628 3602\n" +
    //                                  "Tel:+603 7629 3888\n" +
    //                                  "E-mail: marketing@asia-assistance.com\n" +
    //                                  "www.asia-assistance.com";
    // public const string ITIFooterConstantNew = "Country\n" +
    //                                            "\n" +
    //                                            "Contact Numbers\n" +
    //                                            "\n" +
    //                                            "USA / Canada\n" +
    //                                            "+1 514 448 4417\n" +
    //                                            "\n" +
    //                                            "France / Europe\n" +
    //                                            "+33 9 70 73 22 47\n" +
    //                                            "\n" +
    //                                            "International\n" +
    //                                            "+961 9 211 662\n" +
    //                                            "\n" +
    //                                            "Email: request@swanassistance.com";
    //
    // #endregion
}
