using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business.Common.CustomAttribute;
public class OptionalEmailAddressAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;

        if (string.IsNullOrWhiteSpace(email))
        {
            // Allow null or empty strings
            return ValidationResult.Success;
        }

        try
        {
            var addr = new MailAddress(email);
            return ValidationResult.Success;
        }
        catch
        {
            return new ValidationResult(ErrorMessage ?? "Invalid email address format.");
        }
    }
}
