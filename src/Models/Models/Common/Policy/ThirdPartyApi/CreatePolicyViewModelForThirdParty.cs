using Models.Common.Policy.Policy;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class CreatePolicyViewModelForThirdParty : CreatePolicyViewModel
    {
        public override string PartyId { get => base.PartyId; set => base.PartyId = value; }

    }
}
