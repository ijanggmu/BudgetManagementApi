using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Policy.Policy;

namespace Models.WebApi.Policy.Motor;
public class MotorResponseModel
{
    public bool IsThirdParty { get; set; }
    public bool IsComprehensive { get; set; }
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
    public string ChasisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string Class { get; set; }
    public string RegistrationNumber { get; set; }
    public decimal VoluntaryExcess { get; set; }
    public decimal CompulsoryExcess { get; set; }
    public decimal TotalExcess { get; set; }
    public decimal? CubicCapacity { get; set; }
    public decimal? KiloWatt { get; set; }
    public int Days { get; set; }
    public string YearsFromRegistrationDateYears { get; set; }
    public string YearsFromRegistrationDateYearsBS { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public decimal? RateOfDepreciation { get; set; }
    public decimal? ValueOfAccessories { get; set; }
    public int NCDYears { get; set; }
    public string NCDCerticficate { get; set; }
    public string PhotoOfVechileString { get; set; }
    public List<string> PhotoOfVechile { get; set; }
    public string BlueBookCopyImageString { get; set; }
    public List<string> BlueBookCopyImage { get; set; }
    public List<string>? BlueBookCopyImageurl { get; set; }
}
