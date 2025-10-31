using Microsoft.EntityFrameworkCore;
using Models.Common;
using SharedKernel.Constant;
using Sieve.Models;
using Sieve.Services;

namespace Infrastructure.Common.PaginationAndFilter.Sieve;

public class SieveExtension(ISieveProcessor sieveProcessor) : ISieveExtension
{
    public async Task<(IQueryable<T> result, int totalCount, int totalPage)> ApplySieve<T, TModel>(IQueryable<T> query, TModel model, string defaultFilter = null, string defaultSort = null, bool paginationDisabled = false) where TModel : CommonPaginationRequestModel
    {
        if (string.IsNullOrEmpty(model.Sorts))
            model.Sorts = SystemConstant.DefaultSorting;

        if (!string.IsNullOrEmpty(model.Query))
            model.Filters = $"{model.Query},{model.Filters}";

        if (!string.IsNullOrEmpty(defaultFilter) && string.IsNullOrEmpty(model.Filters) && string.IsNullOrEmpty(model.Query))
            model.Filters = defaultFilter;

        var sieveModel = new SieveModel()
        {
            Filters = model.Filters,
            Sorts = model.Sorts
        };

        var source = sieveProcessor.Apply(sieveModel, query, applyPagination: false);

        var totalCount = await source.CountAsync();
        IQueryable<T> result;

        if (paginationDisabled && model.PageSize < 0)
        {
            result = source;
        }

        else
        {
            result = source
                               .Skip((model.PageNumber - 1) * model.PageSize)
                               .Take(model.PageSize);
        }

        var totalPage = (int)Math.Ceiling(totalCount / (double)model.PageSize);

        return (result, totalCount, totalPage);
    }

}
