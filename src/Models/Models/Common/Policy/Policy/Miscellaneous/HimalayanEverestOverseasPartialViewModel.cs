using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class HimalayanEverestOverseasPartialViewModel:CommonRiskTypePartialViewModel
    {
        public decimal BasicPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal Suminsured { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
        
        // classdetails
        public string TripType { get; set; } //Single//Multiple

        // public string IdType { get; set; }

        [Required(ErrorMessage = "Identification Number is required")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Visiting Country is required")]
        public string VisitingCountry { get; set; }

        public string FatherHusbandName { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "Date Of Birth name is required")]
        public DateTime DateOfBirth { get; set; }

        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Emergency Contact Name is required")]
        public string EmergencyContactName { get; set; }

        [Required(ErrorMessage = "Emergency Contact Number  is required")]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string EmergencyContactNumber { get; set; }
        public bool IncludeFamily { get; set; }

        // public string TravellingCountry { get; set; }

        public string InsuranceType { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public string Province { get; set; }

        [Required(ErrorMessage = "District is required")]
        public string District { get; set; }

        [Required(ErrorMessage = "Municipality is required")]
        public string Municipality { get; set; }

        [Required(ErrorMessage = "Ward is required")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Street Address is required")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "Premium Amount is required")]
        public decimal PremiumAmount { get; set; }

         public decimal ExchangeRate { get; set; }

        public bool IsCorporateAddress { get; set; }
        // public bool NoteExclusion { get; set; }
        // public bool CovidNoteExclusion1 { get; set; }
        // public bool CovidNoteExclusion2 { get; set; }

        public bool IsCovidIncluded  { get; set; }
        public bool IsMultiTrip  { get; set; }

        public string CorporateAddress { get; set; }

        public string Age { get; set; }

        public List<TravelInsuranceFamilyMembers> FamilyMembers { get; set; }
    }
}