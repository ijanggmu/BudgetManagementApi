using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Calculation;
using Microsoft.AspNetCore.Http;
using Models.Common.Policy.Policy.Fire;

namespace Business.Common.PremiumCalculation.Service;
public class PropertyRiskConfigurationService : IPropertyRiskConfigurationService
{
    private ApplicationDataContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PropertyRiskConfigurationService(ApplicationDataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreatePropertyRiskConfiguration(FireRiskConfiguration model)
    {
        var pptyRiskConfig = new PropertyRiskConfiguration()
        {
            RateCode = model.RateCode,
            RiskType = model.RiskType,
            RiskCode = model.RiskCode,
            PropertyDescription = model.PropertyDescription,
            Rate = model.Rate,
            LastModifiedOn = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
            LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name
        };
        await _dbContext.PropertyRiskConfigurations.AddAsync(pptyRiskConfig);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<FireRiskConfiguration> GetSinglePropertyRiskConfigurationById(string id)
    {
        var model = _dbContext.PropertyRiskConfigurations.Single(x => x.Id == id);

        if (model == null)
        {
            return null;
        }
        return new FireRiskConfiguration()
        {
            Id = model.Id,
            RateCode = model.RateCode,
            RiskType = model.RiskType,
            RiskCode = model.RiskCode,
            PropertyDescription = model.PropertyDescription,
            Rate = model.Rate,
            CreatedDate = model.CreatedOn,
        };
    }
    public async Task<FireRiskConfiguration> GetSinglePropertyRiskConfigurationByRiskCode(string riskCode)
    {
        var model = _dbContext.PropertyRiskConfigurations.FirstOrDefault(x => x.RiskCode == riskCode);

        if (model == null)
        {
            return null;
        }
        return new FireRiskConfiguration()
        {
            Id = model.Id,
            RateCode = model.RateCode,
            RiskType = model.RiskType,
            RiskCode = model.RiskCode,
            PropertyDescription = model.PropertyDescription,
            Rate = model.Rate,
            CreatedDate = model.CreatedOn,
        };
    }

    public async Task UpdatePropertyRiskConfiguration(FireRiskConfiguration model)
    {
        var pptyRiskConfig = _dbContext.PropertyRiskConfigurations.Single(x => x.Id == model.Id);

        if (pptyRiskConfig == null)
        {
            throw new Exception("Record not found.");
        }
        pptyRiskConfig.RateCode = model.RateCode;
        pptyRiskConfig.RiskType = model.RiskType;
        pptyRiskConfig.RiskCode = model.RiskCode;
        pptyRiskConfig.PropertyDescription = model.PropertyDescription;
        pptyRiskConfig.Rate = model.Rate;
        pptyRiskConfig.LastModifiedOn = DateTime.UtcNow;
        pptyRiskConfig.CreatedOn = model.CreatedDate;
        pptyRiskConfig.CreatedBy = model.CreatedBy;
        pptyRiskConfig.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;

        _dbContext.PropertyRiskConfigurations.Update(pptyRiskConfig);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<List<FireRiskConfiguration>> GetAllPropertyRiskConfig()
    {
        var pptyConfigs = _dbContext.PropertyRiskConfigurations.Where(x => x.IsDeleted != true).ToList();
        return pptyConfigs.Select(x => new FireRiskConfiguration()
        {
            Id = x.Id,
            RiskCode = x.RiskCode,
            RiskType = x.RiskType,
            RateCode = x.RateCode,
            PropertyDescription = x.PropertyDescription,
            Rate = x.Rate
        }).ToList();
    }
    //public Tuple<int, IQueryable<PropertyRiskConfiguration>> AllIncluding(int skip, int take, string sortColumn, string sortColumnDirection, Expression<Func<PropertyRiskConfiguration, bool>> search, params Expression<Func<PropertyRiskConfiguration, object>>[] includeProperties)
    //{
    //    return _repository.AllIncludingWithQuery(skip, take, sortColumn, sortColumnDirection, search, null, includeProperties);
    //}
    public async Task ImportPropertyRiskConfiguration(List<FireRiskConfiguration> data)
    {
        data.Select(x => { x.CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name; x.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name; return x; });
        try
        {
            var pptyRiskConfig = new List<PropertyRiskConfiguration>();

            foreach (var item in data)
            {
                pptyRiskConfig.Add(new PropertyRiskConfiguration
                {
                    RateCode = item.RateCode,
                    RiskType = item.RiskType,
                    RiskCode = item.RiskCode,
                    PropertyDescription = item.PropertyDescription,
                    Rate = item.Rate,
                    Id = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = item.CreatedBy,
                    LastModifiedBy = item.UpdatedBy

                });
            }
            await _dbContext.PropertyRiskConfigurations.AddRangeAsync(pptyRiskConfig);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
