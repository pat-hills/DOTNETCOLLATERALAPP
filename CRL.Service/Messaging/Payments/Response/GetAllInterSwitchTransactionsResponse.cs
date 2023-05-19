using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class GetAllInterSwitchTransactionsResponse:ResponseBase
    {
        public ICollection<InterSwitchTransactionView> InterSwitchTransactionViews { get; set; }
        public int NumRecords { get; set; }
    }
}
