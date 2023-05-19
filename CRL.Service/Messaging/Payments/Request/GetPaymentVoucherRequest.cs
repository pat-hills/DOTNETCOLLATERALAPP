using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Request
{
    public class GetPaymentVoucherRequest : RequestBase
    {
        public string PaymentVoucherCode { get; set; }
    }
}
