using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.Statistics;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class NoOfCollateralStatViewModel : ReportBaseViewModel
    {
        public NoOfCollateralStatViewModel()
        {
            GroupByList = new List<SelectListItem>();

            GroupByList.Add(new SelectListItem { Text = "Debtor Type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant Type", Value = "8" });
            GroupByList.Add(new SelectListItem { Text = "Debtor State", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
        }
        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
        public ICollection<SelectListItem> GroupByList { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }
        [Display(Name = "Group by")]
        public int? GroupedBy { get; set; }

    }
}