using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class DirectPayTransactionDetailsViewModel
    {
        public DirectPayTransactionDetailsViewModel()
        {
            InterSwitchDetailsView = new InterSwitchDetailsView();
        }
        public InterSwitchDetailsView InterSwitchDetailsView { get; set; }
    }
}