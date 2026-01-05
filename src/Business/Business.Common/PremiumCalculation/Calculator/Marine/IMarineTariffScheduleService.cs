using Models.Common.Policy.Configuration.MarineTariffScheduleConfiguration;

namespace Business.Common.PremiumCalculation.Calculator.Marine;
public interface IMarineTariffScheduleService
{
    Task CreateMarineTariffSchedule(MarineTariffScheduleViewModel model);
    Task DeleteMarineTariffSchedule(string id);
    List<MarineTariffScheduleViewModel> GetAllMarineTariffSchedules();
    Task<MarineTariffScheduleViewModel> GetSingleMarineTariffScheduleByCode(string productCode);
    Task<MarineTariffScheduleViewModel> GetSingleMarineTariffScheduleById(string id);
    Task UpdateMarineTariffSchedule(MarineTariffScheduleViewModel model);
}