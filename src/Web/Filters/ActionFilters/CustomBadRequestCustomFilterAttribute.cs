using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BeemaEdgeApi.Utilities.ResponseWrapper;

namespace BeemaEdgeApi.Filters.ActionFilters;

public class CustomBadRequestCustomFilterAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorsInModelState = context.ModelState
                  .Where(x => x.Value.Errors.Count > 0)
                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(y => y.ErrorMessage));

            List<string> errorsList = [];
            foreach (var error in errorsInModelState)
            {
                foreach (var subError in error.Value)
                {
                    errorsList.Add($"{subError}");
                }
            }

            context.Result = new BadRequestObjectResult(ErrorApiResponse.WrapError(errorsList));
            return;
        }

        base.OnResultExecuting(context);
    }
}
