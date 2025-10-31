using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeemaEdgeApi.Utilities.Audits;

public class AuditDbStorageWriterBackgroundWorker : BackgroundService
{
    private readonly Channel<SaveChangesAudit> _channel;
    private readonly IDbContextFactory<AuditDataContext> _auditDataContextFactory;
    private readonly ILogger<AuditDbStorageWriterBackgroundWorker> _logger;

    public AuditDbStorageWriterBackgroundWorker(
        Channel<SaveChangesAudit> channel,
        IDbContextFactory<AuditDataContext> auditDataContextFactory,
        ILogger<AuditDbStorageWriterBackgroundWorker> logger)
    {
        _channel = channel;
        _auditDataContextFactory = auditDataContextFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var changesAudit in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var context = await _auditDataContextFactory.CreateDbContextAsync(stoppingToken);
                if (changesAudit.Entities.Any())
                {
                    if (changesAudit.Id == Guid.Empty)
                    {
                        await context.AddAsync(changesAudit);
                    }

                }
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

    }
}
