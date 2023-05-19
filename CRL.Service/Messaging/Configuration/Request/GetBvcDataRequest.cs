using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Configuration.Request
{
    public class GetBvcDataRequest : PaginatedRequest
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}
