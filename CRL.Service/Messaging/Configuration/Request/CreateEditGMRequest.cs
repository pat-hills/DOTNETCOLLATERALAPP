using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Configuration.Request
{
    public class CreateGMRequest : RequestBase
    {
        public bool IsEditOrView { get; set; }
        public int Mode { get; set; }
    }
}
