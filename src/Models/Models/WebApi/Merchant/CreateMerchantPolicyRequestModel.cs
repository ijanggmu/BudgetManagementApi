using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.WebApi.Customer.Policy;
using Models.Common.Policy.Policy;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.Common.Policy.Calculation;
using Models.Common;

namespace Models.WebApi.Merchant;
public class CreateMerchantPolicyRequestModel
{
    [Required(ErrorMessage = "InsuranceType is Required")]
    public InsuranceType InsuranceType { get; set; }

    [Required(ErrorMessage = "EffectiveDate is required")]
    public DateTime EffectiveDate { get; set; }

    [Required(ErrorMessage = "ExpiryDate is required")]
    public DateTime ExpiryDate { get; set; }

    public List<string> Financer { get; set; }
    public MotorRequestModel Motor { get; set; }

    public InternationalTravelInsuranceRequestModel ITI { get; set; }

    public string PolicyId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateTime.UtcNow.Date.AddHours(-5).AddMinutes(-45);

        if (EffectiveDate < today)
        {
            yield return new ValidationResult(
                "EffectiveDate cannot be in the past.",
                new[] { nameof(EffectiveDate) });
        }

        if (EffectiveDate > today.AddDays(30))
        {
            yield return new ValidationResult(
                "EffectiveDate cannot be more than 30 days from today.",
                new[] { nameof(EffectiveDate) });
        }

        if (ExpiryDate < EffectiveDate)
        {
            yield return new ValidationResult(
                "ExpiryDate cannot be earlier than EffectiveDate.",
                new[] { nameof(ExpiryDate) });
        }
    }

}

public class MerchantPolicyResponseModel : MessageResponseModel
{
    public string PolicyId { get; set; }
    public PremiumCalculationResponseModel CalculationDetail { get; set; }

}

