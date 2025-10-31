using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous.DNL
{
    public class DNLInsuredPersonExcelData
    {
        public string SN { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }
        public string InsuredId { get; set; }
        public bool IsGpaInsuranceSelected { get; set; }
        public bool IsMedicalInsuranceSelected { get; set; }
        public bool IsCriticalIllnessInsuranceSelected { get; set; }
        public decimal? GpaInsuranceSumInsured { get; set; }
        public decimal? GpaInsurancePremium { get; set; }
        public decimal? RSMDTPremium { get; set; }
        public decimal? MedicalInsuranceSumInsured { get; set; }
        public decimal? MedicalInsurancePremium { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsured { get; set; }
        public decimal? CriticalIllnessInsurancePremium { get; set; }
    }
}
