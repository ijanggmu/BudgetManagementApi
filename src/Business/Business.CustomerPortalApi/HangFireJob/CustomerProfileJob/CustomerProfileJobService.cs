using Data.Context;
using Data.Entities.CustomerEntity;
using Hangfire;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common.Policy.Policy;
using Models.BeemaEdgeApi.Customer.Policy;
using Models.WebApi.Customer.Policy;
using Models.WebApi.Individual;
using SharedKernel.SystemEnum;

namespace Business.BeemaEdgeApi.HangFireJob.CustomerProfileJob;

public class CustomerProfileJobService(ICoreApiService coreApiService, ApplicationDataContext context, ILogger<CustomerProfileJobService> logger) : ICustomerProfileJobService
{
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task<IndividualResponseModel> CreateIndividualAsync(CreateIndividualRequestModel requestModel, string userId)
    {
        logger.LogInformation("Start processing CreateIndividualAsync job for {FullName}", requestModel.FirstName);

        try
        {

            var contactKyc = new ContactKYCViewModel
            {
                CitizenshipNo = requestModel.CitizenshipNo,
                CitizenshipIssueDate = requestModel.CitizenshipIssueDate,
                CitizenshipIssueDistrict = requestModel.CitizenshipIssueDistrict,
                DobBS = DateTime.TryParse(requestModel.DobBS, out var parsedDobBs) ? parsedDobBs : DateTime.MinValue,
                DobAD = requestModel.DobAD,
                PassportNumber = requestModel.PassportNumber,
                PassportIssueDate = requestModel.PassportIssueDate,
                PassportExpiryDate = requestModel.PassportExpiryDate,
                PassportIssuePlace = requestModel.PassportIssuePlace,
                VoterIdNumber = requestModel.VoterIdNumber,
                LicenseNumber = requestModel.LicenseNumber,
                OccupationJson = requestModel.OccupationJson,
                ClientClassification = "C"
            };

            if (!string.IsNullOrWhiteSpace(requestModel.CitizenshipNo))
            {
                contactKyc.IdentificationType = "Citizenship";
                contactKyc.IdentificationNo = requestModel.CitizenshipNo;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.PassportNumber))
            {
                contactKyc.IdentificationType = "Passport";
                contactKyc.IdentificationNo = requestModel.PassportNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.VoterIdNumber))
            {
                contactKyc.IdentificationType = "VoterId";
                contactKyc.IdentificationNo = requestModel.VoterIdNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.LicenseNumber))
            {
                contactKyc.IdentificationType = "License";
                contactKyc.IdentificationNo = requestModel.LicenseNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.NidNumber))
            {
                contactKyc.IdentificationType = "NationalId";
                contactKyc.IdentificationNo = requestModel.NidNumber;
            }

            var contactViewModel = new ContactViewModel
            {
                CourtesyTitle = requestModel.CourtesyTitle,
                FirstName = requestModel.FirstName,
                MiddleName = requestModel.MiddleName,
                LastName = requestModel.LastName,
                Gender = requestModel.Gender,
                PanNo = requestModel.PanNo,
                MaritialStatus = requestModel.MaritialStatus,
                ContactKYCViewModel = contactKyc,
                PaProvince = requestModel.PaProvince,
                PaDistrict = requestModel.PaDistrict,
                PaMunicipality = requestModel.PaMunicipality,
                PaWard = requestModel.PaWard,
                PaStreetAddress = requestModel.PaStreetAddress,
                TmProvince = requestModel.TmProvince,
                TmDistrict = requestModel.TmDistrict,
                TmMunicipality = requestModel.TmMunicipality,
                TmWard = requestModel.TmWard,
                TmStreetAddress = requestModel.TmStreetAddress,
                Individual_Type = requestModel.IndividualType,
                Mobile = requestModel.PhoneNumber,
                Email = requestModel.Email,
                Phone = requestModel.PhoneNumber,
                FirstName_Np = requestModel.FirstNameNepali,
                MiddleName_Np = requestModel.MiddleNameNepali,
                LastName_Np = requestModel.LastNameNepali,
                IsFromCustomerPortal = true
            };

            logger.LogInformation("IndividualPayload :{Payload}", requestModel);

            var response = await coreApiService.CreateIndividualV2(contactViewModel);

            logger.LogInformation("Successfully processed CreateIndividualAsync job for {FullName}", requestModel.FullName);

            var customerToUpdate = await context.Customers
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (customerToUpdate == null)
            {
                logger.LogWarning("Customer with UserId {UserId} not found for updating individual data.", userId);
                return null; // or throw an exception based on your error handling strategy
            }

            customerToUpdate.PartyCode = response.PartyCode;
            customerToUpdate.PartyID = response.PartyId;
            customerToUpdate.DIANumber = response.DIANumber;
            customerToUpdate.CoreSyncStatus = string.IsNullOrEmpty(response.DIANumber) ? CoreSyncStatus.Pending : CoreSyncStatus.Succeeded;

            context.Update(customerToUpdate);
            await context.SaveChangesAsync();

            return new IndividualResponseModel
            {
                PartyCode = response.PartyCode,
                PartyId = response.PartyId,
                DIANumber = response.DIANumber
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process CreateIndividualAsync job for {FullName}", requestModel.FullName);
            throw; // Important: rethrow so Hangfire registers the failure and triggers retries
        }
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task FetchIndividualAndUpdateCustomerAsync(string userId, IndividualCustomerCheckRequestModel requestModel)
    {
        logger.LogInformation("Start processing FetchIndividualAndUpdateCustomerAsync job for {PhoneNumber}", requestModel.PhoneNumber);

        try
        {
            var individualResult = await coreApiService.IndividualCustomerCheck(requestModel);
            logger.LogInformation("Successfully processed CreateIndividualAsync job for {PhoneNumber}", requestModel.PhoneNumber);
            if (individualResult != null)
            {
                await SyncCoreIndividualData(userId, individualResult);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process CreateIndividualAsync job for {PhoneNumber}", requestModel.PhoneNumber);
            throw; // Important: rethrow so Hangfire registers the failure and triggers retries
        }
    }
    private async Task SyncCoreIndividualData(string userId, IndividualCheckResponseModel coreCustomerRequest)
    {
        var customer = await context.Customers
                                    .Where(x => x.UserId == userId && !x.IsDeleted)
                                    .FirstOrDefaultAsync();
        if (customer == null)
        {
            logger.LogWarning("Customer with UserId {UserId} not found for syncing individual data.", userId);
            return;
        }

        if (coreCustomerRequest.Status && coreCustomerRequest.StatusCode == "100" && coreCustomerRequest.Individual != null)
        {// map customer details
            customer.IndividualType = coreCustomerRequest.Individual.Individual_Type;
            customer.Gender = coreCustomerRequest.Individual.Gender;
            customer.MaritalStatus = coreCustomerRequest.Individual.MaritialStatus;
            customer.PanNo = coreCustomerRequest.Individual.PanNo;
            customer.DobBS = coreCustomerRequest.Individual.ContactKYCViewModel.DobBS.ToString();
            customer.CitizenshipNo = coreCustomerRequest.Individual.ContactKYCViewModel.CitizenshipNo;
            customer.CitizenshipIssueDate = coreCustomerRequest.Individual.ContactKYCViewModel.CitizenshipIssueDate;
            customer.PassportNumber = coreCustomerRequest.Individual.ContactKYCViewModel.PassportNumber;
            customer.PassportIssueDate = coreCustomerRequest.Individual.ContactKYCViewModel.PassportIssueDate;
            customer.PassportIssuePlace = coreCustomerRequest.Individual.ContactKYCViewModel.PassportIssuePlace;
            customer.VoterIdNumber = coreCustomerRequest.Individual.ContactKYCViewModel.VoterIdNumber;
            customer.LicenseNumber = coreCustomerRequest.Individual.ContactKYCViewModel.LicenseNumber;
            customer.Occupation = coreCustomerRequest.Individual.ContactKYCViewModel.OccupationJson.FirstOrDefault();
            customer.DIANumber = coreCustomerRequest.Individual.DIANumber;
            customer.PartyCode = coreCustomerRequest.Individual.PartyCode;
            customer.PartyID = coreCustomerRequest.Individual.Id;
            customer.CoreSyncStatus = CoreSyncStatus.Succeeded;

            var existingPermanent = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString());
            if (existingPermanent is not null)
            {
                existingPermanent.Province = coreCustomerRequest.Individual.Address.Province;
                existingPermanent.District = coreCustomerRequest.Individual.Address.District;
                existingPermanent.Municipality = coreCustomerRequest.Individual.Address.Municipality;
                existingPermanent.Ward = coreCustomerRequest.Individual.Address.Ward;
                existingPermanent.StreetAddress = coreCustomerRequest.Individual.Address.StreetAddress;
            }
            var existingTemporary = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString());
            if (existingTemporary is not null)
            {
                existingTemporary.Province = coreCustomerRequest.Individual.Address.Province;
                existingTemporary.District = coreCustomerRequest.Individual.Address.District;
                existingTemporary.Municipality = coreCustomerRequest.Individual.Address.Municipality;
                existingTemporary.Ward = coreCustomerRequest.Individual.Address.Ward;
                existingTemporary.StreetAddress = coreCustomerRequest.Individual.Address.StreetAddress;
            }
            context.Update(customer);
            await context.SaveChangesAsync();
        }
    }


    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task<IndividualResponseModel> UpdateIndividualAsync(CreateIndividualRequestModel requestModel, string userId)
    {
        logger.LogInformation("Start processing UpdateIndividualAsync job for {FullName}", requestModel.FirstName);

        try
        {

            var contactKyc = new ContactKYCViewModel
            {
                CitizenshipNo = requestModel.CitizenshipNo,
                CitizenshipIssueDate = requestModel.CitizenshipIssueDate,
                CitizenshipIssueDistrict = requestModel.CitizenshipIssueDistrict,
                DobBS = DateTime.TryParse(requestModel.DobBS, out var parsedDobBs) ? parsedDobBs : DateTime.MinValue,
                DobAD = requestModel.DobAD,
                PassportNumber = requestModel.PassportNumber,
                PassportIssueDate = requestModel.PassportIssueDate,
                PassportExpiryDate = requestModel.PassportExpiryDate,
                PassportIssuePlace = requestModel.PassportIssuePlace,
                VoterIdNumber = requestModel.VoterIdNumber,
                LicenseNumber = requestModel.LicenseNumber,
                OccupationJson = requestModel.OccupationJson,
                ClientClassification = "C"
            };

            if (!string.IsNullOrWhiteSpace(requestModel.CitizenshipNo))
            {
                contactKyc.IdentificationType = "Citizenship";
                contactKyc.IdentificationNo = requestModel.CitizenshipNo;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.PassportNumber))
            {
                contactKyc.IdentificationType = "Passport";
                contactKyc.IdentificationNo = requestModel.PassportNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.VoterIdNumber))
            {
                contactKyc.IdentificationType = "VoterId";
                contactKyc.IdentificationNo = requestModel.VoterIdNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.LicenseNumber))
            {
                contactKyc.IdentificationType = "License";
                contactKyc.IdentificationNo = requestModel.LicenseNumber;
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.NidNumber))
            {
                contactKyc.IdentificationType = "NationalId";
                contactKyc.IdentificationNo = requestModel.NidNumber;
            }

            var contactViewModel = new ContactViewModel
            {
                CourtesyTitle = requestModel.CourtesyTitle,
                FirstName = requestModel.FirstName,
                MiddleName = requestModel.MiddleName,
                LastName = requestModel.LastName,
                Gender = requestModel.Gender,
                PanNo = requestModel.PanNo,
                MaritialStatus = requestModel.MaritialStatus,
                ContactKYCViewModel = contactKyc,
                PaProvince = requestModel.PaProvince,
                PaDistrict = requestModel.PaDistrict,
                PaMunicipality = requestModel.PaMunicipality,
                PaWard = requestModel.PaWard,
                PaStreetAddress = requestModel.PaStreetAddress,
                TmProvince = requestModel.TmProvince,
                TmDistrict = requestModel.TmDistrict,
                TmMunicipality = requestModel.TmMunicipality,
                TmWard = requestModel.TmWard,
                TmStreetAddress = requestModel.TmStreetAddress,
                Individual_Type = requestModel.IndividualType,
                Mobile = requestModel.PhoneNumber,
                Email = requestModel.Email,
                Phone = requestModel.PhoneNumber,
                FirstName_Np = requestModel.FirstNameNepali,
                MiddleName_Np = requestModel.MiddleNameNepali,
                LastName_Np = requestModel.LastNameNepali,
            };

            logger.LogInformation("IndividualPayload :{Payload}", requestModel);

            var response = await coreApiService.UpdateIndividual(contactViewModel);

            logger.LogInformation("Successfully processed UpdateIndividualAsync job for {FullName}", requestModel.FullName);

            var customerToUpdate = await context.Customers
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (customerToUpdate == null)
            {
                logger.LogWarning("Customer with UserId {UserId} not found for updating individual data.", userId);
                return null; // or throw an exception based on your error handling strategy
            }

            customerToUpdate.PartyCode = response.PartyCode;
            customerToUpdate.PartyID = response.PartyId;
            customerToUpdate.DIANumber = response.DIANumber;
            customerToUpdate.CoreSyncStatus = string.IsNullOrEmpty(response.DIANumber) ? CoreSyncStatus.Pending : CoreSyncStatus.Succeeded;

            context.Update(customerToUpdate);
            await context.SaveChangesAsync();

            return new IndividualResponseModel
            {
                PartyCode = response.PartyCode,
                PartyId = response.PartyId,
                DIANumber = response.DIANumber
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process CreateIndividualAsync job for {FullName}", requestModel.FullName);
            throw; // Important: rethrow so Hangfire registers the failure and triggers retries
        }
    }
}
