using Data.Context;
using Data.Entities.Audit.UserActivites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace BeemaEdgeApi.Utilities.UserActivities;

public class RequestLoggingDbWriterBackgroundWorker : BackgroundService
{
    private readonly Channel<UserActivity> _channel;
    private readonly IDbContextFactory<AuditDataContext> _dataContextFactory;
    private readonly ILogger<RequestLoggingDbWriterBackgroundWorker> _logger;

    public RequestLoggingDbWriterBackgroundWorker(
        Channel<UserActivity> channel,
        IDbContextFactory<AuditDataContext> dataContextFactory,
        ILogger<RequestLoggingDbWriterBackgroundWorker> logger)
    {
        _channel = channel;
        _dataContextFactory = dataContextFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var userActivity in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var context = await _dataContextFactory.CreateDbContextAsync(stoppingToken);
                await context.AddAsync(userActivity);

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Type: {@type}, InformationType: {@informationType}, Message: {@message},UserActivity: {@UserActivity}, Exception: {@Exception}",
                      "API", "Error", $"Error while writing the request log : {ex.Message}", userActivity, ex);
            }
        }
    }
}
