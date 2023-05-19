using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class InterSwitchTransactionIndexViewModel:BaseDetailViewModel
    {
        public InterSwitchTransactionIndexViewModel()
        {
            _InterSwitchTransactionsJqGridViewModel = new _InterSwitchTransactionsJqGridViewModel();
            
        }
        public _InterSwitchTransactionsJqGridViewModel _InterSwitchTransactionsJqGridViewModel { get; set; }
    }    
    
}