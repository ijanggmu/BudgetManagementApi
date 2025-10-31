using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using SharedKernel.Constant;
using BeemaEdgeApi.Utilities.ResponseWrapper;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BeemaEdgeApi.Filters.AuthorizationFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class IndividualAuthorizationFilterAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 👇 Skip if [SkipIndividualAuthorization] is present
        var hasSkipAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<SkipIndividualAuthorizationAttribute>()
            .Any();

        if (hasSkipAttribute)
            return;

        var environment = context.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

        var userId = context.HttpContext.User.FindFirst(TokenKey.UserId)?.Value;

        var tokenHeader = context.HttpContext.Request.Cookies["X-Refresh-Token"];
        tokenHeader = HttpUtility.UrlDecode(HttpUtility.UrlDecode(tokenHeader));

        if (tokenHeader is null || userId is null)
        {
            SetForbiddenResult(context, $"1-{tokenHeader is null}-{userId is null}");
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDataContext)) as ApplicationDataContext;

        var customer = dbContext.Customers
                                .Include(y => y.User)
                                .Where(x => x.User.RefreshToken == tokenHeader &&
                                            x.UserId == userId &&
                                           !x.IsDeleted &&
                                           !x.User.IsDeleted)
                                .Any();

        if (!customer)
            SetForbiddenResult(context, "unauthorized");
    }


    private void SetForbiddenResult(AuthorizationFilterContext context, string msg) //msg for debug purpose
    {
        var errorObject = ErrorApiResponse.WrapError($"Unauthorized {msg}", 401);
        context.Result = new ObjectResult(errorObject)
        {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            ContentTypes = new MediaTypeCollection { "application/json" }
        };
    }
}

