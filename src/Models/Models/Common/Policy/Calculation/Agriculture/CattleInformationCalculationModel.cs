using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Agriculture
{
    public class CattleInformationCalculationModel
    {
        public int SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Age { get; set; }
        public string Height { get; set; }
        public string Color { get; set; }
        public string BreedType { get; set; }
        public string IdentificationCode { get; set; }
        public string Detail { get; set; }
        public string HealthStatus { get; set; }
        public string Name { get; set; }
        public decimal SumInsured { get; set; }
        public decimal NCD { get; set; }
        public decimal? NCDRate { get; set; }
        public bool IsActive { get; set; }
        public decimal BasicDifference { get; set; }
        public decimal Basic { get; set; }
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public bool IsFirstAssetChange { get; set; }
        public decimal BasicPremiumRateForGoat { get; set; }
        public string CattleType { get; set; }

    }
}
