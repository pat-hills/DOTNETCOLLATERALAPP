using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.UI.MVC.Common;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Institution.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.Infrastructure.Messaging;
using CRL.Model.Memberships.IRepository;
using CRL.Service.Messaging.Reporting.Response;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class LegalEntitiesViewModelHelper : ReportBaseViewModelHelper
    {


        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            LegalEntitiesViewModel viewModel = new LegalEntitiesViewModel();
            LegalEntitiesViewModel viewModelSpecific = (LegalEntitiesViewModel)viewModel;
            viewModelSpecific.PartialViewName = "_LegalEntities";
            viewModelSpecific.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.LegalEntitiesViewModel";
            viewModel.Name = "Financial Institution Clients Report";
            PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //viewModelSpecific.UserList = response.Users.ToSelectList();
            //viewModelSpecific.AuditTypeList = response.AuditTypes.ToSelectList();
            viewModel.SecuredPartyList = response.SecuredPartyList.ToSelectListItem();

            return viewModel;

            //Use here to load lookups



        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            //CreditActivitiesViewModel model = (CreditActivitiesViewModel)viewModel;
            LegalEntitiesViewModel view = new LegalEntitiesViewModel();
            response = service.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            ////Prepare for audit report
            //model.UserList = response.Users.ToSelectList();
            //model.AuditTypeList = response.AuditTypes.ToSelectList();
            view.SecuredPartyList = response.SecuredPartyList.ToSelectListItem();

            return viewModel;

            //Use here to load lookups



        }
        public override void ValidateModelState(ModelStateDictionary ModelState, ReportBaseViewModel viewModel)
        {

            //if (viewModel.GenerateDateRange() == null)
            //    ModelState.AddModelError("", "You must filter this report by the audit date!");


        }

        private bool MapParametersToRequest(LegalEntitiesViewModel viewModel, ViewClientInstitutionsRequest request)
        {

            request.CreatedRange = viewModel.GenerateDateRange();
            request.Email = viewModel.Email;
            request.CompanyName = viewModel.CompanyName;
            request.Phone = viewModel.Phone;
            request.SecuredPartyTypes = viewModel.SecuredPartyType;
            request.ClientCode = viewModel.ClientCode;
            request.MembershipStatus = viewModel.StatusType;
            if (viewModel.SecuredPartyType != null && viewModel.SecuredPartyType.Count() > 0)
            {
                request.SecuredPartyTypes = (Array.ConvertAll(viewModel.SecuredPartyType, value => (int?)value));
            }

            return true;
        }


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            LegalEntitiesViewModel model = (LegalEntitiesViewModel)viewModel;
            ViewClientInstitutionsRequest request = new ViewClientInstitutionsRequest();
            request.IsReportRequest = true;
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewClientInstitutionsResponse response = cs.ViewClientInstitutions(request);
            model.InstitutionsView = response.InstitutionGridView;

            List<string[]> CSVData = new List<string[]>();
            string[] Header = { "Entry Date", "Name", "Email", "Phone", "SecuredPartyType", "Status" };
            CSVData.Add(Header);
            foreach (var item in model.InstitutionsView)
            {
                string[] Data = new string[]
                {
                    item.CreatedOn  .ToString (),
                    item.Name    ,                   
                    item.Email   ,
                    item.Phone  ,
                    item.SecuredPartyType    ,                  
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

            LegalEntitiesViewModel model = (LegalEntitiesViewModel)viewModel;
            ViewClientInstitutionsRequest request = new ViewClientInstitutionsRequest();
            request.IsReportRequest = true;
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewClientInstitutionsResponse response = cs.ViewClientInstitutions(request);
            model.InstitutionsView = response.InstitutionGridView;

            viewModel.reportViewer = new ReportViewer();
            ReportDataSource rdsPaymentView = new ReportDataSource();

            viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\Administration\\LegalEntities.rdlc";
            rdsPaymentView.Name = "LegalEntities";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.InstitutionsView;

            ReportParameter parameter = new ReportParameter("ReportTitle", request.CreatedRange != null ? request.CreatedRange.GenerateRangePhrase("Financial Institutions ") : " ");
            viewModel.reportViewer.LocalReport.SetParameters(parameter);

            viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);

            PrepareAuditReportResponse response2 = new PrepareAuditReportResponse();

            response2 = cs.PrepareAuditReport(new RequestBase { SecurityUser = SecurityUser });
            model.SecuredPartyList = response2.SecuredPartyList.ToSelectListItem();
            //model.AuditActionList = response2.AuditActionTypes.ToList();

            if (model.SecuredPartyList != null && model.SecuredPartyType != null)
            {
                foreach (var i in model.SecuredPartyList.Where(s => model.SecuredPartyType.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }
            
            //if(model.StatusTypeList != null && model.StatusType != null)
            //{
            //    foreach (var i in model.StatusTypeList.Where(s => model.StatusType.Contains(Convert.ToInt32(s.Value))))
            //    {
            //        i.Selected = true;
            //    }
            //}
            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;

            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            LegalEntitiesViewModel model = (LegalEntitiesViewModel)viewModel;
            ReportDataSource rdsPaymentView = new ReportDataSource();

            reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            rdsPaymentView.Value = model.InstitutionsView;

            reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            reportViewer.LocalReport.Refresh();
        }



    }
}