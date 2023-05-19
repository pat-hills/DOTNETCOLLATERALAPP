using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class InterSwitchPaymentSummaryViewModel
    {
        public InterSwitchPaymentSummaryViewModel()
        {
            _TransactionDetailsPartialViewModel = new _TransactionDetailsPartialViewModel();
        }
        public _TransactionDetailsPartialViewModel _TransactionDetailsPartialViewModel { get; set; }
    }
}