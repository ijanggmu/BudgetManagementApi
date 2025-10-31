using Models.Common.Policy.Policy;
using Models.WebApi.Customer.Policy;
using Models.WebApi.Individual;

namespace Business.BeemaEdgeApi.HangFireJob.CustomerProfileJob;
public interface ICustomerProfileJobService
{
    Task<IndividualResponseModel> CreateIndividualAsync(CreateIndividualRequestModel requestModel,string userId);
    Task<IndividualResponseModel> UpdateIndividualAsync(CreateIndividualRequestModel requestModel,string userId);
    Task FetchIndividualAndUpdateCustomerAsync(string userId, IndividualCustomerCheckRequestModel requestModel);
}

