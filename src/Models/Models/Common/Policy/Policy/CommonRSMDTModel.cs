using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy
{
    public class CommonRSMDTModel
    {
        public bool IsPersonalAccidentForPaidForDriver { get; set; }
        public bool IsPersonalAccidentForPaidForPassenger { get; set; }
        public bool IsRiotStrikeAndTerrorismForDriver { get; set; }
        public bool IsRiotStrikeAndTerrorismForPassenger { get; set; }
        //[RegularExpression()]
        public int NumberofSeatsIncludingDriver { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public string SumInsuredAmountForPaidDriver { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public string SumInsuredAmountForPassenger { get; set; }
        public int PersonalAccidentForPassengerSeatCount { get; set; }
        public int RiotStrikeAndTerrorismForPassengerSeatCount { get; set; }
        [RequiredIf("HasTailor", true)]
        public decimal? ValueOfTailor { get; set; }
        public bool HasTailor { get; set; }
        public bool UseOfPrivateHire { get; set; }
        public decimal? GoodCarryingCapacity { get; set; }
        public string VehiclePurpose { get; set; }
    }
}
