using Data.Context;
using Data.Entities.EmailLogEntity;
using Microsoft.Extensions.Logging;
namespace Business.Common.Email;

public class MailLogService(ILogger<MailLogService> logger,
    ApplicationDataContext context) : IMailLogService
{
    public async Task LogEmailAsync(EmailLog emailLog)
    {

        await context.EmailLogs.AddAsync(emailLog);
        await context.SaveChangesAsync();

        logger.LogInformation("Email log: {@EmailLog}", emailLog);
    }
}
