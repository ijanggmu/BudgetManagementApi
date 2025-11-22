using System.Threading.Tasks;

namespace Business.Common.TenantDomain;

public interface IQuotationNumberGenerator
{
    Task<string> NextAsync();
}


