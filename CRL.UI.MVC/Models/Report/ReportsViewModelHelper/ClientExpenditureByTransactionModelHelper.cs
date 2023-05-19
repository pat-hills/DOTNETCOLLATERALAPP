using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Messaging.Reporting.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class ClientExpenditureByTransactionModelHelper : ReportBaseViewModelHelper
    {
        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            ClientExpenditureByTransactionViewModel viewModel = new ClientExpenditureByTransactionViewModel();           
            viewModel.PartialViewName = "_ClientExpenditureByTransaction";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.ClientExpenditureByTransactionViewModel";
            viewModel.Name = "Credit Expenditure By Transaction";

            PrepareClientExpenditureReportResponse response = new PrepareClientExpenditureReportResponse();

            response = service.PrepareClientExpenditure(new RequestBase { SecurityUser  = SecurityUser });
            //Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            viewModel.ClientList  = response.Institutions.ToSelectList();
            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
          
            PrepareClientExpenditureReportResponse response = new PrepareClientExpenditureReportResponse();
            ClientExpenditureByTransactionViewModel model = (ClientExpenditureByTransactionViewModel)viewModel;
            response = service.PrepareClientExpenditure(new RequestBase { SecurityUser = SecurityUser });
            //Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            model.ClientList = response.Institutions.ToSelectList();
            return viewModel;



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(ClientExpenditureByTransactionViewModel viewModel, ViewClientExpenditureByTransactionRequest request)
        {        
            request.TransactionDate = viewModel.GenerateDateRange();
         //   request.MembershipCode = viewModel.MembershipCode;
            if (viewModel.InstitutionId != null)
            {
                request.LimitToInstitutionId  = viewModel.InstitutionId ;
            }
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
           

            throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            ClientExpenditureByTransactionViewModel model = (ClientExpenditureByTransactionViewModel)viewModel;
            ViewClientExpenditureByTransactionRequest request = new ViewClientExpenditureByTransactionRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewClientExpenditureByTransactionResponse response = cs.ViewClientExpenditureByTransaction(request);
            model.ClientExpenditureByTransaction  = response.ClientExpenditureByTransaction;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();
           
            
            
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Payment\\ClientExpenditureByTransaction.rdlc";
            rdsPaymentView.Name = "dsClientExpenditureByTransaction";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.ClientExpenditureByTransaction ;

            if (request.TransactionDate!=null && !String.IsNullOrWhiteSpace(request.TransactionDate.GenerateRangePhrase()))
            {
                ReportParameter parameter = new ReportParameter("rpPeriod", request.TransactionDate.GenerateRangePhrase());
                viewModel.reportViewer.LocalReport.SetParameters(parameter);
            }
            if (!String.IsNullOrWhiteSpace(response.ClientName))
            {
                ReportParameter parameter = new ReportParameter("rpClient", response.ClientName);
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