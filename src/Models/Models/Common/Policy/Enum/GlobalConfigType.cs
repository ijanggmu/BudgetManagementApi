using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum GlobalConfigType
{
    [Display(Name = "Stamp Duty")]
    StampDuty = 1,
    [Display(Name = "VAT Rate")]
    VATRate = 2,
    [Display(Name = "Motor Cycle Short Scale Rate (days)")]
    MotorCycleShortScaleRate = 3,
    [Display(Name = "Private Vehicle Short Scale Rate (days)")]
    PrivateVehicleShortScaleRate = 4,
    [Display(Name = "Commercial Vehicle Short Scale Rate (days)")]
    CommercialVehicleShortScaleRate = 5,
    [Display(Name = "Electric Motorcycle Short Scale Rate (days)")]
    ElectricMotorcycleShortScaleRate = 6,
    [Display(Name = "Currency Exchange Rate Configuration")]
    CurrencyExchangeRateConfiguration = 7,
    [Display(Name = "Engineering Special Discount Rate")]
    SpecialDiscountRate = 8,
    [Display(Name = "Engineering Short Scale Rate(days)")]
    EngineeringShortScaleRate = 9,
    [Display(Name = "Miscellaneous Short Scale Rate(days)")]
    MiscellaneousShortScaleRate = 10,
    [Display(Name = "GPA Short Scale Rate(days)")]
    GPAShortScaleRate = 11,
    [Display(Name = "Household Short Scale Rate(days)")]
    HouseHoldShortScaleRate = 12,
    [Display(Name = "Property Short Scale Rate(days)")]
    PropertyShortScaleRate = 13,
    [Display(Name = "GPAT Short Scale Rate(days)")]
    GPATShortScaleRate = 14,
    [Display(Name = "GPAR Short Scale Rate(days)")]
    GPARShortScaleRate = 15,
    [Display(Name = "Vegetables Short Scale Rate(days)")]
    VegetablesShortScaleRate = 16,
    [Display(Name = "Poultry Short Scale Rate(days)")]
    PoultryShortScaleRate = 17,
    [Display(Name = "Agent Commission Rate (Agriculture)")]
    AgricultureAgentCommissionRate = 18,
    [Display(Name = "Agent Commission Rate (Except Agriculture)")]
    AgentCommissionRate = 19,
    [Display(Name = "Nepal Agent TDS")]
    NepalAgentTDSRate = 20,
    [Display(Name = "Cancellation Charge")]
    CancellationCharge = 21,
    [Display(Name = "Service Charge")]
    ServiceCharge = 22,
    [Display(Name = "GPA Rafting Short Scale Rate(days)")]
    GPARaftingShortScaleRate = 23,
    [Display(Name = "Micro Insurance Medical Short Scale Rate(days)")]
    MicroInsuranceMedicalShortScaleRate = 24,
    [Display(Name = "TPL Stamp Price")]
    TPLStampPrice = 25,
    [Display(Name = "PA Short Scale Rate(days)")]
    PAShortScaleRate = 26,
    [Display(Name = "PAT Short Scale Rate(days)")]
    PATShortScaleRate = 27,
    [Display(Name = "HIP Short Scale Rate(days)")]
    HIPShortScaleRate = 28,
    [Display(Name = "RI Facultative Inward VAT Rate")]
    RIFacultativeInwardVatRate = 29,
    [Display(Name = "Technician Commission Rate")]
    TechnicianCommissionRate = 30,
    [Display(Name = "Nepal Technician TDS")]
    NepalTechnicianTDSRate = 31,
    [Display(Name = "TR Short Scale Rate(days)")]
    TRShortScaleRate = 32,
    [Display(Name = "Facultative Inward Service Charge")]
    FacInServiceCharge = 33,
    [Display(Name = "Accounting Subsidy Rate")]
    AccountingSubsidyRate = 34,
    [Display(Name = " Motor Service Charge")]
    MotorServiceCharge = 35,
    [Display(Name = " Subsidy Rate")]
    SubsidyRate = 36,
}
