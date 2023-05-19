using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class GetInterSwitchViewPaymentDetailsResponse : ResponseBase
    {
        public TransactionResponseView TransactionResponseView { get; set; }
        public string Message { get; set; }
        public bool IsAuthorized { get; set; }
        public string TransactionRefNo { get; set; }
        public string Payee { get; set; }
        public string SecurityCode { get; set; }
        public int? PaymentId { get; set; }
        public bool HasEmail { get; set; }
    }
}
