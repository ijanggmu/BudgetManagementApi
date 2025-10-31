using Models.Common.Policy.Calculation.Miscellaneous.GPAT;
using System;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPAT
{
    public class UnNamedIndividualGPATCalculationModel : CommonIndividualGPATCalculationModel
    {
        public int NumberofPerson { get; set; }
        public decimal Rate { get; set; }
    }
}
