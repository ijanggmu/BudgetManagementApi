using System.Threading.Tasks;

namespace Business.Common.TenantDomain;

public interface INotificationService
{
    Task SendEmailAsync(string userId, string subject, string body);
    Task SendSmsAsync(string userId, string message);
    Task SendPushAsync(string userId, string title, string body);
}
