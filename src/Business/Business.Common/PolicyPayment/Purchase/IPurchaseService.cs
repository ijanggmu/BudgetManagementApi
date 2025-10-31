using Models.Common.PolicyPayment;
using SharedKernel.Operation;

namespace Business.Common.PolicyPayment.Purchase;

public interface IPurchaseService
{
    Task<Result<object>> InitiatePaymentAsync(InitiatePaymentRequest requestModel);
    Task<Result<bool>> VerifyAsync(VerifyPaymentRequestModel requestModel);
}




