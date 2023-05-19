using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class DirectPayTransactionIndexViewModel : BaseDetailViewModel
    {
        public DirectPayTransactionIndexViewModel()
        {
            _DirectPayTransactionsJqGridViewModel = new _DirectPayTransactionsJqGridViewModel();
        }

        public _DirectPayTransactionsJqGridViewModel _DirectPayTransactionsJqGridViewModel { get; set; }
    }
}