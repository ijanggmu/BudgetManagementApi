using System;

namespace Models.Common.Policy
{
    public class ViewPorfolioListViewModel
    { 
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Sn { get; set; }
        public string Portfolio { get; set; }
        public string PorfolioClassName { get; set; }
        public bool IsConfigured { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
