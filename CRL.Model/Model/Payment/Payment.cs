using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Memberships;

namespace CRL.Model.Payments
{
    public enum PaymentSource {Paypoint=1, Settlement=2,InterSwitchWebPay=3,InterswitchDirectPay=4}
    public enum PaymentType {Normal=1, Reversal=2 }
    public partial class Payment : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public decimal Amount { get; set; }
        [MaxLength(17)]
        public string PaymentNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public virtual Membership  Membership { get; set; }
        public int? MembershipId { get; set; }
        public virtual PublicUserSecurityCode PublicUserSecurityCode { get; set; }
        public virtual InterSwitchWebPayTransaction InterSwitchWebPayTransaction { get; set; }
        public virtual InterSwitchDirectPayTransaction InterSwitchDirectPayTransaction { get; set; }

        public int? PublicUserSecurityCodeId { get; set; }
        public int? InterSwitchWebPayTransactionId { get; set; }
        public int? InterSwitchDirectPayTransactionId { get; set; }        
        public string T24TransactionNo { get; set; }
        public string Payee { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentSource PaymentSource { get; set; }
        public int? ReversedPaymentId { get; set; }
        public virtual Payment ReversedPayment { get; set; }
        public bool PendingConfirmation { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class InterSwitchWebPayTransaction : AuditedEntityBaseModel<int>, IAggregateRoot
    {

        public int ProductId { get; set; }
        public string BVN { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string CurrencyCode { get; set; }
        public string SiteRedirectUrl { get; set; }
        public string TransactionReference { get; set; }
        public string Hash { get; set; }
        public string PayItemId { get; set; }
        public string SiteName { get; set; }
        public string CustId { get; set; }
        public string CustIdDesc { get; set; }
        public string CustName { get; set; }
        public string CustEmail { get; set; }
        public string CustPhone { get; set; }
        public string CustNameDesc { get; set; }
        public string PayItemName { get; set; }
        public string LocalDteTime { get; set; }
        public bool ResponseCode { get; set; }
        public string TopUpCode { get; set; }
        public bool IsTopUpPayment { get; set; }
        public bool IsAmountInput { get; set; }
        public decimal? BalanceBeforeTopUp { get; set; }
        public InterSwitchWebPayTransactionQueryResponse InterSwitchWebPayTransactionQueryResponse { get; set; }
        public int? InterSwitchWebPayTransactionQueryResponseId { get; set; }

        public void GenerateHash()
        {
            throw new NotImplementedException(); 
        }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
   }

    public partial class InterSwitchWebPayTransactionQueryResponse : AuditedEntityBaseModel<int>, IAggregateRoot
    {

        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string MerchentRef { get; set; }
        public string PaymentRef { get; set; }
        public string RetreivalRef { get; set; }
        public string SplitAcconts { get; set; }
        public DateTime TransactionDate { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class InterSwitchDirectPayTransaction : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string CustName { get; set; }
        public string CustEmail { get; set; }
        public string BVN { get; set; }
        public string CustPhone { get; set; }
        public string DestAccount { get; set; }
        public decimal Amount { get; set; }
        public string Msg { get; set; }
        public string SequenceNo { get; set; }
        public string DealerNo { get; set; }
        public string Passsword { get; set; }
        public string ProductCode { get; set; }
        public string Pin { get; set; }
        public string TopUpCode { get; set; }
        public bool IsTopUpPayment { get; set; }
        public bool IsReversal { get; set; }
        public string RequestTimeStamp { get; set; }
        public string ReceiptNo { get; set; }
        public decimal? BalanceBeforeTopUp { get; set; }
        public bool? IsProcessed { get; set; }
        public InterSwitchDirectPayTransactionQueryResponse InterSwitchDirectPayTransactionQueryResponse { get; set; }
        public int? InterSwitchDirectPayTransactionQueryResponseId { get; set; }

        public void GenerateHash()
        {
            throw new NotImplementedException();
        }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class InterSwitchDirectPayTransactionQueryResponse : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string StatusId { get; set; }
        public string StatusMessage { get; set; }
        public string TxRefId { get; set; }
        public string ValueToken { get; set; }
        public string DealerNo { get; set; }
        public string DestAccount { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public decimal Amount { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class PublicUserSecurityCode : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string PublicUserEmail { get; set; }
        public string SecurityCode { get; set; }
        public decimal Balance { get; set; }
      
     
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

  
   
    
}
