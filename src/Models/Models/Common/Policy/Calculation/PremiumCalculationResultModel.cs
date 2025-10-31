using Models.Common.Policy.Calculation.Fire;
using Models.Common.Policy.Calculation.GPA;
using Models.Common.Policy.Calculation.Marine;
using Models.Common.Policy.Risk;
using Models.Common.Policy.Calculation.Miscellaneous.GPAT;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Common.Policy.Calculation.Miscellaneous.GPAR;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.Calculation.Agriculture;
using Models.Common.Policy.Calculation.Miscellaneous.GPARafting;
using Models.Common.Policy.Calculation.Miscellaneous;
using Models.Common.Policy.Policy.Medical;
using Models.Common.Policy.Policy.Aviation;
using Models.Common.Policy.Calculation.Miscellaneous.DNL;
using Models.Common.Policy.Calculation.Miscellaneous.TR;
using Models.Common.Policy.Policy.Agriculture;

namespace Models.Common.Policy.Calculation
{
    public class PremiumCalculationResultModel
    {
        #region CommonProperties
        public int NumberofPassengers { get; set; }
        public int AgeOfVehicle { get; set; }
        public decimal MinbasicPremium { get; set; }
        public decimal ActualRecoveryCharge { get; set; }

        public decimal RecoveryCharge { get; set; }

