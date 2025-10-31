using System.Text.RegularExpressions;
using Data.Entities.Log;

namespace Business.Common.Sms;

public interface ISmsService
{
    void QueueSms(SmsRequest mailRequest);
    Task SendSmsAsync(SmsRequest request);
    
}
public class SmsRequest
{
    public List<string> To { get; set; }
    public SmsType SmsType { get; set; }
    public string Body { get; set; }
    public IDictionary<string, string> Headers { get; set; }
    public List<string> SensitiveData { get; set; }
    public Dictionary<string, string> Variable { get; set; }
    public string TemplateName { get; set; }

    public bool ValidateMobileNumbers(out List<string> validNumbers, out List<string> invalidNumbers)
    {
        validNumbers = To
            .Where(n => Regex.IsMatch(n, @"^9\d{9}$")) // Validate numbers starting with 9 and length of 10 digits
            .ToList();

        invalidNumbers = To.Except(validNumbers).ToList();

        return validNumbers.Any();
    }
}
