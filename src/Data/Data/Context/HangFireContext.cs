using Microsoft.EntityFrameworkCore;

namespace Data.Context;
public class HangfireDataContext(DbContextOptions<HangfireDataContext> options) : DbContext(options)
{
}
