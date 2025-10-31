using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.Log;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Business.Common.Sms;
public interface ISmsLogService
{
    Task LogSmsAsync(SmsLog request);
}
