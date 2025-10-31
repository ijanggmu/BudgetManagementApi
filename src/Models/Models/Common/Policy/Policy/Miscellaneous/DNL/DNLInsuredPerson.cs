using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous.DNL
{
    public class DNLInsuredPerson
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string InsuredId { get; set; }
        public string Occupation { get; set; }
        public bool IsGpaInsuranceSelected { get; set; }
        public bool IsMedicalInsuranceSelected { get; set; }
        public bool IsCriticalIllnessInsuranceSelected { get; set; }
        public decimal? GpaInsuranceSumInsured { get; set; }
        public decimal? MedicalInsuranceSumInsured { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsured { get; set; }
        public int NumberOfPerson { get; set; }
        public string GroupName { get; set; }
        public decimal? GpaInsuranceSumInsuredPerPerson { get; set; }
        public decimal? MedicalInsuranceSumInsuredPerPerson { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsuredPerPerson { get; set; }
    }
}
