using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class GetBVCRequest : RequestBase
    {
        public string code { get; set; }
    }
}
