using CRL.Model.ModelViews.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class NoOfDebtorStatViewModel : ReportBaseViewModel
    {
        public NoOfDebtorStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Debtor Type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant Type", Value = "8" });
            GroupByList.Add(new SelectListItem { Text = "Debtor State", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
            //GroupByList.Add(new SelectListItem { Text = "Currency", Value = "10" });

            FSStateList = new List<SelectListItem>();

            FSStateList.Add(new SelectListItem { Text = "All registered financing statements", Value = "1", Selected = true });
            FSStateList.Add(new SelectListItem { Text = "Exclude discharged and expired financing statements", Value = "2" });
        }

        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
        [Display(Name = "Women Owned")]
        public bool LimitToWomenOwned { get; set; }
        public List<SelectListItem> FSStateList { get; set; }
        public List<SelectListItem> GroupByList { get; set; }
        [Display(Name = "Group by")]
        public int? GroupedBy { get; set; }
        [Display(Name = "Limit to Financing Statement Status")]
        public int FSState { get; set; }
    }
}