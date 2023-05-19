using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views;
using CRL.Service.Views.Payments;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class InterSwitchUserViewModel
    {
        public InterSwitchUserViewModel()
        {
            InterSwitchUserView = new InterSwitchTransactionView();
        }
        public InterSwitchTransactionView InterSwitchUserView { get; set; }
    }
}