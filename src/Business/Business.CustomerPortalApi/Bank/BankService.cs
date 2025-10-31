using Infrastructure.CoreApi.CoreApi;
using Models.BeemaEdgeApi.Bank;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Bank;
public class BankService(ICoreApiService coreApiService) : IBankService
{
    public async Task<Result<List<BankResponseViewModel>>> GetAllBankAsync()
    {
        var result = await coreApiService.GetBankList();
        return Result<List<BankResponseViewModel>>.Success(result);

    }
}
