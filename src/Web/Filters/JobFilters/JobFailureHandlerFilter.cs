using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire;
using Microsoft.Extensions.Logging;

public class JobFailureHandlerFilter : IServerFilter, IApplyStateFilter
{
    private readonly ILogger<JobFailureHandlerFilter> _logger;
    private readonly IBackgroundJobClient _jobClient;
    private const int MaxRetries = 3;

    public JobFailureHandlerFilter(ILogger<JobFailureHandlerFilter> logger, IBackgroundJobClient jobClient)
    {
        _logger = logger;
        _jobClient = jobClient;
    }

    // Logs start of job execution
    public void OnPerforming(PerformingContext context)
    {
        _logger.LogInformation("Starting job {JobId} ({JobName})", context.BackgroundJob.Id, context.BackgroundJob.Job.Method.Name);
    }

    // Logs job completion or exception
    public void OnPerformed(PerformedContext context)
    {
        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, "Job {JobId} ({JobName}) failed", context.BackgroundJob.Id, context.BackgroundJob.Job.Method.Name);
        }
        else
        {
            _logger.LogInformation("Job {JobId} ({JobName}) completed successfully", context.BackgroundJob.Id, context.BackgroundJob.Job.Method.Name);
        }
    }

    // Handles dead-letter logic & logs on failure state
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        if (context.NewState is FailedState failedState)
        {
            _logger.LogError("Job {JobId} failed with exception: {ExceptionMessage}", context.BackgroundJob.Id, failedState.Exception?.Message);

            var retries = context.GetJobParameter<int>("RetryCount");

            if (retries >= MaxRetries)
            {
                _logger.LogWarning("Job {JobId} exceeded max retries and moved to dead-letter handling", context.BackgroundJob.Id);

                // Enqueue dead-letter handler job
                _jobClient.Enqueue(() => HandleDeadLetterJob(context.BackgroundJob.Id));
            }
        }
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // No action needed
    }

    // This method is invoked as a background job to do any manual handling
    public void HandleDeadLetterJob(string jobId)
    {
        _logger.LogWarning("Handling dead-letter job {JobId} for manual inspection or alerting", jobId);
        // Add email, SMS, alerting, or archive logic here
    }
}
