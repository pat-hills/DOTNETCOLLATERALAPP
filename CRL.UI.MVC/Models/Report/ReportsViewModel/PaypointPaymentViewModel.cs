using CRL.Model.ModelViews.Payments;

using CRL.Service.Views.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class PaypointPaymentViewModel : ReportBaseViewModel
    {
        public ICollection<PaypointPaymentView> PaypointPaymentView { get; set; }
    }
}