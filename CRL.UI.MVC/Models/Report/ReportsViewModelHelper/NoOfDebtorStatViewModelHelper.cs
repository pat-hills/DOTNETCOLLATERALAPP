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
    public class NoOfDebtorStatViewModelHelper : ReportBaseViewModelHelper
    {
        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            NoOfDebtorStatViewModel viewModel = new NoOfDebtorStatViewModel();

            viewModel.PartialViewName = "_NoOfDebtorStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.NoOfDebtorStatViewModel";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            viewModel.Name = "Number of Debtors Report";
            //response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();
            return viewModel;
        }

        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            //var SecurityUser = (SecurityUser)HttpContext.Current.User;
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            //NoOfDebtorStatViewModel model = (NoOfDebtorStatViewModel)viewModel;
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

        private void MapParametersToRequest(NoOfDebtorStatViewModel viewModel, ViewFSStatRequest request)
        {
            request.GroupBy = (GroupByNoOfFSStat)viewModel.GroupedBy;
            request.LimitToWomenOwned = viewModel.LimitToWomenOwned;
            request.RegistrationDate = viewModel.GenerateDateRange();
            request.FSState = viewModel.FSState;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //NoOfDebtorStatViewModel model = (NoOfDebtorStatViewModel)viewModel;
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
            NoOfDebtorStatViewModel model = (NoOfDebtorStatViewModel)viewModel;
            ViewFSStatRequest request = new ViewFSStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewNoOfDebtorStat(request);
            model.CountOfItemStatView = response.CountOfItemStatView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\NoOfCountItem.rdlc" : "\\Statistical\\NoOfCountItem1.rdlc");
            rdsPaymentView.Name = "dsFSStat";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CountOfItemStatView;
            rdsPaymentView.Value = model.CountOfItemStatView; string GroupedBy = model.GroupedBy == 1 ? "Debtor Type" : model.GroupedBy == 2 ? "Secured Creditor Type" :
                model.GroupedBy == 6 ? "Registrant" : model.GroupedBy == 8 ? "Registrant Type" : model.GroupedBy == 4 ? "Debtor State" : model.GroupedBy == 5 ? "Secured Creditor State" : model.GroupedBy == 10 ? "Currency" : "Unknown";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            ReportParameter parameter2 = new ReportParameter("ReportTitle", "No of Debtors by" + GroupedBy);
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
            //NoOfDebtorStatViewModel model = (NoOfDebtorStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}