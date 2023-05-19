using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.User.Request;
using CRL.Service.Messaging.User.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Service.Implementations;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class UsersViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {
           
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            UsersViewModel viewModel = new UsersViewModel();
            UsersViewModel viewModelSpecific = (UsersViewModel)viewModel;

            if (ReportId == 7)
            {
                viewModelSpecific.LoadOnlyIndividualClients = true;
                viewModel.Name = "Individual Client Report";
            }
            else
            {
                viewModel.Name = "Users Report";
            }
            viewModelSpecific.PartialViewName = "_Users";
            viewModelSpecific.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.UsersViewModel";
            
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

        private bool MapParametersToRequest(UsersViewModel viewModel, ViewUsersRequest request)
        {

            request.CreatedRange = viewModel .GenerateDateRange ();
            request.Email = viewModel.Email;
            request.Fullname = viewModel.Fullname;
            request.IsPaypointUser = viewModel.IsPaypointUser;
            request.LoadOnlyIndividualClients = viewModel.LoadOnlyIndividualClients;
            request.MembershipId = viewModel.MembershipId;
            request.Username = viewModel.Username;
            request.UserListOption = viewModel.UserListOption ;
            request.ClientName = viewModel.ClientName;
            request.ClientCode  = viewModel.ClientCode;
            request.AccountStatus = viewModel.AccountStatus;
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            UsersViewModel model = (UsersViewModel)viewModel;
            ViewUsersRequest request = new ViewUsersRequest();
            MapParametersToRequest(model, request);
            request.IsReportRequest = true;
            UserService cs = new UserService ();
            request.SecurityUser = SecurityUser;
            ViewUsersResponse response = cs.ViewUsers(request);
            model.UsersView = response.UserGridView ;

            List<string[]> CSVData = new List<string[]>();
            string[] Header = { "Entry Date","Username", "Fullname", "Phone", "Gender", "Email", "isPaypoint", "Institution", "Status" };
            CSVData.Add(Header);
            foreach (var item in model.UsersView )
            {
                string[] Data = new string[]
                {
                    item.CreatedOn  .ToString (),
                    item.Username   ,
                    item.FullName  ,
                    item.Phone   ,
                    item.Gender ,
                    item.Email   ,
                    item.isPayPoint== true? "Yes":"No" ,
                    item.Institution,
                   item.IsActive ==true?"Yes":"No"
                    
                };

                CSVData.Add(Data);



            }

            return CSVData.ToArray();


            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {

            UsersViewModel model = (UsersViewModel)viewModel;
            ViewUsersRequest request = new ViewUsersRequest();
            MapParametersToRequest(model, request);
            UserService cs = new UserService();
            request.SecurityUser = SecurityUser;
            request.IsReportRequest = true;
            ViewUsersResponse response = cs.ViewUsers(request);            
            model.UsersView = response.UserGridView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Administration\\Users.rdlc";
            rdsPaymentView.Name = "dsUsers";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.UsersView ;
            ReportParameter parameter = new ReportParameter("ShowType", model.LoadOnlyIndividualClients == true? "1":"2");
            ReportParameter parameter1 = new ReportParameter("ReportTitle", request.CreatedRange != null ? request.CreatedRange.GenerateRangePhrase("System Users ") : " ");
            viewModel.reportViewer.LocalReport.SetParameters(parameter); 
            viewModel.reportViewer.LocalReport.SetParameters(parameter1);
            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);


            ReportParameter parameter2 = new ReportParameter("ClientMode",request .SecurityUser .IsOwnerUser ==false ? "1" :"2");
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
            UsersViewModel model = (UsersViewModel)viewModel;
            ReportDataSource rdsPaymentView = new ReportDataSource();

            reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.UsersView ;

            reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            reportViewer.LocalReport.Refresh();
        }



    }
}