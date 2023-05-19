using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Service.Interfaces;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class SearchesViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            SearchesViewModel viewModel = new SearchesViewModel();
            //viewModel.ShowType = ReportId == 3 ? 2 : 1;
            SearchesViewModel viewModelSpecific = (SearchesViewModel)viewModel;
            viewModelSpecific.PartialViewName = "_Searches";
            viewModelSpecific.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.SearchesViewModel";
            viewModel.Name = "Searches Report";
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

            //SearchesViewModel model = (SearchesViewModel)viewModel;
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

        private bool MapParametersToRequest(SearchesViewModel viewModel, ViewSearchesFSRequest request)
        {
            request.MembershipId = viewModel.MembershipId;
            request.ClientName = viewModel.ClientName;
            request.ClientType = viewModel.ClientType;
            request.PublicUserCode = viewModel.PublicUserCode;
            request.SearchCode = viewModel.SearchCode;
            request.SearchDate = viewModel.GenerateDateRange();
            request.SearchType = viewModel.SearchType;
            request.ReturnedResults = viewModel.ReturnedResults;
            request.LimitTo  = viewModel.LimitTo;
            request.GeneratedReportType = viewModel.GeneratedReportType;
            request.UserName = viewModel.Username;
     
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //SearchesViewModel model = (SearchesViewModel)viewModel;
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
            SearchesViewModel model = (SearchesViewModel)viewModel;



            ViewSearchesFSRequest request = new ViewSearchesFSRequest();
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            request.IsReportRequest = true;
            ViewSearchesResponse response = cs.ViewSearchFS (request);
            model.SearchesView  = response.SearchRequestGridView ;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Search\\Searches.rdlc";
            rdsPaymentView.Name = "dsSearches";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.SearchesView;
            string ClientMode = "";
               if (request.SecurityUser.IsOwnerUser == false)
                {
                    ClientMode = "1";
                }
                else if (request.SecurityUser.IsOwnerUser == true)
                {
                    ClientMode = "2";
                }             
           
            else 
                ClientMode ="0";
            ReportParameter parameter= new ReportParameter("ClientMode", ClientMode);
            ReportParameter parameter1 = new ReportParameter("ReportTitle", request.SearchDate != null ? request.SearchDate.GenerateRangePhrase("Searches ") : " ");
            viewModel.reportViewer.LocalReport.SetParameters(parameter1); 
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
          
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
        //    SearchesViewModel model = (SearchesViewModel)viewModel;
        //    ReportDataSource rdsPaymentView = new ReportDataSource();

        //    reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

        //    rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
        //    rdsPaymentView.Value = model.PaymentsView;

        //    reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
        //    reportViewer.LocalReport.Refresh();
        }


    }
}