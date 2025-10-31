using System.Linq.Expressions;
using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace Business.Common.JobHelper;
public class HangfireJobHelper(IBackgroundJobClient backgroundJobClient, ILogger<HangfireJobHelper> logger)
{

    /// <summary>
    /// Enqueue a Hangfire job to a specified queue with logging.
    /// </summary>
    /// <typeparam name="TJob"></typeparam>
    /// <param name="methodCall">Expression representing the job method.</param>
    /// <param name="jobDescription">Optional job description for logging.</param>
    /// <param name="queueName">Name of the queue; defaults to "default" if null or empty.</param>
    public void EnqueueWithLogging<TJob>(
        Expression<Func<TJob, Task>> methodCall,
        string jobDescription = null,
        string queueName = null)
        where TJob : class
    {
        try
        {
            var targetQueue = string.IsNullOrWhiteSpace(queueName) ? "default" : queueName;

            var state = new EnqueuedState(targetQueue);

            backgroundJobClient.Create(methodCall, state);

            if (!string.IsNullOrEmpty(jobDescription))
            {
                logger.LogInformation("Enqueued job: {JobDescription} to queue: {QueueName}", jobDescription, targetQueue);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to enqueue job: {JobDescription}", jobDescription ?? methodCall.ToString());
            throw;
        }
    }
}

