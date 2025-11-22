using FluentValidation;
using BeemaEdgeApi.Controllers.V1.Admin.Branding;

namespace BeemaEdgeApi.Validators.Tenant;

public class UpdateBrandingRequestValidator : AbstractValidator<AdminBrandingController.UpdateBrandingRequest>
{
    public UpdateBrandingRequestValidator()
    {
        RuleFor(x => x.LogoUrl).MaximumLength(2048).When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
        RuleFor(x => x.PaletteJson).MaximumLength(8000).When(x => !string.IsNullOrWhiteSpace(x.PaletteJson));
        RuleFor(x => x.TypographyJson).MaximumLength(8000).When(x => !string.IsNullOrWhiteSpace(x.TypographyJson));
    }
}


