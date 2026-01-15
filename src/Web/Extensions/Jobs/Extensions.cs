using Business.Common.JobHelper;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeemaEdgeApi.Extensions.Jobs;

public static class Extensions
{
    public static IServiceCollection ConfigureJobs(this IServiceCollection services)
    {
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = ["sms", "policy", "default", "emails"];
        });

        services.AddScoped<HangfireJobHelper>();
        services.AddTransient<JobFailureHandlerFilter>();

        // Helper for enqueueing jobs
        return services;
    }
    public static IApplicationBuilder UseJobDashboard(this IApplicationBuilder app, IConfiguration config)
    {
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
        {
            Attempts = 3,
            OnAttemptsExceeded = AttemptsExceededAction.Fail
        });

        var failureHandlerFilter = app.ApplicationServices.GetRequiredService<JobFailureHandlerFilter>();
        GlobalJobFilters.Filters.Add(failureHandlerFilter);

        return app.UseHangfireDashboard(config.GetSection("JobConfig:HangfireRoute").Value, new DashboardOptions
        {
            DashboardTitle = "Job",
            Authorization =
                    [
                new HangfireCustomBasicAuthenticationFilter{
                    User = config.GetSection("JobConfig:HangfireUsername").Value,
                    Pass = config.GetSection("JobConfig:HangfirePassword").Value
                 }
                ],
            IgnoreAntiforgeryToken = true
        });

    }
}

