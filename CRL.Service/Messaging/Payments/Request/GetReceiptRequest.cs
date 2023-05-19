using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Payments.Request
{
    public class GetReceiptRequest:RequestBase
    {
        public int Id { get; set; }
    }
}
