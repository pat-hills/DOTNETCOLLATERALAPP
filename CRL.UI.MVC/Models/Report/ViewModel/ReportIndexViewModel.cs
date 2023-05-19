using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Report;

namespace CRL.UI.MVC.Models.Report.ViewModel
{
    public class ReportIndexViewModel
    {
        public ReportIndexViewModel()
        {
            ReportView = new List<ReportView>();
            _ReportDescriptionViewModel = new ReportDescriptionViewModel();

        }
        public List<ReportView> ReportView { get; set; }
        public List<String> Categories { get; set; }
        public List<String> Modules { get; set; }
        public ReportDescriptionViewModel _ReportDescriptionViewModel { get; set; }
    }
}