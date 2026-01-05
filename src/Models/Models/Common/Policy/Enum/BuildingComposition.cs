using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Enum;
public enum BuildingComposition
{
    [Display(Name = "प्रथम श्रेणी ")]
    FirstClass,
    [Display(Name = "दोस्रो श्रेणी")]
    SecondClass
}

public enum HouseholdInsuranceType
{
    [Display(Name = "सामान्य सम्पत्ति बीमालेख")]
    GeneralPropertyInsurance,
    [Display(Name = "छोटो अवधिको सम्पत्ति बीमालेख")]
    ShortIntervalPropertyInsurance,
    [Display(Name = "मूल्यांकित सम्पत्ति बीमालेख")]
    EvaluationPropertyInsurance,
    [Display(Name = "फ्लोटिङ सम्पत्ति बीमालेख")]
    FloatingPropertyInsurance,
    [Display(Name = "घोषणा सम्पत्ति बीमालेख")]
    AnnouncedPropertyInsurance,
    [Display(Name = " फ्लोटिङ घोषणा सम्पत्ति बीमालेख")]
    FloatingAnnouncedPropertyInsurance,
    [Display(Name = "पुनर्स्थापना सम्पत्ति बीमालेख")]
    RennovationPropertyInsurance,
    [Display(Name = "घर बीमालेख")]
    HomeInsurance
}
public enum NatureOfProperty
{
    [Display(Name = "Commercial")]
    Commercial,
    [Display(Name = "Personal")]
    Personal
}
