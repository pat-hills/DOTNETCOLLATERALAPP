using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Service.Interfaces;
using CRL.Service.Messaging;
using CRL.Service.Messaging.Reporting.Response;
using CRL.UI.MVC.Models.Report.ViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.UI.MVC.Common;
using CRL.Model;
using System.Web.Mvc;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS.Enums;
using CRL.Model.Messaging;


namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class AuditViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

             var SecurityUser = (SecurityUser)HttpContext.Current.User;
            _AuditReportViewModel viewModel = new _AuditReportViewModel();

            viewModel.PartialViewName = "_AuditReport";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel._AuditReportViewModel";
            viewModel.Name = "Audit Report";
            PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            //Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            viewModel.AuditTypeList = response.AuditTypes.ToSelectListItem();
            viewModel.AuditActionList = response.AuditActionTypes.ToList();
            viewModel.Clients = response.Clients.ToSelectListItem();
            return viewModel;

            //Use here to load lookups

            

        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareAuditReportResponse response = new PrepareAuditReportResponse();
           
            _AuditReportViewModel model = (_AuditReportViewModel)viewModel;
            response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            //Prepare for audit report
            //model.UserList = response.Users.ToSelectList();
            model.AuditTypeList = response.AuditTypes.ToSelectListItem();
            model.AuditActionList = response.AuditActionTypes.ToList();
            model.Clients = response.Clients.ToSelectListItem();

            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {
           
            if (viewModel.GenerateDateRange() == null)
                ModelState.AddModelError("", "You must filter this report by the audit date!");

              
        }

        private bool MapParametersToRequest(_AuditReportViewModel viewModel, ViewAuditsRequest request)
        {

            request.MachineName = viewModel.Machiname;
            request.Message = viewModel.Message;
            request.UserId = viewModel.UserId;
            request.AuditAction = viewModel.Action;
            request.ClientId = viewModel.ClientId;
            request.LimitTo = viewModel.LimitTo;
            if (viewModel.AuditType != null && viewModel.AuditType.Count() > 0)
            {
                request.AuditTypeId = (Array.ConvertAll(viewModel.AuditType, value => (int?)value));
            }
            if (viewModel.AuditAction != null && viewModel.AuditAction.Count() > 0)
            {
                request.AuditActionTypes = (Array.ConvertAll(viewModel.AuditAction, value => (int?)value));
            }
            request.CreatedRange = viewModel.GenerateDateRange();
            
            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            _AuditReportViewModel model = (_AuditReportViewModel)viewModel;
             ViewAuditsRequest request = new ViewAuditsRequest();
            MapParametersToRequest(model,request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();           
            request.SecurityUser = SecurityUser;
            ViewAuditsResponse response = cs.ViewAudits(request);
            model.AuditViews = response.AuditViews;

            List<string[]> CSVData = new List<string[]>();
            
            string[] Header = { "Id", "Date", "Audit Type", "Action", "Machine Name", "Message" ,"Url","Performed by", "Performer Entity"};
            CSVData.Add(Header);
            foreach (var audit in model.AuditViews)
            {
                string[] Data = new string[]
                {
                    audit.Id.ToString(),
                    audit.AuditDate.ToString(),
                    audit.AuditType,
                    audit.AuditAction ,
                    audit.MachineName ,
                    audit.Message ,
                    audit.RequestUrl ,
                    audit .NameOfUser ,
                    audit.NameOfLegalEntity ?? "N/A"
                    
                };

                CSVData.Add(Data);
                
                    

            }

            return CSVData.ToArray();

        
            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override ResponseBase LoadReport(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            _AuditReportViewModel model = (_AuditReportViewModel)viewModel;



            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            ViewAuditsRequest request = new ViewAuditsRequest();
            MapParametersToRequest(model, request);       

            request.SecurityUser = SecurityUser;
            ViewAuditsResponse response = cs.ViewAudits(request);
            model.AuditViews = response.AuditViews;

            viewModel.reportViewer  = new ReportViewer();
            ReportDataSource rdsAuditView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";
            rdsAuditView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsAuditView.Value = model.AuditViews;
         
            ReportParameter parameter = new ReportParameter("ClientMode", request.SecurityUser.IsOwnerUser == false ? "1" : "2");
            ReportParameter parameter1 = new ReportParameter("ReportTitle", request.CreatedRange.GenerateRangePhrase("System Audits "));
            viewModel.reportViewer.LocalReport.SetParameters(parameter); 
            viewModel.reportViewer.LocalReport.SetParameters(parameter1);
            

            viewModel.reportViewer.LocalReport.DataSources.Add(rdsAuditView);
            PrepareAuditReportResponse response2 = new PrepareAuditReportResponse();

            response2 = cs.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            model.AuditTypeList = response2.AuditTypes.ToSelectListItem();
            model.AuditActionList = response2.AuditActionTypes.ToList();
            model.Clients = response2.Clients.ToSelectListItem();

            if (model.AuditType != null)
            {
                foreach (var i in model.AuditTypeList.Where(s => model.AuditType.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            //ResponseBase response2= new ResponseBase();
            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;

            //Now let's bind it to the reposrt source

            
            // throw new NotImplementedException();
            
        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            _AuditReportViewModel model = (_AuditReportViewModel)viewModel;
            ReportDataSource rdsAuditView = new ReportDataSource();

            reportViewer.LocalReport .ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            rdsAuditView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsAuditView.Value = model.AuditViews;

            reportViewer.LocalReport .DataSources.Add(rdsAuditView);
            reportViewer.LocalReport.Refresh();
        }

     
    }
}