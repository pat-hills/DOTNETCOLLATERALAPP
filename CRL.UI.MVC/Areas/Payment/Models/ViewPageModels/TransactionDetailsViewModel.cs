using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class TransactionDetailsViewModel
    {
        public TransactionDetailsViewModel()
        {
            _TransactionDetailsPartialViewModel = new _TransactionDetailsPartialViewModel();
        }

        public string TransRefNo { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsResult { get; set; }
        public bool IsTempResponse { get; set; }
        public string QueryTransaction { get; set; }
        public string Search { get; set; }
        public _TransactionDetailsPartialViewModel _TransactionDetailsPartialViewModel { get; set; }
    }
}