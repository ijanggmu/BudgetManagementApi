using Infrastructure.CoreApi.CoreApi;
using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Branch;
public class BranchService(ICoreApiService coreApiService) : IBranchService
{
    public async Task<Result<IEnumerable<BranchDetailViewModel>>> GetAllBranchAsync()
    {
        var result = await coreApiService.GetBranchList();
        return Result<IEnumerable<BranchDetailViewModel>>.Success(result);

    }

}
