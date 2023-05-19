using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging.Reporting.Request;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class NoOfLenderStatViewModelHelper: ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            NoOfLenderStatViewModel viewModel = new NoOfLenderStatViewModel();

            viewModel.PartialViewName = "_NoOfLenderStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.NoOfLenderStatViewModel";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            viewModel.Name = "Number of Clients Report";
            //response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();


            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            //var SecurityUser = (SecurityUser)HttpContext.Current.User;
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            //NoOfLenderStatViewModel model = (NoOfLenderStatViewModel)viewModel;
            ////response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            //////Prepare for audit report
            ////model.UserList = response.Users.ToSelectList();
            ////model.AuditTypeList = response.AuditTypes.ToSelectList();


            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(NoOfLenderStatViewModel viewModel, ViewStatRequest request)
        {
            request.GroupBy = (GroupByNoOfFSStat?)viewModel.GroupedBy;
            request.RegistrationDate = viewModel.GenerateDateRange();
            request.Status = viewModel.ClientStatus;
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //NoOfLenderStatViewModel model = (NoOfLenderStatViewModel)viewModel;
            //ViewPaymentsRequest request = new ViewPaymentsRequest();
            //MapParametersToRequest(model, request);
            //IReportService cs = ObjectFactory.GetInstance<IReportService>();
            //request.SecurityUser = SecurityUser;
            //ViewPaymentsResponse response = cs.ViewPayments(request);
            //model.PaymentsView = response.PaymentViews;

            //List<string[]> CSVData = new List<string[]>();

            //string[] Header = { "Date", "Payment No", "Payment Type", "Source", "Amount", "Paid by", "Client", "Public User Email", "Received By", "Paypoint", "Transaction No" };
            //CSVData.Add(Header);
            //foreach (var item in model.PaymentsView)
            //{
            //    string[] Data = new string[]
            //    {
            //        item.PaymentDate.ToString (),
            //        item.PaymentNo ,
            //        item.PaymentTypeName,
            //        item.PaymentSourceName ,
            //        item.Amount.ToString () ,
            //        item.Payee ,
            //        item.Client ,
            //        item.PublicUserEmail ,
            //        item.PaypointUser,
            //        item.Paypoint ,
            //        item.T24TransactionNo 

            //    };

            //    CSVData.Add(Data);



            //}

            //return CSVData.ToArray();


            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();
            return null;

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            NoOfLenderStatViewModel model = (NoOfLenderStatViewModel)viewModel;



            ViewStatRequest request = new ViewStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewNoOfLendersStat (request);
            model.CountOfItemStatView = response.CountOfItemStatView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Statistical\\NoOfCountItem.rdlc";
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\NoOfCountItem.rdlc" : "\\Statistical\\NoOfCountItem1.rdlc");
            rdsPaymentView.Name = "dsFSStat";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CountOfItemStatView;
            string GroupedBy = model.GroupedBy == 2 ? "Secured Creditor Type" : model.GroupedBy == 5 ? "Secured Creditor State" : "Unknown";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
            string daterange = request.RegistrationDate != null ? request.RegistrationDate.GenerateRangePhrase() + " " : "";
            string ReportTitle = "Number Of Client Registrations By " + GroupedBy;
            ReportParameter parameter2 = new ReportParameter("ReportTitle", ReportTitle);
            
            viewModel.reportViewer.LocalReport.SetParameters(parameter2);


            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);

            ResponseBase response2 = new ResponseBase();
            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;

            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            //NoOfLenderStatViewModel model = (NoOfLenderStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}