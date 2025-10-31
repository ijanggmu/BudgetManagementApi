using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class MicroInsuranceMedicalPartialViewModel
    {
        public DateTime? Dob_AD { get; set; }
        public DateTime? Dob_BS { get; set; }
        public string Father_FullName { get; set; }
        public string PartyCode { get; set; }
        public string Mother_FullName { get; set; }
        public string Remarks { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public int MemberLimit { get; set; }
        public List<MIMInsuredPerson> InsuredPersons { get; set; }
        public decimal SumInsured { get; set; }
    }

    public class MIMInsuredPerson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
        public int Age { get; set; }
        public string HealthCondition { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsActive { get; set; }
    }

    public class MicroInsuranceMedicalEndorsePartialViewModel : MicroInsuranceMedicalPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public List<MIMInsuredPerson> AddedInsuredPersons { get; set; }
        public List<MIMInsuredPerson> UpdatedInsuredPersons { get; set; }
        public List<MIMInsuredPerson> DiscontinuedInsuredPersons { get; set; }

    }
}
