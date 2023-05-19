using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class SubmitInterSwitchDetailsResponse:ResponseBase
    {
        public string TransactionRefNo { get; set; }
        public bool IsTransactionUpdate { get; set; }
    }
}
