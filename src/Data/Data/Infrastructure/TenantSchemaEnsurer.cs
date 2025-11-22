using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Infrastructure;

public interface ITenantSchemaEnsurer
{
    Task EnsureCurrentTenantSchemaAsync(DbContext db);
}

public class TenantSchemaEnsurer : ITenantSchemaEnsurer
{
    private readonly ITenantSchemaProvider _schemaProvider;

    public TenantSchemaEnsurer(ITenantSchemaProvider schemaProvider)
    {
        _schemaProvider = schemaProvider;
    }

    public async Task EnsureCurrentTenantSchemaAsync(DbContext db)
    {
        var schema = _schemaProvider.GetSchemaOrNull();
        if (string.IsNullOrWhiteSpace(schema)) return;

        var sql = $"CREATE SCHEMA IF NOT EXISTS \"{schema}\";";
        await db.Database.ExecuteSqlRawAsync(sql);
    }
}


