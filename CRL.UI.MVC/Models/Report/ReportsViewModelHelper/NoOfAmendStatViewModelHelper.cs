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
using CRL.Service.Messaging.Reporting.Response;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class NoOfAmendStatViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();
            NoOfAmendStatViewModel viewModel = new NoOfAmendStatViewModel();
            if (ReportId != 15)
            {
                viewModel.Name = "Number of Amendments Report";
            }
            else
            {
                viewModel.Name = "Number of Financing Change Statement Report";
            }

            viewModel.PartialViewName = "_NoOfAmendStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.NoOfAmendStatViewModel";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            response = service.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();
            viewModel.ReportId = ReportId;
            viewModel.Clients = response.Clients.ToSelectListItem();
            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();

            
            NoOfAmendStatViewModel model = (NoOfAmendStatViewModel)viewModel;
            response = service.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });
            //////Prepare for audit report
            ////model.UserList = response.Users.ToSelectList();
            ////model.AuditTypeList = response.AuditTypes.ToSelectList();

            model.Clients = response.Clients.ToSelectListItem();
            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(NoOfAmendStatViewModel viewModel, ViewStatRequest request)
        {
            request.GroupBy = (GroupByNoOfFSStat?)viewModel.GroupedBy;
            request.RegistrationDate  = viewModel.GenerateDateRange();
            request.ReportId = viewModel.ReportId;
            request.ClientId = viewModel.ClientId;
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //NoOfAmendStatViewModel model = (NoOfAmendStatViewModel)viewModel;
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
            NoOfAmendStatViewModel model = (NoOfAmendStatViewModel)viewModel;
            ViewStatRequest request = new ViewStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewNoOfAmendStat (request);
            model.CountOfItemStatView = response.CountOfItemStatView;
            string ReportType = viewModel.ReportId != 15 ? "Amendment Type" : "Financing Change Statement Type";

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Statistical\\NoOfCountItem.rdlc";
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\NoOfCountItem3.rdlc" : "\\Statistical\\NoOfCountItem4.rdlc");
            rdsPaymentView.Name = "dsFSStat";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CountOfItemStatView;
            string GroupedBy = model.GroupedBy == 1 ? "Debtor Type" : model.GroupedBy == 2 ? "Secured Creditor Type" : model.GroupedBy == 6 ? "Registrant" : model.GroupedBy == 8 ? "Registrant Type" :
                 model.GroupedBy == 4 ? "Debtor State" : model.GroupedBy == 5 ? "Secured Creditor State" :  "Unknown";
            //ReportParameter parameter = new ReportParameter("GroupBy", "Financing Change Statement");
            ReportParameter parameter = new ReportParameter("GroupBy", ReportType);
            string ReportTitle = "Number Of " + ReportType + "s By " + GroupedBy;
            ReportParameter parameter2 = new ReportParameter("ReportTitle", ReportTitle);
            ReportParameter parameter3 = new ReportParameter("PrimaryGroup", GroupedBy);
            string daterange = request.RegistrationDate != null ? request.RegistrationDate.GenerateRangePhrase() : "";
            //ReportParameter parameter2 = new ReportParameter("ReportTitle", "No Of Financing Change Statement " + daterange);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
            viewModel.reportViewer.LocalReport.SetParameters(parameter2);
            viewModel.reportViewer.LocalReport.SetParameters(parameter3);



            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);

            PrepareClientStatReportResponse response2 = new PrepareClientStatReportResponse();
            response2 = cs.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });          
            model.Clients = response2.Clients.ToSelectListItem();

            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;

            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            //NoOfAmendStatViewModel model = (NoOfAmendStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}