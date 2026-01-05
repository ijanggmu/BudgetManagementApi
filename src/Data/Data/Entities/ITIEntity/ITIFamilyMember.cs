using Data.Entities.BaseEntity;

namespace Data.Entities.ITIEntity;

public class ITIFamilyMember : ApplicationBaseEntity
{
    public string Relation { get; set; }
    public string FullName { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string InternationalTravelInsuranceId { get; set; }
    public InternationalTravelInsurance InternationalTravelInsurance { get; set; }
}
