using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.Statistics;
using System.ComponentModel.DataAnnotations;
//using System.Web.WebPages.Html;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class NoOfLenderStatViewModel : ReportBaseViewModel
    {
        public NoOfLenderStatViewModel()
        {
            GroupByList = new List<SelectListItem>();

            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
        }


        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
        public ICollection<SelectListItem> GroupByList { get; set; }
        [Display(Name = "Grouped by")]
        public int? GroupedBy { get; set; }

        [Display(Name = "Status")]
        public int? ClientStatus { get; set; }
    }
}