using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Messaging;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class MakeDirectPayPaymentResponse:ResponseBase
    {
        public DirectPayResponseView DirectPayResponseView { get; set; }
    }

    public class MakeDirectPayReversalResponse : ResponseBase 
    {
        public DirectPayReversalResponseView DirectPayReversalResponseView { get; set; }
    }
}
