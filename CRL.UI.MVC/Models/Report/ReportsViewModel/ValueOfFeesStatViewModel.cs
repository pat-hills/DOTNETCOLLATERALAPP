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
    public class ValueOfFeesStatViewModel : ReportBaseViewModel
    {
        public ValueOfFeesStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Client Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Client State", Value = "5" });
            GroupByList.Add(new SelectListItem { Text = "Client", Value = "6" });
        }
        public ICollection<ValueOfFeeStatView> ValueOfFeeStatView { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }
        [Display(Name = "Grouped by")]
        public int? GroupedBy { get; set; }
        public ICollection<SelectListItem> GroupByList { get; set; }
    }
}