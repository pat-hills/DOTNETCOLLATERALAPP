using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Models.Report.ViewModel
{
    public class ReportViewModel
    {
        public ReportViewModel(int Id, string ReportName, string CategoryName,string Description)
        {
            this.Id = Id;
            this.CategoryName = CategoryName;
            this.ReportName = ReportName;
            this.Description = Description;
        }

        public int Id { get; set; }
        public string ReportName { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}