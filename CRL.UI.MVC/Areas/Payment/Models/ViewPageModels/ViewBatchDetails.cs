using System.ComponentModel.DataAnnotations;
using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;
using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class ViewBatchDetails : BaseDetailViewModel
    {
        public ViewBatchDetails()
        {
            _ViewBatchDetailsPartailViewModel = new _ViewBatchDetailsPartailViewModel();
        }

        public _ViewBatchDetailsPartailViewModel _ViewBatchDetailsPartailViewModel { get; set; }
        public ClientSettlementSummaryView[] SettledClientsInBatchViews { get; set; }
        public ClientAmountWithRepBankView[] ClientAmountWithRepBankViews { get; set; }
 
        public _BatchDetailsGrid _BatchDetailsGrid { get; set; }
        public _ListOfTransactionsJqGrid _ListOfTransactionsJqGrid { get; set; }
    }
}