using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class COVIDPartialViewModel
    {
        public string InsuredName { get; set; }
        public string Address { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string Telephone { get; set; }
        public string Occupation { get; set; }
        public string OrganizationType { get; set; }
        public string CitizenshipNumber { get; set; }
        public string PanNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string Nominee { get; set; }
        public string Relationship { get; set; }
        public string CoverageType { get; set; }
        public bool IsIndividualOnly { get; set; }
        public decimal PolicyLimitAmount { get; set; }
        public bool IsMaxLimit { get; set; }
        public string PartyType { get; set; }
        public int NumberOfNominees { get; set; }
        public decimal PolicyTarrifAmount { get; set; }
        public List<NomineeDetail> NomineeList { get; set; }
        public string SubsidyType { get; set; }
        public int SubsidyValue { get; set; }
        public decimal GovernmentSubsidyRate { get; set; }
        public decimal CorporateSubsidyRate { get; set; }
        public decimal FamilySubsidyRate { get; set; }
        public decimal SubsidyAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool ExcludeInsured { get; set; }
    }

    public class COVIDEndorsementPartialViewModel
    {
        public List<NomineeDetail> NomineeList { get; set; }
        public List<EndorsedNomineeDetail> AddedNomineeList { get; set; }
        public List<EndorsedNomineeDetail> UpdatedNomineeList { get; set; }
        public List<EndorsedNomineeDetail> DiscontinedNomineeList { get; set; }

        public bool IsIndividualOnly { get; set; }
        public decimal PolicyLimitAmount { get; set; }
        public bool IsMaxLimit { get; set; }
        public int NumberOfNominees { get; set; }
        public decimal PolicyTarrifAmount { get; set; }
        public bool IsManualBasicPremium { get; set; }
    }
    public class NomineeDetail
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string Relation { get; set; }
        public string CitizenshipNumber { get; set; }
        public string NomineeNameRelation { get; set; }
        public string PrimaryId { get; set; } = Guid.NewGuid().ToString();
    }

    public class EndorsedNomineeDetail : NomineeDetail
    {
        public bool IsAdded{ get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
