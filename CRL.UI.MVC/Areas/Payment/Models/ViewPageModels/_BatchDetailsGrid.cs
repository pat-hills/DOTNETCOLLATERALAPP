using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;
using CRL.Service.Views.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _BatchDetailsGrid
    {
        public ClientAmountWithRepBankView[] BatchDetails { get; set; }
        public int BatchId { get; set; }
    }
}