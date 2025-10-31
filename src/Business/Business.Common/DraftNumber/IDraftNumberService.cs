using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Common.DraftNumber;
public interface IDraftNumberService
{
    Task<string> GetNextDraftNumberAsync();
}

public class DraftNumberService(ApplicationDataContext context) : IDraftNumberService
{
    private readonly ApplicationDataContext _context = context;

    public async Task<string> GetNextDraftNumberAsync()
    {
        var nextVal = await _context.Database
            .SqlQueryRaw<long>("SELECT nextval('draft_number_seq') AS \"Value\"")
            .FirstAsync();


        return $"DDN-{nextVal.ToString().PadLeft(7, '0')}";
    }
}
