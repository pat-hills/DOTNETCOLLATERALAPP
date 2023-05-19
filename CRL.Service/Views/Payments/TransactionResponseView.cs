using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Service.Views.Payments
{
    [XmlRoot("TransactionQueryResponse")]
    public class TransactionResponseView
    {
        [XmlElement("BankCode")]
        public string BankCode { get; set; }
        [XmlElement("ResponseCode")]
        public string ResponseCode { get; set; }
        [XmlElement("ResponseDescription")]
        public string ResponseDescription { get; set; }
        [XmlElement("Amount")]
        public decimal Amount { get; set; }
        [XmlElement("CardNumber")]
        public string CardNumber { get; set; }
        [XmlElement("MerchantReference")]
        public string MerchantReference { get; set; }
        [XmlElement("SplitAccounts")]
        public string SplitAccounts { get; set; }
        [XmlElement("PaymentReference")]
        public string PaymentReference { get; set; }
        [XmlElement("RetrievalReferenceNumber")]
        public string RetrievalReferenceNumber { get; set; }
        [XmlElement("TransactionDate")]
        public DateTime TransactionDate { get; set; }
    }

   
    public class TransactionResponseView2
    {
      
        public string BankCode { get; set; } 
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string MerchantReference { get; set; }
        public List<string> SplitAccounts { get; set; }
        public string PaymentReference { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
