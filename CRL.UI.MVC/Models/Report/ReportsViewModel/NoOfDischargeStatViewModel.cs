using CRL.Model.ModelViews.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class NoOfDischargeStatViewModel : ReportBaseViewModel
    {
        public NoOfDischargeStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Debtor Type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant Type", Value = "8" });
            GroupByList.Add(new SelectListItem { Text = "Debtor State", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
        }

        public List<SelectListItem> GroupByList { get; set; }
        [Display(Name = "Group by")]
        public int GroupedBy { get; set; }
        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
    }
}