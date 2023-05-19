using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Model.Messaging
{
    public class ViewAuditDetailsRequest : RequestBase
    {
        public int AuditId { get; set; }
    }
}
