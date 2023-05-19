using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Payments;
using System.ComponentModel.DataAnnotations;

namespace CRL.Model.ModelViews.Payments
{

    public class ExpenditureByTransactionView
    {
        public string ServiceFeeType { get; set; }
        public decimal Fee { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
    public class CreditActivityView
    {
        public int? Id{get;set;}
        public DateTime EntryDate { get; set; }
        public decimal Amount { get; set; }
        public string CreditOrDebit { get; set; }
        public string ServiceFeeType { get; set; }
        public string PrepaidOrPostpaid { get; set; }
        public ServiceFees? ServiceFeeTypeId { get; set; }
        public string AccountTransactionType { get; set; }
        public AccountTransactionCategory AccountTypeTransactionId { get; set; }
        public decimal NewPrepaidBalanceAfterTransaction { get; set; }
        public decimal NewPostpaidBalanceAfterTransaction { get; set; }
        public string Narration { get; set; }
        public string NameOfLegalEntity { get; set; }
        public string NameOfRepresnetative { get; set; }
        public int? AccountReconcileId { get; set; }
        public int? SettlementAccountTransactionId { get; set; }
        public int? PostPaidRepresentativeMembershipId { get; set; }
        public int? MembershipId { get; set; }
        public int? AccountBatchId { get; set; }    
        public string NameOfUser { get; set; }
        public string UserLoginId { get; set; }
        public string UserUnit { get; set; }
        public string AffectedClient { get; set; }
        public bool IsReconciled { get; set; }
        public bool isSelected { get; set; }

    }

    public class SummarisedCreditActivityView
    {
        public string ClientName { get; set; }
        public string RepresentativeClientName { get; set; }
        public int ClientAccountType { get; set; }
        public decimal Amount { get; set; }

    }

        public class SummarisedClientExpenditureView
    {
            public int Year { get; set; }
            public int MonthNum { get; set; }
        public string ClientName { get; set; }
        public string ClientType { get; set; }
        public decimal Amount { get; set; }

    }

    public class SummarisedCreditActivityByRepresentativeBank
    {
        public string Client { get; set; }       
        public decimal Amount { get; set; }

    }

    public class ClientAmountSelectionView : ClientAmountView
    {
        
        public bool isSelected { get; set; }
    }

    /// <summary>
    /// Represents a list of clients in a batch postpaid whose settlement are yet to be confirmed
    /// </summary>
    public class ClientAmountView
    {
        public int MembershipId { get; set; }
        public string Client { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public decimal Amount { get; set; }
    }

    public class ClientAmountWithRepBankView : ClientAmountView
    {
        public string RepresentativeBank { get; set; }
        public int? RepresentativeBankMembershipID { get; set; }

    }

    /// <summary>
    /// Represents a list of clients in a batch postpaid whose settlement are yet to be confirmed
    /// </summary>
    public class ClientSettlementSummaryView : ClientAmountView
    {      
      
        public DateTime SettledDate { get; set; }
        public int ReconcilationNumber { get; set; }
    }



   

   
}
