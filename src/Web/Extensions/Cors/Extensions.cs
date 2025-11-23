using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeemaEdgeApi.Extensions.Cors;

public static class Extensions
{
    private const string CorsPolicy = nameof(CorsPolicy);
    
    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config.GetSection("CORSUrls").Get<List<string>>() ?? new List<string>();
        
        return services.AddCors(opt =>
        {
            opt.AddPolicy(CorsPolicy, policy =>
            {
                policy.AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(origin =>
                      {
                          if (string.IsNullOrEmpty(origin))
                              return false;

                          if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                              return false;

                          var originHost = uri.Host.ToLowerInvariant();
                          var originPort = uri.Port;
                          var originScheme = uri.Scheme.ToLowerInvariant();

                          // Check each allowed origin
                          foreach (var allowedOrigin in allowedOrigins)
                          {
                              if (string.IsNullOrEmpty(allowedOrigin))
                                  continue;

                              // Support wildcard subdomains: *.example.com
                              if (allowedOrigin.StartsWith("*."))
                              {
                                  var domain = allowedOrigin.Substring(2).ToLowerInvariant();
                                  if (originHost == domain || originHost.EndsWith("." + domain))
                                  {
                                      // For wildcards, allow both http and https in development
                                      if (originScheme == "http" || originScheme == "https")
                                          return true;
                                  }
                              }
                              // Exact match
                              else
                              {
                                  var allowedLower = allowedOrigin.ToLowerInvariant();
                                  
                                  // Parse allowed origin if it's a full URL
                                  if (Uri.TryCreate(allowedLower, UriKind.Absolute, out var allowedUri))
                                  {
                                      // Full URL match: check scheme, host, and port
                                      if (allowedUri.Scheme == originScheme &&
                                          allowedUri.Host.ToLowerInvariant() == originHost &&
                                          (allowedUri.Port == -1 || originPort == -1 || allowedUri.Port == originPort))
                                      {
                                          return true;
                                      }
                                  }
                                  // Host-only match (backward compatibility)
                                  else if (allowedLower == originHost)
                                  {
                                      // For host-only matches, allow both http and https
                                      // In production, you may want to restrict to https only
                                      if (originScheme == "http" || originScheme == "https")
                                          return true;
                                  }
                              }
                          }

                          return false;
                      })
                      .AllowCredentials()
                      .WithExposedHeaders("Content-Disposition");
            });
        });
    }
    
    internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicy);
    }
}
