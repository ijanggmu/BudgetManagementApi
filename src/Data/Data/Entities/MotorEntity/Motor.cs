using Data.Entities.BaseEntity;

namespace Data.Entities.MotorEntity;
public class Motor : ApplicationBaseEntity
{
    public bool IsThirdParty { get; set; }
    public bool IsComprehensive { get; set; }
    public string Type { get; set; }
    public string PartyId { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Financer { get; set; }
    public string Model { get; set; }
    public string SubModel { get; set; }
    public bool PurchasedNewOld { get; set; }
    public string DateOfPurchase { get; set; }
    public string ChasisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public decimal VoluntaryExcess { get; set; }
    public decimal CompulsoryExcess { get; set; }
    public decimal TotalExcess { get; set; }
    public decimal? CubicCapacity { get; set; }
    public decimal? KilloWatt { get; set; }
    public int Days { get; set; }
    public int VehicleType { get; set; }
    public string YearsFromRegistrationDateYears { get; set; }
    public string YearsFromRegistrationDateYearsBS { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public int? AgeOfVehicle { get; set; }
    public decimal? RateOfDepreciation { get; set; }
    public decimal? ValueOfAccessories { get; set; }
    public bool? VehicleForHireOrReward { get; set; }
    public bool? ParkingPlaceGarage { get; set; }
    public bool? ParkingGarageOpen { get; set; }
    public bool? Maintenance { get; set; }
    public int NCDYears { get; set; }
    public int NumberofSeatsIncludingDriver { get; set; }
    public bool RiotStrike { get; set; }
    public string BlueBookCopyImageUrl { get; set; }
    public string NCDCerticficate { get; set; }
    public string PhotoOfVechile { get; set; }
    public string BlueBookCopyImage { get; set; }
}
