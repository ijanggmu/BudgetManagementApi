namespace Models.Common.Policy.Policy.GPA
{
    public class NamedInsuredPerson : InsuredPerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal TransactionSumInsured { get; set; }
        //public string Designation { get; set; }
    }

    public class EndorsedNamedInsuredPerson: EndorsedInsuredPerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public decimal TransactionSumInsured { get; set; }
        //public string Designation { get; set; }

    }
}
