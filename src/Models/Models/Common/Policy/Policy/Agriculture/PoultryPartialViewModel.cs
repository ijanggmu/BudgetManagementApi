using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class PoultryPartialViewModel : ILocation
    {
        [Required]
        public string NomineeFullName { get; set; }
        public string NomineeRelation { get; set; }
        public string NomineeFatherName { get; set; }
        public string NomineeMotherName { get; set; }
        [Required]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string NomineeContactNumber { get; set; }
        [Required]
        public string NomineeAddress { get; set; }
        [Required]
        public string PoultryName { get; set; }
        public string LandProvince { get; set; }
        public string LandDistrict { get; set; }
        public string LandMunicipality { get; set; }
        public decimal? CorporateDiscountRate { get; set; }

        public string LandWard { get; set; }
        public string LandStreetAddress { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Enter numbers from 0-9")]
        public int TotalNumberOfBirds{ get; set; }
        [Required]
        public string BirdType{ get; set; }
        [Required]
        public string AgeofBird{ get; set; }
        [Required]
        public string Batch{ get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        [Required]
        public decimal SumInsured{ get; set; }
        public decimal? BasicPremiumRate { get; set; }
        public string PoultryType { get; set; }
        public string[] Locations { get; set; }
        public string Name { get; set; }
        public decimal? BroilerBasicPremiumRate { get; set; }
    }

    public class PoultryEndorsementPartialViewModel : PoultryPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
        public decimal FutureSumInsured { get; set; }
        public decimal RefundSubsidyRate { get; set; }
        public decimal RefundSubsidyAmount { get; set; }
    }
}
