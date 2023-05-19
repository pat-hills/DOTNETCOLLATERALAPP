using CRL.Model.ModelViews.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class NoOfServiceStatViewModel : ReportBaseViewModel
    {
        public NoOfServiceStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Client Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Client State", Value = "5" });
            GroupByList.Add(new SelectListItem { Text = "Client", Value = "6" });
        }
        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
        public ICollection<SelectListItem> GroupByList { get; set; }
        [Display(Name = "Group by")]
        public int? GroupedBy { get; set; }
    }
}