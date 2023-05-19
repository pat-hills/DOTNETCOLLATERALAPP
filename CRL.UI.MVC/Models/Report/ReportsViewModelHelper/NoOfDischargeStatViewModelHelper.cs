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
using CRL.Model.FS.Enums;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class NoOfDischargeStatViewModelHelper : ReportBaseViewModelHelper
    {
        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            NoOfDischargeStatViewModel viewModel = new NoOfDischargeStatViewModel();

            viewModel.PartialViewName = "_NoOfDischargeStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.NoOfDischargeStatViewModel";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            viewModel.Name = "Number of Discharges Report";
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

            //NoOfDischargeStatViewModel model = (NoOfDischargeStatViewModel)viewModel;
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

        private void MapParametersToRequest(NoOfDischargeStatViewModel viewModel, ViewStatRequest request)
        {
            request.GroupBy = (GroupByNoOfFSStat)viewModel.GroupedBy;
            request.RegistrationDate  = viewModel.GenerateDateRange();
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //NoOfDischargeStatViewModel model = (NoOfDischargeStatViewModel)viewModel;
            //ViewPaymentsRequest request = new ViewPaymentsRequest();
            //MapParametersToRequest(model, request);
            //IReportService cs = ObjectFactory.GetInstance<IReportService>();
            //request.SecurityUser = SecurityUser;
            //ViewPaymentsResponse response = cs.ViewPayments(request);
            //model.PaymentsView = response.PaymentViews;

            //List<string[]> CSVData = new List<string[]>();

            //string[] Header = { "Date", "Payment No", "Payment Type", "Source", "Amount", "Paid by", "Client", "Public User ClientEmailtEmail", "Received By", "Paypoint", "Transaction No" };
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
            NoOfDischargeStatViewModel model = (NoOfDischargeStatViewModel)viewModel;
            ViewStatRequest request = new ViewStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewNoOfDischargeStat(request);
            model.CountOfItemStatView = response.CountOfItemStatView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\NoOfCountItem.rdlc" : "\\Statistical\\NoOfCountItem1.rdlc");
            rdsPaymentView.Name = "dsFSStat";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CountOfItemStatView;
            string GroupedBy = model.GroupedBy == 1 ? "Debtor Type" : model.GroupedBy == 2 ? "Secured Creditor Type" :
                 model.GroupedBy == 6 ? "Registrant" : model.GroupedBy == 8 ? "Registrant Type" : model.GroupedBy == 4 ? "Debtor State" : model.GroupedBy == 5 ? "Secured Creditor State" : "Unknown";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            ReportParameter parameter2 = new ReportParameter("ReportTitle", "No of Discharges by" + GroupedBy);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
            viewModel.reportViewer.LocalReport.SetParameters(parameter2);
            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);

            ResponseBase response2 = new ResponseBase();
            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;
        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            //NoOfDischargeStatViewModel model = (NoOfDischargeStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}