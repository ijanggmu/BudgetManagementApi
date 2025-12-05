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
        
        // Get logger for initial logging
        var tempServiceProvider = services.BuildServiceProvider();
        var loggerFactory = tempServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("BeemaEdgeApi.Extensions.Cors");
        
        // Log loaded CORS URLs
        logger.LogInformation("CORS Configuration: Loaded {Count} allowed origins", allowedOrigins.Count);
        for (int i = 0; i < allowedOrigins.Count; i++)
        {
            logger.LogInformation("CORS Allowed Origin [{Index}]: '{Origin}'", i, allowedOrigins[i]);
        }
        
        // Create logger for origin checks (will be used in closure)
        var originLogger = loggerFactory.CreateLogger("CorsOriginCheck");
        
        return services.AddCors(opt =>
        {
            opt.AddPolicy(CorsPolicy, policy =>
            {
                policy.AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(origin =>
                      {
                          
                          try
                          {
                              originLogger.LogInformation("CORS Check: Evaluating origin '{Origin}'", origin);
                              
                              if (string.IsNullOrEmpty(origin))
                              {
                                  originLogger.LogWarning("CORS Check: Origin is null or empty");
                                  return false;
                              }

                              if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                              {
                                  originLogger.LogWarning("CORS Check: Failed to parse origin '{Origin}' as URI", origin);
                                  return false;
                              }

                              var originHost = uri.Host.ToLowerInvariant();
                              var originPort = uri.Port;
                              var originScheme = uri.Scheme.ToLowerInvariant();
                              
                              originLogger.LogInformation("CORS Check: Parsed origin - Host: '{Host}', Port: {Port}, Scheme: '{Scheme}'", 
                                  originHost, originPort == -1 ? "default" : originPort.ToString(), originScheme);

                              // Check each allowed origin
                              foreach (var allowedOrigin in allowedOrigins)
                              {
                                  if (string.IsNullOrEmpty(allowedOrigin))
                                  {
                                      originLogger.LogDebug("CORS Check: Skipping empty allowed origin");
                                      continue;
                                  }

                                  originLogger.LogDebug("CORS Check: Comparing against allowed origin '{AllowedOrigin}'", allowedOrigin);

                                  // Support wildcard subdomains: *.example.com
                                  if (allowedOrigin.StartsWith("*."))
                                  {
                                      var domain = allowedOrigin.Substring(2).ToLowerInvariant();
                                      originLogger.LogDebug("CORS Check: Wildcard pattern detected, domain: '{Domain}'", domain);
                                      
                                      if (originHost == domain || originHost.EndsWith("." + domain))
                                      {
                                          originLogger.LogDebug("CORS Check: Host matches wildcard pattern");
                                          // For wildcards, allow both http and https in development
                                          if (originScheme == "http" || originScheme == "https")
                                          {
                                              originLogger.LogInformation("CORS Check: ✓ ALLOWED - Matched wildcard pattern '{Pattern}'", allowedOrigin);
                                              return true;
                                          }
                                          else
                                          {
                                              originLogger.LogWarning("CORS Check: Host matched but scheme '{Scheme}' not allowed for wildcard", originScheme);
                                          }
                                      }
                                      else
                                      {
                                          originLogger.LogDebug("CORS Check: Host '{Host}' does not match wildcard pattern '{Pattern}'", originHost, allowedOrigin);
                                      }
                                  }
                                  // Exact match
                                  else
                                  {
                                      var allowedLower = allowedOrigin.ToLowerInvariant();
                                      
                                      // Parse allowed origin if it's a full URL
                                      if (Uri.TryCreate(allowedLower, UriKind.Absolute, out var allowedUri))
                                      {
                                          originLogger.LogDebug("CORS Check: Allowed origin is full URL - Host: '{Host}', Port: {Port}, Scheme: '{Scheme}'", 
                                              allowedUri.Host, allowedUri.Port == -1 ? "default" : allowedUri.Port.ToString(), allowedUri.Scheme);
                                          
                                          // Full URL match: check scheme, host, and port
                                          if (allowedUri.Scheme == originScheme &&
                                              allowedUri.Host.ToLowerInvariant() == originHost &&
                                              (allowedUri.Port == -1 || originPort == -1 || allowedUri.Port == originPort))
                                          {
                                              originLogger.LogInformation("CORS Check: ✓ ALLOWED - Full URL match with '{AllowedOrigin}'", allowedOrigin);
                                              return true;
                                          }
                                          else
                                          {
                                              originLogger.LogDebug("CORS Check: Full URL mismatch - Scheme: {SchemeMatch}, Host: {HostMatch}, Port: {PortMatch}",
                                                  allowedUri.Scheme == originScheme,
                                                  allowedUri.Host.ToLowerInvariant() == originHost,
                                                  (allowedUri.Port == -1 || originPort == -1 || allowedUri.Port == originPort));
                                          }
                                      }
                                      // Host-only match (backward compatibility)
                                      else if (allowedLower == originHost)
                                      {
                                          originLogger.LogDebug("CORS Check: Host-only match found");
                                          // For host-only matches, allow both http and https
                                          // In production, you may want to restrict to https only
                                          if (originScheme == "http" || originScheme == "https")
                                          {
                                              originLogger.LogInformation("CORS Check: ✓ ALLOWED - Host-only match with '{AllowedOrigin}'", allowedOrigin);
                                              return true;
                                          }
                                          else
                                          {
                                              originLogger.LogWarning("CORS Check: Host matched but scheme '{Scheme}' not allowed", originScheme);
                                          }
                                      }
                                      else
                                      {
                                          originLogger.LogDebug("CORS Check: Host '{Host}' does not match allowed origin '{AllowedOrigin}'", originHost, allowedOrigin);
                                      }
                                  }
                              }

                              originLogger.LogWarning("CORS Check: ✗ DENIED - Origin '{Origin}' did not match any allowed origins", origin);
                              return false;
                          }
                          catch (Exception ex)
                          {
                              originLogger.LogError(ex, "CORS Check: Exception while evaluating origin '{Origin}'", origin);
                              return false;
                          }
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
