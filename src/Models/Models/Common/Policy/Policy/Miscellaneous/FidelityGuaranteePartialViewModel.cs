using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class FidelityGuaranteePartialViewModel
    {
        public List<EmployeeDetails> EmployeeDetailsList { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBurglarySelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsFidelityGuaranteeSelected { get; set; }
        public bool IsRateManual { get; set; }
        public decimal BurglaryRate { get; set; }
        public decimal FidelityGuaranteeRate { get; set; }
        public string ConfirmationDetails { get; set; }
        public bool IsAggregateSI{ get; set; }
        public decimal AggregateSI { get; set; }
        public int EmployeeCount { get; set; }
        public decimal PAAmount { get; set; }
        public decimal TotalPAAmount { get; set; }

    }
    public class EmployeeDetails
    {
        public string SN { get; set; }
        public string RefNo { get; set; }
        public string EmployeeName{ get; set; }
        public int Age{ get; set; }
        public string Occupancy{ get; set; }
        public decimal SumInsured { get; set; }
        public decimal FullSumInsured { get; set; }
    }
    public class EndorseEmployeeDetails: EmployeeDetails
    {
        public decimal TransactionSumInsured { get; set; }
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
    }
    public class FidelityGuaranteeEndorsePartialViewModel: FidelityGuaranteePartialViewModel
    {
        public List<EndorseEmployeeDetails> AddedEndorseEmployeeDetails { get; set; }
        public List<EndorseEmployeeDetails> UpdatedEndorseEmployeeDetails { get; set; }
        public List<EndorseEmployeeDetails> DiscontinuedEndorseEmployeeDetails { get; set; }
        public decimal TransactionBasicPremium { get; set; }
    }
}
