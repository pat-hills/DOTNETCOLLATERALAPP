using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _DirectPayTransactionsJqGridViewModel : _InterSwitchJqGrid
    {
        public string PaymentVoucherCode { get; set; }
    }


    
    
}