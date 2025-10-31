using System.Collections.Generic;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class IRMISPolicy
    {
        public string thirdPartyPremiumAmount { get; set; }

        public string accidentalPremiumAmount { get; set; }
        public string policyIssuenceId { get; set; }
        public string partyDetail { get; set; }
        public string agentDetail { get; set; }
        public string policyNo { get; set; }
        public string name { get; set; }
        public string portfolio { get; set; }
        public string subPortfolio { get; set; }
        public string sumInsured { get; set; }
        public string premiumAmount { get; set; }
        public string rsmdtAmount { get; set; }
        public string subsidizedPremiumAmount { get; set; }
        public string totalPremiumAmount { get; set; }
        public string effectiveDate { get; set; }
        public string expiryDate { get; set; }
        public string documentNo { get; set; }
        public string documentType { get; set; }
        public string documentIssuedDate { get; set; }
        public string branchProvinceId { get; set; }
    }

    public class IRMISClaimDetailsJSON
    {
        public string agentCompanyCode { get; set; }
        public List<SurveyorDetails> surveyorDetails { get; set; }
        public string intimationDate { get; set; }
        public string dateOfLoss { get; set; }
        public string thirdPartyPremiumAmount { get; set; }
        public string accidentalPremiumAmount { get; set; }
        public string registerDate { get; set; }
        public string agentName { get; set; }
        public string agentLicenseNo { get; set; }
        public string surveyorCompanyCode { get; set; }
        public string surveyorName { get; set; }
        public string surveyorLicenseNo { get; set; }
        public string policyNo { get; set; }
        public string claimType { get; set; }
        public string claimNo { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string mobileNo { get; set; }
        public string email { get; set; }
        public string fatherMotherName { get; set; }
        public string grandFatherGrandMotherName { get; set; }
        public string portfolio { get; set; }
        public string subPortfolio { get; set; }
        public string agricultureSector { get; set; }
        public string agricultureSubSector { get; set; }
        public string claimAmount { get; set; }
        public string sumInsured { get; set; }
        public string premiumAmount { get; set; }
        public string rsmdtAmount { get; set; }
        public string subsidizedPremiumAmount { get; set; }
        public string totalPremiumAmount { get; set; }
        public string effectiveDate { get; set; }
        public string expiryDate { get; set; }
        public string documentNo { get; set; }
        public string documentType { get; set; }
        public string documentIssuedDistrict { get; set; }
        public string documentIssuedDate { get; set; }
        public string branchProvinceId { get; set; }
        public string branchDistrictId { get; set; }
        public string branchLocalUnitId { get; set; }
        public string provinceId { get; set; }
        public string districtId { get; set; }
        public string localUnitId { get; set; }
        public string wardNo { get; set; }
        public string tole { get; set; }
        public string temporaryProvinceId { get; set; }
        public string temporaryDistrictId { get; set; }
        public string temporaryLocalUnitId { get; set; }
        public string temporaryWardNo { get; set; }
        public string temporaryTole { get; set; }
    }

    public class PostApiResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string Extras { get; set; }
    }

    public class ClaimApiResponse
    {
        public List<SurveyorDetails> SurveryorDetails { get; set; }
        public string ClaimType { get; set; }
        public string ClaimNumber { get; set; }
        public decimal ClaimAmount { get; set; }
        public string OccranceDate { get; set; }
        public string RegisteredDate { get; set; }

        public string IntimationDate { get; set; }

        public string PolicyIssuanceId { get; set; }
    }

    public class SurveyorDetails
    {
        public string surveyorCompanyCode { get; set; }
        public string surveyorName { get; set; }
        public string surveyorLicenseNo { get; set; }
        public string surveyorFeeAmount { get; set; }
        public string claimNo { get; set; }
    }

    public static class IrmisRecordType
    {
        public const string FreshPolicy = "Fresh";
        public const string EndorsedPolicy = "Endorsed";
        public const string Claim = "Claim";
    }

    public static class ResponseStatusCode
    {
        public const int FailedWhileParse = 777;
    }


    public static class IrmisUrl
    {
        public const string FreshPolicyTaskUrl = "api/InsuranceAuthority/FreshPolicyTask";
        public const string EndorsedPolicyTaskUrl = "api/InsuranceAuthority/EndorsedPolicyTask";
        public const string ClaimReportingDetailsTaskUrl = "api/InsuranceAuthority/ClaimReportingDetailsTask";
        public const string NonLifeClaimDetailsUrl = "api/v1/NonLifeReportingClaimDetails/NonLifeClaimDetails";
        public const string NonLifeEndorsementPolicyUrl = "api/v1/NonLifePolicyReporting/NonLifeEndorsementPolicy";
        public const string NonLifeFreshPolicyUrl = "api/v1/NonLifePolicyReporting/NonLifeFreshPolicy";
        public const string TokenUrl = "api/auth/token";
        public const string GetAddressByBranchCodeUrl = "api/Branch/GetAddressByBranchCode";
        public const string GetClaimDetailsForIRMISUrl = "api/Claim/GetClaimDetailsForIRMIS";
        public const string Bearer = "Bearer";
        public const string Basic = "Basic";
        public const string MeadiaType = "application/json";
    }

    public static class FakeResponse
    {
        public const string dummyResponse = "{\"Code\":\"000\",\"Message\":\" No Content\"}";
        public const string successCode = "\"Code\":\"000\"";
    }
}