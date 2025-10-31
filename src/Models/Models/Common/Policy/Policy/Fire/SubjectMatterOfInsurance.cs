using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Fire
{
    public class SubjectMatterOfInsurance : ILocation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        //Seciton 1: Proposers Detail
        // Name
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FirstNameNepali { get; set; }
        public string MiddleNameNepali { get; set; }
        public string LastNameNepali { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }
        public string NameNepali { get; set; }

        public bool IsIndividual { get; set; }

        // Address
        public string Province { get; set; }
        public string Roof { get; set; }
        public decimal UploadSumInsured { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
        public string BuildingComplexName { get; set; }
        public string HouseNumber { get; set; }

        public string KittaNumber { get; set; }

        //Others
        public string Occupation { get; set; }
        public string BankOrFinance { get; set; }
        public string BuildingComposition { get; set; }
        public string BuildingCompositionDescription { get; set; }
        public string[] NatureOfOccupancy { get; set; }

        //Section 2.1 : Subject Matter of Insurance   
        public string Walls { get; set; }
        public string Floor { get; set; }
        public decimal NumberOfFloors { get; set; }
        public string AddressOfPremisis { get; set; }
        public string IsBuildingAttachedDetached { get; set; }
        public string BuildingOccupiedAs { get; set; }
        public decimal ValueOfBuilding { get; set; }

        // For calculation
        public decimal SumInsuredAmount { get; set; }
        public decimal BasicPremium { get; set; }

        public decimal RsmdtPremium { get; set; }

        //Section 2.2 : Details of Contents
        public bool LoadContentsFromFile { get; set; }

        public bool IsNewFile { get; set; }

        // If loaded from file
        public CommonFileViewModel ContentDetailsFile { get; set; }

        public List<FireContentExcelData> HouseholdContentExcelData { get; set; }

        //If entered manually
        public decimal Building { get; set; }
        public decimal Equipment { get; set; }
        public decimal RawMaterials { get; set; }
        public decimal WorkInProgress { get; set; }
        public decimal FinishedGoods { get; set; }
        public decimal SemiFinishedGoods { get; set; }
        public decimal MoneyAndJewellery { get; set; }
        public decimal FurnitureFixtureOrFitting { get; set; }
        public decimal OtherItems { get; set; }
        public decimal Art { get; set; }

        //Section 3
        public string HasAnyOtherCompanyRefusedToInsureOrRenew { get; set; }
        public string IfInsuredNameOfInsurer { get; set; }
        public string LossDuringFiveYears { get; set; }

        //Section 4
        public string PersonalAccident { get; set; }
        public string NameOfBeneficiary { get; set; }
        public string RelationBetweenInsuredPersonAndBeneficiary { get; set; }

        //Section 5
        public string RiskCode { get; set; }
        public string RiskType { get; set; }
        public string PropertyDescription { get; set; }
        public decimal LoadingMultiplier { get; set; }
        public string InsuranceDescription { get; set; }

        //section 6
        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionRSMDTPremium { get; set; }
        public decimal TransactionSumInsuredAmount { get; set; }
        public bool IsAdded { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }

        //lockdown discount
        public string Identifier { get; set; }
        public bool IsContentListResetDuringRenewal { get; set; }

        public string[] Locations { get; set; }
    }
    public class CommonFileViewModel
    {
        public IFormFile CommonFile { get; set; }
    }
}
