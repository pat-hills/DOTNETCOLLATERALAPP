using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Payments;

using CRL.Model.ModelViews.Payments;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Messaging
{
    public class ViewPaypointPaymentsResponse : ResponseBase
    {
        public ICollection<PaypointPaymentView> PaypointPaymentView { get; set; }
        public int NumRecords { get; set; }

    }
    public class ViewPaymentsResponse:ResponseBase
    {
        public ICollection<PaymentView> PaymentViews { get; set; }
        public int NumRecords { get; set; }
     
    }
    public class ViewSummaryClientExpendituresResponse : ResponseBase
    {
        public ICollection<SummarisedClientExpenditureView> SummarisedClientExpenditures { get; set; }    
        public int NumRecords { get; set; }

    }
    public class ViewClientExpenditureByTransactionResponse : ResponseBase
    {
        public ICollection<ExpenditureByTransactionView> ClientExpenditureByTransaction { get; set; }
        public string ClientName { get; set; }
        public int NumRecords { get; set; }

    }

    

    public class ViewSummaryPostpaidTransactionsResponse : ResponseBase
    {
        public ICollection<SummarisedCreditActivityView> SummarisedCreditActivities { get; set; }
        public ICollection<SummarisedCreditActivityByRepresentativeBank> SummarisedCreditActivitiesRepresentativeBank { get; set; }
        public ICollection<CreditActivityView> PostpaidTransactionDetails { get; set; }
        public int NumRecords { get; set; }
        public AccountBatchView AccountBatchView { get; set; }
     
    }

    public class ViewAccountBatchesResponse : ResponseBase
    {
        public ICollection<AccountBatchView> AccountBatches { get; set; }
        public int NumRecords { get; set; }
    }

    public class ViewReconcilationsResponse : ResponseBase
    {
        public ICollection<AccountReconcilation> AccountReconcilations { get; set; }
        public int NumRecords { get; set; }
    }
    
}
