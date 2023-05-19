using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.ModelViews.Payments;


namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class SubPostpaidClientsViewModel
    {
        public SubPostpaidClientsViewModel()
        {
            _ViewBatchDetailsPartailViewModel = new _ViewBatchDetailsPartailViewModel();
        }
        public _ViewBatchDetailsPartailViewModel _ViewBatchDetailsPartailViewModel { get; set; }
        public int? BatchNo { get; set; }
        public ClientAmountSelectionView[] PostpaidSubClientsCreditActivites { get; set; }
    }
}