        public decimal VehicleCapacity { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SumInsuredAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal EcoFriendlyDiscountAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal BasicPremium { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal BasicPremiumA { get; set; }
        public decimal FullBasicPremiumA { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal BasicPremiumB { get; set; }
        public decimal FullBasicPremiumB { get; set; }
        public decimal SecondaryBasicPremiumRate { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal AgeLoadingAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal AgeLoadingRate { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal VoluntaryExcessAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal NoClaimDiscountAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal CalculatedSubTotalA { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SubTotalA { get; set; }
        public decimal PAforInsurer { get; set; }


        [Column(TypeName = "decimal(15, 4)")]
        public decimal PrimaryBasicPremiumAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal ThirdPartyAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal ThirdPartyAmountNet { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal ThirdPartyNoClaimDiscountAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal PAofInsured { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SubTotalB { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SumInsuredForInsurer { get; set; }
        public decimal PAAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal RiotAndStrikeAndMdAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TerrorismAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal TerrorismAmountA { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal TerrorismAmountB { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal PayToRiderAndOnePillionRiderAmount { get; set; }
        public decimal PayToRiderAndOnePillionSumInsured { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SubTotalC { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal SubTotalD { get; set; }

        public decimal PoolPremiumAmount { get; set; }
        public decimal FireUnselectedRSMDTAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal StampDutyAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal VatPercent { get; set; }
        public decimal Balance { get; set; }
        public int? NumberOfHelpers { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public decimal BasicPremiumRateA { get; set; }
        public decimal BasicPremiumRateB { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal VoluntaryExcessRate { get; set; }

        public decimal DirectDiscountRate { get; set; }
        public decimal NoClaimDiscountRate { get; set; }
        public decimal OwnUseDiscountRate { get; set; }
        public decimal OwnUseDiscountAmount { get; set; }
        public decimal BurglaryRate { get; set; }
        public decimal BurglaryAmount { get; set; }
        public decimal FidelityGuaranteeRate { get; set; }
        public decimal FidelityGuaranteeAmount { get; set; }
        public decimal ThirdPartyNoClaimDiscountRate { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal DirectDiscountAmount { get; set; }
        public decimal DirectDiscountAmountPA { get; set; }
        public decimal DirectDiscountAmountPAHelper { get; set; }
        public decimal DirectDiscountAmountPAPassenger { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal LayupDiscountOwnDamage { get; set; }
        public decimal LayupDiscountThirdParty { get; set; }
        public decimal LayupDiscountRSMDT { get; set; }
        public decimal LayupDiscountRSMD { get; set; }
        public decimal LayupDiscountPA { get; set; }
        public decimal LayupDiscountPADriver { get; set; }
        public decimal LayupDiscountPAPassenger { get; set; }
        public decimal LayupDiscountPAHelper { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal ActualDirectDiscountAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal PremiumAfterDirectDiscount { get; set; }
        public decimal PremiumAfterLockdownDiscount { get; set; }
        public decimal PremiumAfterLockdownRSMDTDiscount { get; set; }


        [Column(TypeName = "decimal(15, 4)")]
        public decimal VatAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal NetPremiumAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalSumInsuredAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalSumInsuredAmountA { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalSumInsuredAmountB { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalSumInsuredInCurrencyOfValue { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalPremiumAmount { get; set; }

        public decimal EPremiumRate { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal EPremiumAmount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal EPremiumLimit { get; set; }

        public decimal GroupDiscountRate { get; set; }
        public bool IsGroupDiscountRate { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal GroupDiscountAmount { get; set; }

        public decimal RSMDTRate { get; set; }
        public decimal IncludingPropertyRate { get; set; }
        public decimal RSMDRateA { get; set; }
        public decimal RSMDRate { get; set; }
        public decimal RSMDRateB { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal RSMDTAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal RSMDAmountA { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal RSMDAmountB { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal RSMDTDirectDiscountAmount { get; set; }
        [Column(TypeName = "decimal(15, 4)")]
        public decimal RSMDTAfterDirectDiscount { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalODP { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal TotalABCDE { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public decimal PreviousNetPremium { get; set; }
        public decimal PreviousTotalPremium { get; set; }

        [Column(TypeName = "decimal(15, 4)")]
        public int RemainingNumberOfDays { get; set; }
        public decimal ProRataEndorsementPremium { get; set; }
        public decimal NetEndorsementPremium { get; set; }
        public decimal TotalEndorsementPremium { get; set; }
        public decimal MedicalBenefitPA { get; set; }
        public decimal AdditionalMedicalBenefitPA { get; set; }
        public decimal AgentCommissonRate { get; set; }
        public decimal AgentCommissonAmount { get; set; }
        public decimal NepalAgentTDSRate { get; set; }
        public decimal NepalAgentTDSAmount { get; set; }
        public decimal TechnicianCommissionRate { get; set; }
        public decimal TechnicianCommissionAmount { get; set; }
        public decimal NepalTechnicianTDSRate { get; set; }
        public decimal NepalTechnicianTDSAmount { get; set; }

        public List<IndividualCriticalIllnessCalculationModel> IndividualCriticalIllnessCalculationModels { get; set; }
        public List<IndividualGPACalculationModel> IndividualGPACalculationModels { get; set; }

        public List<IndividualGPAComparisonModel> IndividualGPAComparisonModels { get; set; }

        public List<FirePremiumCalculationModel> FirePremiumCalculationModels { get; set; } = new List<FirePremiumCalculationModel>();
        public DateTime ExpiryDate { get; set; }
        public string TypeOfPolicy { get; set; }
        public decimal RSMDTDriver { get; set; }
        public decimal RSMDTHelper { get; set; }
        public decimal RSMDTPassenger { get; set; }

        public decimal PAforDriver { get; set; }
        public decimal PAforPassenger { get; set; }
        public decimal PAforHelper { get; set; }
        public decimal RSMDT { get; set; }
        public decimal RSMDTSuminsuredDriver { get; set; }
        public decimal RSMDTSuminsuredPassenger { get; set; }
        public decimal RSMDTSuminsuredHelper { get; set; }

        // EV Extended Warranty
        public decimal LimitOfLiabilityPerVehicle { get; set; }
        public decimal PremiumRate { get; set; }

        public decimal LoadingRate { get; set; }
        public decimal TransportRate { get; set; }
        public decimal TransportAmount { get; set; }
        public decimal DepreciationRate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal ReplacementRate { get; set; }
        public decimal ReplacementAmount { get; set; }
        public decimal TerrorismRate { get; set; }
        public decimal TerrorismRateA { get; set; }
        public decimal TerrorismRateB { get; set; }
        public decimal PAToDriverAndPillionRiderRate { get; set; }
        public int Days { get; set; }
        public bool IsProRata { get; set; }
        public decimal ShortScaleRate { get; set; }
        public decimal ActualSubTotalB { get; set; }
        public decimal ThirdPartyAmountForPrint { get; set; }

        public decimal EndSubTotalA { get; set; }
        public decimal EndBasicPremium { get; set; }
        public decimal EndSubTotalB { get; set; }
        public decimal EndThirdParty { get; set; }
        public decimal EndSubTotalC { get; set; }
        public decimal EndPAAmount { get; set; }
        public decimal EndSubTotalD { get; set; }
        public decimal EndRSMDT { get; set; }
        public decimal FirstLossDeclarationSumInsured { get; set; }
        public decimal OwnDamageSpecialDiscountAmount { get; set; }
        public decimal ShortScaleOwnDamageSpecialDiscountAmount { get; set; }
        public decimal ShortScaleTPLSpecialDiscountAmount { get; set; }
        public decimal SumInsuredAmountWithoutMedical { get; set; }
        public decimal TotalPLSumInsured { get; set; }
        public decimal TotalPLSumInsuredWithoutCoinsuranceRate { get; set; }
        public decimal FirstLossSumInsured { get; set; }
        public decimal FirstLossSumInsuredWithoutCoinsuranceRate { get; set; }
        public decimal TotalSumInsuredWithoutCoinsuranceRate { get; set; }
        public decimal EndSRCCPremium { get; set; }
        public decimal EndGrossPremium { get; set; }
        public decimal EndTotalPremium { get; set; }
        public decimal EndMinimumGrossPremium { get; set; }
        public decimal EndNetPremium { get; set; }
        public decimal EndCalculatedGrossPremium { get; set; }
        public string RiskTypeSelected { get; set; }

        public decimal TotalPAPremiumAmount { get; set; }
        public decimal TotalRSMDTPremiumAmount { get; set; }
        public decimal TotalMedicalPremiumAmount { get; set; }
        public List<IndividualDNLCalculationModel> IndividualDNLCalculationModels { get; set; }


        #endregion

        #region GPAR Properties
        public decimal PersonalAccidentAmount { get; set; }
        public decimal MedicalUptoBaseCampAmount { get; set; }
        public decimal MedicalAbove1900ftAmount { get; set; }
        public decimal SearchNRescueAmount { get; set; }
        public decimal InCaseOfDeathAmount { get; set; }
        public decimal Above1900Rate { get; set; }

        public decimal UptoBaseCampRate { get; set; }
        public MedicalBenefitUptoBaseCampGPARCalculationModel MedicalBenefitUptoBaseCampGPARCalculationModel { get; set; }
        public MedicalBenefitMoreThan1900ftGPARCalculationModel MedicalBenefitMoreThan1900ftGPARCalculationModel { get; set; }
        public OtherBenefitSearchNRescueGPARCalculationModel OtherBenefitSearchNRescueGPARCalculationModel { get; set; }
        public OtherBenefitInCaseOfDeathGPARCalculationModel OtherBenefitInCaseOfDeathGPARCalculationModel { get; set; }
        public List<NamedIndividualGPARCalculationModel> NamedIndividualGPARCalculationModels { get; set; }
        public List<UnNamedIndividualGPARCalculationModel> UnNamedIndividualGPARCalculationModels { get; set; }
        public bool IsMedicalBenifit { get; set; }
        public bool IsOtherBenifit { get; set; }

        public List<RateViewModel> DesignationWiseRateViewModels { get; set; }
        #endregion

        #region TR
        public decimal TotalMedicalAmountAbove19000ft { get; set; }
        public decimal TotalMedicalAmountBelow19000ft { get; set; }
        public decimal TotalAdministrativeStaff { get; set; }
        // public decimal NamedMedicalPremiumBelow19000ft{ get; set; }
        // public decimal NamedMedicalPremiumAbove19000ft{ get; set; }
        // public decimal UnNamedMedicalPremiumBelow19000ft{ get; set; }
        // public decimal UnNamedMedicalPremiumAvove19000ft{ get; set; }
        public decimal CorporateDiscountAmount { get; set; }
        //Total basic premium after corporate discount
        public decimal TBPAfterCorporateDiscount { get; set; }
        public decimal ShortScaleCorporateDiscountAmount { get; set; }
        public decimal CorporateDiscountRate { get; set; }
        public List<NamedIndividualTRCalculationModel> NamedIndividualTRCalculationModels { get; set; }
        public List<UnNamedIndividualTRCalculationModel> UnNamedIndividualTRCalculationModels { get; set; }
        public OtherBenefitSearchNRescueTRCalculationModel OtherBenefitSearchNRescueTRCalculationModel { get; set; }
        public OtherBenefitInCaseOfDeathTRCalculationModel OtherBenefitInCaseOfDeathTRCalculationModel { get; set; }
        public List<StudentList> SSIStudentList { get; set; }

        #endregion

        #region GPAT Properties
        public List<NamedIndividualGPATCalculationModel> NamedIndividualGPATCalculationModels { get; set; }
        public List<UnNamedIndividualGPATCalculationModel> UnNamedIndividualGPATCalculationModels { get; set; }
        public List<ClassificationRiskCoverageRateViewModel> ClassificationRiskCoverageRateViewModels { get; set; }
        #endregion

        #region GPA Rafting Properties
        public List<NamedIndividualGPARaftingCalculationModel> NamedIndividualGPARaftingCalculationModels { get; set; }
        public List<UnNamedIndividualGPARaftingCalculationModel> UnNamedIndividualGPARaftingCalculationModels { get; set; }
        #endregion

        #region Risk Details
        //risk details
        public MotorRiskViewModel MotorRiskViewModel { get; set; }

        public PrivateVehicleRiskViewModel PrivateVehicleRiskViewModel { get; set; }
        public ElectricMotorcycleRiskViewModel ElectricMotorcycleRiskViewModel { get; set; }

        //risk calculation display model
        public RiskDetails RiskDetails { get; set; } = new RiskDetails();
        public decimal AnnualRevenue { get; set; }
        public decimal InsuredAmount { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal PolicyPeriod { get; set; }
        public decimal ProRataPremium { get; set; }
        public decimal AmountPerSeatCapacity { get; set; }
        public decimal AmountPerHorsePower { get; set; }
        public decimal ActualAmountPerSeatCapacity { get; set; }
        public decimal TrailorAmount { get; set; }
        public decimal ActualTrailorAmount { get; set; }
        public decimal TrailorRate { get; set; }
        public decimal TPLperCC { get; set; }
        public decimal TPLperKW { get; set; }
        public decimal BasicTPLPerKW { get; set; }
        public decimal TPLperTon { get; set; }
        public decimal TPLperHorsePower { get; set; }
        public decimal PrivateHireAmount { get; set; }
        public decimal BasicPremiumForPrint { get; set; }
        public decimal PoolPremiumForPrint { get; set; }
        public decimal ThirdPartyPremiumForPrint { get; set; }
        public decimal AmountPerExcessTons { get; set; }
        public decimal AmountPerCarryingCapacity { get; set; }
        public decimal MinimumTon { get; set; }
        public decimal minRSMDTAmount { get; set; }
        public decimal SumInsuredWithTrailerForRSMDTTractor { get; set; }
        public decimal RSMDTforDriverRate { get; set; }
        public decimal RSMDTforPassengerRate { get; set; }
        public decimal RSMDTForHelperRate { get; set; }
        public decimal PaforDriverAmount { get; set; }
        public decimal PaforPassengerAmount { get; set; }
        #endregion

        #region Marine & MCE Specific
        public decimal ToleranceRate { get; set; }
        public decimal IncrementalCostRate { get; set; }
        public decimal DutyRate { get; set; }
        public decimal PerUnitNRSValue { get; set; }
        public decimal ModeOfTransitDiscountRate { get; set; }
        public decimal ContainerDisocuntRate { get; set; }
        public decimal NonDeliveryPremiumRate { get; set; }
        public decimal WaterDamagePremiumRate { get; set; }
        public decimal TPNDPremiumRate { get; set; }
        public decimal SRCCRate { get; set; }
        public List<MarineIndividualCalculation> MarineIndividualCalculations { get; set; }
        public decimal ModeOfTransitDiscountAmount { get; set; }
        public decimal PremiumAfterModeOfTransitDiscount { get; set; }
        public decimal ContainerDiscountAmount { get; set; }
        public decimal PremiumAfterContainerDiscount { get; set; }
        public decimal NonDeliveryPremiumAmount { get; set; }
        public decimal PremiumWithNonDelivery { get; set; }
        public decimal WaterDamagePremiumAmount { get; set; }
        public decimal PremiumWithWaterDamage { get; set; }
        public decimal TPNDPremiumAmount { get; set; }
        public decimal PremiumWithTPND { get; set; }
        public decimal InitialBasicpremium { get; set; }
        public decimal InitialRSMDTpremium { get; set; }
        public decimal DirectDiscountOnBasic { get; set; }
        public decimal PremiumAfterDirectDiscountOnBasic { get; set; }
        public decimal LCorDPDiscountRate { get; set; }
        public decimal SpecialLCorDPDiscountOnBasic { get; set; }
        public decimal TotalBasicPremium { get; set; }
        public decimal SRCCPremiumAmount { get; set; }
        public decimal DirectDiscountOnSRCC { get; set; }
        public decimal PremiumAfterDirectDiscountOnSRCC { get; set; }
        public decimal SpecialLCorDPDiscountOnSrcc { get; set; }
        public decimal TotalSRCCPremiumAmount { get; set; }
        public decimal GrossPremiumAmount { get; set; }
        public decimal PremiumAfterStamp { get; set; }
        public string CurrencyOfValue { get; set; }
        public decimal MinimumGrossPremium { get; set; }
        public decimal CalculatedGrossPremium { get; set; }
        public decimal FullBasicPremiumWithoutInstallment { get; set; }
        public decimal FullSRCCPremiumWithoutInstallment { get; set; }

        //Additional for MCE
        public decimal AmountInsured { get; set; }
        public decimal ErectionSumInsured { get; set; }
        public bool IsTPLSelected { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public decimal BasicPremiumBeforeDirectDiscount { get; set; }
        public decimal TPLPremiumBeforeDirectDiscount { get; set; }
        public decimal RsmdtPremiumBeforeDirectDiscount { get; set; }


        #endregion
        #region MBPI
        public bool IsPlanASelected { get; set; }
        public bool IsPlanBSelected { get; set; }
        #endregion

        #region SSI

        public decimal? AnyOnePersonAmount { get; set; }
        public decimal? AnyOneAccidentAmount { get; set; }
        public decimal? AnyOnePolicyPeriodAmount { get; set; }

        #endregion

        #region MCE

        public decimal? BodilyInjury { get; set; }
        public decimal? PropertyDamage { get; set; }
        public decimal? BodilyInjuryPerPerson { get; set; }

        #endregion
        public decimal ChildDiscountAmount { get; set; }
        public decimal ChildDiscountRate { get; set; }
        public decimal AdditionalPremiumforMaxAge { get; set; }

        #region Electrical Equipment
        public decimal BasicPremiumRateForSectionA { get; set; }
        public decimal BasicPremiumAmountForSectionA { get; set; }
        public decimal TotalSumInsuredAmountForSectionA { get; set; }
        public decimal BasicPremiumRateForSectionB { get; set; }
        public decimal BasicPremiumAmountForSectionB { get; set; }
        public decimal TotalSumInsuredAmountForSectionB { get; set; }
        public decimal BasicPremiumRateForSectionC { get; set; }
        public decimal BasicPremiumAmountForSectionC { get; set; }
        public decimal TotalSumInsuredAmountForSectionC { get; set; }
        public decimal AggregateBasicPremium { get; set; }

        public decimal RSRate { get; set; }
        public decimal RSAmount { get; set; }
        public decimal MDRate { get; set; }
        public decimal MDAmount { get; set; }
        public decimal AggregateRSMDT { get; set; }
        public decimal SpecialDiscountRate { get; set; }
        public decimal BasicPremiumSpecialDiscountAmount { get; set; }
        public decimal RSMDTSpecialDiscountAmount { get; set; }
        #endregion


        #region Construction Plant And Machinery
        public decimal InitialTPL { get; set; }
        public decimal TPLAmount { get; set; }
        public decimal TPLRate { get; set; }
        public decimal TPLSpecialDiscountAmount { get; set; }
        public decimal TPLSuminsured { get; set; }
        public decimal TotalTPLSuminsured { get; set; }
        public decimal SumInsuredWithoutTPL { get; set; }
        #endregion

        #region Boiler
        public decimal BoilerAndPressurePlantSumInsured { get; set; }
        public decimal TransactionBoilerAndPressurePlantSumInsured { get; set; }
        public decimal SurroundingPropertiesSumInsured { get; set; }
        public decimal TransactionSurroundingPropertiesSumInsured { get; set; }
        public decimal TPLSumInsured { get; set; }
        public decimal TransactionTPLSumInsured { get; set; }
        #endregion

        #region Contract All Risks
        public decimal OwnDamageSumInsuredAmount { get; set; }
        public decimal TotalOwnDamageSumInsuredAmount { get; set; }
        public decimal ContractPriceAmount { get; set; }
        public string ContractPriceCurrency { get; set; }
        public decimal EscallationRate { get; set; }
        public decimal MaterialsSuppliedByPrincipalAmount { get; set; }
        public string MaterialsSuppliedByPrincipalCurrency { get; set; }
        public decimal EmployersExistingPropertyAmount { get; set; }
        public string EmployersExistingPropertyCurrency { get; set; }
        public decimal ClearanceOfDebrisAmount { get; set; }
        public decimal ForeignDebrisAmount { get; set; }
        public string ClearanceOfDebrisCurrency { get; set; }
        public string ForeignDebrisCurrency { get; set; }
        public decimal ArchitectsConsultantFeeAmount { get; set; }
        public string ArchitectsConsultantFeeCurrency { get; set; }
        public decimal ContractPriceNRSAmount { get; set; }
        public decimal TotalContractPriceNRSAmount { get; set; }
        public decimal EscallationNRSAmount { get; set; }
        public decimal TotalEscallationNRSAmount { get; set; }
        public decimal MaterialsSuppliedByPrincipalNRSAmount { get; set; }
        public decimal TotalMaterialsSuppliedByPrincipalNRSAmount { get; set; }
        public decimal EmployersExistingPropertyNRSAmount { get; set; }
        public decimal TotalEmployersExistingPropertyNRSAmount { get; set; }
        public decimal ClearanceOfDebrisNRSAmount { get; set; }
        public decimal ForeignDebrisNRSAmount { get; set; }
        public decimal TotalClearanceOfDebrisNRSAmount { get; set; }
        public decimal ArchitectsConsultantFeeNRSAmount { get; set; }
        public decimal TotalArchitectsConsultantFeeNRSAmount { get; set; }
        public decimal ConstructionPlantAndEquipmentAmount { get; set; }
        public string ConstructionPlantAndEquipmentCurrency { get; set; }
        public decimal ConstructionPlantAndEquipmentNRSAmount { get; set; }
        public decimal TotalConstructionPlantAndEquipmentNRSAmount { get; set; }
        public decimal BodilyInjuryAmount { get; set; }
        public string BodilyInjuryCurrency { get; set; }
        public decimal BodilyInjuryNRSAmount { get; set; }
        public decimal TotalBodilyInjuryNRSAmount { get; set; }
        public decimal PropertyDamageAmount { get; set; }
        public string PropertyDamageCurrency { get; set; }
        public decimal PropertyDamageNRSAmount { get; set; }
        public decimal TotalPropertyDamageNRSAmount { get; set; }
        public decimal BodilyInjuryPerPersonNRSAmount { get; set; }
        public decimal TotalBodilyInjuryPerPersonNRSAmount { get; set; }
        public decimal BodilyInjuryPerPersonAmount { get; set; }
        public string BodilyInjuryPerPersonCurrency { get; set; }
        public decimal CivilEngineeringWorkTemporary { get; set; }
        public decimal CivilEngineeringWorkPermanent { get; set; }
        public decimal CivilEngineeringWorkTemporaryNRSAmount { get; set; }
        public decimal CivilEngineeringWorkPermanentNRSAmount { get; set; }
        public decimal TotalCivilEngineeringWorkTemporaryNRSAmount { get; set; }
        public decimal TotalCivilEngineeringWorkPermanentNRSAmount { get; set; }
        public ContractorAllRiskRSMDT BelowFirstThreshold { get; set; }
        public ContractorAllRiskRSMDT AboveFirstThreshold { get; set; }
        public ContractorAllRiskRSMDT AboveSecondThreshold { get; set; }
        public decimal TransactionOwnDamageSumInsuredAmount { get; set; }
        public decimal TPLDirectDiscountAmount { get; set; }

        public decimal RSAmountWithoutCoinsuranceCaseA { get; set; }
        public decimal MDAmountWithoutCoinsuranceCaseA { get; set; }
        public decimal TerrorismAmountWithoutCoinsuranceCaseA { get; set; }
        public decimal RSAmountWithoutCoinsuranceCaseB { get; set; }
        public decimal MDAmountWithoutCoinsuranceCaseB { get; set; }
        public decimal TerrorismAmountWithoutCoinsuranceCaseB { get; set; }
        public decimal RSAmountWithoutCoinsuranceCaseC { get; set; }
        public decimal MDAmountWithoutCoinsuranceCaseC { get; set; }
        public decimal TerrorismAmountWithoutCoinsuranceCaseC { get; set; }
        public decimal GrossPremiumForPrint { get; set; }
        public decimal PremiumAfterStampForPrint { get; set; }
        public decimal VatAmountForPrint { get; set; }
        public decimal NetPremiumForPrint { get; set; }
        public bool IsInstallmentPremium { get; set; }

        #endregion

        #region Banker's Blanket
        public decimal LoadingForDishonestyOfEmployees { get; set; }
        public decimal NumberOFEmployess { get; set; }
        public decimal MaximumAmountInATM { get; set; }
        public decimal LoadingForDishonestyOfEmployeesAmount { get; set; }
        public decimal BasicPremiumRateForAddtionalSIClauseA { get; set; }
        public decimal BasicPremiumRateForAddtionalSIClauseB { get; set; }
        public decimal? AdditionalSumInsuredAmountClauseA { get; set; }
        public decimal? AdditionalSumInsuredAmountClauseB { get; set; }
        public decimal BasicPremiumSIClauseA { get; set; }
        public decimal BasicPremiumSIClauseB { get; set; }
        public decimal BasicRSMDTPremiumSIClauseA { get; set; }
        public decimal BasicRSMDTPremiumSIClauseB { get; set; }
        #endregion

        #region Medical
        public MedicalEmployeeType BasicEmployeePremium { get; set; } = new MedicalEmployeeType();
        public MedicalEmployeeType RSMDTEmployeePremium { get; set; } = new MedicalEmployeeType();
        public MedicalEmployeeType TotalEmployeePremium { get; set; } = new MedicalEmployeeType();
        public int NumberOfPeople { get; set; }

        public decimal PremiumPerPerson { get; set; }
        public decimal ParentExtraCharge { get; set; }
        public int NumberOfParents { get; set; }
        public decimal PlanAPremium { get; set; }
        public decimal FullPlanAPremium { get; set; }
        public decimal PlanBPremium { get; set; }
        public decimal FullPlanBPremium { get; set; }
        public decimal PlanCPremium { get; set; }
        public decimal FullPlanCPremium { get; set; }
        public decimal PlanDPremium { get; set; }
        public decimal FullPlanDPremium { get; set; }
        public decimal PlanEPremium { get; set; }
        public decimal PlanFPremium { get; set; }
        public decimal PlanGPremium { get; set; }
        public decimal PlanHPremium { get; set; }
        public decimal PlanIPremium { get; set; }
        public decimal PlanJPremium { get; set; }
        public int PlanANumberOfEmployees { get; set; }
        public int PlanBNumberOfEmployees { get; set; }
        public int PlanCNumberOfEmployees { get; set; }
        public int PlanDNumberOfEmployees { get; set; }
        public int PlanENumberOfEmployees { get; set; }
        public int PlanFNumberOfEmployees { get; set; }
        public int PlanGNumberOfEmployees { get; set; }
        public int PlanHNumberOfEmployees { get; set; }
        public int PlanINumberOfEmployees { get; set; }
        public int PlanJNumberOfEmployees { get; set; }
        public decimal TotalAdditionalBenefitsAmount { get; set; }
        public decimal TotalAdditionalBenefitsRSMDTAmount { get; set; }
        public decimal UnnamedChildBasicPremium { get; set; }
        public decimal UnnamedChildRSMDTPremium { get; set; }
        public decimal UnnamedChildAnnualBasicPremium { get; set; }
        public decimal UnnamedChildAnnualRSMDTPremium { get; set; }
        #endregion

        #region Agriculture
        public decimal GovernmentSubsidyRate { get; set; }
        public decimal GovernmentSubsidyAmount { get; set; }
        public decimal BalanceRate { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal PAOfInsured { get; set; }
        public decimal CorporateDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public List<decimal> InitialBasicPremiumCattle { get; set; } = new List<decimal>();
        public List<decimal> NoClaimDiscountCattle { get; set; } = new List<decimal>();
        public List<decimal> IndividualPremiumCattle { get; set; } = new List<decimal>();

        public decimal FishBasicPremium { get; set; }
        public decimal FishPremiumRate { get; set; }
        public decimal PondBasicPremium { get; set; }
        public decimal PondPremiumRate { get; set; }
        public decimal SpecialFarmingTechniquePremium { get; set; }
        public decimal SpecialFarmingTechniqueRate { get; set; }
        public List<FishInsuredPersons> FishInsuredPersonsRecords { get; set; }
        public List<InsuredPersons> CerealInsuredPersonsRecords { get; set; }
        //Cattle
        public List<CattleInformationCalculationModel> CattleRecords { get; set; }
        #endregion

        #region PersonalHealthInsurance
        public PHIDetail PHIDetail { get; set; }
        public decimal HSumInsured { get; set; }
        public decimal? CISumInsured { get; set; }
        public decimal HPremium { get; set; }
        public decimal? CIPremium { get; set; }
        #endregion
        #region Burglary
        public decimal DeclarationSumInsured { get; set; }
        public decimal FirstLossGrossPremium { get; set; }

        #endregion

        #region Aviation
        public List<AircraftPremiumInstalmentViewModel> PremiumInstalmentAviation { get; set; }
        public int InstalmentNumber { get; set; }
        public decimal NetPremiumUSD { get; set; }
        public decimal SumInsuredExtortionAmount { get; set; }
        public decimal SumInsuredExtortionRate { get; set; }
        public decimal InstalmentPremiumUSD { get; set; }
        public decimal VATAmountUSD { get; set; }

        #endregion

        public decimal TravelDirectDiscount { get; set; }
        public decimal ThresholdAmount { get; set; }
        public decimal SumInsuredToDisplay { get; set; }
        public decimal ExcessAmount { get; set; }

        //money
        public decimal SumInsuredCashInSafeTypeA { get; set; }
        public decimal SumInsuredCashInSafeTypeB { get; set; }
        public decimal SumInsuredAnnualAggregateTypeA { get; set; }
        public decimal SumInsuredAnnualAggregateTypeB { get; set; }
        public decimal SumInsuredAnnualAggregateTypeC { get; set; }
        public decimal CashInSafeBasicPremiumA { get; set; }
        public decimal CashInSafeBasicPremiumB { get; set; }
        public decimal CashInTransitBasicPremiumA { get; set; }
        public decimal CashInTransitBasicPremiumB { get; set; }
        public decimal CashInTransitBasicPremiumC { get; set; }
        public decimal CashInSafeRSMDAmountA { get; set; }
        public decimal CashInSafeRSMDAmount { get; set; }
        public decimal IncludingProperytAmount { get; set; }
        public decimal CashInCounterRSMDAmount { get; set; }
        public decimal CashInSafeRSMDAmountB { get; set; }
        public decimal CashInTransitRSMDAmountA { get; set; }
        public decimal CashInTransitRSMDAmount { get; set; }
        public decimal CashInTransitRSMDAmountB { get; set; }
        public decimal CashInTransitRSMDAmountC { get; set; }
        public decimal CashInSafeTerrorismAmountA { get; set; }
        public decimal CashInSafeTerrorismAmount { get; set; }
        public decimal CashInCounterTerrorismAmount { get; set; }
        public decimal CashInSafeTerrorismAmountB { get; set; }
        public decimal CashInTransitTerrorismAmountA { get; set; }
        public decimal CashInTransitTerrorismAmount { get; set; }
        public decimal CashInTransitTerrorismAmountB { get; set; }
        public decimal CashInTransitTerrorismAmountC { get; set; }
        public decimal CashInSafeBasicPremiumRateA { get; set; }
        public decimal CashInSafeBasicPremiumRateB { get; set; }
        public decimal CashInTransitBasicPremiumRateA { get; set; }
        public decimal CashInTransitBasicPremiumRateB { get; set; }
        public decimal CashInTransitBasicPremiumRateC { get; set; }
        //added for cash in counter

        public decimal IncludePropertyBasicPremiumRate { get; set; }
        public decimal CashInTransitBasicPremiumRate { get; set; }
        public decimal CashInCounterBasicPremiumRate { get; set; }
        public decimal CashInSafeBasicPremiumRate { get; set; }

        public decimal SumInsuredCashInSafe { get; set; }
        public decimal CashInSafeBasicPremium { get; set; }
        public decimal SumInsuredCashInTransit { get; set; }
        public decimal CashInTransitBasicPremium { get; set; }
        public decimal SumInsuredCashInCounter { get; set; }
        public decimal CashInCounterBasicPremium { get; set; }







        public decimal EndorsementSum { get; set; }
        public bool IsRSMDTSelected { get; set; }
        public bool IsPropertyIncluded { get; set; }
        //endorsement cancellation
        public decimal ServiceCharge { get; set; }
        public decimal CancellationServiceCharge { get; set; }
        public decimal CancellationVatAmount { get; set; }
        public decimal CancellationTaxableAmount { get; set; }
        public decimal NetCancellationAmount { get; set; }
        public decimal TINetPremiumAmount { get; set; }
        public PolicyDetail PolicyDetail { get; set; } = new PolicyDetail();

        public decimal GroupPersonalAccidentInsuranceRate { get; set; }
        public decimal MedicalInsuranceRate { get; set; }
        public decimal CriticalIllnessInsuranceRate { get; set; }
        public decimal BasicGroupPersonalAccidentInsuranceAmount { get; set; }
        public decimal BasicMedicalInsuranceAmount { get; set; }
        public decimal BasicCriticalIllnessInsuranceAmount { get; set; }

        //miscellaneous
        public List<HIPInsuredPersonCalculationModel> HIPInsuredRecord { get; set; }
        public List<HIPInsuredPersonCalculationModel> MedicalInsuredRecord { get; set; }
        public decimal FuturePremium { get; set; }
        public decimal FutureSumInsured { get; set; }

        public decimal BasicPremiumUSDBeforeSpecialDiscount { get; set; }
        public decimal BasicPremiumBeforeSpecialDiscount { get; set; }
        public decimal BudgetPlanBasicPremium { get; set; }
        public decimal BasicPremiumBeforeBudgetPlan { get; set; }
        public decimal BudgetPlanRate { get; set; }

        public decimal BasicDiscountAmount { get; set; }
        public decimal TPLDiscountAmount { get; set; }
        public decimal PoolDiscountAmount { get; set; }
        public decimal GrossDiscountAmount { get; set; }
        public bool IsCoinsurance { get; set; }
        public decimal HGIShareRate { get; set; }
        public decimal FullSumInsured { get; set; }

        public decimal BasicPremiumWithoutCoinsuranceRate { get; set; }
        public decimal BasicPremiumBWithoutCoinsuranceRate { get; set; }
        public decimal BasicPremiumCWithoutCoinsuranceRate { get; set; }

        public decimal RSMDTPremiumWithoutCoinsuranceRate { get; set; }
        public decimal RSMDTPremiumWithoutCoinsuranceRateCaseA { get; set; }
        public decimal IncludingPropertyPremiumWithoutCoinsuranceRateCase { get; set; }
        public decimal RSMDTPremiumWithoutCoinsuranceRateCaseB { get; set; }
        public decimal RSMDTPremiumWithoutCoinsuranceRateCaseC { get; set; }
        public decimal TPLPremiumWithoutCoinsuranceRate { get; set; }
        public decimal PaidDriverAndPassengersPremiumWithoutCoinsuranceRate { get; set; }
        public decimal BasicPremiumAWithoutCoinsuranceRate { get; set; }
        public decimal UnnamedChildBasicPremiumWithoutCoInsuranceRate { get; set; }
        public decimal TerrorismAmountAWithoutCoinsuranceRate { get; set; }
        public decimal TerrorismAmountBWithoutCoinsuranceRate { get; set; }
        public decimal TerrorismAmountCWithoutCoinsuranceRate { get; set; }

        public decimal BasicGovernmentSubsidyAmount { get; set; }
        public decimal BasicBalanceAmount { get; set; }
        public decimal PoolGovernmentSubsidyAmount { get; set; }
        public decimal PoolBalanceAmount { get; set; }
        public decimal GpaInsuranceSumInsured { get; set; }
        public decimal FullGpaInsuranceSumInsured { get; set; }
        public decimal GpaInsuranceInsuredAmount { get; set; }
        public decimal FullGpaInsuranceInsuredAmount { get; set; }
        public decimal MedicalInsuranceSumInsured { get; set; }
        public decimal FullMedicalInsuranceSumInsured { get; set; }
        public decimal MedicalInsuranceInsuredAmount { get; set; }
        public decimal FullMedicalInsuranceInsuredAmount { get; set; }
        public decimal CriticalIllnessInsuranceSumInsured { get; set; }
        public decimal FullCriticalIllnessInsuranceSumInsured { get; set; }
        public decimal CriticalIllnessInsuranceInsuredAmount { get; set; }
        public decimal FullCriticalIllnessInsuranceInsuredAmount { get; set; }
        public decimal SumInsuredAmountPHI { get; set; }
        public bool NoServiceCharge { get; set; }

        //TIBP
        public decimal USDPerDayRate { get; set; }
        public decimal NumberOfDays { get; set; }
        public decimal CalculatedDays { get; set; }
        public decimal EndorsedNumberOfDays { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CovidLoadingCharge { get; set; }
        public decimal CovidLoadingChargeRate { get; set; }
        public int TotalNumberOfDaysRemaining { get; set; }


        //gpa 
        public decimal AdditionalRiskRate { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }

        //gpa-player
        public decimal BasicRate { get; set; }
        public decimal GameRiskRate { get; set; }

        //PA
        public decimal AdditionalRiskAmount { get; set; }
        public decimal AdditionalRiskAmountBeforeShortScale { get; set; }
        public decimal AdditionalMedicalBenefitAmountBeforeShortScale { get; set; }
        public bool AdditionalRisk { get; set; }

    }
    public class PremiumCalculationResponseModel
    {
        public decimal SumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal ThirdPartyPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal PersonalAccidentPremium { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal Premium { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Region { get; set; }
        public string Plan { get; set; }
        public decimal PremiumUSD { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal StampDuty { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NetPremium { get; set; }
        public decimal GovernmentSubsidyAmount { get; set; }
        public decimal PayableAmount { get; set; }

    }
    public class MedicalEmployeeType
    {
        public decimal EmployeePremium { get; set; }
        public decimal EmployeeAndSpousePremium { get; set; }
        public decimal EmployeeAndFamilyTwoPremium { get; set; }
        public decimal EmployeeAndFamilyThreePremium { get; set; }
    }
    public class ContractorAllRiskRSMDT
    {
        public decimal RSMDTLevel { get; set; }
        public decimal RSRate { get; set; }
        public decimal MDRate { get; set; }
        public decimal TerrorismRate { get; set; }
    }

    public class PolicyDetail
    {
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int TotalDays { get; set; }
        public int ExtendedDays { get; set; }
        public int ReductionDays { get; set; }
    }
    public class PHIDetail
    {
        public PHIInsured InsuredPersons { get; set; }
        public PHIInsured InsuredForPartner { get; set; }
        public PHIInsured InsuredForChildren { get; set; }
        public PHIInsured InsuredForChildrenOne { get; set; }
    }
}
