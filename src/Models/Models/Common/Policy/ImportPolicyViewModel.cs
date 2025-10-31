using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class ImportPolicyViewModel
    {
        public decimal SumInsured { get; set; }
        public string PortfolioAlias { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string PartyId { get; set; }
        public int? AgentCode { get; set; }
        public bool IsComprehensive { get; set; }
        public bool IsRSMDT { get; set; }
        public bool IsThirdParty { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public bool PurchaseType { get; set; }
        public decimal CubicCapacity { get; set; }
        public decimal VoluntaryExcess { get; set; }
        public int NCDYear { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ManufactureYear { get; set; }
        public int Days { get; set; }
        public string BranchCode { get; set; }
        public string ChassisNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Class { get; set; }
        public string Portfolio { get; set; }
        public string PartyType { get; set; }
        public string FO { get; set; }
        public string User { get; set; }

        public int level { get; set; }


        public bool HasTailor { get; set; }
        public bool HasPrivateHire { get; set; }
        public int SeatCapacity { get; set; }
        public int? ValueOfTrailor { get; set; }
        public string Type { get; set; }
        public decimal CompulsoryExcess { get; set; }
    }
}
