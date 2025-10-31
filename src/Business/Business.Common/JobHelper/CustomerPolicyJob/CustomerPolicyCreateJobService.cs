using Business.Common.JobHelper.CustomerPolicyJob;
using Business.Common.Policy;
using Data.Context;
using Data.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Common.Policy.Policy;
using Models.WebApi.Policy.Motor;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.HangFireJob.CustomerProfileJob;

public class CustomerPolicyCreateJobService(ApplicationDataContext context, IPolicyService policyService) : ICustomerPolicyCreateJobService
{
    public async Task<Result<MessageResponseModel>> CreatePolicyAsync(InsuranceType insuranceType, string transactionId, CancellationToken ct)
    {

        if (insuranceType == InsuranceType.ThirdPartyBike)
        {
            var paymentTransction = await context.PaymentTransactions
            .Include(x => x.PolicyDraft)
            .ThenInclude(p => p.Motor)
            .Include(x => x.PolicyDraft)
            .ThenInclude(x => x.Customer)
            .Where(x => x.Id == transactionId && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken: ct);

            if (paymentTransction == null)
            {
                return null;
            }

            var policyDraft = paymentTransction.PolicyDraft;
            if (insuranceType == InsuranceType.ThirdPartyBike)
            {
                var createMotorPolicy = new MotorPolicyRequestModel
                {
                    MotorPartial = new MotorPartialModel
                    {
                        IsThirdParty = policyDraft.Motor.IsThirdParty,
                        IsComprehensive = policyDraft.Motor.IsComprehensive,
                        Type = policyDraft.Motor.Type,
                        ManufactureYear = policyDraft.Motor.ManufactureYear,
                        ManufactureCompany = policyDraft.Motor.ManufactureCompany,
                        Model = policyDraft.Motor.Model,
                        PurchasedNewOld = policyDraft.Motor.PurchasedNewOld,
                        DateOfPurchase = policyDraft.Motor.DateOfPurchase,
                        ChasisNumber = policyDraft.Motor.ChasisNumber,
                        EngineNumber = policyDraft.Motor.EngineNumber,
                        RegistrationNumber = policyDraft.Motor.RegistrationNumber,
                        CubicCapacity = policyDraft.Motor.CubicCapacity,
                        Days = policyDraft.Motor.Days.ToString(),
                        YearsFromRegistrationDateYears = policyDraft.Motor.YearsFromRegistrationDateYears.ToString(),
                        YearsFromRegistrationDateYearsBS = policyDraft.Motor.YearsFromRegistrationDateYearsBS,
                        CurrentMarketPrice = policyDraft.Motor.CurrentMarketPrice.ToString(),
                        AgeOfVehicle = policyDraft.Motor.AgeOfVehicle,
                        RiotStrike = policyDraft.Motor.RiotStrike,
                        VechileType = policyDraft.Motor.VehicleType,
                        KilloWatt = policyDraft.Motor.KilloWatt
                    },

                    EffectiveDate = policyDraft.EffectiveDate,
                    ExpiryDate = policyDraft.ExpiryDate,
                    ProposedDate = policyDraft.CreatedOn,
                    NetPremium = policyDraft.NetPremium,
                    PayablePremium = policyDraft.PayableAmount,
                    DraftNumber = policyDraft.DraftNo,
                    Gateway = paymentTransction.PaymentGateway
                };
                var policy = await policyService.CreateMotorcyclePolicy(createMotorPolicy, paymentTransction.PolicyDraft.Customer.UserId, ct);


                return policy;
            }

        }
        if (insuranceType == InsuranceType.InternationalTravel)
        {
            var paymentTransction = await context.PaymentTransactions
            .Include(x => x.PolicyDraft)
            .ThenInclude(p => p.ITI)
            .Include(x => x.PolicyDraft)
            .ThenInclude(x => x.Customer)
            .Where(x => x.Id == transactionId && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken: ct);

            if (paymentTransction == null)
            {
                return null;
            }
            var policy = await policyService.CreateITIPolicy(paymentTransction.Id, ct);
            return policy;
        }
        return null;

    }
}
