using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;

using CRL.Service.Interfaces;
using CRL.Service.Messaging;
using CRL.Service.Messaging.Reporting.Response;
using CRL.UI.MVC.Common;
using CRL.UI.MVC.Models.Report;
using CRL.UI.MVC.Models.Report.ViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using CRL.Infrastructure.Messaging;

namespace CRL.UI.MVC.Controllers
{
    [MasterEventAuthorizationAttribute]
    public class ReportController : Controller
    {
        SecurityUser SecurityUser
        {
            get
            {
                return (SecurityUser)this.HttpContext.User;

            }
        }
        //
        // GET: /Report/

        //public ActionResult Index()
        //{
        //    //List all reports here
        //    ReportIndexViewModel viewModel = new ReportIndexViewModel();
        //    IReportService rp = ObjectFactory.GetInstance<IReportService>();
        //    ViewReportsResponse response = rp.ViewReports(new RequestBase { SecurityUser= SecurityUser });
        //    viewModel.ReportView = response.ReportsView;
        //    viewModel.Categories = viewModel.ReportView .Select(c => c.CategoryName).Distinct().ToList();


        //    return View(viewModel);
          
        //}
        public ActionResult Index()
        {
            ReportIndexViewModel viewModel = new ReportIndexViewModel();
            IReportService rp = ObjectFactory.GetInstance<IReportService>();
            ViewReportsResponse response = rp.ViewReports(new RequestBase { SecurityUser = SecurityUser });
            viewModel.ReportView = response.ReportsView;
          


            viewModel.Categories = viewModel.ReportView .Select(c => c.CategoryName).Distinct().ToList();
            viewModel.Modules = viewModel.ReportView.Select(c => c.ModuleName).Distinct().ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ReportDescription(string module)
        {

            IReportService rp = ObjectFactory.GetInstance<IReportService>();
            ViewReportsResponse response = rp.ViewReports(new RequestBase { SecurityUser = SecurityUser });


            ReportDescriptionViewModel viewModel2 = new ReportDescriptionViewModel();

            if (!(module == "All Reports"))
            {
                viewModel2.AllReports = false;
                viewModel2.ReportView = response.ReportsView.Where(s => s.ModuleName.ToLower() == module.ToLower()).ToList();
            }

            else
            {

                viewModel2.AllReports = true;
                viewModel2.ReportView = response.ReportsView;
            }
            viewModel2.Category = module;


            return PartialView("~/Views/Report/_ReportDescription.cshtml", viewModel2);
        }



        //
        // GET: /Report/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

      

        //
        // GET: /Report/Create

        public ActionResult LoadReport(int Id)
        {
            //Call the service to load the info for this report
            //Use reflection to instantiate the particular viewmodelhelper from the view model helper
            //We will have a base viewmodel all reportviewmodel which all reportviewmodel will
            //everyviewmodelhelper will take the reportviewmodel and then create the specific instance and 

            //GET THE REPORT
            IReportService rp = ObjectFactory.GetInstance<IReportService>();
            RequestBase request = new RequestBase();
            request.Id = Id;
            request.SecurityUser = SecurityUser;
            request.UserIP = AuditHelper.GetUserIP(this.Request);
            request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
            GetReportByIdResponse response = rp.GetReportById(request);

            //NOW CREATE THE BASE VIEW MODEL
            
            
            Assembly assembly = Assembly.Load("CRL.UI.MVC");
            Type ViewModelHelperType = assembly.GetType("CRL.UI.MVC.Models.Report.ReportsViewModelHelper."+ response.Report.ViewModelHelper);
            var viewModelHelper = Activator.CreateInstance( ViewModelHelperType);

            MethodInfo methodInfo = ViewModelHelperType.GetMethod("BuildModel", BindingFlags.Instance | BindingFlags.Public);

            object[] parameters = new object[] {rp, Id};
            //Call the method to load the object
            ReportBaseViewModel viewModel = (ReportBaseViewModel)methodInfo.Invoke(viewModelHelper, parameters);            
            viewModel.ViewModelHelper = "CRL.UI.MVC.Models.Report.ReportsViewModelHelper." + response.Report.ViewModelHelper;


            viewModel.ShowPagination = false;
            viewModel.PaginateRecords = response.Report.PaginateRecords;
            viewModel.RecordsPage = 1;

            if (response.Report.MaximumNumberOfRecords == null)
            {
                viewModel.ReportMaximumRecords = Constants.MAX_NUM_RECORDS;
            }
            else
            {
                viewModel.ReportMaximumRecords = (int)response.Report.MaximumNumberOfRecords;
            }

            if (response.Report.DefaultNumberOfRecords == null)
            {
                viewModel.MaximumRecords = Constants.DEFAULT_NUM_RECORDS;
            }
            else
            {
                viewModel.MaximumRecords = (int)response.Report.DefaultNumberOfRecords;
            }


            return View(viewModel);
        }

        //
        // POST: /Report/Create
        [HttpPost]
        public ActionResult  LoadExcelReport(ReportBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                Excel.Application excel = new Excel.Application();
                Excel.Workbook workBook = excel.Workbooks.Add();
                Excel.Worksheet sheet = workBook.ActiveSheet;




                //Now let's call the view model again
                Assembly assembly = Assembly.Load("CRL.UI.MVC");
                Type ViewModelHelperType = assembly.GetType(model.ViewModelHelper);
                var viewModelHelper = Activator.CreateInstance(ViewModelHelperType);

                MethodInfo methodInfo = ViewModelHelperType.GetMethod("LoadCSV", BindingFlags.Instance | BindingFlags.Public);
                object[] parameters = new object[] { model, this.SecurityUser };

                string[][] CsvContent;

                //Call the method to load the object
                CsvContent = (string[][])methodInfo.Invoke(viewModelHelper, parameters);




                for (int i = 0; i < CsvContent.Length; i++)
                {
                    string[] CsvLine = CsvContent[i];
                    for (int j = 0; j < CsvLine.Length; j++)
                    {
                        sheet.Cells[i + 1, j + 1] = CsvLine[j];
                    }
                }

                string tempPath = AppDomain.CurrentDomain.BaseDirectory;
                string filename = DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + SecurityUser.Id + "_temp";//date time added to be sure there are no name conflicts
                workBook.SaveAs(tempPath + filename + ".xls");//create temporary file from the workbook
                tempPath = workBook.FullName;//name of the file with path and extension
                workBook.Close();
                byte[] result = System.IO.File.ReadAllBytes(tempPath);//change to byte[]
                return File(result, "application/vnd.ms-excel", filename + ".xls");
            }
            else
            {
                return View(model);
            }

          

        }


