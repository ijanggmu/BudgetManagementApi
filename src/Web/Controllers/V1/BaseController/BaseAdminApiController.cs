using Microsoft.AspNetCore.Mvc;


namespace BeemaEdgeApi.Controllers.V1.BaseController
{
    [ApiExplorerSettings(GroupName = "Admin")]
    public class BaseAdminApiController : BaseApiController
    {
    }
    [ApiExplorerSettings(GroupName = "Tenant")]
    public class BaseTenantAdminApiController : BaseApiController
    {
    }
}

