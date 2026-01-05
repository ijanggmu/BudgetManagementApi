using Models.Common.Policy;

namespace Business.Common.PremiumCalculation.Service;
public interface IPropertySubsidySILimitService
{
    List<PropertySubsidySILimitViewModel> GetAllSubsidySI();
    Task<PropertySubsidySILimitViewModel> GetSingleSubsidySILimit(string id);
    Task UpdateSubsidySILimitAsync(PropertySubsidySILimitViewModel model);
}