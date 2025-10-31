using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class BUPAPartialViewModel
    {
        [Range(1, 366, ErrorMessage = "Please enter a value less than or equal to 366")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 366")]
        [Required(ErrorMessage = "Days is required")]
        public int PolicyPeriodInDays { get; set; }

        [RequiredIf("IsBusinessTravel", false)]
        [Required(ErrorMessage = "Passport Number is required")]
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

        public string TravelType { get; set; }
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
        public string CorporateAddress { get; set; }
        public string Age { get; set; }

        public bool ToRenewPolicy { get; set; }

        [Required(ErrorMessage = "Renew policy number is required")]
        [Remote("PolicyNumberValidation", "Policy", HttpMethod = "GET")]
        public string RenewPolicyNumber { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime RenewStartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime RenewEndDate { get; set; }

        [Required(ErrorMessage = "Remarks is required")]
        public string RenewRemarks { get; set; }

        public string ReferenceNumber { get; set; }

        [Required(ErrorMessage = "Geographical area is required")]
        public string GeographicalArea { get; set; }

        public bool IsBusinessTravel { get; set; }
        public int NumberOfInsuredDays { get; set; }
    }
}
