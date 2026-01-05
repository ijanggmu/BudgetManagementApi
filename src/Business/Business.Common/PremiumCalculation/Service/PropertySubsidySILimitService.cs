using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Common.Policy;

namespace Business.Common.PremiumCalculation.Service;
public class PropertySubsidySILimitService : IPropertySubsidySILimitService
{
    private ApplicationDataContext _db;
    private IHttpContextAccessor _httpContextAccessor;
    public PropertySubsidySILimitService(IHttpContextAccessor httpContextAccessor, ApplicationDataContext db)
    {

        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    //public Tuple<int, IQueryable<PropertySubsidySILimit>> AllIncluding(int skip,
    //int take,
    //string sortColumn,
    //string sortColumnDirection,
    //Expression<Func<PropertySubsidySILimit, bool>> search,

    //params Expression<Func<PropertySubsidySILimit, object>>[] includeProperties)
    //{
    //    return _propertySubsidySILimitRepository.AllIncludingWithQuery(skip, take, sortColumn, sortColumnDirection, search, null, includeProperties);
    //}

    public List<PropertySubsidySILimitViewModel> GetAllSubsidySI()
    {
        var SubsidySI = _db.PropertySubsidySILimits.Where(x => !x.IsDeleted).OrderBy(x => x.SILimit);
        return SubsidySI.Select(x => new PropertySubsidySILimitViewModel
        {
            Id = x.Id,
            SILimit = x.SILimit,
            SILabel = x.SILabel,
            CreatedBy = x.CreatedBy,
            CreatedDate = x.CreatedOn.ToString()
        }).ToList();
    }
    public async Task<PropertySubsidySILimitViewModel> GetSingleSubsidySILimit(string id)
    {
        return _db.PropertySubsidySILimits
    .Where(x => x.Id == id)
    .Select(x => new PropertySubsidySILimitViewModel
    {
        Id = x.Id,
        SILabel = x.SILabel,
        SILimit = x.SILimit,
        CreatedBy = x.CreatedBy,
        CreatedDate = x.CreatedOn.ToString()
    })
    .FirstOrDefault();

    }

    public async Task UpdateSubsidySILimitAsync(PropertySubsidySILimitViewModel model)
    {
        var existingEntity = await _db.PropertySubsidySILimits
            .FirstOrDefaultAsync(e => e.Id == model.Id);

        if (existingEntity == null)
        {
            throw new ArgumentException($"Entity with Id {model.Id} not found.");
        }

        existingEntity.SILabel = model.SILabel;
        existingEntity.SILimit = model.SILimit;
        existingEntity.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
        existingEntity.LastModifiedOn = DateTime.UtcNow;
        existingEntity.CreatedBy = model.CreatedBy;
        existingEntity.CreatedOn = DateTime.TryParse(model.CreatedDate, out DateTime createdDate)
            ? createdDate
            : throw new ArgumentException("Invalid CreatedDate format.");

        _db.PropertySubsidySILimits.Update(existingEntity);
        await _db.SaveChangesAsync();
    }
}
