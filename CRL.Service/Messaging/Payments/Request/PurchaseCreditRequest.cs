using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Payments;
using CRL.Infrastructure.Messaging;

using CRL.Model.ModelViews.Payments;

namespace CRL.Service.Messaging.Payments.Request
{
    public class MakePaymentRequest:RequestBase
    {
        public PaymentView PaymentView { get; set; }
        public string UrlLink { get; set; }
       
    }

    public class CreatePostpaidBatchRequest : RequestBase
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BatchType { get; set; }
        public string Batchcomment { get; set; }

    }
        public class ReconcileBatchedPostpaidTransactionsRequest : RequestBase
        {
            public int BatchId { get; set; }
            public int[] RepresentativeInstitution { get; set; }
            public string ReconcileComment { get; set; }

        }

        public class GetReconcileRequest : RequestBase
        {
            public int? BatchId { get; set; }
            public  bool IncludeReconciledTransactions {get;set;}

        }

        public class GetConfirmClientsInPostpaidBatchRequest : RequestBase
        {
            public int? BatchId { get; set; }
            
        }
    
}