        [HttpPost]
        public ActionResult LoadReport(ReportBaseViewModel model)
        {

                //Now let's call the view model again
                Assembly assembly = Assembly.Load("CRL.UI.MVC");
                Type ViewModelHelperType = assembly.GetType(model.ViewModelHelper);
                var viewModelHelper = Activator.CreateInstance(ViewModelHelperType);
                MethodInfo methodValidateReport = ViewModelHelperType.GetMethod("ValidateModelState", BindingFlags.Instance | BindingFlags.Public);
                object[] methodValidateReportParam = new object[] { ModelState, model };
                methodValidateReport.Invoke(viewModelHelper, methodValidateReportParam);


                if (ModelState.IsValid)
                {
                    MethodInfo methodInfo = ViewModelHelperType.GetMethod("LoadReport", BindingFlags.Instance | BindingFlags.Public);
                    object[] parameters = new object[] { model, this.SecurityUser };
                    //Call the method to load the object
                    methodInfo.Invoke(viewModelHelper, parameters);
                    model.LoadReport = true; //This will cause it to render
                    model.reportViewer.SizeToReportContent = true;

                    model.ShowPagination = true;
                    //Also deal with the business rules error and return a response class here
                }
                else
                {
                    MessageInfo _msg = new MessageInfo
                    {
                        MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                        Message = "Busness Validation Errors were detected"
                    };
                    ViewBag.TitleMessage = _msg;

                   
                       
                       
                }
                MethodInfo methodInfo2 = ViewModelHelperType.GetMethod("BuildModelAfterError", BindingFlags.Instance | BindingFlags.Public);
                IReportService rp2 = ObjectFactory.GetInstance<IReportService>();
                object[] parameters2 = new object[] { rp2, model };
                //methodInfo2.Invoke(viewModelHelper, parameters2);
                model = (ReportBaseViewModel)methodInfo2.Invoke(viewModelHelper, parameters2);
                if (model.LimitRecordsOptions.Count() <= 1)
                {
                    model.ShowPagination = false;
                }
                return View(model);
        }


        //public ActionResult LocalReportExample()
        //{
        //    SetLocalReport();
        //    return View();
        //}

        //private void SetLocalReport()
        //{
        //    ReportViewer reportViewer = new ReportViewer();
        //    reportViewer.ProcessingMode = ProcessingMode.Local;
        //    reportViewer.SizeToReportContent = true;
        //    reportViewer.Width = Unit.Percentage(100);
        //    reportViewer.Height = Unit.Percentage(100);

        //    _AuditReportViewModel model = new _AuditReportViewModel();
        //    IClientService cs = ObjectFactory.GetInstance<IClientService>();
        //    ViewAuditsRequest request = new ViewAuditsRequest();
        //    request.SecurityUser = SecurityUser;
        //    ViewAuditsResponse response = cs.ViewAudits(request);
        //    model.AuditViews = response.AuditViews;

           
        //    ReportDataSource rdsAuditView = new ReportDataSource();

        //    reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

        //    rdsAuditView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
        //    rdsAuditView.Value = model.AuditViews;

        //    reportViewer.LocalReport.DataSources.Add(rdsAuditView);          

        //    ViewBag.ReportViewer = reportViewer;

        //}
    }
}
