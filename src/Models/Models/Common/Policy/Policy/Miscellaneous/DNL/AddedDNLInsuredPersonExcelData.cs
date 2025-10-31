using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous.DNL
{
    public class AddedDNLInsuredPersonExcelData : DNLInsuredPersonExcelData
    {
        public decimal? GpaInsurancePremium { get; set; }
        public decimal? MedicalInsurancePremium { get; set; }
        public decimal? CriticalIllnessInsurancePremium { get; set; }
        public decimal? RSMDTPremium { get; set; }
    }
}
