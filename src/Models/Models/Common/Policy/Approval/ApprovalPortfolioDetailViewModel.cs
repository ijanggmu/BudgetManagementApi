using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Approval
{
    public class ApprovalPortfolioDetailViewModel
    {
        public Detail[] RiskDetails { get; set; }
        public Detail[] PremiumDetails { get; set; }
        public List<DetailInfo> ClassDetails { get; set; }
        public string PortfolioAlias { get; set; }
        public List<InsuredPersonsDetail> InsuredPersonsDetails { get; set; }
    }

    public class DetailInfo
    {
        public List<Detail> DetailList { get; set; }
    }
    public class Detail
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class InsuredPersonsDetail
    {
        public List<string> TableTitles { get; set; }
        public List<List<string>> Values { get; set; }
    }
}
