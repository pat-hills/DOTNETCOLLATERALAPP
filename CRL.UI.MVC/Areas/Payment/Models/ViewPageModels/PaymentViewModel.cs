using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Payments;
using CRL.UI.MVC.Common;

using CRL.Model.ModelViews.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class PaymentViewModel : BaseDetailViewModel
    {
        public PaymentView PaymentView { get; set; }


    }
}