using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Configuration;
using CRL.Model.FS;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews;
using CRL.Model.Search;
using CRL.Model.Memberships;

namespace CRL.Model.Payments
{
    public enum AccountTransactionCategory {Payment=1, ReversalPayment=2,CreditTransaction =3, SettlementPayment=4}
    public enum AccountSourceCategory { Prepaid = 1, Postpaid = 2 }
    public enum CreditOrDebit { Credit = 1, Debit = 2 }
    public class AccountTransaction : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public decimal Amount { get; set; }
        public CreditOrDebit CreditOrDebit { get; set; }
        public AccountTransactionCategory AccountTypeTransactionId { get; set; }
        public AccountSourceCategory AccountSourceTypeId { get; set; }
        public LKAccountTransactionCategory AccountTypeTransaction { get; set; }
        public LKServiceFeeCategory ServiceFeeType { get; set; }
        public System.Nullable<ServiceFees> ServiceFeeTypeId { get; set; }          
        public decimal NewPrepaidBalanceAfterTransaction { get; set; }
        public decimal NewPostpaidBalanceAfterTransaction { get; set; }
        /// <summary>
        /// Represents the reconcilation class that this postpaid transaction has been created for
        /// </summary>
        public AccountReconcilation AccountReconcile { get; set; }
        /// <summary>
        ///  Represents the id of the reconcilattion record that this postpaid transaction has been created for
        /// </summary>
        public int? AccountReconcileId { get; set; }
        /// <summary>
        /// This represents the settlement id accounttransaction for which this postpaid transaction has been settled by
        /// </summary>
        public int? SettlementAccountTransactionId { get; set; }
        /// <summary>
        /// This represents the settlement accounttransaction for which this postpaid transaction has been settled by
        /// </summary>
        public AccountTransaction SettlementAccountTransaction{ get; set; }
        public string Narration { get; set; }
        /// <summary>
        /// Represents the representative membership for which a report will be generated for postpaid debiting
        /// </summary>
        public Membership  PostPaidRepresentativeMembership {get;set;}
        public int? PostPaidRepresentativeMembershipId { get; set; }
        public AccountBatch AccountBatch { get; set; }
        public int? AccountBatchId { get; set; }        
        public Membership Membership { get; set; }
        public int? MembershipId { get; set; }
        


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentAccountTransaction : AccountTransaction
    {
        public Payment Payment { get; set; }
        public int PaymentId { get; set; }
    }
    public class CreateFSAccountTransaction : AccountTransaction
    {
        public FinancialStatement FinanacialStatement { get; set; }
        public int FinancialStatementId { get; set; }
    }
    public class ActivityAccountTransaction : AccountTransaction
    {
        public FinancialStatementActivity FinancialStatementActivity { get; set; }
        public FinancialStatementActivity FinancialStatementActivityId { get; set; }
    }
    public class SearchAccountTransaction : AccountTransaction
    {
        public  SearchFinancialStatement   SearchFinancialStatement { get; set; }
        public int SearchFinancialStatementId { get; set; }
    }

     [Serializable]
    public partial class LKAccountTransactionCategory : EntityBase<AccountTransactionCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string FinancialStatementCategoryName { get; set; }
      


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


     [Serializable]
     public partial class AccountBatch : AuditedEntityBaseModel<int>, IAggregateRoot
     {
         [MaxLength(50)]
         public string Name { get; set; }
         public DateTime? PeriodStartDate { get; set; }
         public DateTime? PeriodEndDate { get; set; }
         [MaxLength(255)]
         public string Comment { get; set; }
         public bool isReconciled { get; set; }
         public bool ConfirmSubPostpaidAccount { get; set; }
         public int InstitutionId { get; set; }
         public virtual Institution Institution { get; set; }
         
         protected override void CheckForBrokenRules()
         {
             throw new NotImplementedException();
         }
     }

     public partial class AccountReconcilation : AuditedEntityBaseModel<int>, IAggregateRoot
     {
         [MaxLength(50)]
         public string Name { get; set; }
         [MaxLength(255)]
         public string Comment { get; set; }
         
         protected override void CheckForBrokenRules()
         {
             throw new NotImplementedException();
         }
     }

    
}
