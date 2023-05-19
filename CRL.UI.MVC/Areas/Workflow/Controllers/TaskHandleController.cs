using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Service.Implementations;
using CRL.Service.Interfaces;
using CRL.Service.Interfaces.FinancialStatement;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Messaging.Workflow.Response;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Workflow;
using CRL.UI.MVC.Areas.FinancialStatement.Models.FinancialStatementActivity.ViewPageModels.Shared;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ModelHelpers;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ViewPageModels;
using CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels;
using CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels.Shared;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using StructureMap;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Areas.Workflow.Controllers
{
    [MasterEventAuthorizationAttribute]
    public class TaskHandleController : CRLController
    {
     
       
     
        //
        // GET: /Workflow/TaskHandle/

        public ActionResult Index()
        {
        

            TaskIndexViewModel viewModel = new TaskIndexViewModel();
          
            return View(viewModel);
        }

        public PartialViewResult GetPartialViewOfTaskJqGrid(_TaskJqGridViewModel _TaskJqGridViewModel)
        {
            _TaskJqGridViewModel.TaskType = new Dictionary<string, string>();
            _TaskJqGridViewModel.TaskType.Add("1", "Registration of Financing Statement");
            _TaskJqGridViewModel.TaskType.Add("2", "Amendment or Cancellation of Financing Statement");
            _TaskJqGridViewModel.TaskType.Add("3", "Postpaid Membership Request");
            _TaskJqGridViewModel.TaskType.Add("4", "Registration of Client");
            _TaskJqGridViewModel.TaskType.Add("5", "Update of Financing Statement");
             _TaskJqGridViewModel.TaskType.Add("6", "Subordination of Financing Statement");
             _TaskJqGridViewModel.TaskType.Add("7", "Transfer of Financing Statement");
             _TaskJqGridViewModel.TaskType.Add("8", "Cancellation of Financing Statement");


            return PartialView("~/Areas/Workflow/Views/Shared/_TaskJqGrid.cshtml", _TaskJqGridViewModel);
         
        }

        public ActionResult GetJsonTaskGridForJQGRID(GridSettings grid, _TaskJqGridViewModel viewModel)
        {

            ViewMyTasksRequest request = new ViewMyTasksRequest();

            if (grid.IsSearch)
            {
                request.SubmittedBy = grid.Where.rules.Any(r => r.field == "SubmittedBy") ? grid.Where.rules.FirstOrDefault(r => r.field == "SubmittedBy").data : string.Empty;

                string LoanType = grid.Where.rules.Any(r => r.field == "CaseType") ?
                grid.Where.rules.FirstOrDefault(r => r.field == "CaseType").data : string.Empty;
                if (LoanType != String.Empty)
                {
                    request.CaseType  = Convert.ToInt32(LoanType);

                }

               
            }

            //Map view model to the rquest object
            //_FSJqGridViewModelHelper.MapViewModelToViewFSRequest(viewModel, request, grid);
            request.SecurityUser = SecurityUser;
            //request.SortColumn = grid.SortColumn;
            //request.SortOrder = grid.SortOrder;
            request.CreatedRange = viewModel.GenerateDateRange();
            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            //Service call
            IWorkflowService wf = ObjectFactory.GetInstance<IWorkflowService>();
            ViewMyTasksResponse response = wf.ViewMyTasks(request);
            TaskGridView[] ArrayRows = response.TaskGridView.ToArray();


            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Workflow/TaskHandle/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Workflow/TaskHandle/Create

        public ActionResult HandleTask(int CaseId)
        {
         

              //public static IFinancingStatementService fs;
           IWorkflowService  wf = ObjectFactory.GetInstance<IWorkflowService >();
           GetTaskHandleResponse response = wf.GetDataForTaskHandle(new GetTaskHandleRequest { CaseId = CaseId, SecurityUser = SecurityUser });

           if (response.MessageInfo.MessageType == MessageType.AlreadyHandled || response .MessageInfo .MessageType == MessageType.BusinessValidationError 
               || response .MessageInfo .MessageType == MessageType.Error )
           {
               //Go to the grid page with the error
               //Later we may not need to redirect
               TempData["TitleMessage"] = response.MessageInfo;
               return RedirectToAction("Index", "TaskHandle", new { Area = "Workflow" });
           }
            //**Check if case is closed and if it is then go to the error page
            TaskHandleViewModel viewModel = new TaskHandleViewModel();
            viewModel._TaskHandleViewModel = new Models.ViewPageModels.Shared._TaskHandleViewModel();
            viewModel._TaskHandleViewModel.TaskHandleView = response.TaskHandleView;
            viewModel ._TaskHandleViewModel.Tasks = response.TaskHandleView.Tasks.ToSelectList ();
            if (response.isPayable )
            {
                viewModel._TaskHandleViewModel.isPayable = response.isPayable;
                viewModel._TaskHandleViewModel.isPostpaid = response.isPostpaid;
                viewModel._TaskHandleViewModel.Amount = response.Amount;
                viewModel._TaskHandleViewModel.CurrentCredit = response.CurrentCredit;
            }
            viewModel.CaseId = CaseId;
            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatementActivity)
            {

                if (response.FSActivityDetailSummaryView is RenewalFSActivitySummaryView)
                {
                    RenewalFSActivitySummaryView _fsActivityView = ((RenewalFSActivitySummaryView)response.FSActivityDetailSummaryView);

                    _SummaryViewRenewalViewModel _summaryViewModel = new _SummaryViewRenewalViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;

                    _summaryViewModel.Date1 = _fsActivityView.BeforeExpiryDate;
                    _summaryViewModel.Date2 = _fsActivityView.AfterExpiryDate;

                }
                else if (response.FSActivityDetailSummaryView is UpdateFSActivitySummaryView)
                {
                    UpdateFSActivitySummaryView _fsActivityView = ((UpdateFSActivitySummaryView)response.FSActivityDetailSummaryView);

                    _SummaryViewUpdateViewModel _summaryViewModel = new _SummaryViewUpdateViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;

                    _summaryViewModel.UpdateChangeDescription = _fsActivityView.UpdateChangeDescription;
                    viewModel.NewAndPreviousFS = new NewAndPreviousFSViewModel ();
                    //viewModel.NewAndPreviousFS.AfterUpdateFS = new FSViewModel();
                    viewModel.NewAndPreviousFS.AfterUpdateFS  = new FSViewModel();
                    viewModel.NewAndPreviousFS.BeforeUpdateFS  = new FSViewModel();
                    viewModel.NewAndPreviousFS.AfterUpdateFS.Val  = "2";
                    viewModel.NewAndPreviousFS.BeforeUpdateFS.Val  = "1";


                    FSViewModelHelper.BuildViewModelForView(viewModel.NewAndPreviousFS.BeforeUpdateFS, response.BeforeUpdateFSView, response.SectorsOfOperation);
                    //FSViewModelHelper.BuildViewModelForView(viewModel.NewAndPreviousFS.AfterUpdateFS , response.);
                    FSViewModelHelper.BuildViewModelForView(viewModel.NewAndPreviousFS.AfterUpdateFS, response.FSView, response.SectorsOfOperation);

                    if (!(viewModel.NewAndPreviousFS.BeforeUpdateFS.FSView.FinancialStatementTransactionTypeId == FinancialStatementTransactionCategory.Lien))
                    {
                        viewModel.NewAndPreviousFS.BeforeUpdateFS._ParticipantViewModel.SectorOfOperationList = response.SectorsOfOperation;
                        viewModel.NewAndPreviousFS.AfterUpdateFS._ParticipantViewModel.SectorOfOperationList = response.SectorsOfOperation;
                    }


                }
                else if (response.FSActivityDetailSummaryView is DischargeFSActivitySummaryView)
                {
                    DischargeFSActivitySummaryView _fsActivityView = ((DischargeFSActivitySummaryView)response.FSActivityDetailSummaryView);


                    _SummaryViewDischargeViewModel _summaryViewModel = new _SummaryViewDischargeViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;

                    _summaryViewModel.DischargeType = _fsActivityView.DischargedTypeName;
                    _summaryViewModel.PartiallyDischargedCollaterals = _fsActivityView.PartialDischargedCollaterals;

                }
                else if (response.FSActivityDetailSummaryView is DischargeFSDueToErrorActivitySummaryView)
                {
                    DischargeFSDueToErrorActivitySummaryView _fsActivityView = ((DischargeFSDueToErrorActivitySummaryView)response.FSActivityDetailSummaryView);


                    _SummaryViewDischargeDueToErrorViewModel _summaryViewModel = new _SummaryViewDischargeDueToErrorViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;


                }
                else if (response.FSActivityDetailSummaryView is SubordinateFSActivitySummaryView)
                {
                    SubordinateFSActivitySummaryView _fsActivityView = ((SubordinateFSActivitySummaryView)response.FSActivityDetailSummaryView);


                    _SummaryViewSubordinateViewModel _summaryViewModel = new _SummaryViewSubordinateViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;


                   

                    _summaryViewModel.SubordinatingPartyView = _fsActivityView.SubordinatingPartyView;
                    if (_fsActivityView.SubordinatingPartyView is IndividualSubordinatingPartyView)
                    {
                        _summaryViewModel.SubordinateToIndividual = true;
                    }
                
                }
                else if (response.FSActivityDetailSummaryView is AssignmentFSActivitySummaryView)
                {
                    AssignmentFSActivitySummaryView _fsActivityView = ((AssignmentFSActivitySummaryView)response.FSActivityDetailSummaryView);
                    _SummaryViewAssignmentViewModel _summaryViewModel = new _SummaryViewAssignmentViewModel();
                    viewModel._SummaryViewAmendmentViewModel = _summaryViewModel;
                    _summaryViewModel.AssignmentType = _fsActivityView.AssignmentType;
                    _summaryViewModel.Assignee = _fsActivityView.Assignee;
                    _summaryViewModel.Assignor = _fsActivityView.Assignor;
                    _summaryViewModel.AssigneeClientCode = _fsActivityView.AssigneeDetails.ClientCode;
                    _summaryViewModel.AssignorClientCode  = _fsActivityView.AssignorDetails.ClientCode;
                    _summaryViewModel.AssignmentDescription = _fsActivityView.AssignmentDescription;

                }

                viewModel._SummaryViewAmendmentViewModel.ActivityCode = response.FSActivityDetailSummaryView.ActivityCode;
                viewModel._SummaryViewAmendmentViewModel.ActivityDate = response.FSActivityDetailSummaryView.ActivityDate;




            }
            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatement ||
                (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatementActivity && !(response.FSActivityDetailSummaryView is UpdateFSActivitySummaryView)))
            {
                viewModel.FSViewModel = new FSViewModel();
                FSViewModelHelper.BuildViewModelForView(viewModel.FSViewModel, response.FSView, response.SectorsOfOperation);

                if (!(viewModel.FSViewModel.FSView.FinancialStatementTransactionTypeId == FinancialStatementTransactionCategory.Lien))
                {
                    viewModel.FSViewModel._ParticipantViewModel.SectorOfOperationList = response.SectorsOfOperation;
                }
            }

            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.Membership)
            {

                viewModel._SetupPostpaidMembershipViewModel = new Membership.Models.ViewPageModels.Client._SetupPostpaidMembershipViewModel();
                viewModel._SetupPostpaidMembershipViewModel.MembershipView = response.MembershipView;
                if (response.UserView != null)
                {
                    viewModel ._UserViewModel = new Membership.Models.ViewPageModels.Shared.User._UserViewModel ();
                    viewModel._UserViewModel .UserView = response.UserView ;
                }
                else if (response.InstitutionView != null)
                {
                    viewModel._InstitutionViewModel = new Membership.Models.ViewPageModels.Shared.Institution._InstitutionViewModel();
                    viewModel._InstitutionViewModel.InstitutionView = response.InstitutionView ;
                }

                   


            }

            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.Membership)
            {

                viewModel._SetupPostpaidMembershipViewModel = new Membership.Models.ViewPageModels.Client._SetupPostpaidMembershipViewModel();
                viewModel._SetupPostpaidMembershipViewModel.MembershipView = response.MembershipView;
                if (response.UserView != null)
                {
                    viewModel._UserViewModel = new Membership.Models.ViewPageModels.Shared.User._UserViewModel();
                    viewModel._UserViewModel.UserView = response.UserView;
                }
                else if (response.InstitutionView != null)
                {
                    viewModel._InstitutionViewModel = new Membership.Models.ViewPageModels.Shared.Institution._InstitutionViewModel();
                    viewModel._InstitutionViewModel.InstitutionView = response.InstitutionView;
                }

               




            }

            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.MembershipRegistration )
            {

              
                if (response.UserView != null)
                {
                    viewModel._UserViewModel = new Membership.Models.ViewPageModels.Shared.User._UserViewModel();
                    viewModel._UserViewModel.UserView = response.UserView;
                }
                 if (response.InstitutionView != null)
                {
                    viewModel._InstitutionViewModel = new Membership.Models.ViewPageModels.Shared.Institution._InstitutionViewModel();
                    viewModel._InstitutionViewModel.InstitutionView = response.InstitutionView;
                }




            }

            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.PaypointUserAssigment)
            {

                if (response.UserGridView != null)
                {
                    viewModel._UsersListViewModel = new Membership.Models.ViewPageModels.Shared.User._UsersListViewModel();
                    viewModel._UsersListViewModel.UsersGridViewModel = response.UserGridView;
                
                }        
            }

            //Generate a unique token for this request and add it to the view model
            viewModel.UniqueGuidForm = Guid.NewGuid().ToString("N") + this.SecurityUser.Id .ToString("0000");



            //Mandatory Actions
          //  viewModel.Actions = new int[] {48, 49}; 
            viewModel.Actions = viewModel._TaskHandleViewModel.TaskHandleView.MandatoryCommentTasks;
            return View(viewModel);
        }

        //
        // POST: /Workflow/TaskHandle/Create

        [HttpPost]
        public ActionResult HandleTask(TaskHandleViewModel viewModel)
        {
            //We need to first of all see which and redirect to the appropirate
            //**We need to review this action for now please just add the comment for the action at the front end
            //Task handler section needs serious review
            HandleWorkItemRequest request = new HandleWorkItemRequest();
            request.Comment = viewModel._TaskHandleViewModel.TaskHandleView.Comment;
            request.WorkItemId = viewModel._TaskHandleViewModel.TaskHandleView.SelectedTask;
            request.WorkflowId = viewModel._TaskHandleViewModel.TaskHandleView.WorkflowId.Value;
            request.TransitionId = Convert.ToInt32(viewModel._TaskHandleViewModel.TaskHandleView.TaskTransitions.Where(s => Convert.ToInt32(s.Split(',').GetValue(0)) == request.WorkItemId).Select(e => e.Split(',').GetValue(1)).Single());
            request.CaseId = viewModel.CaseId;
            request.wfTaskType = viewModel._TaskHandleViewModel .TaskHandleView .WFTaskType ;
            request.SecurityUser = SecurityUser;
            request.UniqueGuidForm = viewModel.UniqueGuidForm;
            request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
            request.UserIP = AuditHelper.GetUserIP(this.Request);
            ResponseBase response = new ResponseBase();
            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatement)
            {

                IFinancingStatementService fs = FS;
               

                int numRetries = 0;
                do
                {
                    numRetries++;
                   
                    response = fs.HandleTask(request);
                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2)
                    {
                        fs = FS;
                    }



                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2);

               
             
            }
            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.FinancialStatementActivity)
            {

                IFinancingStatementService fs = FS;
              

                int numRetries = 0;
                do
                {
                    numRetries++;                    
                    response = fs.HandleTask(request);
                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2)
                    {
                        fs = FS;
                    }



                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2);

               
                
               
              
            }

            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.Membership)
            {
               
                IClientService cs = ObjectFactory.GetInstance<IClientService>();
                response = cs.HandleTask(request);
               
                
            }
            if (viewModel._TaskHandleViewModel.TaskHandleView.CaseType == Model.WorkflowEngine.Enums.WorkflowRequestType.MembershipRegistration)
            {

                IMembershipRegistrationService cs = ObjectFactory.GetInstance<IMembershipRegistrationService>();
                request.Misc = Url.Action("Login", "Account", new { Area = "" }, Request.Url.Scheme);
                response = cs.HandleTask(request);


            }
            if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict)
            {
                response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                response.MessageInfo.Message = "An error occured whiles trying to submit your request.  Please try again!";
            }

            if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
              response.MessageInfo.MessageType == MessageType.Error ||response .MessageInfo .MessageType == MessageType.AlreadyHandled )
            {
                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = response.MessageInfo.Message
                };
                if (String.IsNullOrWhiteSpace(_msg.Message))
                {
                    _msg.Message = "Business Validation Errors were detected";
                }
                TempData["TitleMessage"] = response.MessageInfo;
                return RedirectToAction("HandleTask", "TaskHandle", new { Area = "Workflow", CaseId = viewModel.CaseId });
            }
            else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            else
            {
                //Later we may not need to redirect
                TempData["TitleMessage"] = response.MessageInfo;
                return RedirectToAction("Index", "TaskHandle", new { Area = "Workflow" });
                
            }
            
        }


        //
        // GET: /Workflow/TaskHandle/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Workflow/TaskHandle/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Workflow/TaskHandle/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Workflow/TaskHandle/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
