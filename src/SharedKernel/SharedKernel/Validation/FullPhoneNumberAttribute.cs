using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace SharedKernel.Validation;
public class FullPhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var phone = value as string;
        if (string.IsNullOrWhiteSpace(phone))
            return ValidationResult.Success;

        try
        {
            var util = PhoneNumberUtil.GetInstance();
            var parsed = util.Parse(phone, null); // null means it expects the dialing code (e.g., +977)
            if (util.IsValidNumber(parsed))
                return ValidationResult.Success;
        }
        catch
        {
            // Ignored
        }

        return new ValidationResult(ErrorMessage ?? "Invalid phone number.");
    }
}
