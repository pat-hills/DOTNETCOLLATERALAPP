using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;

using CRL.Service.Interfaces;
using Microsoft.Reporting.WebForms;
using CRL.Infrastructure.Messaging;

namespace CRL.UI.MVC.Models.Report
{
    public class ViewModelHelperResponse:ResponseBase 
    {
         
    }
    public abstract class ReportBaseViewModelHelper
    {


        public int? ReportId { get; set; }
        public abstract  ReportBaseViewModel  BuildModel(IReportService service, int ReportId);
        public abstract ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser);
        public abstract void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer);
        public abstract void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel);
        public abstract string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser);
        public abstract ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel);
         
        
    }
}