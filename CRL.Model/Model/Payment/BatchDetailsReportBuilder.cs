using CRL.Model.ModelViews.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Payments
{
    public abstract class BatchDetailsReportBuilder
    {


        protected ICollection <ClientAmountView> dsPostpaidClientTotalExpenditure { get; set; }
        protected ICollection<ClientSettlementSummaryView> dsSettledPayments { get; set; }
        protected ICollection<ExpenditureByTransactionView> dsClientExpenditureByTransaction { get; set; }
        protected ICollection<AccountBatchView> dsAccountBatchView { get; set; }



        public BatchDetailsReportBuilder()
        {

        }

        public void BuildBatchDetailReport(ICollection<ClientAmountView> _PostpaidClientTotalExpenditure,
                                           ICollection<ClientSettlementSummaryView> _SettledPayments,
            ICollection<ExpenditureByTransactionView> _ClientExpenditureByTransaction,
            AccountBatchView _AccountBatchView

            )
        {

            dsPostpaidClientTotalExpenditure = _PostpaidClientTotalExpenditure;
            dsSettledPayments = _SettledPayments;
            dsClientExpenditureByTransaction = _ClientExpenditureByTransaction;
            dsAccountBatchView = new List<AccountBatchView>();
            dsAccountBatchView.Add(_AccountBatchView);
        }

        public abstract Byte[] GenerateVerificationReport();

    }

}
