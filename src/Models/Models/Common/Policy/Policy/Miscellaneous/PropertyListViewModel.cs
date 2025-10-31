using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class PropertyListViewModel
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string Particular { get; set; }
        public string IdentificationCode { get; set; }
        public decimal Value { get; set; }
        public decimal ValueInPrintForDeletion { get; set; }
        public decimal FullValue { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsActive { get; set; }
    }
    public class AllRiskPartialViewModel
    {
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool RiotStrike { get; set; }
        public bool IsPremiumManual { get; set; }
        public decimal SumInsured { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }
        public string PolicyIssuedAddress { get; set; }
        public string[] AllRiskEndorsements { get; set; }
        public List<PropertyListViewModel> Properties { get; set; }
        public List<PropertyListViewModel> AddedProperties { get; set; }
        public List<PropertyListViewModel> UpdatedProperties { get; set; }
        public List<PropertyListViewModel> DeletedProperties { get; set; }
        public string ConfirmationDetails { get; set; }
        public string DeductiblesText { get; set; }
        public decimal ExcessAmount { get; set; }
        public decimal ExcessRate { get; set; }
        public string Geographical { get; set; }
        public string Business { get; set; }
    }

}
