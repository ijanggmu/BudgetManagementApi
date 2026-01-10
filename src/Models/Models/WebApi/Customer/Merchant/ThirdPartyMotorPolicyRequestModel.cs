using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy;

namespace Models.WebApi.Customer.Merchant;
public class ThirdPartyMotorPolicyRequestModel
{
    public string Type { get; set; }
    [Required(ErrorMessage = "EffectiveDate is required")]
    public DateTime EffectiveDate { get; set; }

    [Required(ErrorMessage = "ExpiryDate is required")]
    public DateTime ExpiryDate { get; set; }
    public VehicleType VechileType { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Model { get; set; }
    public string SubModel { get; set; }
    public string YearsFromRegistrationDateYears { get; set; } = "";
    [Required]
    public string ChasisNumber { get; set; }
    [Required]
    public string EngineNumber { get; set; }
    [Required]
    public string RegistrationNumber { get; set; }
    public decimal? CubicCapacity { get; set; }
    public decimal? KiloWatt { get; set; }
    public int Days { get; set; }
    public List<string> PhotoOfVechile { get; set; } = new List<string>();
    public List<string> BlueBookCopyImage { get; set; } = new List<string>();
    public List<string> BlueBookCopyImageUrl { get; set; } = new List<string>();
}
