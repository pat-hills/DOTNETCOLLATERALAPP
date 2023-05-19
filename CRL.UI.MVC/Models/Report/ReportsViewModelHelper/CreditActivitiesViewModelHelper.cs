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

    public class CreditActivitiesViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service,int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            CreditActivitiesViewModel viewModel = new CreditActivitiesViewModel();
            
            if (ReportId == 21)
            {
                viewModel.InBatchMode = true;
            }
            viewModel.PartialViewName = "_ClientActivities";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.CreditActivitiesViewModel";
            viewModel.Name = "Credit Activities Report";
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

            //CreditActivitiesViewModel model = (CreditActivitiesViewModel)viewModel;
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

        private bool MapParametersToRequest(CreditActivitiesViewModel viewModel, ViewCreditActivitiesRequest request)
        {

            request.MembershipId = viewModel.MembershipId;
            request.AccountBatchId = viewModel.AccountBatchId;
            request.AccountReconcileId = viewModel.AccountReconcileId;
            request.AccountTypeTransactionId = viewModel.AccountTypeTransactionId;
            request.CreditOrDebit = viewModel.CreditOrDebit;
            request.Narration = viewModel.Narration;
            request.ServiceFeeTypeId = viewModel.ServiceFeeTypeId;
            request.ClientName  = viewModel.ClientName;
            request.TransactionDate = viewModel.GenerateDateRange();
            request.SettlementType = viewModel.SettlementType;
            request.LimitTo = viewModel.LimitTo;
            request.AccountReconcileId = viewModel.AccountReconcileId;
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            CreditActivitiesViewModel model = (CreditActivitiesViewModel)viewModel;
            ViewCreditActivitiesRequest request = new ViewCreditActivitiesRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewCreditActivitiesResponse response = cs.ViewCreditActivities(request);
            model.CreditActivitiesView = response.CreditActivityView;

            List<string[]> CSVData = new List<string[]>();

            string[] Header = { "Date", "Type", "Fee For", "Cr / Db", "Amount", "Narration", "Postpaid Bal", "Prepaid Bal", "Caused by user", "Caused by client", "Representative client", "Batch No", "Reconciled No" };
            CSVData.Add(Header);
            foreach (var item in model.CreditActivitiesView)
            {
                string[] Data = new string[]
                {
                    item.EntryDate .ToString (),
                    item.AccountTransactionType  ,
                    item.ServiceFeeType ,
                    item.CreditOrDebit  ,
                    item.Amount.ToString () ,
                    item.Narration  ,
                    item.NewPostpaidBalanceAfterTransaction.ToString () ,
                    item.NewPrepaidBalanceAfterTransaction .ToString () ,
                    item.NameOfUser ,
                    item.NameOfLegalEntity  ,
                    item.NameOfRepresnetative ,
                    item.AccountBatchId.HasValue ? item.AccountBatchId.Value.ToString ("0000000") : "",
                    item.AccountReconcileId.HasValue ? item.AccountReconcileId .Value.ToString ("0000000") : "", 
                    
                };

                CSVData.Add(Data);



            }

            return CSVData.ToArray();


            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {

            CreditActivitiesViewModel model = (CreditActivitiesViewModel)viewModel;
            ViewCreditActivitiesRequest request = new ViewCreditActivitiesRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewCreditActivitiesResponse response = cs.ViewCreditActivities(request);
            model.CreditActivitiesView = response.CreditActivityView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Payment\\CreditActivities.rdlc";
            rdsPaymentView.Name = "dsAccountTransaction";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CreditActivitiesView;

            ReportParameter parameter1 = new ReportParameter("ReportTitle", request.TransactionDate != null ? request.TransactionDate.GenerateRangePhrase("Account Transactions ") : " ");
            viewModel.reportViewer.LocalReport.SetParameters(parameter1); 

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
            CreditActivitiesViewModel model = (CreditActivitiesViewModel)viewModel;
            ReportDataSource rdsPaymentView = new ReportDataSource();

            reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.CreditActivitiesView;

            reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            reportViewer.LocalReport.Refresh();
        }



    }
}