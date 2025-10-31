using System.ComponentModel.DataAnnotations;
using Models.WebApi.Customer.Policy;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy
{
    public class PremiumCalculateRequestModel
    {
        [Required(ErrorMessage = "Insurance type is required")]
        public InsuranceType InsuranceType { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.ThirdPartyBike, ErrorMessage = "Third-party bike insurance is required.")]
        public ThirdPartyBikeInsuranceModel ThirdPartyBikeInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.FullBike, ErrorMessage = "Full bike insurance is required.")]
        public BikeFullInsuranceModel FullBikeInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.ThirdPartyPrivateCar, ErrorMessage = "Third-party private car insurance is required.")]
        public ThirdPartyPrivateCarInsuranceModel ThirdPartyPrivateCarInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.FullPrivateCar, ErrorMessage = "Private car insurance is required.")]
        public PrivateCarInsuranceModel PrivateCarInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.FullCommercialVehicle, ErrorMessage = "Commercial vehicle insurance is required.")]
        public CommercialVehicleInsuranceModel CommercialVehicleInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.Travel, ErrorMessage = "Travel insurance is required.")]
        public TravelInsuranceRequestModel TravelInsurance { get; set; }
        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.InternationalTravel, ErrorMessage = "International travel insurance is required.")]
        public InternationalTravelInsurancePremiumCalculatorRequestModel InternationalTravelInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.Home, ErrorMessage = "Home insurance is required.")]
        public HomeInsuranceRequestModel HomeInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.Property, ErrorMessage = "Property insurance is required.")]
        public PropertyInsuranceRequestModel PropertyInsurance { get; set; }

        [RequiredIf(nameof(InsuranceType), (int)InsuranceType.Marine, ErrorMessage = "Marine insurance is required.")]
        public MarineInsuranceRequestModel MarineInsurance { get; set; }
    }
}



