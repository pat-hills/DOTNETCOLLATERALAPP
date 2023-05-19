using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Model.Messaging;
using CRL.Infrastructure.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class FSListViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            FSListViewModel viewModel = new FSListViewModel();
            viewModel.ShowType = FilterFinancingStatement.Active;

            viewModel.PartialViewName = "_FSList";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.FSListViewModel";
            viewModel.Name = "Financing Statement List Report";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();

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

            //FSListViewModel model = (FSListViewModel)viewModel;
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

        private bool MapParametersToRequest(FSListViewModel viewModel, ViewFSRequest request)
        {

            request.FinancialStatementTransactionTypeId = viewModel .FinancialStatementTransactionTypeId ;
            request.FinancialStatementLoanTypeId = viewModel.FinancialStatementLoanTypeId;
            request.CollateralTypeId = viewModel.CollateralTypeId;
            request.RegistrationNo = viewModel.RegistrationNo;
            request.ShowActive = viewModel.ShowType;
            request.RegistrationDate  = viewModel.GenerateDateRange();
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //FSListViewModel model = (FSListViewModel)viewModel;
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
            FSListViewModel model = (FSListViewModel)viewModel;



            ViewFSRequest request = new ViewFSRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewFSResponse response = cs.ViewFS (request);
            model.FSGridView  = response.FSGridView ;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FSList.rdlc";
            rdsPaymentView.Name = "dsFs";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.FSGridView;

         
        

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
            //FSListViewModel model = (FSListViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}