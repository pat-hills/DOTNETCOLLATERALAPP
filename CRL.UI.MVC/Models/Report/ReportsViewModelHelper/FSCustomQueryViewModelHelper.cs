using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging.Reporting.Response;
using CRL.UI.MVC.Models.Report.ReportsViewModel;
using Microsoft.Reporting.WebForms;
using StructureMap;
using CRL.UI.MVC.Common;

using CRL.Model.Common.Enum;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS.Enums;
using CRL.Model.Messaging;
using CRL.Service.Messaging.Reporting.Request;


namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public class FSCustomQueryViewModelHelper : ReportBaseViewModelHelper
    {
     

        public override ReportBaseViewModel BuildModel(Service.Interfaces.IReportService service, int ReportId)
        {

            var SecurityUser = (SecurityUser)HttpContext.Current.User;
            FSCustomQueryViewModel viewModel = new FSCustomQueryViewModel();
          //  viewModel.ShowType =1;

            viewModel.PartialViewName = "_FSCustomQuery";
            viewModel.ViewModelType = "CRL.UI.MVC.Models.Report.ReportsViewModel.FSCustomQueryViewModel";
            PrepareCustomQueryReportResponse response = new PrepareCustomQueryReportResponse();
            viewModel.Name = "Financing Statement Custom Query Report";
            response = service.PrepareFSCustomQueryReport(new RequestBase { SecurityUser = SecurityUser });
            viewModel.Countries = response.Countries.ToSelectListItem();
            viewModel.Countys = response.Countys.ToSelectListItem();
            viewModel.MaximumAmountSecuredCurrencyList = response.Currencies.ToSelectListItem();
            viewModel.CollateralSubTypesList = response.CollateralSubTypes.ToSelectListItem();
            viewModel.SectorOfOperationList = response.SectorsOfOperation.ToSelectListItem();
            response.SecuringPartyIndustryTypes.Add(new Infrastructure.Helpers.LookUpView { LkValue = 0, LkName = "Individual" });
            response.DebtorTypes.Add(new Infrastructure.Helpers.LookUpView { LkValue = 0, LkName = "Individual" });
            viewModel.DebtorTypes = response.DebtorTypes.ToSelectListItem();
            viewModel.SecuredPartyList = response.SecuringPartyIndustryTypes.ToSelectListItem ();
            viewModel.Clients = response.Clients.ToSelectListItem();

            
         
            return viewModel;

        }
        public override ReportBaseViewModel BuildModelAfterError(Service.Interfaces.IReportService service, ReportBaseViewModel viewModel)
        {
            //var SecurityUser = (SecurityUser)HttpContext.Current.User;
            //PrepareAuditReportResponse response = new PrepareAuditReportResponse();

            //FSCustomQueryViewModel model = (FSCustomQueryViewModel)viewModel;
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

        private bool MapParametersToRequest(FSCustomQueryViewModel viewModel, ViewFSCustomQueryRequest request)
        {
            request.ShowListOrStatistical = viewModel.ReportType == 1 ? 1 : 2;
            request.GroupBy = (Service.Messaging.Reporting.Request.GroupByNoOfFSStat)viewModel.GroupedBy;

            request.IncludeSections = viewModel.ShowSection;
            

            if (viewModel.FinancialStatementTransactionTypeId != null && viewModel.FinancialStatementTransactionTypeId.Count() >0)
            {

                request.FinancialStatementTransactionTypeId = Array.ConvertAll(viewModel.FinancialStatementTransactionTypeId, value => (FinancialStatementTransactionCategory)value);
            }
            if (viewModel.FinancialStatementLoanTypeId != null && viewModel.FinancialStatementLoanTypeId.Count() > 0)
            {

                request.FinancialStatementLoanTypeId  = Array.ConvertAll(viewModel.FinancialStatementLoanTypeId, value => (System.Nullable<FinancialStatementLoanCategory>)value);
            }
            if (viewModel.CollateralTypeId != null && viewModel.CollateralTypeId.Count() > 0)
            {

                request.CollateralTypeId = Array.ConvertAll(viewModel.CollateralTypeId, value => (CollateralCategory)value);
            }
            if (viewModel.CollateralSubtypeId != null && viewModel.CollateralSubtypeId.Count() > 0)
            {

                request.CollateralSubTypeId  = Array.ConvertAll(viewModel.CollateralSubtypeId, value => (int)value);
            }
            if (viewModel.SectorOfOperationId != null && viewModel.SectorOfOperationId.Count() > 0)
            {

                request.SectorOfOperationId = Array.ConvertAll(viewModel.SectorOfOperationId, value => (int?)value);
            }
            if (viewModel.DebtorTypeId != null && viewModel.DebtorTypeId .Count() > 0)
            {

                request.DebtorTypeId = Array.ConvertAll(viewModel.DebtorTypeId, value => (DebtorCategory?)value);
            }
            if (viewModel.SecuredPartyTypeId  != null && viewModel.SecuredPartyTypeId.Count() > 0)
            {

                request.SecuredPartyTypeId  = Array.ConvertAll(viewModel.SecuredPartyTypeId , value => (int?)value);
            }
            if (viewModel.MaximumCurrency  != null && viewModel.MaximumCurrency .Count() > 0)
            {

                request.MaximumCurrency  = Array.ConvertAll(viewModel.MaximumCurrency , value => (int?)value);
            }

            if(viewModel.DebtorCountryId != null && viewModel.DebtorCountryId.Count() > 0)
            {
                request.DebtorCountryId = viewModel.DebtorCountryId;
            }

            if (viewModel.DebtorCountyId != null && viewModel.DebtorCountyId.Count() > 0)
            {
                request.DebtorCountyId = Array.ConvertAll(viewModel.DebtorCountyId, value => (int?)value);
            }

            request.FSState = viewModel.FSState;
            request.MajorityOwnershipId = viewModel.MajorityOwnershipId;
            request.ExistingRelationshipId = viewModel.ExistingRelationshipId;
            if (viewModel.ReportType == 1)
            {
                if (viewModel.LimitToFirstItem != null && viewModel.LimitToFirstItem.Count() > 0)
                {
                    request.LimitToFirstDebtor = viewModel.LimitToFirstItem.Contains(1);
                    request.LimitToFirstSecuredParty = viewModel.LimitToFirstItem.Contains(2);
                }
            }
            else
            {
                request.LimitToFirstDebtor = true;
                request.LimitToFirstSecuredParty = true;

            }
          
            request.RegistrationDate  = viewModel.GenerateDateRange();
            request.FSStateType = viewModel.FSStateType;
            request.ClientId = viewModel.ClientId;
            ReportsViewModelHelper.MapBaseParametersToRequestBase(viewModel, request);
            return true;
        }

     


        public override string[][] LoadCSV(ReportBaseViewModel viewModel, SecurityUser SecurityUser)
        {
            //FSCustomQueryViewModel model = (FSCustomQueryViewModel)viewModel;
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

            FSCustomQueryViewModel model = (FSCustomQueryViewModel)viewModel;

           

            ViewFSCustomQueryRequest request = new ViewFSCustomQueryRequest();
            //////////////
           
            ///////////////////
            MapParametersToRequest(model, request);
            IReportService cs = ObjectFactory.GetInstance<IReportService>();
            request.SecurityUser = SecurityUser;
            ViewFSCustomQueryResponse  response = cs.ViewFSCustomQuery2(request);
            viewModel.reportViewer = new ReportViewer();
            if (model.ReportType ==1)
            {
               
                model.FSCustomReportView  = response.FSCustomReportView;
                //FSCustomQuerySubReportEvent subReportEvent = new FSCustomQuerySubReportEvent();
                //subReportEvent.SecuredParties = response.SecuredPartyReportView;
                //subReportEvent.Debtors = response.DebtorReportView;
                //subReportEvent.Collaterals = response.CollateralReportView;

            
                //viewModel.SubreportEventClass = subReportEvent;

                //            viewModel .reportViewer .LocalReport.
                //        viewModel.reportViewer.LocalReport.SubreportProcessing += new
                //SubreportProcessingEventHandler(SubReportProcessingEventHandler);
                ReportDataSource rdsPaymentView = new ReportDataSource();

                //viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FS.rdlc";
                viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FSListCustom.rdlc";
                rdsPaymentView.Name = "dsFS";  //This refers to the dataset name in the RDLC file
                if (model.ShowSection == null)
                    model.ShowSection = new int[]{0};
                ReportParameter parameter = new ReportParameter("ShowSecuredParty",model.ShowSection .Contains (1) ? "true":"false" );
                ReportParameter parameter1 = new ReportParameter("ShowDebtor", model.ShowSection.Contains (2) ? "true" : "false");
                ReportParameter parameter2 = new ReportParameter("ShowCollateral", model.ShowSection.Contains (3) ? "true" : "false");
                viewModel.reportViewer.LocalReport.SetParameters(parameter);
                viewModel.reportViewer.LocalReport.SetParameters(parameter1);
                viewModel.reportViewer.LocalReport.SetParameters(parameter2);
                rdsPaymentView.Value = model.FSCustomReportView;
                viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            }
            else 
            {
                model.FSCustomQueryStatView  = response.FSCustomQueryStatView ;
                ReportDataSource rdsPaymentView = new ReportDataSource();
                ReportParameter parameter = new ReportParameter("GroupBy", model.GroupedBy == 1 ? "Debtor Type" :
                    model.GroupedBy == 2 ? "Secured Creditor Type" :
                      model.GroupedBy == 4 ? "Debtor State" :
                       model.GroupedBy == 5 ? "Secured Creditor State" :
                        model.GroupedBy == 6 ? "Registrant" :
                        model.GroupedBy == 8 ? "Registrant Type" : "Unknown"
                );
                if (model.ReportType == 3)
                    viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FSCustomRawStat.rdlc";
                else if (model.StatisticalReportType == 1 || model.StatisticalReportType == 3)
                {

                    ReportParameter parameter2 = new ReportParameter("ShowNoOfFS", model.StatisticalReportType == 1 ? "true" : "false");
                    viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FSCustomStatbyValueNo.rdlc";
                    viewModel.reportViewer.LocalReport.SetParameters(parameter2);
                }
                else
                 viewModel.reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "\\FinancingStatement\\FSCustomStatbyNo.rdlc";
                viewModel.reportViewer.LocalReport.SetParameters(parameter);
                rdsPaymentView.Name = "dsFSCustomQueryStatView";  //This refers to the dataset name in the RDLC file
                rdsPaymentView.Value = model.FSCustomQueryStatView ;
                viewModel.reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            }
          

         
        

          
        


            PrepareCustomQueryReportResponse response2 = new PrepareCustomQueryReportResponse();

            response2 = cs.PrepareFSCustomQueryReport(new RequestBase { SecurityUser = SecurityUser });
            model.Countries = response2.Countries.ToSelectListItem();
            model.Countys = response2.Countys.ToSelectListItem();
            model.MaximumAmountSecuredCurrencyList = response2.Currencies.ToSelectListItem();
            model.CollateralSubTypesList = response2.CollateralSubTypes.ToSelectListItem();
            model.SectorOfOperationList = response2.SectorsOfOperation.ToSelectListItem();
            model.Clients = response2.Clients.ToSelectListItem();
            //foreach (var sectors in model.SectorOfOperationList)
            //{
            //    if (model.SectorOfOperationId.Contains(Convert.ToInt32(sectors.Value)))
            //        sectors .Selected =true;

            //}
            response2.SecuringPartyIndustryTypes.Add(new Infrastructure.Helpers.LookUpView { LkValue = 0, LkName = "Individual" });
            response2.DebtorTypes.Add(new Infrastructure.Helpers.LookUpView { LkValue = 0, LkName = "Individual" });
            model.DebtorTypes = response2.DebtorTypes.ToSelectListItem ();
            model.SecuredPartyList = response2.SecuringPartyIndustryTypes.ToSelectListItem ();
            if (model.FinancialStatementTransactionTypeId != null)
            {
                foreach (var i in model.FSTransactionTypeList.Where(s => model.FinancialStatementTransactionTypeId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.FinancialStatementLoanTypeId  != null)
            {
                foreach (var i in model.FSLoanTypeList.Where(s => model.FinancialStatementLoanTypeId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.MaximumCurrency  != null)
            {
                foreach (var i in model.MaximumAmountSecuredCurrencyList .Where(s => model.MaximumCurrency .Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.SecuredPartyTypeId  != null)
            {
                foreach (var i in model.SecuredPartyList.Where(s => model.SecuredPartyTypeId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
              
            }

            if (model.DebtorTypeId  != null)
            {
                foreach (var i in model.DebtorTypes.Where(s => model.DebtorTypeId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
             
            }

            if (model.DebtorCountyId  != null)
            {
                foreach (var i in model.Countys.Where(s => model.DebtorCountyId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.DebtorCountryId  != null)
            {
                foreach (var i in model.Countries.Where(s => model.DebtorCountryId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.SectorOfOperationId  != null)
            {
                foreach (var i in model.SectorOfOperationList .Where(s => model.SectorOfOperationId .Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }


            if (model.CollateralSubtypeId  != null)
            {
                foreach (var i in model.CollateralSubTypesList.Where(s => model.CollateralSubtypeId.Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.CollateralTypeId  != null)
            {
                foreach (var i in model.CollateralTypeList .Where(s => model.CollateralTypeId .Contains(Convert.ToInt32(s.Value))))
                {
                    i.Selected = true;
                }
            }

            if (model.ShowSection  != null)
            {
                foreach (var i in model.ShowSectionList)
                {
                    if (model.ShowSection .Contains (Convert.ToInt32(i.Value)))
                        i.Selected = true;
                    else
                        i.Selected = false;
                }
            }

            if (model.LimitToFirstItem  != null)
            {
                foreach (var i in model.LimitToFirstItemList )
                {
                    if (model.LimitToFirstItem.Contains(Convert.ToInt32(i.Value)))
                        i.Selected = true;
                    else
                        i.Selected = false;
                }
            }

            //Map objects for Serverside Pagination
            ReportsViewModelHelper.MapResponseToBaseParameters(model, response);

            ResponseBase response3 = new ResponseBase();
            response2.MessageInfo.Message = "Success";
            response2.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response2;

            //Now let's bind it to the reposrt source


            // throw new NotImplementedException();

        }

        public override void ProcessReport(ReportBaseViewModel viewModel, ReportViewer reportViewer)
        {
            //FSCustomQueryViewModel model = (FSCustomQueryViewModel)viewModel;
            //ReportDataSource rdsPaymentView = new ReportDataSource();

            //reportViewer.LocalReport.ReportPath = Constants.GetReportPath + "AuditReport.rdlc";

            //rdsPaymentView.Name = "dsAuditReport";  //This refers to the dataset name in the RDLC file
            //rdsPaymentView.Value = model.PaymentsView;

            //reportViewer.LocalReport.DataSources.Add(rdsPaymentView);
            //reportViewer.LocalReport.Refresh();
        }


    }
}