using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;
using CRL.Service.Views.Payments;

using CRL.Model.ModelViews.Payments;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _SubPostpaidClientsJqGridViewModel : BaseDetailViewModel
    {
        public CreditActivityView[] PostpaidSubClientsCreditActivites { get; set; }
        public int? BatchNo { get; set; }
    }
}