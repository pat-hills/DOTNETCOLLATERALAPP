using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;
using CRL.Model.Payments;
using CRL.Model.ModelViews.Payments;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Payments.IRepository
{


    public interface IPaymentRepository : IWriteRepository<Payment, int>
    {
        int CreateBatch(DateTime? StartDate, DateTime? EndDate, int? BatchType, string Batchcomment, int ExecutedBy, string UserIP, string RequestUrl);
        int OwnerReconcilePayment(int BatchId, string ReconcileComment, int ExecutedBy, string UserIP, string RequestUrl);
        int UndoBatch(int BatchId, int ExecutedBy, string UserIP, string RequestUrl);
        int ConfirmSubPostpaidClients(int BatchId, int[] RepresentativeInstitution, string ReconcileComment, int ExecutedBy, string UserIP, string RequestUrl);
        Payment GetWebPayPayment(int InterSwitchTransactionId);
        Payment GetDirectPayPayment(int InterSwitchTransactionId);
        Payment GetPaymentBySecurityCode(int SecurityCodeId);
        PaymentView GetPaymentView(int Id);
    }
    public interface IAccountTransactionRepository : IWriteRepository<AccountTransaction, int>
    {
        ICollection<ClientAmountWithRepBankView> ViewClientExpenditureSummary(int BatchId);
        ViewCreditActivitiesResponse CustomViewCreditActivities(ViewCreditActivitiesRequest request);
        ICollection<ClientSettlementSummaryView> ViewClientSettlementSummary(int BatchId);
        ICollection<ExpenditureByTransactionView> ViewClientExpendituresByTransaction(int BatchId);
        ICollection<ClientAmountView> GetMyPostpaidClientExpenditures(int BatchId);
        ICollection<ClientAmountSelectionView> GetClientForConfirmationInPostpaidBatch(int BatchId);
    }
    public interface IPublicUserSecurityCodeRepository : IWriteRepository<PublicUserSecurityCode, int>
    {

    }
    public interface IFeeRepository : IWriteRepository<Fee, int>
    {

    }
    public interface IAccountBatchRepository : IWriteRepository<AccountBatch, int>
    {

        ViewAccountBatchesResponse GetAccountBatches(ViewAccountBatchesRequest request);
        AccountBatchView GetAccountBatchView(int BatchID);
    }
    public interface IInterSwitchRepository : IWriteRepository<InterSwitchWebPayTransaction, int>
    {
        InterSwitchWebPayTransaction GetTransactionByRefNo(string tranRefNo);
        bool IsGeneratedReferenceCode(string referenceCode);
    }

    public interface IInterSwitchDirectPayRepository : IWriteRepository<InterSwitchDirectPayTransaction, int>
    {
        InterSwitchDirectPayTransaction GetTransactionBySeqNo(string sequenceNo);
        InterSwitchDirectPayTransaction GetTransactionByDestAcc(string destAcc);
        bool IsReversed(string sequenceNo);
        bool IsGeneratedPaymentCode(string paymentCode);
    }
}
