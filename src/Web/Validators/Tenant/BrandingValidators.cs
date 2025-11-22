using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class UpdateBrandingDtoValidator : AbstractValidator<UpdateBrandingDto>
{
    public UpdateBrandingDtoValidator()
    {
        RuleFor(x => x.LogoUrl)
            .MaximumLength(2048).WithMessage("Logo URL cannot exceed 2048 characters.")
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Logo URL must be a valid absolute URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));

        RuleFor(x => x.PaletteJson)
            .MaximumLength(8000).WithMessage("Palette JSON cannot exceed 8000 characters.")
            .Must(json => string.IsNullOrEmpty(json) || IsValidJson(json))
            .WithMessage("Palette JSON must be valid JSON.")
            .When(x => !string.IsNullOrWhiteSpace(x.PaletteJson));

        RuleFor(x => x.TypographyJson)
            .MaximumLength(8000).WithMessage("Typography JSON cannot exceed 8000 characters.")
            .Must(json => string.IsNullOrEmpty(json) || IsValidJson(json))
            .WithMessage("Typography JSON must be valid JSON.")
            .When(x => !string.IsNullOrWhiteSpace(x.TypographyJson));
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}


