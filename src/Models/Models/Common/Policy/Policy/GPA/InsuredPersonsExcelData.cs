namespace Models.Common.Policy.Policy.GPA
{
    public class InsuredPersonsExcelData
    {
        public string SN { get; set; }
        public string Name { get; set; }
        public string InsurerId { get; set; }
        public int Age { get; set; }
        public int NoOfPeople { get; set; }
        public string NatureOfOccupation { get; set; }
        public decimal SumInsured { get; set; }
        public string EmployeeId { get; set; }
        public string Classification { get; set; }
        public bool IsDSelected { get; set; }
        public bool IsESelected { get; set; }
        public decimal? MedicalBenefitsSumInsured { get; set; }
        public decimal Pa { get; set; }
        public decimal Medical { get; set; }
        public decimal RSMDT { get; set; }
        public string PrimaryId { get; set; }
        public bool ISAbove1900Ft { get; set; }
        public decimal Rate { get; set; }
        public string Designation { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        
        //Gpa-Player
        public string PhoneNumber { get; set; }
        public string Address { get; set; }


    }

    public class AddedInsuredPersonsExcelData : InsuredPersonsExcelData
    {
        public decimal BasicPremium { get; set; }
        public decimal? EPremium { get; set; }
        public decimal DirectDiscount { get; set; }
        public decimal GroupDiscount { get; set; }
        public int NumberOfPerson { get; set; }
        public InsuredPersonClassification InsuredPersonClassifcaiton { get; set; }
    }
}
