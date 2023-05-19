using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    

    public class PaymentIndexViewModel : BaseDetailViewModel
    {
        public PaymentIndexViewModel()
            : base()
        {
            _PaymentJqGridViewModel = new _PaymentJqGridViewModel();
        }

        public _PaymentJqGridViewModel _PaymentJqGridViewModel { get; set; }

    }
}