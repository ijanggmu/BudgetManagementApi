using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.BeemaEdgeApi.TwoFactor;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Admin.TwoFactor
{
    public class AdminTwoFactorController(ICustomerTwoFactorService customerTwoFactorService) : BaseAdminApiController
    {
        [HttpGet("Setup")]
        public async Task<IActionResult> Set2Fa(CancellationToken cancellationToken = default) => HandleResult(await customerTwoFactorService.Set2FaAsync(cancellationToken));

        [HttpPut("Disable")]
        public async Task<IActionResult> Disable2Fa(CancellationToken cancellationToken = default) => HandleResult(await customerTwoFactorService.Disable2FaAsync(cancellationToken));

        [HttpGet("Verify/{code}")]
        public async Task<IActionResult> Verify2Fa(string code, CancellationToken cancellationToken = default) => HandleResult(await customerTwoFactorService.ValidateTotpCodeAsync(code, cancellationToken));

        [HttpGet("BackupCodes")]
        public async Task<IActionResult> GetAll2FaBackUpCodes(CancellationToken cancellationToken = default) => HandleResult(await customerTwoFactorService.GetAll2FaBackUpCodesAsync(cancellationToken));
    }
}

