using Microsoft.EntityFrameworkCore;

namespace Data.Context;
public class HangfireDataContext : DbContext
{
    public HangfireDataContext(DbContextOptions<HangfireDataContext> options) : base(options)
    {
    }
}
