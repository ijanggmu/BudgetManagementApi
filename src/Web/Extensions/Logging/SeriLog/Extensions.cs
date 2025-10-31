using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SharedKernel.Constant;

namespace BeemaEdgeApi.Extensions.Logging.SeriLog;

public static class Extensions
{

    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var seqOptions = builder.Configuration.GetSection(nameof(SeqOptions)).Get<SeqOptions>();
        builder.Host.UseSerilog();
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.With<CorrelationIdEnricher>()
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), "Logs/logs.log", LogEventLevel.Information, rollingInterval: RollingInterval.Day);

        if (seqOptions.EnableSeq)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.SeqUrl, LogEventLevel.Information, apiKey: seqOptions.ApiKey);
        }

        if (seqOptions.EnableSerilog)
        {
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        return builder;
    }

    public class CorrelationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var correlationId = GetCorrelationId();
            if (!string.IsNullOrEmpty(correlationId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(nameof(SystemConstant.CorrelationId), correlationId));
            }
        }

        private static string GetCorrelationId()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext?.Items.TryGetValue(SystemConstant.CorrelationId, out var correlationId) == true)
            {
                return correlationId.ToString();
            }
            return null;
        }
    }

    public class SeqOptions
    {
        public string ApiKey { get; set; }
        public string SeqUrl { get; set; }
        public bool EnableSerilog { get; set; }
        public bool EnableSeq { get; set; }
    }
}
