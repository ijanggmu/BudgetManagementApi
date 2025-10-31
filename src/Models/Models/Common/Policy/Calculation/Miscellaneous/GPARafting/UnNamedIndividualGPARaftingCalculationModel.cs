using Models.Common.Policy.Calculation.Miscellaneous.GPAT;
using System;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPARafting { 
    public class UnNamedIndividualGPARaftingCalculationModel : CommonIndividualGPATCalculationModel
    {
        public int NumberofPerson { get; set; }
        public decimal Rate { get; set; }
    }
}
