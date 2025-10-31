using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class StudentSafetyInsurancePartialViewModel:CommonRiskTypePartialViewModel
    {
        public decimal BasicPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal Suminsured { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
        //New fields
        [Required]
        [Range(0.001,100,ErrorMessage = "Please enter a value less than 100")]
        public decimal BasicPremiumRate { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal? AnyOnePersonAmount { get; set; }
        public decimal? AnyOneAccidentAmount { get; set; }
        public decimal? AnyOnePolicyPeriodAmount { get; set; }
        public string PolicyPeriodText { get; set; }
        public string InsuredPersonsText { get; set; }
        public List<StudentList> StudentList { get; set; }
    }
    public class StudentList
    {
        public string SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string RollNo { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsAdded { get; set; }
    }
    public class SSIEndorsementPartialViewModel: StudentSafetyInsurancePartialViewModel
    {
        public List<StudentList> AddedStudentList { get; set; }
        public List<StudentList> UpdatedStudentList { get; set; }
        public List<StudentList> DiscontinuedStudentList { get; set; }
    }
}