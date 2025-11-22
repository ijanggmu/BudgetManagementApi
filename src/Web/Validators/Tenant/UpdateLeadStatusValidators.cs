using FluentValidation;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Validators.Tenant;

public class UpdateLeadStatusDtoValidator : AbstractValidator<UpdateLeadStatusDto>
{
    public UpdateLeadStatusDtoValidator()
    {
        RuleFor(x => x.Status).NotEmpty().Must(status => 
            System.Enum.TryParse<Data.Entities.Tenant.LeadStatus>(status, true, out _))
            .WithMessage("Status must be a valid LeadStatus value (New, Qualified, Contacted, Quoted, Won, Lost)");
    }
}

