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
    public class ValueOfFSStatViewModel : ReportBaseViewModel
    {
        public ICollection<ValueOfFSStatView> ValueOfFSStatView { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }

    }
}