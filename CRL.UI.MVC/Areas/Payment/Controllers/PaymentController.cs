using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Views.Payments;
using CRL.UI.MVC.Areas.Payment.Models.ModelHelpers;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using StructureMap;
using CRL.Model.Messaging;
using CRL.UI.MVC.Common.CustomActionFilters;
using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.Payments;

using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Model.Payments;
using CRL.Service.Messaging.Common.Request;

namespace CRL.UI.MVC.Areas.Payment.Controllers
{
    [MasterEventAuthorizationAttribute]
    public class PaymentController : CRLController 
    {
      
        //
        // GET: /Payment/Payment/

        public ActionResult Index(int ShowType)
        {


            PaymentIndexViewModel viewModel = new PaymentIndexViewModel();
            var guid = Guid.NewGuid().ToString("N") + SecurityUser.Id.ToString("0000");

            viewModel._PaymentJqGridViewModel = new _PaymentJqGridViewModel();
            viewModel._PaymentJqGridViewModel.UniqueGuidForm = guid;
            viewModel._PaymentJqGridViewModel.ShowType = ShowType;

            return View(viewModel);
        }

        public PartialViewResult GetPartialViewOfPaymentIndexGrid(_PaymentJqGridViewModel _PaymentJqGridViewModel)
        {
            //GetDataForFSViewResponse response = fs.GetDataForFSView();
            //_FSJqGridViewModel.FinancialStatementLoanType = response.FinancialStatementLoanType.ToDictionaryString();
            //_FSJqGridViewModel.FinancialStatementTransactionTypes = response.FinancialStatementTransactionTypes.ToDictionaryString();
            //_FSJqGridViewModel.CollateralTypes = response.CollateralTypes.ToDictionaryString();
            //_FSJqGridViewModel.InRequestMode = InRequestMode;

            return PartialView("~/Areas/Payment/Views/Shared/_PaymentJqGrid.cshtml", _PaymentJqGridViewModel);
        }

        public ActionResult GetJsonPaymentGridForJQGRID(GridSettings grid, _PaymentJqGridViewModel viewModel)
        {

            ViewPaymentsRequest request = new ViewPaymentsRequest();

            //Map view model to the rquest object
            _PaymentJqGridViewModelHelper.MapViewModelToViewPaymentRequest(viewModel, request, grid);
            request.SecurityUser = SecurityUser;
            //Service call
            ViewPaymentsResponse response = PY.ViewPayments(request);
            PaymentView[] ArrayRows = response.PaymentViews.ToArray();


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
        // GET: /Payment/Payment/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }



        public ActionResult SelectRegisteredClient()
        {
            if (!SecurityUser.IsPaypointUser)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            SelectRegisteredClientViewModel viewModel = new SelectRegisteredClientViewModel();
            GetDataForSelectClientResponse response = PY.GetDataForSelectClient();
            viewModel.LegalEntityClientList = response.LegalEntityClients.ToSelectList();
            viewModel.IndividualClientList = response.IndividualClients.ToSelectList();
            viewModel.IsSelectedIndividual = 2;
            viewModel.SearchByClientCodeOrName = 1;
            return View(viewModel);
        }

        // POST: /Membership/MembershipRegistration/
        [HttpPost]
        public ActionResult SelectRegisteredClient(SelectRegisteredClientViewModel viewModel, string submitButton)
        {
            if (!SecurityUser.IsPaypointUser)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            if (submitButton == "Find Client")
            {
                // if (viewModel.RegisteredOrPublicUser == true)
                //{

                //    return RedirectToAction("CreatePayment");

                // }

                //Validate here
                GetClientSummaryViewRequest request = new GetClientSummaryViewRequest();
                if (viewModel.SearchByClientCodeOrName == 1)
                {

                    request.ClientCode = viewModel.SearchClientCode;
                }
                else
                {
                    if (viewModel.IsSelectedIndividual == 1)
                    {
                        request.MembershipId = viewModel.SelectedIndividualMembershipId;
                    }
                    else
                    {
                        request.MembershipId = viewModel.SelectedLegalEntityMembershipId;
                    }
                }

                viewModel.ClientSummaryView = PY.GetClientSummaryView(request).ClientSummaryView;
                if (viewModel.ClientSummaryView == null)
                {
                    MessageInfo _msg = new MessageInfo
                    {
                        MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                        Message = "Client code does not exist!"
                    };
                    ViewBag.TitleMessage = _msg;
                }
            }
            else
            {

                if (viewModel.ClientSummaryView != null)
                {
                    return RedirectToAction("CreatePayment", new { MembershipId = viewModel.ClientSummaryView.MembershipId });
                }


            }
            GetDataForSelectClientResponse response = PY.GetDataForSelectClient();
            viewModel.LegalEntityClientList = response.LegalEntityClients.ToSelectList();
            viewModel.IndividualClientList = response.IndividualClients.ToSelectList();
            return View(viewModel);


        }


        [HttpPost]
        public ActionResult ReversePayment(int Id, string UniqueGuidForm)
        {

            //Not supported
            return RedirectToAction("Error", "Error", new { Area = "", Error = "Not supported or invalid operation" });


            ResponseBase response;
            int numRetries = 0;
            do
            {





                numRetries++;
                response = PY.MakeReversalPayment(new RequestBase
                {
                    Id = Id,
                    SecurityUser = this.SecurityUser,
                    RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath,
                    UserIP = AuditHelper.GetUserIP(this.Request),
                    UniqueGuidForm = UniqueGuidForm
                });
                if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2)
                {
                    this.NewPaymentService (PY);
                }
                numRetries++;
            } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2);

