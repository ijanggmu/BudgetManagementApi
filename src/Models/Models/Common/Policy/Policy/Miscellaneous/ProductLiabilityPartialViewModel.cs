using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class ProductLiabilityPartialViewModel
    {
        public decimal? SumInsured { get; set; }
        [Required]
        public decimal FutureAnnualAggregateLimit { get; set; }
        [Required (ErrorMessage = "Transaction Amount is Required")]
        public decimal ? TransactionAnnualAggregateLimit { get; set; }
        public decimal Premium { get; set; }
        public decimal TransactionPremium { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public string Business { get; set; }
        public string LimitLiability { get; set; }
        public decimal AnnualAggregateLimit { get; set; }
        public string TerritorialLimit { get; set; }
        public string JurisdictionLimit { get; set; }
        public string Deductibles { get; set; }
        public string Endorsement { get; set; }
        public List<ProductDetails> ProductDetails { get; set; }
        public List<ProductDetails> AddedProcductDetails { get; set; }
        public List<ProductDetails> UpdatedProcductDetails { get; set; }
        public List<ProductDetails> DiscontinuedProcductDetails { get; set; }
    }
    public class ProductDetails
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
        public string IntendedUse { get; set; }
        public decimal EstimatedAnnualSalesTurnover { get; set; }
    }
}
