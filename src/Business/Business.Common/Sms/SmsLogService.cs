using Data.Context;
using Data.Entities.Log;
using Microsoft.Extensions.Logging;

namespace Business.Common.Sms;

public class SmsLogService : ISmsLogService
{
    private readonly ILogger<SmsLogService> _logger;
    private readonly ApplicationDataContext _context;

    public SmsLogService(ILogger<SmsLogService> logger,
        ApplicationDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task LogSmsAsync(SmsLog requestModel)
    {
        await _context.SmsLogs.AddAsync(requestModel);
        await _context.SaveChangesAsync();
    }
}
