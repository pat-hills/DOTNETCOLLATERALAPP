using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Request
{
    public class GetAllInterSwitchTransactionsRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string TransactionRefNo { get; set; }
        public int TransactionLogType { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
