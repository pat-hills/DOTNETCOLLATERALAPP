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
    public class MakePaymentResponse:ResponseBase
    {
        public int Id { get; set; }
        //public PaymentView PaymentView { get; set; }
       
    }

    public class CreatePostpaidBatchResponse: ResponseBase
    {
        public int BatchNo { get; set; }
    }

    public class GetReconcileResponse : ResponseBase
    {
        public ICollection<ClientAmountWithRepBankView> ClientAmountWithRepBankView { get; set; }
        public AccountBatchView AccountBatchView {get;set;}
        
        public ICollection<ClientSettlementSummaryView> SettledClientsInBatchViews { get; set; }

    }

    public class GetConfirmClientsInPostpaidBatchResponse: ResponseBase
    {
        public ICollection<ClientAmountSelectionView> ClientsInPostpaidBatchToConfirmView { get; set; }
        public AccountBatchView AccountBatchView { get; set; }
        public decimal? TotalBatchAmount { get; set; }
        public ICollection<ClientSettlementSummaryView> SettledClientsInBatchViews { get; set; }

    }
}
