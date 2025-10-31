using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy;

namespace Models.WebApi.Customer.Merchant;
public class ComprehensiveMotorPolicyRequestModel
{
    public bool RiotStrike { get; set; }
    public string Type { get; set; }
    public VechileType VechileType { get; set; }
    public int? AgeOfVehicle { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Financer { get; set; }
    public string Model { get; set; }
    public string SubModel { get; set; }
    public bool PurchasedNewOld { get; set; }
    public string DateOfPurchase { get; set; }
    [Required(ErrorMessage = "Chasis number is required.")]
    public string ChasisNumber { get; set; }

    [Required(ErrorMessage = "Engine number is required.")]
    public string EngineNumber { get; set; }

    [Required(ErrorMessage = "Registration number is required.")]
    public string RegistrationNumber { get; set; }
    public decimal VoluntaryExcess { get; set; }
    public decimal CompulsoryExcess { get; set; }
    public decimal TotalExcess { get; set; }
    public decimal? CubicCapacity { get; set; }
    public decimal? KiloWatt { get; set; }
    public int Days { get; set; }
    public string YearsFromRegistrationDateYears { get; set; } = "2024-02-14";
    public string YearsFromRegistrationDateYearsBS { get; set; }

    [Required(ErrorMessage = "Current market price is required.")]
    public decimal CurrentMarketPrice { get; set; }
    public decimal? RateOfDepreciation { get; set; }
    public decimal? ValueOfAccessories { get; set; }
    public int NCDYears { get; set; }
    public string NCDCerticficate { get; set; }
    public List<string> PhotoOfVechile { get; set; }
    public List<string> BlueBookCopyImage { get; set; }
    public List<string> BlueBookCopyImageUrl { get; set; }
}
