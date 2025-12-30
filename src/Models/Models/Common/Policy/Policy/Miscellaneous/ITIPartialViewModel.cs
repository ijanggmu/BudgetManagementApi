using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models.Common.Policy.Enum;
using Models.WebApi.Customer.Policy;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class ITIPartialViewModel
    {
        [Range(1, 366, ErrorMessage = "Please enter a value less than or equal to 366")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 366")]
        [Required(ErrorMessage = "Days is required")]
        public int PolicyPeriodInDays { get; set; }

        public TripType TripType { get; set; } //Single//Multiple
        public string IdType { get; set; } //Single//Multiple

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

        public string Age { get; set; }

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

        public string TravellingCountry { get; set; }
        public TravelInsuranceType InsuranceType { get; set; }
        public InternationalTravelInsuranceConfigType TravelPlanType { get; set; }
        public string TypeOfInsured { get; set; }

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
        public string CorporateAddress { get; set; }
        public bool IsCovid19Coverage { get; set; }

        public List<ITIFamilyMembers> FamilyMembers { get; set; }
        public List<ITIGroupMembers> GroupMembers { get; set; }

        public DateTime EffectiveDateBeforeEndorsement { get; set; }
        public DateTime ExpiryDateBeforeEndorsement { get; set; }
        public string SelectedCurrency { get; set; }
    }

    public class ITIFamilyMembers
    {
        public string Relation { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
    }
    public class ITIGroupMembers
    {
        public int SN { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string Phone { get; set; }
        public string Address { get; set; }
        //[RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}
