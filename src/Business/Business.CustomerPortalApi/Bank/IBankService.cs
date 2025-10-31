using Models.BeemaEdgeApi.Bank;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Bank;
public interface IBankService
{
    Task<Result<List<BankResponseViewModel>>> GetAllBankAsync();
}
