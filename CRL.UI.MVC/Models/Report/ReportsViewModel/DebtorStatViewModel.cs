using CRL.Model.ModelViews.Statistics;
using CRL.Service.Views.FinancialStatement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class DebtorStatViewModel : ReportBaseViewModel
    {
        public DebtorStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Debtor type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured party type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant type", Value = "8" });
            GroupByList.Add(new SelectListItem { Text = "Debtor county", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured party county", Value = "5" });
            GroupByList.Add(new SelectListItem { Text = "Transaction type", Value = "9" });

            FSStateList = new List<SelectListItem>();

            FSStateList.Add(new SelectListItem { Text = "All registered financing statements", Value = "1", Selected = true });
            FSStateList.Add(new SelectListItem { Text = "Exclude discharged and expired financing statements", Value = "2" });

            FSPeriodList = new List<SelectListItem>();

            //FSPeriodList.Add(new SelectListItem { Text = "New Registrations Only", Value = "1", Selected = true });
            //FSPeriodList.Add(new SelectListItem { Text = "Transitioned Registrations Only", Value = "2" });
            //FSPeriodList.Add(new SelectListItem { Text = "New and Transitioned Registrations", Value = "3" });

        }
        public ICollection<CountOfItemStatView> FSStatView { get; set; }
        public ICollection<ValueOfFSStatView> ValueOfFSStatView { get; set; }
        public List<SelectListItem> FSStateList { get; set; }
        public List<SelectListItem> FSPeriodList { get; set; }

        public List<SelectListItem> GroupByList { get; set; }
        [Display(Name = "Grouped by")]
        public int GroupedBy { get; set; }
        [Display(Name = "Women Owned")]
        public bool LimitToWomenOwned { get; set; }
        [Display(Name = "Include Graph")]
        public bool IncludeGraph { get; set; }
        [Display(Name = "Limit to Financing Statement Status")]
        public int FSState { get; set; }


        /// <summary>
        /// 1 - New Registrations Only, 2 - Transitioned Registrations Only , 3 - New and Transitioned Registrations
        /// </summary>
        [Display(Name = "Period Of Registration")]
        public int FSPeriod { get; set; }


    }
}