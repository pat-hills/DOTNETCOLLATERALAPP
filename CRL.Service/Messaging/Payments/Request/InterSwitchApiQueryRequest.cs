using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Request
{
    public class InterSwitchApiQueryRequest : RequestBase
    {
        public string TransactionRefNo { get; set; }
        public string UrlLink { get; set; }
        public bool IsReQuery { get; set; }
    }
}
