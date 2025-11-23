using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BeemaEdgeApi.Extensions.Cors;

public static class Extensions
{
    private const string CorsPolicy = nameof(CorsPolicy);
    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        return services.AddCors(opt =>
        {
            opt.AddPolicy(CorsPolicy, policy =>
            {
                policy.AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(origin =>
                      {

                          var allowedDomains = config.GetSection("CORSUrls").Get<List<string>>();

                          Uri uri;
                          if (Uri.TryCreate(origin.ToLower(), UriKind.Absolute, out uri))
                          {
                              Log.Information("CORS origin check for: {Origin}", origin);
                              var host = uri.Host;
                              Log.Information("CORS origin check for: {Origin Host}", host);
                              var bools = allowedDomains.Contains(host);
                              //return bools;
                              return bools;
                          }
                          else
                          {
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
