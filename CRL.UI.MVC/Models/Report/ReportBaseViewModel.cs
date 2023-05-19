using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;
using Microsoft.Reporting.WebForms;
using ReportViewerForMvc;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report
{
    public  class ReportBaseViewModel : BaseSearchFilterViewModel
    {
        public ReportBaseViewModel()
        {
            LimitRecordsOptions = new List<SelectListItem>();
        }

        public virtual string PartialViewName { get; set; }
        public virtual string ViewModelType { get; set; }
        public virtual string ViewModelHelper { get; set; }
        public bool LoadReport { get; set; }
        public int? ReportId { get; set; }
        public ReportViewer reportViewer { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public SubreportEventClass SubreportEventClass { get; set; }

        public int ReportMaximumRecords { get; set; }
        [Display(Name = "Maximum Records")]
        public int MaximumRecords { get; set; }
        [Display(Name = "Page")]
        public int RecordsPage { get; set; }
        public bool ShowPagination { get; set; }
        public bool PaginateRecords { get; set; }
        public List<SelectListItem> LimitRecordsOptions { get; set; }
    }
}