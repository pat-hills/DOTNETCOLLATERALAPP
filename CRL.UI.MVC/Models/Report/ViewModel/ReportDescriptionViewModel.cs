using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Report;

namespace CRL.UI.MVC.Models.Report.ViewModel
{
    public class ReportDescriptionViewModel
    {
        public ReportDescriptionViewModel()
        {
            ReportView = new List<ReportView>();
            AllReports = true;

        }
        public List<ReportView> ReportView { get; set; }
        public bool AllReports { get; set; }
        public string Category { get; set; }
    }
}