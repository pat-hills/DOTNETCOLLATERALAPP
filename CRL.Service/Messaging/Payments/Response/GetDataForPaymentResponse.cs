using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Payments;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class GetDataForViewPaymentResponse:ResponseBase
    {
        public PaymentView PaymentView { get; set; }
    }
}
