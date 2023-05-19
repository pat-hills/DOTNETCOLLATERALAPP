using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Response
{
    public class GetDataForSelectClientResponse:ResponseBase
    {
        public ICollection<LookUpView> LegalEntityClients { get; set; }
        public ICollection<LookUpView> IndividualClients { get; set; }
    }
}
