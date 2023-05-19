using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Request
{
    public class MakeDirectPayPaymentRequest:RequestBase
    {
        public DirectPayRequestView DirectPayRequestView { get; set; }
    }

    public class MakeDirectPayReversalRequest : RequestBase
    {
        public DirectPayReversalRequestView DirectPayReversalRequestView { get; set; }
    }
}
