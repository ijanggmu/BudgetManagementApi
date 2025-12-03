using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.WebApi.Customer.Policy;

namespace Data.Entities.ITIEntity;

public class InternationalTravelrConfiguration : IEntityTypeConfiguration<InternationalTravelInsurance>
{
    public void Configure(EntityTypeBuilder<InternationalTravelInsurance> builder)
    {

        builder.Property(x => x.TripType)
            .HasConversion(v => v.ToString(),
                 v => (TripType)Enum.Parse(typeof(TripType), v));

        builder.Property(x => x.InsuranceType)
           .HasConversion(v => v.ToString(),
                v => (TravelInsuranceType)Enum.Parse(typeof(TravelInsuranceType), v));

        builder.Property(x => x.PlanType)
           .HasConversion(v => v.ToString(),
                v => (TravelPlanType)Enum.Parse(typeof(TravelPlanType), v));

    }
}

[EntityTypeConfiguration(typeof(InternationalTravelrConfiguration))]
public class InternationalTravelInsurance : ApplicationBaseEntity
{
    public int PolicyPeriodInDays { get; set; }
    public TripType TripType { get; set; }
    public string IdType { get; set; }
    public string PassportNumber { get; set; }
    public string VisitingCountry { get; set; }
    public string FatherHusbandName { get; set; }
    public List<ITIFamilyMember> FamilyMembers { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactNumber { get; set; }
    public string TravellingCountry { get; set; }
    public TravelInsuranceType InsuranceType { get; set; }
    public string TypeOfInsured { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public TravelPlanType PlanType { get; set; }
}
