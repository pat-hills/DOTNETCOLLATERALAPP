using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Payments;

using CRL.Model.ModelViews.Payments;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class SummaryClientExpendituresViewModel : ReportBaseViewModel
    {
        public SummaryClientExpendituresViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Client Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Client State", Value = "5" });
        }
        public ICollection<SummarisedClientExpenditureView> SummarisedClientExpenditures { get; set; }
        [Display(Name = "Group by")]
        public int? GroupedBy { get; set; }
        public ICollection<SelectListItem> GroupByList { get; set; }
    }
}