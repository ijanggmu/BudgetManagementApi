using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace BeemaEdgeApi.Extensions.OpenApi;

/// <summary>
/// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
/// </summary>
/// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("Customer Portal APi");
        var info = new OpenApiInfo()
        {
            Title = "BeemaEdgeApi",
            Version = description.ApiVersion.ToString(),
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        info.Description = text.ToString();

        return info;
    }
}

