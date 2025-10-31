namespace Models.Common.Policy.Policy
{
    public class IndividualCustomerCheckRequestModel(string firstName, string middleName, string lastName, string phoneNumber)
    {
        public string FirstName { get; set; } = firstName;
        public string MiddleName { get; set; } = middleName;
        public string LastName { get; set; } = lastName;
        public string PhoneNumber { get; set; } = phoneNumber;
    }
}



