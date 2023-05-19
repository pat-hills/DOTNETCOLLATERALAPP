using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class SummaryBillByClientViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            SummaryBillByClientViewModel viewModel = new SummaryBillByClientViewModel();
            if (ReportId == 20)
            {
                viewModel.InBatchMode = true;
            }
            viewModel.Name = "Postpaid Bill By Client Report";
            viewModel.PartialViewName = "_SummaryBillByClient";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.SummaryBillByClientViewModel";
            viewModel.SettlementType = 1;
            viewModel.LimitTo = 1;
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

            //PaymentsViewModel model = (PaymentsViewModel)viewModel;
            ////response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            //////Prepare for audit report
            ////model.UserList = response.Users.ToSelectList();
            ////model.AuditTypeList = response.AuditTypes.ToSelectList();


            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {
            if (((SummaryBillByClientViewModel)viewModel).InBatchMode == true)
            {
                if (((SummaryBillByClientViewModel)viewModel).BatchId == null)
                {

                    ModelState.AddModelError("BatchId", "Please specify a batch no!");
                }
            }



        }

        private bool MapParametersToRequest(SummaryBillByClientViewModel viewModel, ViewSummaryPostpaidTransactionsRequest request)
        {
            request.BatchId = viewModel.BatchId;
            request.LimitTo = viewModel.LimitTo;
            request.SettlementType = viewModel.SettlementType;
            request.TransactionDate = viewModel.GenerateDateRange();


            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //SummaryBillViewModel model = (PaymentsViewModel)viewModel;
            // ViewPaymentsRequest request = new ViewPaymentsRequest();
            // MapParametersToRequest(model, request);
            // IReportService cs = ObjectFactory.GetInstance<IReportService>();
            // request.SecurityUser = SecurityUser;
            // ViewPaymentsResponse response = cs.ViewPayments(request);
            // model.PaymentsView = response.PaymentViews;

            // List<string[]> CSVData = new List<string[]>();

            // string[] Header = { "Date", "Payment No", "Payment Type", "Source", "Amount", "Paid by", "Client", "Public User Email", "Received By", "Paypoint", "Transaction No" };
            // CSVData.Add(Header);
            // foreach (var item in model.PaymentsView)
            // {
            //     string[] Data = new string[]
            //     {
            //         item.PaymentDate.ToString (),
            //         item.PaymentNo ,
            //         item.PaymentTypeName,
            //         item.PaymentSourceName ,
            //         item.Amount.ToString () ,
            //         item.Payee ,
            //         item.Client ,
            //         item.PublicUserEmail ,
            //         item.PaypointUser,
            //         item.Paypoint ,
            //         item.T24TransactionNo 

            //     };

            //     CSVData.Add(Data);



            // }

            // return CSVData.ToArray();


            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();
            return null;

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            SummaryBillByClientViewModel model = (SummaryBillByClientViewModel)viewModel;



            ViewSummaryPostpaidTransactionsRequest request = new ViewSummaryPostpaidTransactionsRequest();
            request.ViewType = 2;
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewSummaryPostpaidTransactionsResponse response = cs.ViewSummaryBillByClient (request);
            model.SummarisedCreditActivityView  = response.SummarisedCreditActivities ;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Payment\\SummaryBillByClient.rdlc";
            rdsPaymentView.Name = "dsSummaryBillByClient";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.SummarisedCreditActivityView;

            if (request.TransactionDate != null && !String.IsNullOrWhiteSpace(request.TransactionDate.GenerateRangePhrase()))
            {
                ReportParameter parameter = new ReportParameter("rpPeriod", request.TransactionDate.GenerateRangePhrase());
                viewModel.reportViewer.LocalReport.SetParameters(parameter);
            }

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
            //SummaryBillViewModel model = (PaymentsViewModel)viewModel;
            // ReportDataSource rdsPaymentView = new ReportDataSource();

            // reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            // rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            // rdsPaymentView.Value = model.PaymentsView;

            // reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            // reportViewer.LocalReport.Refresh();
        }


    }
}