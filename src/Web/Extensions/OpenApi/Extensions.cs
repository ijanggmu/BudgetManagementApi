using System;
using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BeemaEdgeApi.Extensions.OpenApi;

public static class Extensions
{
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(opt =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Cookie,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "cookie"
            };

            var securityRequirement = new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "CookieAuth"
                                    }
                                },
                                new string[]{}
                            }
                         };
            opt.AddSecurityDefinition("CookieAuth", securityScheme);
            opt.AddSecurityRequirement(securityRequirement);
            opt.CustomSchemaIds(type => type.FullName);
        });
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.DefaultApiVersion = ApiVersion.Default;
            options.SubstituteApiVersionInUrl = true;
        })
        .EnableApiVersionBinding();
        return services;
    }

    public static WebApplication UseOpenApi(this WebApplication app, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (configuration.GetValue<bool>("AppSettings:EnableSwagger") && app.Environment.EnvironmentName != "Prod")
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(DocExpansion.None);
                options.DisplayRequestDuration();
                var swaggerEndpoints = app.DescribeApiVersions()
                    .Select(desc => new
                    {
                        Url = $"/swagger/{desc.GroupName}/swagger.json",
                        Name = desc.GroupName.ToUpperInvariant()
                    });

                foreach (var endpoint in swaggerEndpoints)
                {
                    options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
                }
            });
        }
        return app;
    }
}