            if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict)
            {
                response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                response.MessageInfo.Message = "An error occured whiles trying to submit your request.  Please try again!";
            }

            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
            {
                string result = response.MessageInfo.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            else
            {

                string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "failure" : response.MessageInfo.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Payment/Payment/Create

        public ActionResult CreatePayment(int? MembershipId = null)
        {
            if (!SecurityUser.IsPaypointUser)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            PaymentViewModel viewModel = new PaymentViewModel();
            var guid = Guid.NewGuid().ToString("N") + SecurityUser.Id.ToString("0000");
            viewModel.UniqueGuidForm = guid;
            //Make sure this is a paypoint officer

            viewModel.PaymentView = new PaymentView();
            viewModel.PaymentView.PaymentDate = DateTime.Now;


            viewModel.PaymentView.Amount = 0;
            //viewModel.PaymentView.AdjustmentPayment = false;
            if (MembershipId != null)
            {
                viewModel.PaymentView.MembershipId = MembershipId;

                viewModel.PaymentView.IsPublicUser = false;
            }
            else
            {
                viewModel.PaymentView.IsPublicUser = true;
            }
            return View(viewModel);
        }

        //
        // POST: /Payment/Payment/Create
        [HttpPost, CaptchaMvc.Attributes.CaptchaVerify("The security code you entered was incorrect!")]
        public ActionResult CreatePayment(PaymentViewModel viewModel)
        {

            if (viewModel.PaymentView.Amount == 0)
            {

                ModelState.AddModelError("PaymentView.Amount", "Amount cannot be zero!");
            }

            if (ModelState.IsValid)
            {

                MakePaymentRequest request = new MakePaymentRequest();
                request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
                request.UserIP = AuditHelper.GetUserIP(this.Request);
                request.PaymentView = viewModel.PaymentView;
                request.UrlLink = Url.Action("Search", "Search", new { Area = "Search" }, Request.Url.Scheme);
                request.SecurityUser = this.SecurityUser;
                request.UniqueGuidForm = viewModel.UniqueGuidForm;
                request.PaymentView.PaymentSource =PaymentSource.Paypoint;
                MakePaymentResponse response;
                int numRetries = 0;
                do
                {


                    numRetries++;
                    response = PY.MakePayment(request);
                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2)
                    {
                        this.NewPaymentService (PY );
                    }
                    numRetries++;
                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetries < 2);

                if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict)
                {
                    response.MessageInfo.MessageType = MessageType.BusinessValidationError;
                    response.MessageInfo.Message = "An error occured whiles trying to submit your request.  Please try again!";
                }


                //If we are okay then let us redirect to the View action.
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("View", new { Id = response.Id });


                }
                else if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
           response.MessageInfo.MessageType == MessageType.Error || response.MessageInfo.MessageType == MessageType.AlreadyHandled)
                {
                    MessageInfo _msg = new MessageInfo
                    {
                        MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                        Message = response.MessageInfo.Message
                    };
                    if (String.IsNullOrWhiteSpace(_msg.Message))
                    {
                        _msg.Message = "Busness Validation Errors were detected";
                    }
                    ViewBag.TitleMessage = _msg;
                }
                else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }
            }
            else
            {
                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already

                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Busness Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;
            }


            return View(viewModel);

        }


        public ActionResult View(int Id = 0)
        {

            //Check the role of the user i.e if it is CRL Administrator

            //Validate parameters or view model


            //Create the view model which will be associated with the corresponding view
            PaymentViewModel viewModel = new PaymentViewModel();

            //Get data necessary for preparing view for adding new client
            GetDataForViewPaymentResponse response = PY.GetView(new GetDataForViewPaymentRequest { Id = Id, SecurityUser = SecurityUser });
            viewModel.PaymentView = response.PaymentView;


            return View("ViewPayment", viewModel);

        }

        public ActionResult Download(int Id)
        {
            GetReceiptRequest request = new GetReceiptRequest { Id = Id, SecurityUser = SecurityUser };

            GetReceiptResponse response = PY.GetReceiptRequest(request);

            //System.Net.Mime.MediaTypeNames.Application.Octet
            return File(
                response.AttachedFile, response.AttachedFileType, response.AttachedFileName);
        }

        public ActionResult DownloadBatchDetailReport(int Id)
        {
            RequestBase request = new RequestBase
            {
                Id = Id,
              
             

            };

            this.HttpRequestToServiceRequest(request);
            request.Id = Id;
            GetFileAttachmentResponse response = PY.GenerateBatchDetailReport(request);

            //System.Net.Mime.MediaTypeNames.Application.Octet
            return File(
                response.AttachedFile, response.AttachedFileType, response.AttachedFileName);
        }

        
        public ActionResult GenerateBatch()
        {

            GenerateBatchViewModel viewModel = new GenerateBatchViewModel();
            return View("GenerateBatchIndex",viewModel);
        }

        
        [HttpPost]
        public ActionResult GenerateBatch(GenerateBatchViewModel viewModel)
        {
            DateRange range = null;
            DateTime? startDate = null;
            DateTime? endDate = null;
            bool selectAllBatches = viewModel._ListOfTransactionsJqGrid.SelectAllOustandingTransactions;
            int? batchType = viewModel._ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched;
            //If we did not select all transactons then let's use date range
            if (!selectAllBatches) {
                range = viewModel._ListOfTransactionsJqGrid.GenerateDateRange();
                if (range != null) {
                    startDate = range.StartDate;
                    endDate = range.EndDate;
                }
                else 
                {
                    ModelState.AddModelError(String.Empty, "Please specify a date!");
                }                    
            }
            if (ModelState.IsValid) {
                return RedirectToAction("PreviewAndGenerateBatch", new { startDate = startDate, endDate = endDate, selectAllBatch = selectAllBatches, batchType = batchType });    
            }
            return View("GenerateBatchIndex", viewModel);
        }
        
        public ActionResult PreviewAndGenerateBatch(DateTime? startDate, DateTime? endDate, bool selectAllBatch, int? batchType)
        {
            GenerateBatchViewModel viewModel = new GenerateBatchViewModel();
            viewModel._ListOfTransactionsJqGrid.StartDate = startDate;
            viewModel._ListOfTransactionsJqGrid.EndDate = endDate;
            viewModel._ListOfTransactionsJqGrid.SelectAllOustandingTransactions = selectAllBatch;
            viewModel._ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched = batchType;

          return View("PreviewAndGenerateBatch", viewModel);
        }

        
        [HttpPost]
        public ActionResult GenerateBatchForReconciliation(GenerateBatchViewModel viewModel)
        {
            DateRange range;
            //If we did not select all transactons then let's use date range
            if (!viewModel._ListOfTransactionsJqGrid.SelectAllOustandingTransactions)
            {
                range = viewModel._ListOfTransactionsJqGrid.GenerateDateRange();
                if (range == null)
                {
                    ModelState.AddModelError(String.Empty, "Please specify a date!");
                }

            }

         

            if (ModelState.IsValid)
            {

                //Create the postpaid request object
              CreatePostpaidBatchRequest request = new CreatePostpaidBatchRequest();
              this.HttpRequestToServiceRequest(request);
              
                //Get the period if one was selected
              if (!viewModel._ListOfTransactionsJqGrid.SelectAllOustandingTransactions)
              {
                  range = viewModel._ListOfTransactionsJqGrid.GenerateDateRange();
                  request.StartDate = range.StartDate;
                  request.EndDate = range.EndDate;
              }

                
            request.BatchType = viewModel._ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched;

          
                request.Batchcomment = viewModel._ListOfTransactionsJqGrid.BatchComment;
                 CreatePostpaidBatchResponse response ;
                //Call service and handle any concurrency issues
                int numRetriesAfterConcurrencyError = 0;
                do
                {
                    numRetriesAfterConcurrencyError++;
                    response = PY.CreatePostpaidBatch(request);

                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries)
                    {
                        NewInstitutionService(IS); //Get a new service to try again: We may need to optimize this code
                    }
                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries);


                HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response);

                if (handleServiceResponse.ActionResult != null)
                {
                    return handleServiceResponse.ActionResult;
                }             
                else
                {
                    TempData["TitleMessage"] = response.MessageInfo;                  
                    return RedirectToAction("ViewBatches");
                }

            
            }
            else
            {
                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already

                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Busness Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;
            }

            return View("PreviewAndGenerateBatch", viewModel);
        }


        public ActionResult GetSummaryViewOfPostpaidTransactions(_ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {
            return PartialView("~/Areas/Payment/Views/Shared/_SummaryOfPostPaidTransactionsJqGrid.cshtml", _ListOfTransactionsJqGrid);
        }


        public ActionResult GetJsonSummaryViewOfPostpaidTransactions(GridSettings grid, _ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {

            ViewSummaryPostpaidTransactionsRequest request = new ViewSummaryPostpaidTransactionsRequest();

            //Map view model to the rquest object
            request.TransactionDate = _ListOfTransactionsJqGrid.GenerateDateRange();
            request.SecurityUser = SecurityUser;
            request.BatchTypeId = _ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched;
            if (_ListOfTransactionsJqGrid.BatchId != null)
            {
                request.BatchId = _ListOfTransactionsJqGrid.BatchId;
            }
            if (grid.IsSearch)
            {
                //request. = grid.Where.rules.Any(r => r.field == "TransactionDate") ?
                //grid.Where.rules.FirstOrDefault(r => r.field == "PaymentNo").data : string.Empty;



                //string PaymentType = grid.Where.rules.Any(r => r.field == "PaymentType") ?
                //    grid.Where.rules.FirstOrDefault(r => r.field == "PaymentType").data : string.Empty;
                //if (PaymentType != String.Empty)
                //{
                //    request.PaymentTypeId = Convert.ToInt32(PaymentType);

                //}


            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.SecurityUser = SecurityUser;

            //Service call
            ViewSummaryPostpaidTransactionsResponse response = PY.ViewSummaryPostpaidTransactions(request);
            SummarisedCreditActivityView[] ArrayRows = response.SummarisedCreditActivities.ToArray();


            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult GetSummaryViewOfPostpaidTransactionsByRepresentativeBank(_ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {

            return PartialView("~/Areas/Payment/Views/Shared/_SummaryOfPostpaidPerRepresentativeClientJqGrid.cshtml", _ListOfTransactionsJqGrid);
        }


        public ActionResult GetJsonForSummaryViewOfPostpaidTransactionsByRepresentativeBank(GridSettings grid, _ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {

            ViewSummaryPostpaidTransactionsRequest request = new ViewSummaryPostpaidTransactionsRequest();

            //Map view model to the rquest object
            request.TransactionDate = _ListOfTransactionsJqGrid.GenerateDateRange();
            request.SecurityUser = SecurityUser;
            if (_ListOfTransactionsJqGrid.BatchId != null)
            {
                request.BatchId = _ListOfTransactionsJqGrid.BatchId;
            }
            request.BatchTypeId = _ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched;
            if (grid.IsSearch)
            {
                //request. = grid.Where.rules.Any(r => r.field == "TransactionDate") ?
                //grid.Where.rules.FirstOrDefault(r => r.field == "PaymentNo").data : string.Empty;



                //string PaymentType = grid.Where.rules.Any(r => r.field == "PaymentType") ?
                //    grid.Where.rules.FirstOrDefault(r => r.field == "PaymentType").data : string.Empty;
                //if (PaymentType != String.Empty)
                //{
                //    request.PaymentTypeId = Convert.ToInt32(PaymentType);

                //}


            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.SecurityUser = SecurityUser;


            //Service call
            ViewSummaryPostpaidTransactionsResponse response = PY.ViewSummaryPostpaidByBankTransaction(request);
            SummarisedCreditActivityByRepresentativeBank[] ArrayRows = response.SummarisedCreditActivitiesRepresentativeBank.ToArray();

            _ListOfTransactionsJqGrid.NumberOfRows = response.NumRecords;
            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetDetailViewOfPostPaidTransactions(_ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {

            return PartialView("~/Areas/Payment/Views/Shared/_DetailsOfPostPaidTransactionsJqGrid.cshtml", _ListOfTransactionsJqGrid);
        }


        public ActionResult GetJsonDetailViewOfPostPaidTransactions(GridSettings grid, _ListOfTransactionsJqGrid _ListOfTransactionsJqGrid)
        {

            ViewSummaryPostpaidTransactionsRequest request = new ViewSummaryPostpaidTransactionsRequest();

            //Map view model to the rquest object
            request.TransactionDate = _ListOfTransactionsJqGrid.GenerateDateRange();
            request.SecurityUser = SecurityUser;
            request.BatchTypeId = _ListOfTransactionsJqGrid.AllOrBatchedOrUnBatched;
            if (_ListOfTransactionsJqGrid.BatchId != null)
            {
                request.BatchId = _ListOfTransactionsJqGrid.BatchId;
            }
            if (grid.IsSearch)
            {
                //request. = grid.Where.rules.Any(r => r.field == "TransactionDate") ?
                //grid.Where.rules.FirstOrDefault(r => r.field == "PaymentNo").data : string.Empty;



                //string PaymentType = grid.Where.rules.Any(r => r.field == "PaymentType") ?
                //    grid.Where.rules.FirstOrDefault(r => r.field == "PaymentType").data : string.Empty;
                //if (PaymentType != String.Empty)
                //{
                //    request.PaymentTypeId = Convert.ToInt32(PaymentType);

                //}


            }

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.SecurityUser = SecurityUser;
            //Service call
            ViewSummaryPostpaidTransactionsResponse response = PY.ViewPostpaidTransactionDetails(request);
            CreditActivityView[] ArrayRows = response.PostpaidTransactionDetails.ToArray();


            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ReconcileBatches(ViewBatchDetails viewModel)//string ReconcileComment, int BatchId)
        {
            
            if (ModelState.IsValid)
            {
                ReconcileBatchedPostpaidTransactionsRequest request = new ReconcileBatchedPostpaidTransactionsRequest();
               
                request.ReconcileComment = viewModel._ViewBatchDetailsPartailViewModel.ReconcileComment;
                request.SecurityUser = SecurityUser;
                request.BatchId = viewModel._BatchDetailsGrid.BatchId;
                request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
                request.UserIP = AuditHelper.GetUserIP(this.Request);

                CreatePostpaidBatchResponse response = PY.ReconcileBatchedPostpaidTransactions(request);
                //GenerateBatchedPostpaidTransactionsResponse response = new GenerateBatchedPostpaidTransactionsResponse();
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    //var result = "Success";
                    //return Json(result, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("ViewBatches");

                }
                else if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
            response.MessageInfo.MessageType == MessageType.Error || response.MessageInfo.MessageType == MessageType.AlreadyHandled)
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
                    ViewBag.TitleMessage = _msg;
                }
                else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }
            }
            else
            {
                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already

                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Busness Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;
            }
            viewModel.ChangeFormMode(EditMode.Edit);
            return View("ReconcileBatch", viewModel);
        }

    
        public ActionResult ViewBatches()
        {

            AccountBatchesIndexViewModel viewModel = new AccountBatchesIndexViewModel();
            return View("IndexOfAccountBatches", viewModel);
        }


        public PartialViewResult GetAccountBatchesPartialView(_ViewAccountBatchesJqGrid _ViewAccountBatchesJqGrid)
        {
            _BatchJqGridViewModelHelper.CreateViewModelForGrid(_ViewAccountBatchesJqGrid);
            return PartialView("~/Areas/Payment/Views/Shared/_ViewAccountBatches.cshtml", _ViewAccountBatchesJqGrid);
        }

        public ActionResult GetJsonForAccountBatchesJqGrid(GridSettings grid, _ViewAccountBatchesJqGrid _ViewAccountBatchesJqGrid)
        {

            ViewAccountBatchesRequest request = new ViewAccountBatchesRequest();

            //Map view model to the rquest object 
            _BatchJqGridViewModelHelper.MapViewModelToViewAccountBatchRequest(_ViewAccountBatchesJqGrid, request, grid);
            this.HttpRequestToServiceRequest(request);
            
           
            //Service call
          
         
            //  var ArrayRows = response.AccountBatches.Select(new .ToArray();

            //Service call to view users
            ViewAccountBatchesResponse response = PY.ViewAccountBatches(request);

            //For success we return json result
            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
            {
                //We are not going to return a view model but a special object and so how do we do this, let's jsut do it here
                AccountBatchView[] ArrayRows = response.AccountBatches .ToArray();
                var result = new { total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize), page = grid.PageIndex, records = response.NumRecords, rows = ArrayRows };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Return json error if there was one
                string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "failure" : response.MessageInfo.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ReconcileBatches(int? Id)
        {
            ViewBatchDetails viewModel = new ViewBatchDetails();
            viewModel._BatchDetailsGrid = new _BatchDetailsGrid();

            GetReconcileRequest request = new GetReconcileRequest();
            viewModel.ChangeFormMode(EditMode.Edit);
            request.BatchId = (int)Id;
            GetReconcileResponse response = PY.GetBatchDetails(request);
            viewModel._ViewBatchDetailsPartailViewModel.BatchNo = (int)Id;
            viewModel._ViewBatchDetailsPartailViewModel.BatchName = response.AccountBatchView.Name;
            viewModel._ViewBatchDetailsPartailViewModel.BatchComment = response.AccountBatchView.Comment;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodStartDate = response.AccountBatchView.PeriodStartDate;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodEndDate = response.AccountBatchView.PeriodEndDate;
            viewModel._ViewBatchDetailsPartailViewModel.TotalBatchExpenses = response.AccountBatchView.TotalExpenses;
            viewModel._ViewBatchDetailsPartailViewModel.TotalSetltement = response.AccountBatchView.TotalSettlement;
            viewModel._ViewBatchDetailsPartailViewModel.TotalSetltement = response.AccountBatchView.TotalExpenses - response.AccountBatchView.TotalSettlement;

            viewModel._BatchDetailsGrid.BatchDetails = response.ClientAmountWithRepBankView.ToArray();
            viewModel.ClientAmountWithRepBankViews = response.ClientAmountWithRepBankView.ToArray();
            viewModel.SettledClientsInBatchViews = response.SettledClientsInBatchViews.ToArray();
            viewModel._BatchDetailsGrid.BatchId = viewModel._ViewBatchDetailsPartailViewModel.BatchNo;
            viewModel._ListOfTransactionsJqGrid = new _ListOfTransactionsJqGrid();
            viewModel._ListOfTransactionsJqGrid.BatchId = (int)Id;
            return View("ReconcileBatch", viewModel);

        }



        public ActionResult ViewBatchDetails(int? Id)
        {

            ViewBatchDetails viewModel = new ViewBatchDetails();
            viewModel._BatchDetailsGrid = new _BatchDetailsGrid();
            viewModel.ChangeFormMode(EditMode.View);
            GetReconcileRequest request = new GetReconcileRequest();
            request.BatchId = (int)Id;
            GetReconcileResponse response = PY.GetBatchDetails(request);
            viewModel._ViewBatchDetailsPartailViewModel.BatchNo = (int)Id;
            viewModel._ViewBatchDetailsPartailViewModel.BatchName = response.AccountBatchView.Name;
            viewModel._ViewBatchDetailsPartailViewModel.BatchComment = response.AccountBatchView.Comment;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodStartDate = response.AccountBatchView.PeriodStartDate;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodEndDate = response.AccountBatchView.PeriodEndDate;
            viewModel._ViewBatchDetailsPartailViewModel.TotalBatchExpenses = response.AccountBatchView.TotalExpenses;
            viewModel._ViewBatchDetailsPartailViewModel.TotalSetltement = response.AccountBatchView.TotalSettlement;
            viewModel._ViewBatchDetailsPartailViewModel.TotalOustandinBatchAmount = response.AccountBatchView.TotalExpenses - response.AccountBatchView.TotalSettlement;

            viewModel.ClientAmountWithRepBankViews = response.ClientAmountWithRepBankView.ToArray();
            viewModel.SettledClientsInBatchViews = response.SettledClientsInBatchViews.ToArray();
            viewModel._BatchDetailsGrid.BatchId = viewModel._ViewBatchDetailsPartailViewModel.BatchNo;
            viewModel._ListOfTransactionsJqGrid = new _ListOfTransactionsJqGrid();
            viewModel._ListOfTransactionsJqGrid.BatchId = (int)Id;
            return View("ReconcileBatch", viewModel);
        }

        public PartialViewResult GetBatchDetailsPartialView(int? BatchId, _BatchDetailsGrid _BatchDetailsGrid)
        {
            GetReconcileRequest request = new GetReconcileRequest();
            request.BatchId = _BatchDetailsGrid.BatchId;

            GetReconcileResponse response = PY.GetBatchDetails(request);
            //_BatchDetailsGrid.BatchDetails = response.ClientAmountWithRepBankView.ToArray();
            return PartialView("~/Areas/Payment/Views/Shared/_BatchDetailsGrid.cshtml", _BatchDetailsGrid);
            
        }

        public ActionResult ReconcileSubPostpaidClients(int id) 
        {
            SubPostpaidClientsViewModel viewModel = new SubPostpaidClientsViewModel();

            RequestBase request = new RequestBase();
            request.Id = id;
            request.SecurityUser = SecurityUser;

            GetConfirmClientsInPostpaidBatchResponse response = PY.GetConfirmClientsInPostpaidBatch(request);

            viewModel._ViewBatchDetailsPartailViewModel.BatchNo = (int)id;
            viewModel._ViewBatchDetailsPartailViewModel.BatchName = response.AccountBatchView.Name;
            viewModel._ViewBatchDetailsPartailViewModel.BatchComment = response.AccountBatchView.Comment;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodStartDate = response.AccountBatchView.PeriodStartDate;
            viewModel._ViewBatchDetailsPartailViewModel.PeriodEndDate = response.AccountBatchView.PeriodEndDate;
            viewModel._ViewBatchDetailsPartailViewModel.TotalBatchExpenses = response.AccountBatchView.TotalExpenses;
            viewModel._ViewBatchDetailsPartailViewModel.TotalSetltement = response.AccountBatchView.TotalSettlement;
            viewModel._ViewBatchDetailsPartailViewModel.TotalOustandinBatchAmount = response.AccountBatchView.TotalExpenses - response.AccountBatchView.TotalSettlement;

            ClientAmountSelectionView[] ArrayRows = response.ClientsInPostpaidBatchToConfirmView.ToArray();
            viewModel.PostpaidSubClientsCreditActivites = ArrayRows;
            viewModel.BatchNo = id;


            return View("ReconcileSubPostpaidClients",viewModel);
        }

        [HttpPost]
        public ActionResult ReconcileSubPostpaidClients(ClientAmountSelectionView[] PostpaidSubClientsCreditActivites, SubPostpaidClientsViewModel viewModel)
        {
            if (SecurityUser.IsOwnerUser == false)
            {
                if (PostpaidSubClientsCreditActivites == null || PostpaidSubClientsCreditActivites.Where(s => s.isSelected).Count() == 0)
                {
                    ModelState.AddModelError("_BatchDetailsGrid", "Please select one or more items to reconcile!");
                }
            }
            if (ModelState.IsValid)
            {
                ReconcileBatchedPostpaidTransactionsRequest request = new ReconcileBatchedPostpaidTransactionsRequest();
                request.RepresentativeInstitution = PostpaidSubClientsCreditActivites.Where(s => s.isSelected).Select(s => s.MembershipId).ToArray();// selectedIds;
                request.ReconcileComment = viewModel._ViewBatchDetailsPartailViewModel.ReconcileComment;
                request.SecurityUser = SecurityUser;
                request.BatchId = (int)viewModel.BatchNo;
                request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
                request.UserIP = AuditHelper.GetUserIP(this.Request);

                CreatePostpaidBatchResponse response = PY.ReconcileBatchedPostpaidTransactions(request);
                //GenerateBatchedPostpaidTransactionsResponse response = new GenerateBatchedPostpaidTransactionsResponse();
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    //var result = "Success";
                    //return Json(result, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("ViewBatches");

                }
                else if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
            response.MessageInfo.MessageType == MessageType.Error || response.MessageInfo.MessageType == MessageType.AlreadyHandled)
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
                    ViewBag.TitleMessage = _msg;
                }
                else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }
            }
            else
            {
                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already

                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Busness Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;
            }
            return View("ReconcileSubPostpaidClients", viewModel);
        }

       

        public ActionResult GenerateBatchReport(int id) 
        {
            return null;
        }

        //
        [HttpPost]
        [AuthorizeAllAdministratorOnly]
        public ActionResult Delete(int id)
        {
            DeleteItemRequest request = new DeleteItemRequest { Id = id, SecurityUser = this.SecurityUser };
            this.HttpRequestToServiceRequest(request);
            ResponseBase response = PY.UndoBatch(request);
            return (this.HandleServiceResponseJSON(response).ActionResult);
        }

    }
}
