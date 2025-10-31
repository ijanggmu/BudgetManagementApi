using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.WebApi.Merchant;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Merchant;
public interface IMerchantPolicyService
{
    Task<Result<MerchantPolicyResponseModel>> SubmitDraftAsync(CreateMerchantPolicyRequestModel model, CancellationToken cancellationToken);

}
