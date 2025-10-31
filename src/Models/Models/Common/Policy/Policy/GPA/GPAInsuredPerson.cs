using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.GPA
{
    public class GPAInsuredPerson
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int NumberOfPerson { get; set; }
        public string NatureOfOccupation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal? MedicalBenefitSumInsured { get; set; }
        public bool IsActive { get; set; }
        public bool IsDSelected { get; set; }
        public bool IsESelected { get; set; }
        public string Classification { get; set; }
        public string Designation { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        
        //Gpa-player
        public string PhoneNumber { get; set; }
        public string Adddress { get; set; }
        
        //Gpa -player end

    }
}
