using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.UI.MVC.Common;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging.Reporting.Request;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.Service.Messaging.Reporting.Response;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class ValueOfFSStatViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            FSStatViewModel viewModel = new FSStatViewModel();
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();
            viewModel.PartialViewName = "_FSStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.FSStatViewModel";
            viewModel.Name = "Value of Financing Statement Report";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            response = service.PrepareClientStatReport(new RequestBase { SecurityUser = SecurityUser });
            //response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();


            viewModel.Clients = response.Clients.ToSelectListItem();
            return viewModel;


            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareClientStatReportResponse response = new PrepareClientStatReportResponse();


            FSStatViewModel model = (FSStatViewModel)viewModel;
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

        private bool MapParametersToRequest(FSStatViewModel viewModel, ViewFSStatRequest request)
        {

            request.GroupBy = (GroupByNoOfFSStat)viewModel.GroupedBy;
            request.RegistrationDate = viewModel.GenerateDateRange();
            request.LimitToWomenOwned = viewModel.LimitToWomenOwned;
            request.FSState = viewModel.FSState;
            request.FSStateType = viewModel.FSStateType;
            request.ClientId = viewModel.ClientId;
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //FSStatViewModel model = (FSStatViewModel)viewModel;
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
            FSStatViewModel model = (FSStatViewModel)viewModel;



            ViewFSStatRequest request = new ViewFSStatRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewStatResponse response = cs.ViewValueOfFSStat(request);
            model.ValueOfFSStatView = response.ValueOfFSStatView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Statistical\\ValueOfFSStat.rdlc";
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\ValueOfFSStat.rdlc" : "\\Statistical\\ValueOfFSStat1.rdlc");
            rdsPaymentView.Name = "dsValueOfFS";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.ValueOfFSStatView;
            string GroupedBy = model.GroupedBy == 1 ? "Debtor Type" :
               model.GroupedBy == 2 ? "Secured Creditor Type" :
               model.GroupedBy == 6 ? "Registrant" :
               model.GroupedBy == 8 ? "Registrant Type" :
               model.GroupedBy == 4 ? "Debtor State" :
                model.GroupedBy == 5 ? "Secured Creditor State" :
                 model.GroupedBy == 10 ? "Currency" :
               "Unknown";
            string CurrencyParameter = model.GroupedBy == 10 ? "Currency Code" : "Currency";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            ReportParameter param = new ReportParameter("CurrencyParameter", CurrencyParameter);
            viewModel.reportViewer.LocalReport.SetParameters(param);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);

            string daterange = request.RegistrationDate != null ? request.RegistrationDate.GenerateRangePhrase() + " " : "";

            string ReportTitle = "Value Of Financing Statements " + daterange + "By " + GroupedBy;
            if (model.LimitToWomenOwned)
                ReportTitle += " (Women Owned)";

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
            //FSStatViewModel model = (FSStatViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}