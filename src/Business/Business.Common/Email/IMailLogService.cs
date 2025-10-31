using System.Threading.Tasks;
using Data.Entities.EmailLogEntity;
namespace Business.Common.Email;

public interface IMailLogService
{
    Task LogEmailAsync(EmailLog emailLog);
}
