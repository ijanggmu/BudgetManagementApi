using System;
using System.Collections.Generic;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class QrCodeResponse
    {
        public string policyNumber { get; set; }
        public string PolicyPeriod { get; set; }
        public string IssueDate { get; set; }
        public string InsuredName { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Premium { get; set; }
        public string Branch { get; set; }
        public string Reciept { get; set; }
        public string Portfolio { get; set; }
        public string documentNumber { get; set; }
        public string invoiceNumber { get; set; }
        public string PassportNo { get; set; }
        public string Occupation { get; set; }
        public string Benefits { get; set; }
        public string DateOfBirth { get; set; }
        public string Plan { get; set; }
        public List<TravelInsuranceFamilyMembers> TIFamilyMembers { get; set; }
    }
    public class TravelInsuranceFamilyMembers
    {
        public string Relation { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
    }
}