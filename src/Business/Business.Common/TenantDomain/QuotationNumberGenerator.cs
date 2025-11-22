using System;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Business.Common.TenantDomain;

public class QuotationNumberGenerator : IQuotationNumberGenerator
{
    private readonly ApplicationDataContext _db;

    public QuotationNumberGenerator(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task<string> NextAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyyMMdd");
        var count = await _db.Set<Quotation>().CountAsync(q => q.Number.StartsWith(today));
        return $"{today}-{count + 1:0000}";
    }
}


