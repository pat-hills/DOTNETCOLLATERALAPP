using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views.Payments;

using CRL.Model.ModelViews.Payments;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class SummaryBillViewModel: ReportBaseViewModel
    {
        public SummaryBillViewModel()
            : base()
        {
         

            SettlementList = new List<SelectListItem>();
            SettlementList.Add(new SelectListItem { Text = "Unsettled postpaid bill", Value = "1" });
            SettlementList.Add(new SelectListItem { Text = "Settled postpaid bill", Value = "2" });
            
          
      
        }
        public ICollection<SummarisedCreditActivityByRepresentativeBank> SummarisedCreditActivitiesRepresentativeBank { get; set; }
        public bool InBatchMode { get; set; }
         [Display(Name = "Report Type")]
         public int? SettlementType { get; set; }
         [Display(Name = "Limit To")]
         public int? LimitTo { get; set; }
         public int? BatchId { get; set; }       
        public List<SelectListItem> SettlementList { get; set; }
      

    }
}