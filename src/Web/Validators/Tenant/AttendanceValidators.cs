using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class AttendanceDtoValidator : AbstractValidator<AttendanceDto>
{
    public AttendanceDtoValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
        RuleFor(x => x.Remarks).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}

