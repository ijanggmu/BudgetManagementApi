using Models.Common.Policy.Calculation.Agriculture;
using Models.Common.Policy.Calculation.Engineering;
using Models.Common.Policy.Calculation.Miscellaneous;
using Models.Common.Policy.Calculation.Miscellaneous.MBPI;
using Models.Common.Policy.Calculation.Motor;

namespace Models.Common.Policy.Calculation
{
    public class RiskDetails
    {
        public MotorcycleDetailCalculationResult MotorcycleDetailCalculationResult { get; set; }
        public MBPIDetailCalculationResult MBPIDetailCalculationResult { get; set; }
        public PrivateVehicleDetailCalculationResult PrivateVehicleDetailCalculationResult { get; set; }
        public GoodsCarryingVehicleDetailCalculationResult GoodsCarryingVehicleDetailCalculationResult { get; set; }
        public AgricultureForestryVehicleDetailCalculationResult AgricultureForestryVehicleDetailCalculationResult { get; set; }
        public TractorDetailCalculationResult TractorDetailCalculationResult { get; set; }
        public ConstructionEquipmentDetailCalculationResult ConstructionEquipmentDetailCalculationResult { get; set; }
        public TankerDetailCalculationResult TankerDetailCalculationResult { get; set; }
        public AmbulanceDetailCalculationResult AmbulanceDetailCalculationResult { get; set; }
        public PassengerCarryingVehicleDetailCalculationResult PassengerCarryingVehicleDetailCalculationResult { get; set; }
        public ElectricVehicleDetailCalculationResult ElectricVehicleDetailCalculationResult { get; set; }
        public ElectricMotorcycleDetailCalculationResult ElectricMotorcycleDetailCalculationResult { get; set; }
        public TaxiDetailCalculationResult TaxiDetailCalculationResult { get; set; }
        public TempoDetailCalculationResult TempoDetailCalculationResult { get; set; }
        public ElectricCommercialVehicleCalculationResult ElectricCommercialVehicleCalculationResult { get; set; }

        //Engineering
        public ElectricalEquipmentDetailCalculationResult ElectricalEquipmentDetailCalculationResult { get; set; }
        public BusinessMachineAndEquipmentDetailCalculationResult BusinessMachineAndEquipmentDetailCalculationResult { get; set; }
        public ConstructionPlantAndMachineryDetailCalculationResult ConstructionPlantAndMachineryDetailCalculationResult { get; set; }
        public MachineryBreakdownDetailCalculationResult MachineryBreakdownDetailCalculationResult { get; set; }
        public BoilerDetailCalculationResult BoilerDetailCalculationResult { get; set; }
        public ContractorAllRiskDetailCalculationResult ContractorAllRiskDetailCalculationResult { get; set; }
        //Miscellaneous
        public BurglaryDetailCalculationResult BurglaryDetailCalculationResult { get; set; }
        public MoneyDetailcalculationResult MoneyDetailCalculationResult { get; set; }
        public NCITDetailCalulcationResult NCITDetailCalculationResult { get; set; }
        public BankersBlanketDetailCalculationResult BankersBlanketDetailCalculationResult { get; set; }
        public AllRiskDetailCalculationResult AllRiskDetailCalculationResult { get; set; }
        public FidelityGuaranteeDetailCalculationResult FidelityGuaranteeDetailCalculationResult { get; set; }
        public PADetailcalculationResult PADetailcalculationResult { get; set; }
        public PHIDetailcalculationResult PHIDetailcalculationResult { get; set; }
        public MIMDetailcalculationResult MIMDetailcalculationResult { get; set; }
        public PATDetailcalculationResult PATDetailcalculationResult { get; set; }
        public PublicLiabilityCalculationResult PublicLiabilityCalculationResult { get; set; }

        //Agriculture
        public VegetablesDetailsCalculationResult VegetablesDetailsCalculationResult { get; set; }
        public PoultryDetailsCalculationResult PoultryDetailsCalculationResult { get; set; }
        public CattleDetailCalculationResult CattleDetailCalculationResult { get; set; }
    }
}
