using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class PaypointPaymentViewModelHelper
    : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PaypointPaymentViewModel viewModel = new PaypointPaymentViewModel();
          
            viewModel.PartialViewName = "_PaypointPayment";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.PaypointPaymentViewModel";
            viewModel.Name = "Paypoint Payment Summary Report";
         


            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
           
            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(PaypointPaymentViewModel viewModel, ViewPaypointPaymentsRequest request)
        {

        
            request.PaymentDate = viewModel.GenerateDateRange();
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
 


            throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            PaypointPaymentViewModel model = (PaypointPaymentViewModel)viewModel;



            ViewPaypointPaymentsRequest request = new ViewPaypointPaymentsRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewPaypointPaymentsResponse response = cs.ViewPaypointPayments(request);
            model.PaypointPaymentView  = response.PaypointPaymentView ;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();
          
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Payment\\PaypointPaymentSummary.rdlc";
            rdsPaymentView.Name = "dsPaypointPaymentView";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.PaypointPaymentView ;
  if (request.PaymentDate != null && !String.IsNullOrWhiteSpace(request.PaymentDate .GenerateRangePhrase()))
            {
                ReportParameter parameter = new ReportParameter("rpPeriod", request.PaymentDate .GenerateRangePhrase());
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
            PaymentsViewModel model = (PaymentsViewModel)viewModel;
            ReportDataSource rdsPaymentView = new ReportDataSource();

            reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.PaymentsView;

            reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            reportViewer.LocalReport.Refresh();
        }


    }
}