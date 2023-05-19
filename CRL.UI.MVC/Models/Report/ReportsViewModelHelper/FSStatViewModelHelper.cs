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
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class FSStatViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            FSStatViewModel viewModel = new FSStatViewModel();
         
            viewModel.PartialViewName = "_FSStat";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.FSStatViewModel";
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();
            viewModel.Name = "Number of Financing Statements Report";

            PrepareViewFSStatRequest request = new PrepareViewFSStatRequest();
            request.SecurityUser = SecurityUser;

            PrepareViewStateResponse response = service.PrepareNoOfFSStat(request);

            //response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();
            viewModel.Lgas = response.LGAs;
            viewModel.Countys = response.Countys.ToSelectList();
            viewModel.Clients = response.Clients.ToSelectListItem();

            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareViewStateResponse response = new PrepareViewStateResponse();


            FSStatViewModel model = (FSStatViewModel)viewModel;
            PrepareViewFSStatRequest request = new PrepareViewFSStatRequest();
            request.SecurityUser = SecurityUser ;
            response = service.PrepareNoOfFSStat(request);
            model.Lgas = response.LGAs;
            model.Countys = response.Countys.ToSelectList();
            model.Clients = response.Clients.ToSelectListItem();
            return viewModel;



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(FSStatViewModel viewModel, ViewFSStatRequest request)
        {

            request.GroupBy = (GroupByNoOfFSStat)viewModel.GroupedBy;
            request.LimitToWomenOwned = viewModel.LimitToWomenOwned;
            request.RegistrationDate = viewModel.GenerateDateRange();
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
            ViewStatResponse response = cs.ViewNoOfFSStat (request);
            model.FSStatView  = response.CountOfItemStatView ;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Statistical\\NoOfCountItem.rdlc";
            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + (viewModel.ReportTypeId == 1 ? "\\Statistical\\NoOfCountItem.rdlc" : "\\Statistical\\NoOfCountItem1.rdlc");
            rdsPaymentView.Name = "dsFSStat";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.FSStatView;
            string GroupedBy = model.GroupedBy == 1 ? "Debtor Type" :
                model.GroupedBy == 2 ? "Secured Creditor Type" :
                model.GroupedBy == 6 ? "Registrant" :
                model.GroupedBy == 8 ? "Registrant Type" :
                model.GroupedBy == 4 ? "Debtor State" :
                model.GroupedBy == 5 ? "Secured Creditor State" :
                model.GroupedBy == 10 ? "Currency" :
                "Unknown";
            ReportParameter parameter = new ReportParameter("GroupBy", GroupedBy);
            string daterange = request.RegistrationDate != null ? request.RegistrationDate.GenerateRangePhrase() + " " : "";
            string ReportTitle = "Number of Financing Statements " + daterange +  "By " + GroupedBy;
                
            if (model.LimitToWomenOwned)
                ReportTitle += " (Women Owned)";
            ReportParameter parameter2 = new ReportParameter("ReportTitle", ReportTitle);
            viewModel.reportViewer.LocalReport.SetParameters(parameter);
            viewModel.reportViewer.LocalReport.SetParameters(parameter2);



            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);

            PrepareViewFSStatRequest request3 = new PrepareViewFSStatRequest();
            request3.SecurityUser = SecurityUser;
            PrepareViewStateResponse response3 = cs.PrepareNoOfFSStat(request3);

            //response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();
            model.Lgas = response3.LGAs;
            model.Countys = response3.Countys.ToSelectList();
            model.Clients = response3.Clients.ToSelectListItem();

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