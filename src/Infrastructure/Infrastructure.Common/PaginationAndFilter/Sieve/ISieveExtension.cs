using Models.Common;

namespace Infrastructure.Common.PaginationAndFilter.Sieve
{
    public interface ISieveExtension
    {
        Task<(IQueryable<T> result, int totalCount, int totalPage)> ApplySieve<T, TModel>(IQueryable<T> query, TModel model, string defaultFilter = null, string defaultSort = null, bool paginationDisabled = false) where TModel : CommonPaginationRequestModel;
    }
}
