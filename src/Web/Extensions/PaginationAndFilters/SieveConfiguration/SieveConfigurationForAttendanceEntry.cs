using Data.Entities.Tenant;
using Sieve.Services;

namespace CustomerPortalApi.Extensions.PaginationAndFilters.SieveConfiguration;

public class SieveConfigurationForAttendanceEntry : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<AttendanceEntry>(p => p.Id)
           .CanFilter()
           .CanSort();

        mapper.Property<AttendanceEntry>(p => p.UserId)
           .CanFilter()
           .CanSort();

        mapper.Property<AttendanceEntry>(p => p.Type)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.Latitude)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.Longitude)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.Timestamp)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.Remarks)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.TenantId)
          .CanFilter()
          .CanSort();

        mapper.Property<AttendanceEntry>(p => p.CreatedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<AttendanceEntry>(p => p.CreatedBy)
           .CanFilter()
           .CanSort();

        mapper.Property<AttendanceEntry>(p => p.LastModifiedOn)
           .CanFilter()
           .CanSort();

        mapper.Property<AttendanceEntry>(p => p.IsDeleted)
           .CanFilter()
           .CanSort();
    }
}

