using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class PaymentsViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PaymentsViewModel viewModel = new PaymentsViewModel();
            viewModel.ShowType = ReportId ==3? 2:1;

            viewModel.PartialViewName = "_PaymentsReport";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.PaymentsViewModel";
            viewModel.Name = ReportId ==3?  "My Payment Report":"Payments Received Report";
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

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(PaymentsViewModel viewModel, ViewPaymentsRequest request)
        {

            request.MembershipId  = viewModel.MembershipId ;
            request.Payee = viewModel.Payee;
            request.PaymentNo  = viewModel.PaymentNo ;
            request.PaymentTypeId = viewModel.PaymentTypeId;
            request.PaypointName = viewModel.PaypointName;
            request.PaypointUserName = viewModel.PaypointUserName;
            request.PublicUserEmail = viewModel.PublicUserEmail;
            request.ShowType = viewModel.ShowType;
            request.TransactionNo  = viewModel.TransactionNo ;
            request.PaymentSourceId = viewModel.PaymentSourceId;
            request.IsPublicUser = viewModel.IsPublicUser;
            request.PaymentDate  = viewModel.GenerateDateRange();
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            PaymentsViewModel model = (PaymentsViewModel)viewModel;
            ViewPaymentsRequest request = new ViewPaymentsRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewPaymentsResponse response = cs.ViewPayments (request);
            model.PaymentsView = response.PaymentViews;

            List<string[]> CSVData = new List<string[]>();

            string[] Header = {  "Date", "Payment No", "Payment Type", "Source", "Amount", "Paid by", "Client","Public User Email","Received By", "Paypoint", "Transaction No" };
            CSVData.Add(Header);
            foreach (var item in model.PaymentsView)
            {
                string[] Data = new string[]
                {
                    item.PaymentDate.ToString (),
                    item.PaymentNo ,
                    item.PaymentTypeName,
                    item.PaymentSourceName ,
                    item.Amount.ToString () ,
                    item.Payee ,
                    item.Client ,
                    item.PublicUserEmail ,
                    item.PaypointUser,
                    item.Paypoint ,
                    item.T24TransactionNo 
                    
                };

                CSVData.Add(Data);



            }

            return CSVData.ToArray();


            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            PaymentsViewModel model = (PaymentsViewModel)viewModel;


          
            ViewPaymentsRequest request = new ViewPaymentsRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewPaymentsResponse response = cs.ViewPayments(request);
            model.PaymentsView = response.PaymentViews;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Payment\\Payments.rdlc";
            rdsPaymentView.Name = "dsPayments";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.PaymentsView ;

            ReportParameter parameter = new ReportParameter("ShowType", model.ShowType.ToString ());
            
            
string ClientMode = "";
            if (model.ShowType == 1)
            { 
               
                if (request.SecurityUser.IsOwnerUser == false)
                {
                    ClientMode = "1";
                }
                else if (request.SecurityUser.IsOwnerUser == true)
                {
                    ClientMode = "2";
                }
              
            }
            else 
                ClientMode ="0";
            ReportParameter parameter2 = new ReportParameter("ClientMode", ClientMode);
            ReportParameter parameter1 = new ReportParameter("ReportTitle", request.PaymentDate != null ? request.PaymentDate.GenerateRangePhrase("Payments Received ") : " ");
            viewModel.reportViewer.LocalReport.SetParameters(parameter1); 
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
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