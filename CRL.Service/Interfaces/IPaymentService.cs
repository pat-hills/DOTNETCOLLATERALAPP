using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.Common.Request;

namespace CRL.Service.Interfaces
{
    public interface IPaymentService
    {
        GetDataForSelectClientResponse GetDataForSelectClient(int? membershipId = null);
        GetDataForViewPaymentResponse GetView(GetDataForViewPaymentRequest request);
        MakePaymentResponse MakePayment(MakePaymentRequest request);
        GetClientSummaryViewResponse GetClientSummaryView(GetClientSummaryViewRequest request);
        ViewPaymentsResponse ViewPayments(ViewPaymentsRequest request);
        GetReceiptResponse GetReceiptRequest(GetReceiptRequest request);
        MakePaymentResponse MakeReversalPayment(RequestBase request);
        ViewAccountBatchesResponse ViewAccountBatches(ViewAccountBatchesRequest request);
        ViewSummaryPostpaidTransactionsResponse ViewSummaryPostpaidTransactions(ViewSummaryPostpaidTransactionsRequest request);
        ViewSummaryPostpaidTransactionsResponse ViewSummaryPostpaidByBankTransaction(ViewSummaryPostpaidTransactionsRequest request);
        ViewSummaryPostpaidTransactionsResponse ViewPostpaidTransactionDetails(ViewSummaryPostpaidTransactionsRequest request);
        ViewReconcilationsResponse ViewReconcilations(ViewReconcilationsRequest request);
        CreatePostpaidBatchResponse CreatePostpaidBatch(CreatePostpaidBatchRequest request);
        CreatePostpaidBatchResponse ReconcileBatchedPostpaidTransactions(ReconcileBatchedPostpaidTransactionsRequest request);
        GetReconcileResponse GetBatchDetails(GetReconcileRequest request);
        SubmitInterSwitchDetailsResponse CreateEditInterSwitchUserDetails(SubmitInterSwitchDetailsRequest request);
        GetInterSwitchDetailsResponse GetInterSwitchDetails(GetInterSwitchDetailsRequest request);
        InterSwitchApiQueryResponse GetInterSwitchApiQueryResult(InterSwitchApiQueryRequest request);
        GetInterSwitchViewPaymentDetailsResponse ViewSummaryInterSwitchTransaction(GetInterSwitchViewPaymentDetailsRequest request);
        GetAllInterSwitchTransactionsResponse GetInterSwitchTransactions(GetAllInterSwitchTransactionsRequest request);
        GetAllDirectPayTransactionsResponse GetDirectPayTransactions(GetAllDirectPayTransactionsRequest request);
        MakeDirectPayPaymentResponse MakePaymentDirectPayPayment(MakeDirectPayPaymentRequest request);
        MakeDirectPayReversalResponse MakeDirectPayReversal(MakeDirectPayReversalRequest request);
        GetFileAttachmentResponse GenerateBatchDetailReport(RequestBase request);
        GetConfirmClientsInPostpaidBatchResponse GetConfirmClientsInPostpaidBatch(RequestBase request);
        ResponseBase UndoBatch(DeleteItemRequest request);
        CreateDirectPayDetailsResponse CreateDirectPayDetails(CreateDirectPayDetailsRequest request);
        GetPaymentVoucherResponse GetPaymentVoucher(GetPaymentVoucherRequest request);
        GetDirectPayTransactionDetailsResponse GetDirectPayTransactionDetails(GetDirectPayTransactionDetailsRequest request);
    }
}
