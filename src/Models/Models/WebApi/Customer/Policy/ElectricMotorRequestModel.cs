using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BeemaEdgeApi.Customer.Policy;
public class ElectricMotorRequestModel
{
    public bool IsThirdParty { get; set; }
    public bool IsComprehensive { get; set; }
    public Guid? PartyId { get; set; }
    public string PortfolioId { get; set; }
    public string Type { get; set; }
    public string ManufactureYear { get; set; }
    public string ManufactureCompany { get; set; }
    public string Model { get; set; }
    public string SubModel { get; set; }
    public bool PurchasedNewOld { get; set; }
    public DateTime DateOfPurchase { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public double VoluntaryExcess { get; set; }
    public double CompulsoryExcess { get; set; }
    public double TotalExcess { get; set; }
    public string KiloWatt { get; set; }
    public int Days { get; set; }
    public string RegistrationDateAD { get; set; }
    public string RegistrationDateBS { get; set; }
    public double CurrentMarketPrice { get; set; }
    public int? AgeOfVehicle { get; set; }
    public double? RateOfDepreciation { get; set; }
    public double? ValueOfAccessories { get; set; }
    public bool? VehicleForHireOrReward { get; set; }
    public bool? ParkingPlaceGarage { get; set; }
    public bool? ParkingGarageOpen { get; set; }
    public bool? Maintenance { get; set; }
    public int NCDYears { get; set; }
    public int NumberOfSeatsIncludingDriver { get; set; }
    public bool RiotStrike { get; set; }
}
