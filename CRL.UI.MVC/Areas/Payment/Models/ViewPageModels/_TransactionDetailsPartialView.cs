using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _TransactionDetailsPartialViewModel
    {
        public _TransactionDetailsPartialViewModel()
        {
          TransactionResponseView = new TransactionResponseView();  
        }
        public TransactionResponseView TransactionResponseView { get; set; }
        public bool IsPageLoad { get; set; }
        public string Payee { get; set; }
        public string Message { get; set; }
        public string TransactionRefNo { get; set; }
        public string SecurityCode { get; set; }
        public int? PaymentId { get; set; }
        public bool HasEmail { get; set; }
    }
}