using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;


namespace BeemaEdgeApi.Controllers.V1.BaseController
{
    [ApiExplorerSettings(GroupName = "Individual")]
    [IndividualAuthorizationFilter]
    public class BaseIndividualApiController : BaseApiController
    {
    }
}

