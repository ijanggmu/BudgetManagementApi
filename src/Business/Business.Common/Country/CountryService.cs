using Data.Context;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Common.Country;
using SharedKernel.Operation;

namespace Business.Common.Country;

public class CountryService(ApplicationDataContext dbContext, ISieveExtension sieveExtension) : ICountryService
{
    public async Task<Result<List<CountryResponseModel>>> GetCountryListAsync(CommonPaginationRequestModel requestModel)
    {

        var query = dbContext.Countries.Where(x => x.IsActive).AsNoTracking();
        requestModel.PageSize = -1;
        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel, paginationDisabled: true);

        var countryList = await result.Select(c => new CountryResponseModel
        {
            CountryId = c.Id,
            CountryName = c.CountryName,
            CountryDialingCode = c.CountryDialingCode,
            CountryISO2 = c.CountryISO2,
            CountryFlagIcon = c.CountryFlagIcon
        }).ToListAsync();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            CurrentPage = requestModel.PageNumber,
            PageSize = requestModel.PageSize,
            TotalPages = totalPage,
        };

        return Result<List<CountryResponseModel>>.Success(countryList, pagination);
    }

}

