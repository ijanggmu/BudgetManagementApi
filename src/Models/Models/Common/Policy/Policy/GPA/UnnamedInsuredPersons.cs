namespace Models.Common.Policy.Policy.GPA
{
    public class UnnamedInsuredPersons : InsuredPerson
    {
        public int Count { get; set; }
        public int ChangeCount { get; set; }
        public decimal TransactionSumInsured { get; set; }

    }
    public class EndorsedUnnamedInsuredPersons : EndorsedInsuredPerson
    {
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int ChangeInCount { get; set; }
        public decimal TransactionSumInsured { get; set; }
    }
}
