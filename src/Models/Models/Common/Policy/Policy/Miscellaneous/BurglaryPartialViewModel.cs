using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class BurglaryPartialViewModel : ILocation
    {
        public List<MaterialOfBurglaryInsurance> MaterialsOfInsurance { get; set; }
        public List<MaterialOfBurglaryInsurance> AddMaterialsOfInsurance { get; set; }
        public List<MaterialOfBurglaryInsurance> UpdateMaterialsOfInsurance { get; set; }
        public List<MaterialOfBurglaryInsurance> DeleteMaterialsOfInsurance { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal BasicRate { get; set; }
        public string BuildingOwner { get; set; }
        public bool FirstLoss { get; set; }
        public bool IsOtherAssets { get; set; }
        public decimal DeclarationSumInsured { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal FirstLossSumInsured { get; set; }
        public decimal TotalFirstLossSumInsured { get; set; }
        public string RiskAddress { get; set; }
        public string ImageUrl { get; set; }
        public string DeductiblesText { get; set; }
        public string Endorsement { get; set; }
        public string Name { get; set; }
        public string[] Locations { get; set; }
    }

    public class MaterialOfBurglaryInsurance
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal UnitValue { get; set; }
        public decimal FullUnitValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal SumInsured { get; set; }
        public decimal FullSumInsured { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }
}
