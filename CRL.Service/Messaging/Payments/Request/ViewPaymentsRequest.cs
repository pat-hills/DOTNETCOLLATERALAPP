using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Views.Payments;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Request
{
    public class ViewPaypointPaymentsRequest : PaginatedRequest
    {
        public DateRange PaymentDate { get; set; }
    }

    public class ViewPaymentsRequest:PaginatedRequest
    {
        public string PaymentNo { get; set; }
        public DateRange PaymentDate { get; set; }
        public string Payee { get; set; }
        public string PublicUserEmail { get; set; }       
        public int? MembershipId { get; set; }
        public int? PaymentTypeId { get; set; }
         
           public int? PaymentSourceId { get; set; }
        public string ClientName { get; set; }
        public int ShowType { get; set; }
        public string PaypointUserName { get; set; }
        public string PaypointName { get; set; }
        public string TransactionNo { get; set; }
        public short? IsPublicUser { get; set; }
       
        
    }
}
