using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.BeemaEdgeApi.AccountValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;

namespace BeemaEdgeApi.Controllers.V1.Admin.AccountValidation;

[AllowAnonymous]
public class AdminAccountValidationController(ICustomerAccountValidationService customerAccountValidationService) : BaseAdminApiController
{
    [HttpGet("CheckUsername")]
    public async Task<IActionResult> CheckUsername([FromQuery] UsernameValidationRequest requestModel, CancellationToken cancellationToken = default)
    {
        var exists = await customerAccountValidationService.IsUsernameTakenAsync(requestModel.Username, cancellationToken);
        return Ok(new { exists });
    }

    [HttpGet("CheckEmail")]
    public async Task<IActionResult> CheckEmail([FromQuery] EmailValidationRequest requestModel, CancellationToken cancellationToken = default)
    {
        var exists = await customerAccountValidationService.IsEmailTakenAsync(requestModel.Email, cancellationToken);
        return Ok(new { exists });
    }

    [HttpGet("CheckPhoneNumber")]
    public async Task<IActionResult> CheckPhoneNumber([FromQuery] PhoneNumberValidationRequest requestModel, CancellationToken cancellationToken = default)
    {
        var exists = await customerAccountValidationService.IsPhoneNumberTakenAsync(requestModel.PhoneNumber, cancellationToken);
        return Ok(new { exists });
    }
}

