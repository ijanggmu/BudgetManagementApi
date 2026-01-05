using Data.Context;
using Data.Entities.Marine;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Common.Policy.Configuration.MarineTariffScheduleConfiguration;

namespace Business.Common.PremiumCalculation.Calculator.Marine;
public class MarineTariffScheduleService : IMarineTariffScheduleService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDataContext _db;

    public MarineTariffScheduleService(IHttpContextAccessor httpContextAccessor, ApplicationDataContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public async Task CreateMarineTariffSchedule(MarineTariffScheduleViewModel model)
    {
        var entity = new MarineTariffSchedule
        {
            ProductCategory = model.ProductCategory,
            ProductCategoryCode = model.ProductCategoryCode,
            Product = model.Product,
            ProductCode = model.ProductCode,
            AllRiskValue = model.AllRiskValue,
            BasicRiskValue = model.BasicRiskValue,
            MinimumRisk = model.MinimumRiskValue,
            ProductDescription = model.ProductDescription,
            LastModifiedOn = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
            LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name
        };
        await _db.MarineTariffSchedules.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteMarineTariffSchedule(string id)
    {
        MarineTariffSchedule marineTariffSchedule =
            await _db.MarineTariffSchedules.Where(x => x.Id == id).FirstOrDefaultAsync();
        marineTariffSchedule.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public List<MarineTariffScheduleViewModel> GetAllMarineTariffSchedules()
    {
        return _db.MarineTariffSchedules.Where(x => !x.IsDeleted).Select(x =>
        new MarineTariffScheduleViewModel
        {
            Id = x.Id,
            ProductCategory = x.ProductCategory,
            ProductCategoryCode = x.ProductCategoryCode,
            Product = x.Product,
            ProductCode = x.ProductCode,
            AllRiskValue = x.AllRiskValue,
            BasicRiskValue = x.BasicRiskValue,
            MinimumRiskValue = x.MinimumRisk,
            ProductDescription = x.ProductDescription,
            CreatedDate = x.CreatedOn.ToShortDateString(),
        }).ToList();
    }

    public async Task<MarineTariffScheduleViewModel> GetSingleMarineTariffScheduleById(string id)
    {
        var marineTariffSchedule = await _db.MarineTariffSchedules.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (marineTariffSchedule == null)
        {
            return null;
        }

        return new MarineTariffScheduleViewModel
        {
            Id = marineTariffSchedule.Id,
            ProductCategory = marineTariffSchedule.ProductCategory,
            ProductCategoryCode = marineTariffSchedule.ProductCategoryCode,
            Product = marineTariffSchedule.Product,
            ProductCode = marineTariffSchedule.ProductCode,
            AllRiskValue = marineTariffSchedule.AllRiskValue,
            BasicRiskValue = marineTariffSchedule.BasicRiskValue,
            MinimumRiskValue = marineTariffSchedule.MinimumRisk,
            ProductDescription = marineTariffSchedule.ProductDescription,
            CreatedDate = marineTariffSchedule.CreatedOn.ToShortDateString(),
        };
    }

    public async Task<MarineTariffScheduleViewModel> GetSingleMarineTariffScheduleByCode(string productCode)
    {
        var marineTariffSchedule = await _db.MarineTariffSchedules.Where(x => x.ProductCode == productCode).FirstOrDefaultAsync();

        if (marineTariffSchedule == null)
        {
            return null;
        }

        return new MarineTariffScheduleViewModel
        {
            Id = marineTariffSchedule.Id,
            ProductCategory = marineTariffSchedule.ProductCategory,
            ProductCategoryCode = marineTariffSchedule.ProductCategoryCode,
            Product = marineTariffSchedule.Product,
            ProductCode = marineTariffSchedule.ProductCode,
            AllRiskValue = marineTariffSchedule.AllRiskValue,
            BasicRiskValue = marineTariffSchedule.BasicRiskValue,
            MinimumRiskValue = marineTariffSchedule.MinimumRisk,
            ProductDescription = marineTariffSchedule.ProductDescription,
            CreatedDate = marineTariffSchedule.CreatedOn.ToShortDateString(),
        };
    }

    public async Task UpdateMarineTariffSchedule(MarineTariffScheduleViewModel model)
    {
        var entity = new MarineTariffSchedule
        {
            Id = model.Id,
            ProductCategory = model.ProductCategory,
            ProductCategoryCode = model.ProductCategoryCode,
            Product = model.Product,
            ProductCode = model.ProductCode,
            AllRiskValue = model.AllRiskValue,
            BasicRiskValue = model.BasicRiskValue,
            MinimumRisk = model.MinimumRiskValue,
            ProductDescription = model.ProductDescription,
            LastModifiedOn = DateTime.UtcNow,
            CreatedOn = Convert.ToDateTime(model.CreatedDate),
            CreatedBy = model.CreatedBy,
            LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name
        };
        _db.MarineTariffSchedules.Update(entity);
        await _db.SaveChangesAsync();
    }

    //public Tuple<int, IQueryable<MarineTariffSchedule>> AllIncluding(int skip, int take, string sortColumn, string sortColumnDirection, Expression<Func<MarineTariffSchedule, bool>> search, params Expression<Func<MarineTariffSchedule, object>>[] includeProperties)
    //{
    //    return _db.MarineTariffSchedules(skip, take, sortColumn, sortColumnDirection, search, null, includeProperties);
    //}

    //public async Task ImportMarineTariffSchedule(List<MarineTariffScheduleViewModel> data)
    //{
    //    data.Select(x => { x.CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name; x.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name; return x; });
    //    await _marineTariffScheduleRepository.ImportMarineTariffSchedule(data);
    //}
}
