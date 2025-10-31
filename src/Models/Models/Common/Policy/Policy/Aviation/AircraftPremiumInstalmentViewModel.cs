using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AircraftPremiumInstalmentViewModel
    {
        public int InstalmentNumber { get; set; }
        [Required]
        public DateTime InstalmentDate { get; set; }
        public decimal InstalmentPercentage { get; set; }
        [Required]
        public decimal InstalmentPremiumAmount { get; set; }
    }
}
