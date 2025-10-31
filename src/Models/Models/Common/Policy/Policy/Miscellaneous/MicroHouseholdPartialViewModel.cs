using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class MicroHouseholdPartialViewModel : ILocation
    {
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 365")]
        [Required(ErrorMessage = "Days is required")]
        public int PolicyPeriodInDays { get; set; }

        [Required(ErrorMessage = "Citizenship Number is required")]
        public string CitizenshipNo { get; set; }

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

        [Required(ErrorMessage = "Province is required")]
        public string InsuredProvince { get; set; }

        [Required(ErrorMessage = "District is required")]
        public string InsuredDistrict { get; set; }

        [Required(ErrorMessage = "Municipality is required")]
        public string InsuredMunicipality { get; set; }

        [Required(ErrorMessage = "Ward is required")]
        public string InsuredWard { get; set; }

        [Required(ErrorMessage = "Street Address is required")]
        public string InsuredStreetAddress { get; set; }

        [Required(ErrorMessage = "Contact Number is required")]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string ContactNo { get; set; }

        public decimal SIBuilding { get; set; }
        public decimal SIKitchenItems { get; set; }
        public decimal SIElectronicEquipments { get; set; }
        public decimal SISolarPanels { get; set; }
        public decimal SIFarmingEquipments { get; set; }
        public decimal SIOthers { get; set; }

        [Required(ErrorMessage = "Total Sum Insured is required")]
        public decimal TotalSI { get; set; }
        public string KittaNumber { get; set; }
        public string[] Locations { get; set; }
        public string Name { get; set; }
    }

    public class MicroHouseHoldEndorsementPartialViewModel: MicroHouseholdPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
    }
}
