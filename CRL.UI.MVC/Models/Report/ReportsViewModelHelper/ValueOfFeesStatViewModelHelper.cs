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
    public class ValueOfFeesStatViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            ValueOfFeesStatViewModel viewModel = new ValueOfFeesStatViewModel();
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();

            viewModel.PartialViewName = "_ValueOfFeesStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.ValueOfFeesStatViewModel";
            viewModel.Name = "Value of Fees By Transaction Report";
            response = service.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });


            viewModel.Clients = response.Clients.ToSelectListItem();
            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();


            ValueOfFeesStatViewModel model = (ValueOfFeesStatViewModel)viewModel;
            response = service.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });
            //////Prepare for audit report
            ////model.UserList = response.Users.ToSelectList();
            ////model.AuditTypeList = response.AuditTypes.ToSelectList();

            model.Clients = response.Clients.ToSelectListItem();
            return viewModel;




        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(ValueOfFeesStatViewModel viewModel, ViewStatRequest request)
        {
            request.GroupBy = (GroupByNoOfFSStat?)viewModel.GroupedBy;
            request.RegistrationDate = viewModel.GenerateDateRange();
            request.ClientId = viewModel.ClientId;            
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //ValueOfFeesStatViewModel model = (ValueOfFeesStatViewModel)viewModel;
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
            ValueOfFeesStatViewModel model = (ValueOfFeesStatViewModel)viewModel;



            ViewStatRequest request = new ViewStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewValueOfFeesStat (request);
            model.ValueOfFeeStatView = response.ValueOfFeeStatView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();
            
            //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Statistical\\ValueOfFeesStat.rdlc";
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\ValueOfFeesStat.rdlc" : "\\Statistical\\ValueOfFeesStat1.rdlc");
            rdsPaymentView.Name = "dsValueOfFees";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.ValueOfFeeStatView ;

            string daterange = request.RegistrationDate != null ? request.RegistrationDate.GenerateRangePhrase() + " " : "";
            string GroupedBy = model.GroupedBy == null ? "Service Type" : model.GroupedBy == 2 ? "Client Type" : model.GroupedBy == 6 ? "Client" : model.GroupedBy == 5 ? "Client State" : "Unknown";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
            string ReportTitle = "Value Of Transaction Fees By " + GroupedBy;
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
            //ValueOfFeesStatViewModel model = (ValueOfFeesStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}