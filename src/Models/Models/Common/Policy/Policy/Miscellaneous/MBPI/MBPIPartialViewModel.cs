using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Miscellaneous.MBPI
{
    public class MBPIPartialViewModel
    {
        public List<MobileInformationDetail> MobileInformationDetails { get; set; }
        public string PlanType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal BasicPremiumAmountPerPerson { get; set; }
        public string TerritorialLimits { get; set; }
        public string DeductableNotes { get; set; }
        public string EndorsementNotes { get; set; }
        public decimal NetPremiumAmount { get; set; }
        public decimal TotalPremiumAmount { get; set; }
        public decimal GrossPremiumAmount { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public int NumberofPerson { get; set; } = 1;
        public IFormFile MBPIFile { get; set; }
        public string PolicyIssuanceId { get; set; }

        public decimal TransactionPremium { get; set; }
        public decimal BasicPremium { get; set; }


        public class MobileInformationDetail
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string SN { get; set; }
            public string MobileOwner { get; set; }
            public string IMEI2 { get; set; }
            public string IMEI1 { get; set; }
            public string NetworkProvider { get; set; }
            public string PhoneNumber { get; set; }

            public bool IsUpdate { get; set; }
            public bool IsNew { get; set; }

            public bool IsExisting { get; set; }


        }
    }

    public class MBPIEndorsementPartialViewModel : MBPIPartialViewModel
    {
        public List<MobileInformationDetail> AddedMobileInformationDetail { get; set; }
        public List<MobileInformationDetail> UpdatedMobileInformationDetail { get; set; }
        public List<MobileInformationDetail> DiscontinuedMobileInformationDetail { get; set; }

    }
}
