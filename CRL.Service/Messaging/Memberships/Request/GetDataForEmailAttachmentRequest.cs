using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class GetDataForEmailAttachmentRequest : RequestBase
    {
        public int Id { get; set; }
    }
}
