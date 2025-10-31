using Models.Common.Policy.Policy;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Branch;
public interface IBranchService
{
    Task<Result<IEnumerable<BranchDetailViewModel>>> GetAllBranchAsync();
}
