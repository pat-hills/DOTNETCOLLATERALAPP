using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Service.Interfaces;
using CRL.UI.MVC.Areas.Search.Models.ModelHelpers;
using CRL.UI.MVC.Areas.Search.Models.ViewPageModels;
using MvcJqGrid;
using StructureMap;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ViewPageModels;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Implementations;
using CRL.Service.Interfaces.FinancialStatement;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Infrastructure.Enums;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ModelHelpers;
using CRL.UI.MVC.Areas.FinancialStatement.Models.FinancialStatement.ViewPageModels.Shared;
using CRL.Infrastructure.Configuration;
using CRL.UI.MVC.Common;
using CRL.Model.Messaging;
using CRL.UI.MVC.Areas.Search.Models;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;
using CRL.Model.Payments;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CRL.UI.MVC.Areas.Search.Models.ViewPageModels;

namespace CRL.UI.MVC.Areas.Search.Controllers
{
    public class SearchController : CRLController
    {

        public ActionResult Index()
        {
            FSSearchesIndexViewModel viewModel = new FSSearchesIndexViewModel();
            if (!this.HttpContext.Request.IsAuthenticated)
            {
                if (TempData["PublicSecurityCode"] != null)
                {
                    viewModel._FSSearchesViewModel.PublicUserSecurityCode = TempData["PublicSecurityCode"].ToString();
                }
                //else if (Session["PublicUserCode"] != null)
                //{
                //    viewModel._FSSearchesViewModel.PublicUserSecurityCode = Session["PublicUserCode"].ToString();
                //}
                else
                {
                    return RedirectToAction("ViewSearches", "Search");
                }
            }

            return View(viewModel);

        }
        public PartialViewResult GetPartialViewOfFSSearchesGrid(_FSSearchesViewModel _FSSearchesViewModel)
        {
            //Calls the partial view
            //Calls the partial view
            //GetDataForAmendViewResponse response = AS.GetDataForAmendView();
            //_FSActivityJqGridViewModel.FinancialStatementActivityType = response.FinancialStatementActivityType.ToDictionaryString();
            return PartialView("_FSSearchesJqGrid", _FSSearchesViewModel);
        }
        public ActionResult GetJsonFSSearchesJqGrid(GridSettings grid, _FSSearchesViewModel viewModel)
        {
            ViewSearchesFSRequest request = new ViewSearchesFSRequest();
            //If we are using MVCJqGrid then 
            //Map view model to the rquest object
            _FSSearchesViewModelHelper.MapViewModelToViewFSRequest(viewModel, request, grid);
            request.SecurityUser = SecurityUser;
            //Service call
            ViewSearchesResponse response = SS.ViewSearchFS(request);
            SearchRequestGridView[] ArrayRows = response.SearchRequestGridView.ToArray();

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
        // GET: /Search/Search/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        //
        // GET: /Search/Search/Create

        public ActionResult Search(bool NonLegalEffect = false, string SecurityCode = null)
        {
            if (Constants.ApplySearchRoleToSearch && SecurityUser != null && SecurityUser.InstitutionId != null)
            {

                if (!SecurityUser.IsInRole("Search"))
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }

            }

            //Create the search view
            //**Need to validate the code if this code was passed by url , we need to decrypt it first
            SearchViewModel viewModel = new SearchViewModel();
            GetSearchRequest request = new GetSearchRequest();
            this.HttpRequestToServiceRequest(request);
            viewModel._SearchResultJqGridViewModel = new _SearchResultJqGridViewModel();
            viewModel._SearchResultJqGridViewModel.SearchParam = new SearchParam();
            viewModel._SearchResultJqGridViewModel.SearchParam.SearchType = 1;

            viewModel._SearchResultJqGridViewModel.isNonLegalEffect = NonLegalEffect;
            viewModel.IsCertified = true;
            request.SecurityCodeForPublicUser = SecurityCode;

            var response = SS.GetSearch(request);

            HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response, true);

            //If our response had an action result then we should return the action result
            if (handleServiceResponse.ActionResult != null)
            {
                return handleServiceResponse.ActionResult;
            }

            viewModel._SearchResultJqGridViewModel.isPayable = response.IsPayable;
            viewModel._SearchResultJqGridViewModel.BusinessPrefixes = response.BusinessPrefixes.ToSelectList();
            if (!String.IsNullOrWhiteSpace(SecurityCode) && SecurityUser == null)
            {
                Session["PublicUserCode"] = SecurityCode;
            }


            //**Check if we have a security code and if we do then assign it tot he request class
            //**If the get search works then good else go to error page
            //**if the response returned the security code then we need to create the session
            //**we should also have a specific page or area for entering the security code
            //**Consider loading more info for the public user





            TempData["SearchParameter"] = null;

            //NIGERIA ON
            return View("LegalSearchMultiple", viewModel);

            //LIBERIA OFF      return View("LegalSearch", viewModel );
        }
        public ActionResult LoadSearch(int SearchId, bool IsPreviousSearch = false)
        {
            //**If we are a public user then load the security code to the request

            SearchViewModel viewModel = new SearchViewModel();
            viewModel._SearchResultJqGridViewModel = new _SearchResultJqGridViewModel();


            GetExpiredSearchResultRequest request3 = new GetExpiredSearchResultRequest();
            request3.SearchFinancialStatementId = SearchId;
            this.HttpRequestToServiceRequest(request3);
            if (Session["PublicUserCode"] != null)
                request3.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();

            GetExpiredSearchResultResponse response3 = SS.GetExpiredSearchResults(request3);
            if (response3.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }

            viewModel._SearchResultJqGridViewModel.ExpiredSearchResults = response3.ExpiredResultsRegNo;
            viewModel._SearchResultJqGridViewModel.HasExpiredAllResults = response3.HasExpiredAllResults;
            viewModel._SearchResultJqGridViewModel.GeneratedSearchReportsReNos = response3.GeneratedSearchReportsRegNo;
            if (Constants.SHOW_GENERATED_REPORTS_LIST)
            {
                viewModel._SearchResultJqGridViewModel.HasGeneratedReport = response3.HasSearchReports;
                viewModel._SearchResultJqGridViewModel.SearchReports = response3.SearchReports;
            }

            SearchRequest request = new SearchRequest();
            if (Session["PublicUserCode"] != null)
                request.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
            this.HttpRequestToServiceRequest(request);
            request.Id = SearchId;
            var response = SS.FilterOrLoadSearch(request);

            if (response.ResultsFromCAC != null)
            {
                //var searchResults = new CACSearch();

                //searchResults = JsonConvert.DeserializeObject<CACSearch>(response.CACResults);
                viewModel.IsCACResults = true;
                viewModel.CACSearch = response.ResultsFromCAC;
            }
            viewModel._SearchResultJqGridViewModel.SearchParam = response.SearchParam;
            //viewModel._SearchResultJqGridViewModel.SearchParam.SearchType = 1;
            viewModel._SearchResultJqGridViewModel.BusinessPrefixes = response.BusinessPrefixes.ToSelectList();
            viewModel._SearchResultJqGridViewModel.SearchResultView = response.SearchResultView.ToArray();
            viewModel._SearchResultJqGridViewModel.SearchId = SearchId;
            viewModel._SearchResultJqGridViewModel.SearchResultCount = response.NumRecords;
            viewModel._SearchResultJqGridViewModel.Pages = (int)Math.Ceiling((double)response.NumRecords / request.PageSize);
            viewModel._SearchResultJqGridViewModel.SearchDate = response.SearchDate;
            viewModel._SearchResultJqGridViewModel.AssignedFsId = response.AssignedFSId;
            viewModel._SearchResultJqGridViewModel.IsAfterSearch = true;
            viewModel._SearchResultJqGridViewModel.IsPreviousSearch = IsPreviousSearch;

            viewModel.IsViewMode = true;
            viewModel._SearchResultJqGridViewModel.DoNotGenerateSearchAlert = true;

            return View("LegalSearchMultiple", viewModel);
        }
        [HttpPost]
        public ActionResult Search(SearchViewModel viewModel, int currentPage = 0)
        {

            //viewModel._SearchResultJqGridViewModel.SearchId = Convert.ToInt32(Request["_SearchResultJqGridViewModel.SearchParam.SearchType"]);

            if (viewModel.ViewAndGenerateReport != null)
            {
                //AssignFSToSearchRequest requesta = new AssignFSToSearchRequest();
                //this.HttpRequestToServiceRequest(requesta);
                //if (Session["PublicUserCode"] != null)
                //    requesta.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
                //requesta.SearchId = viewModel._SearchResultJqGridViewModel.SearchId;
                //requesta.SelectedFS = viewModel._SearchResultJqGridViewModel.SelectedId;
                //var responsea = SS.AssignFSToSearch(requesta);

                //if (responsea.MessageInfo.MessageType == MessageType.Success)
                //{
                //    TempData["SearchResultParameters"] = new int[] { viewModel._SearchResultJqGridViewModel.SearchId, viewModel._SearchResultJqGridViewModel.SelectedId };
                //    return RedirectToAction("SearchResult");
                //}
                //else if (responsea.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                //{
                //    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                //}
                //else
                //{
                //    MessageInfo _msg = new MessageInfo { MessageType = responsea.MessageInfo.MessageType, Message = responsea.MessageInfo.Message };
                //    ViewBag.TitleMessage = _msg;
                //    return RedirectToAction("Search", "Search", new { Area = "Search" });

                //}

                if (viewModel._SearchResultJqGridViewModel.SelectedId == 0)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });

                }

                int IsPreviousSearch = viewModel._SearchResultJqGridViewModel.IsPreviousSearch ? 1 : 0;
                TempData["SearchResultParameters"] = new int[] { viewModel._SearchResultJqGridViewModel.SearchId, viewModel._SearchResultJqGridViewModel.SelectedId, IsPreviousSearch };
                TempData["PayableTransaction"] = viewModel._SearchResultJqGridViewModel.isPayable;
                Session["PublicUserCode"] = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                return RedirectToAction("SearchResult");

            }



            GetExpiredSearchResultRequest request3 = new GetExpiredSearchResultRequest();
            request3.SearchFinancialStatementId = viewModel._SearchResultJqGridViewModel.SearchId;
            this.HttpRequestToServiceRequest(request3);

            //New Addition
            if (viewModel._SearchResultJqGridViewModel.isPayable)
            {

                if (Session["PublicUserCode"] != null)
                {
                    request3.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
                }
                if (!String.IsNullOrWhiteSpace(viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo))
                {
                    request3.SecurityCodeForPublicUser = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                }


                GetExpiredSearchResultResponse response3 = SS.GetExpiredSearchResults(request3);
                if (response3.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }

                viewModel._SearchResultJqGridViewModel.ExpiredSearchResults = response3.ExpiredResultsRegNo;
                viewModel._SearchResultJqGridViewModel.HasExpiredAllResults = response3.HasExpiredAllResults;
                viewModel._SearchResultJqGridViewModel.GeneratedSearchReportsReNos = response3.GeneratedSearchReportsRegNo;

            }
            TempData["SearchParameter"] = viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo;

            if (String.IsNullOrEmpty(viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo))
            {
                ModelState.AddModelError("_SearchResultJqGridViewModel", "You must provide a search parameter");
            }


            if (viewModel._SearchResultJqGridViewModel.SearchParam.SearchType != 2)
            {
                IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.SearchParam.BusinessPrefix"));
                ICollection<string> copyKeys = new List<string>();
                foreach (var mykey in mykeys)
                {
                    string s = String.Copy(mykey);
                    copyKeys.Add(s);
                }

                foreach (var mykey in copyKeys)
                {
                    ModelState.Remove(mykey);
                }
            }

            if (this.HttpContext.Request.IsAuthenticated || !viewModel._SearchResultJqGridViewModel.isPayable)
            {
                IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone")
                                 || s.Contains("_SearchResultJqGridViewModel.BVN"));
                ICollection<string> copyKeys = new List<string>();
                foreach (var mykey in mykeys)
                {
                    string s = String.Copy(mykey);
                    copyKeys.Add(s);
                }

                foreach (var mykey in copyKeys)
                {
                    ModelState.Remove(mykey);
                }

            }

            if (viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo != null)
            {
                ValidateSecurityCodeRequest request1 = new ValidateSecurityCodeRequest();
                request1.SecurityCode = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                ValidateSecurityCodeResponse response1 = SS.ValidateSecurityCode(request1);

                if (response1.HasInfo || !response1.IsValid)
                {

                    IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone")
                || s.Contains("_SearchResultJqGridViewModel.BVN"));


                    ICollection<string> copyKeys = new List<string>();
                    foreach (var mykey in mykeys)
                    {
                        string s = String.Copy(mykey);
                        copyKeys.Add(s);
                    }

                    foreach (var mykey in copyKeys)
                    {
                        ModelState.Remove(mykey);
                    }

                }
                else if (response1.IsValid && response1.PaymentType == PaymentSource.InterSwitchWebPay)
                {
                    IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone"));

                    ICollection<string> copyKeys = new List<string>();
                    foreach (var mykey in mykeys)
                    {
                        string s = String.Copy(mykey);
                        copyKeys.Add(s);
                    }

                    foreach (var mykey in copyKeys)
                    {
                        ModelState.Remove(mykey);
                    }
                }

            }

            //Now validate errors relating to specific MVC issues and relating to the field attributes
            bool MVCValidationErrors = !ModelState.IsValid;


            if (!MVCValidationErrors)
            {

                //Create the service request
                SearchRequest request = new SearchRequest();
                this.HttpRequestToServiceRequest(request);
                if (viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo != null)
                {
                    Session["PublicUserCode"] = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                    request.SecurityCodeForPublicUser = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                }

                //Map view model to the service request               
                SearchViewModelHelper.MapToCreateSubmitRequest(viewModel, request);

                //Call the application service function
                SearchResponse response;

                int numRetriesAfterConcurrencyError = 0;
                do
                {
                    numRetriesAfterConcurrencyError++;
                    response = SS.Search(request);


                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries)
                    {
                        this.NewSearchService(SS);
                    }
                    numRetriesAfterConcurrencyError++;

                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries);

                HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response);

                if (handleServiceResponse.ActionResult != null)
                {
                    return handleServiceResponse.ActionResult;
                }
                else if (handleServiceResponse.MapValidationsToModelState == true)
                {
                    FSViewModelHelper.MapValidationErrorsToModelState(this.Request, this.ModelState, response.ValidationErrors);
                }
                else if (handleServiceResponse.NoErrorsEncountered)
                {
                    if (response.SearchResultView != null)
                        viewModel._SearchResultJqGridViewModel.SearchResultView = response.SearchResultView.ToArray();


                    viewModel._SearchResultJqGridViewModel.SearchId = response.Id;
                    viewModel._SearchResultJqGridViewModel.SearchResultCount = response.NumRecords;
                    viewModel._SearchResultJqGridViewModel.Pages = (int)Math.Ceiling((double)response.NumRecords / request.PageSize);
                }
            }
            else
            {
                MessageInfo _msg = new MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "Business Validation Errors were detected" };
                ViewBag.TitleMessage = _msg;
            }

            GetSearchRequest requestb = new GetSearchRequest();
            this.HttpRequestToServiceRequest(requestb);
            var response2 = SS.GetSearchQ(requestb);
            viewModel._SearchResultJqGridViewModel.BusinessPrefixes = response2.BusinessPrefixes.ToSelectList();

            if (!MVCValidationErrors)
            {
                viewModel.IsViewMode = true;
            }
            else
            {
                viewModel.IsViewMode = false;
            }

            viewModel._SearchResultJqGridViewModel.IsAfterSearch = true;

            if (viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefix != 0)
            {
                viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefixName =
                viewModel._SearchResultJqGridViewModel.BusinessPrefixes.Where(
                    s => s.Value == viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefix.ToString()).FirstOrDefault().Text;
            }

            return View("LegalSearchMultiple", viewModel);

        }



        [HttpPost]
        public ActionResult AjaxSearch(SearchViewModel viewModel, int currentPage = 0)
        {

            var value = Convert.ToInt32(Request["searchidtype"]);
            var clas = Request["clas"];

            viewModel = new SearchViewModel();
            viewModel._SearchResultJqGridViewModel.SearchParam = new SearchParam();
            viewModel._SearchResultJqGridViewModel.SearchParam.SearchType = value;
            viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo = Request["boroweridnumber"];
            if (Request["isNonLegalEffect"] != null)
            {
                viewModel._SearchResultJqGridViewModel.isNonLegalEffect = bool.Parse(Request["isNonLegalEffect"]);
            }
            if (Request["isPayable"] != null)
            {
                viewModel._SearchResultJqGridViewModel.isPayable = bool.Parse(Request["isPayable"]);
            }
            if (Request["PublicUserReceiptNo"] != null)
            {
                Session["PublicUserCode"] = Request["PublicUserReceiptNo"];
                viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo = Session["PublicUserCode"].ToString();
            }
            if (viewModel._SearchResultJqGridViewModel.SearchParam.SearchType == 2)
            {
                var prefixValue = Convert.ToInt32(Request["prefixValue"]);
                viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefix = prefixValue;
            }
            if (viewModel.ViewAndGenerateReport != null)
            {
                //AssignFSToSearchRequest requesta = new AssignFSToSearchRequest();
                //this.HttpRequestToServiceRequest(requesta);
                //if (Session["PublicUserCode"] != null)
                //    requesta.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
                //requesta.SearchId = viewModel._SearchResultJqGridViewModel.SearchId;
                //requesta.SelectedFS = viewModel._SearchResultJqGridViewModel.SelectedId;
                //var responsea = SS.AssignFSToSearch(requesta);

                //if (responsea.MessageInfo.MessageType == MessageType.Success)
                //{
                //    TempData["SearchResultParameters"] = new int[] { viewModel._SearchResultJqGridViewModel.SearchId, viewModel._SearchResultJqGridViewModel.SelectedId };
                //    return RedirectToAction("SearchResult");
                //}
                //else if (responsea.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                //{
                //    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                //}
                //else
                //{
                //    MessageInfo _msg = new MessageInfo { MessageType = responsea.MessageInfo.MessageType, Message = responsea.MessageInfo.Message };
                //    ViewBag.TitleMessage = _msg;
                //    return RedirectToAction("Search", "Search", new { Area = "Search" });

                //}

                if (viewModel._SearchResultJqGridViewModel.SelectedId == 0)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });


                    //    return Json("AuthorizationError");
                }

                int IsPreviousSearch = viewModel._SearchResultJqGridViewModel.IsPreviousSearch ? 1 : 0;
                TempData["SearchResultParameters"] = new int[] { viewModel._SearchResultJqGridViewModel.SearchId, viewModel._SearchResultJqGridViewModel.SelectedId, IsPreviousSearch };
                TempData["PayableTransaction"] = viewModel._SearchResultJqGridViewModel.isPayable;

                //   return Json("SearchResult");

                return RedirectToAction("SearchResult");

            }



            GetExpiredSearchResultRequest request3 = new GetExpiredSearchResultRequest();
            request3.SearchFinancialStatementId = viewModel._SearchResultJqGridViewModel.SearchId;
            this.HttpRequestToServiceRequest(request3);

            //New Addition
            if (viewModel._SearchResultJqGridViewModel.isPayable)
            {

                if (Session["PublicUserCode"] != null)
                {
                    request3.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
                }
                if (!String.IsNullOrWhiteSpace(viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo))
                {
                    request3.SecurityCodeForPublicUser = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                }


                GetExpiredSearchResultResponse response3 = SS.GetExpiredSearchResults(request3);
                if (response3.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    //    return Json("AuthrizationError");
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }

                viewModel._SearchResultJqGridViewModel.ExpiredSearchResults = response3.ExpiredResultsRegNo;
                viewModel._SearchResultJqGridViewModel.HasExpiredAllResults = response3.HasExpiredAllResults;
                viewModel._SearchResultJqGridViewModel.GeneratedSearchReportsReNos = response3.GeneratedSearchReportsRegNo;

            }
            TempData["SearchParameter"] = viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo;

            if (String.IsNullOrEmpty(viewModel._SearchResultJqGridViewModel.SearchParam.BorrowerIDNo))
            {
                ModelState.AddModelError("_SearchResultJqGridViewModel", "You must provide a search parameter");
            }


            if (viewModel._SearchResultJqGridViewModel.SearchParam.SearchType != 2)
            {
                IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.SearchParam.BusinessPrefix"));
                ICollection<string> copyKeys = new List<string>();
                foreach (var mykey in mykeys)
                {
                    string s = String.Copy(mykey);
                    copyKeys.Add(s);
                }

                foreach (var mykey in copyKeys)
                {
                    ModelState.Remove(mykey);
                }
            }

            if (this.HttpContext.Request.IsAuthenticated || !viewModel._SearchResultJqGridViewModel.isPayable)
            {
                IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone")
                                 || s.Contains("_SearchResultJqGridViewModel.BVN"));
                ICollection<string> copyKeys = new List<string>();
                foreach (var mykey in mykeys)
                {
                    string s = String.Copy(mykey);
                    copyKeys.Add(s);
                }

                foreach (var mykey in copyKeys)
                {
                    ModelState.Remove(mykey);
                }

            }

            if (viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo != null)
            {
                ValidateSecurityCodeRequest request1 = new ValidateSecurityCodeRequest();
                request1.SecurityCode = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                ValidateSecurityCodeResponse response1 = SS.ValidateSecurityCode(request1);

                if (response1.HasInfo || !response1.IsValid)
                {

                    IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone")
                || s.Contains("_SearchResultJqGridViewModel.BVN"));


                    ICollection<string> copyKeys = new List<string>();
                    foreach (var mykey in mykeys)
                    {
                        string s = String.Copy(mykey);
                        copyKeys.Add(s);
                    }

                    foreach (var mykey in copyKeys)
                    {
                        ModelState.Remove(mykey);
                    }

                }
                else if (response1.IsValid && response1.PaymentType == PaymentSource.InterSwitchWebPay)
                {
                    IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone"));

                    ICollection<string> copyKeys = new List<string>();
                    foreach (var mykey in mykeys)
                    {
                        string s = String.Copy(mykey);
                        copyKeys.Add(s);
                    }

                    foreach (var mykey in copyKeys)
                    {
                        ModelState.Remove(mykey);
                    }
                }

            }

            //Now validate errors relating to specific MVC issues and relating to the field attributes
            bool MVCValidationErrors = !ModelState.IsValid;


            if (!MVCValidationErrors)
            {

                //Create the service request
                SearchRequest request = new SearchRequest();
                request.SearchParam = new SearchParam();
                request.clas = clas;
                request.Prefix = Request["prefix"];
                request.SearchParam.BorrowerIDNo = Request["boroweridnumber"];
                request.SearchParam.SearchType = viewModel._SearchResultJqGridViewModel.SearchParam.SearchType;
                //CACSearchrequest request2 = new CACSearchrequest();
                this.HttpRequestToServiceRequest(request);
                //this.HttpRequestToServiceRequest(request2);
                //request2.apiUrl = Constants.cacurl;
                if (viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo != null)
                {
                    Session["PublicUserCode"] = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                    request.SecurityCodeForPublicUser = viewModel._SearchResultJqGridViewModel.PublicUserReceiptNo;
                }


                //Map view model to the service request               
                SearchViewModelHelper.MapToCreateSubmitRequest(viewModel, request);

                //Call the application service function
                SearchResponse response;
                //CACSearchresponse cacResponse;
                int numRetriesAfterConcurrencyError = 0;
                do
                {
                    numRetriesAfterConcurrencyError++;
                    response = SS.Search(request);

                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries)
                    {
                        this.NewSearchService(SS);
                    }
                    numRetriesAfterConcurrencyError++;

                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries);

                HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response);

                if (handleServiceResponse.ActionResult != null)
                {
                    return handleServiceResponse.ActionResult;
                }
                else if (handleServiceResponse.MapValidationsToModelState == true)
                {
                    FSViewModelHelper.MapValidationErrorsToModelState(this.Request, this.ModelState, response.ValidationErrors);
                }
                else if (handleServiceResponse.NoErrorsEncountered)
                {
                    if (response.SearchResultView != null) viewModel._SearchResultJqGridViewModel.SearchResultView = response.SearchResultView.ToArray();


                    if (viewModel._SearchResultJqGridViewModel.SearchParam.SearchType == 2)
                    {
                        var searchResults = new CACSearch();

                        if (response.CACResults != "Error connectiong to CAC")
                        {
                            if (response.CACResults.Contains("<br />"))
                            {
                                searchResults.Name = "Error connectiong to CAC";
                            }
                            else
                            {
                                searchResults = JsonConvert.DeserializeObject<CACSearch>(response.CACResults);
                            }

                        }
                        else
                        {
                            searchResults.Name = "Error connectiong to CAC";
                        }

                        viewModel.IsCACResults = true;
                        viewModel.CACSearch = searchResults;
                    }
                    viewModel._SearchResultJqGridViewModel.SearchId = response.Id;
                    viewModel._SearchResultJqGridViewModel.SearchResultCount = response.NumRecords;
                    viewModel._SearchResultJqGridViewModel.Pages = (int)Math.Ceiling((double)response.NumRecords / request.PageSize);
                }

            }

            else
            {
                MessageInfo _msg = new MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "Business Validation Errors were detected" };
                ViewBag.TitleMessage = _msg;
            }

            GetSearchRequest requestb = new GetSearchRequest();
            this.HttpRequestToServiceRequest(requestb);
            var response2 = SS.GetSearchQ(requestb);
            viewModel._SearchResultJqGridViewModel.BusinessPrefixes = response2.BusinessPrefixes.ToSelectList();

            if (!MVCValidationErrors)
            {
                viewModel.IsViewMode = true;
            }
            else
            {
                viewModel.IsViewMode = false;
            }

            viewModel._SearchResultJqGridViewModel.IsAfterSearch = true;

            if (viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefix != 0)
            {
                viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefixName =
                viewModel._SearchResultJqGridViewModel.BusinessPrefixes.Where(
                    s => s.Value == viewModel._SearchResultJqGridViewModel.SearchParam.BusinessPrefix.ToString()).FirstOrDefault().Text;
            }

            //   return Json(viewModel);

            return View("LegalSearchMultiple", viewModel);

        }

        public ActionResult SearchResult()
        {
            int[] searchIds = (int[])TempData["SearchResultParameters"];
            var viewModel = new ResultSearchViewModel();
            if (searchIds == null)
            {
                return RedirectToAction("Search");
            }
            int SearchId = (int)searchIds[0];

            int fsId = (int)searchIds[1];

            if (viewModel == null) { viewModel = new ResultSearchViewModel(); }

            viewModel._SearchResultJqGridViewModel = new _SearchResultJqGridViewModel();
            viewModel._SearchResultJqGridViewModel.IsPreviousSearch = (int)searchIds[2] == 1 ? true : false;

            viewModel._SearchResultJqGridViewModel.isPayable = (bool)TempData["PayableTransaction"];

            SearchRequest request = new SearchRequest();
            if (Session["PublicUserCode"] != null)
                request.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
            this.HttpRequestToServiceRequest(request);
            request.Id = SearchId;
            var response = SS.FilterOrLoadSearch(request);

            viewModel._SearchResultJqGridViewModel.SearchParam = response.SearchParam;
            //viewModel._SearchResultJqGridViewModel.SearchParam.SearchType = 1;
            viewModel._SearchResultJqGridViewModel.BusinessPrefixes = response.BusinessPrefixes.ToSelectList();
            viewModel._SearchResultJqGridViewModel.SearchResultView = response.SearchResultView.ToArray();
            viewModel._SearchResultJqGridViewModel.SearchId = SearchId;
            viewModel._SearchResultJqGridViewModel.SelectedId = fsId;
            viewModel._SearchResultJqGridViewModel.SearchResultCount = response.NumRecords;
            viewModel._SearchResultJqGridViewModel.Pages = (int)Math.Ceiling((double)response.NumRecords / request.PageSize);
            viewModel._SearchResultJqGridViewModel.SearchDate = response.SearchDate;

            viewModel.IsViewMode = true;

            GetFSViewRequest request2 = new GetFSViewRequest() { Id = fsId };
            request2.IsRequestFromSearchResult = true;
            //Add security, url and IP to the request class
            this.HttpRequestToServiceRequest(request);


            //Create the view model which will be associated with the corresponding view
            FSViewModel viewModel2 = new FSViewModel();

            //We need a different service for getting just the details for viewing
            GetFSViewResponse response2 = FS.GetView(request2);

            //Handle the response and if there was an error detected then we should log this error to ideally where we came from or to an error page
            //or to 
            HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response, true);
            if (handleServiceResponse.ActionResult != null)
            {
                return handleServiceResponse.ActionResult;
            }
            //We must have a separate view fs which must be use in the search controller
            //Check if users are in the same membership

            //**Build the view model.  We should have a separate method called he view instead of passing the edit mode as this can me the 
            //**build method very complex
            FSViewModelHelper.BuildViewModelForView(viewModel2, response2.FSView, response2.SectorsOfOperation);
            viewModel2.ViewModeFromViewAction = true;

            //Viewmodel activities should only be put in the inheried FS View model of view
            if (response2.FSView.FSActivityGridView != null)
            {
                viewModel2._ActivitiesViewModel = new _ActivitiesViewModel();
                viewModel2._ActivitiesViewModel.ActivityGridView = response2.FSView.FSActivityGridView;
                viewModel2._ActivitiesViewModel.MembershipId = response2.FSView.MembershipId;
            }
            viewModel.fsViewModel = viewModel2;
            viewModel.RegistrationNo = viewModel2.FSView.RegistrationNo;
            viewModel.SearchId = SearchId;

            TrackSearchResultRequest request3 = new TrackSearchResultRequest();
            if (Session["PublicUserCode"] != null)
                request3.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
            this.HttpRequestToServiceRequest(request3);
            request3.IsGenerateReport = false;
            request3.SearchFinancialStatementId = SearchId;
            request3.RegistrationNo = viewModel2.FSView.RegistrationNo;
            request3.PayableTransaction = viewModel._SearchResultJqGridViewModel.isPayable;
            TrackSearchResultResponse response3 = SS.TrackSearchResult(request3);

            if (response3.MessageInfo.MessageType == MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            if (response3.MessageInfo.MessageType == MessageType.Error)
            {
                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.Error,
                    Message = response3.MessageInfo.Message
                };
                TempData["TitleMessage"] = _msg;
                return RedirectToAction("LoadSearch", "Search", new { Area = "Search", SearchId = SearchId, IsPreviousSearch = false });
            }

            viewModel._SearchResultJqGridViewModel.HasGeneratedReport = response3.HasSearchReport;

            return View(viewModel);
        }

        public JsonResult AjaxGenerateReport(SearchViewModel viewModel)
        {
            GenerateSearchReportRequest request = new GenerateSearchReportRequest();
            this.HttpRequestToServiceRequest(request);

            // We did this to avoid making the email field mandatory ****** This can be done at the front end though
            if (!Request.IsAuthenticated && (viewModel.SendAsMail && String.IsNullOrWhiteSpace(viewModel.PublicUserEmail)))
            {
                request.SendAsMail = false;
            }
            else if (Request.IsAuthenticated && !viewModel.SendAsMail)
            {
                request.SendAsMail = false;
            }
            else if (!Request.IsAuthenticated && !viewModel.SendAsMail)
            {
                request.SendAsMail = false;
            }

            else
            {
                request.SendAsMail = true;
            }

            //Make sure we have a selected financing statement
            //if (viewModel._SearchResultJqGridViewModel.SearchResultView != null)
            //{

            //    request.SelectedFS = viewModel._SearchResultJqGridViewModel.SearchResultView.Where(s => s.IsSelected == true).Select(s => s.Id).ToArray();
            //    if (request.SelectedFS.Length == 0)
            //    {
            //        return Json("Please select one or more of search result to generate search report", JsonRequestBehavior.AllowGet);
            //    }
            //}

            request.SelectedFS = viewModel._SearchResultJqGridViewModel.SelectedId;
            request.PayableTransaction = viewModel._SearchResultJqGridViewModel.isPayable;
           // request.publicUsrEmail = viewModel.PublicUserEmail == null ? request.SecurityUser.Email : viewModel.PublicUserEmail;
           // request.SendAsMail = viewModel.SendAsMail;


            //Assign public user code if available
            if (Session["PublicUserCode"] != null)
                request.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString(); //Why do we need this I think it is for the search request id

            IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("_SearchResultJqGridViewModel.Phone")
                   || s.Contains("_SearchResultJqGridViewModel.BVN") || s.Contains("_SearchResultJqGridViewModel.SearchParam.BusinessPrefix"));
            ICollection<string> copyKeys = new List<string>();
            foreach (var mykey in mykeys)
            {
                string s = String.Copy(mykey);
                copyKeys.Add(s);
            }

            foreach (var mykey in copyKeys)
            {
                ModelState.Remove(mykey);
            }

            if (ModelState.IsValid)
            {
                SearchViewModelHelper.MapToGenerateSearchReport(viewModel, request);
                GenerateSearchReportResponse response = SS.GenerateSearchReport(request);

                if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
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
                    return Json(ViewBag.TitleMessage, JsonRequestBehavior.AllowGet);
                }
                else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    string result = "You are not authorized to perform this operation!";
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }
                else
                {
                    string result = "success";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already
                string result = "Error encountered trying to save file";
                return Json(result, JsonRequestBehavior.AllowGet);

            }



        }


        public ActionResult DownloadReport(int SearchId, string RegistrationNo)
        {
            DownloadSearchReportRequest request = new DownloadSearchReportRequest
            {
                isCertified = true,
                SearchId = SearchId,
                Id = SearchId
            };
            this.HttpRequestToServiceRequest(request);
            if (Session["PublicUserCode"] != null)
                request.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
            request.RegistrationNo = RegistrationNo;
            DownloadSearchReportResponse response = SS.DownloadSearchReport(request);
            HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response);

            TrackSearchResultRequest request3 = new TrackSearchResultRequest();
            if (Session["PublicUserCode"] != null)
                request3.SecurityCodeForPublicUser = Session["PublicUserCode"].ToString();
            this.HttpRequestToServiceRequest(request3);
            request3.IsGenerateReport = true;
            request3.SearchFinancialStatementId = SearchId;
            request3.RegistrationNo = RegistrationNo;
            TrackSearchResultResponse response3 = SS.TrackSearchResult(request3);
            if (response3.MessageInfo.MessageType == MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }

            if (response3.MessageInfo.MessageType == MessageType.Error)
            {
                return null;
            }


            if (handleServiceResponse.ActionResult != null)
            {
                return handleServiceResponse.ActionResult;
            }

            else
            {
                return File(response.AttachedFile, response.AttachedFileType, response.AttachedFileName);
            }

        }


        public JsonResult CheckSecurityCode(string SecurityCode)
        {
            ValidateSecurityCodeRequest request = new ValidateSecurityCodeRequest();
            request.SecurityCode = SecurityCode;
            ValidateSecurityCodeResponse response = SS.ValidateSecurityCode(request);
            var result = new
                             {
                                 IsValid = response.IsValid,
                                 HasInfo = response.HasInfo,
                                 PaymentType = (int)response.PaymentType,
                                 Balance = response.Balance
                             };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View the details of a financing statement and is anonymous to allow the search controller access this for public users.
        /// This is however not advisable because we do not want users just getting here by passing any Id.  We need that to come from a button 
        /// posts the Id and we will work on that
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ViewFS(string EncryptedId)
        {


            //Create the view model which will be associated with the corresponding view
            FSViewModel viewModel = new FSViewModel();

            //We need a different service for getting just the details for viewing
            int Id = Convert.ToInt32(Util.Decrypt(EncryptedId));

            GetFSViewRequest request = new GetFSViewRequest() { Id = Id };
            //Add security, url and IP to the request class
            this.HttpRequestToServiceRequest(request);

            //We need a different service for getting just the details for viewing
            GetFSViewResponse response = FS.GetView(request);

            //We must have a separate view fs which must be use in the search controller
            //Check if users are in the same membership

            //**Build the view model.  We should have a separate method called he view instead of passing the edit mode as this can me the 
            //**build method very complex
            FSViewModelHelper.BuildViewModelForView(viewModel, response.FSView, response.SectorsOfOperation);


            //We must have a separate view fs which must be use in the search controller
            //Check if users are in the same membership
            if (!this.HttpContext.Request.IsAuthenticated)
            {
                viewModel.HideStatisicalInfoInReadMode = true;
            }
            else
            {
                //Over here we need to make sure that the user came from the search link
                if (SecurityUser.IsOwnerUser == false && response.FSView.MembershipId != SecurityUser.MembershipId)
                {
                    viewModel.HideStatisicalInfoInReadMode = true;
                }

            }
            //Viewmodel activities should only be put in the inheried FS View model of view
            if (response.FSView.FSActivityGridView != null)
            {
                viewModel._ActivitiesViewModel = new _ActivitiesViewModel();

                viewModel._ActivitiesViewModel.ActivityGridView = response.FSView.FSActivityGridView;
            }






            return View("~/Areas/FinancialStatement/Views/FinancialStatement/DetailViewFS.cshtml", viewModel);

        }

        public ActionResult ViewSearches()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ViewSearches(ViewSearchesViewModel viewModel)
        {
            string errorMessage = "";

            if (String.IsNullOrWhiteSpace(viewModel.PublicUserSecurityCode))
            {
                errorMessage = "Please provide a valid security code";
            }

            if (ModelState.IsValid)
            {
                ValidateSecurityCodeRequest request = new ValidateSecurityCodeRequest();
                request.SecurityCode = viewModel.PublicUserSecurityCode;
                ValidateSecurityCodeResponse response = SS.ValidateSecurityCode(request);
                if (response.IsValid)
                {
                    TempData["PublicSecurityCode"] = viewModel.PublicUserSecurityCode;
                    Session["PublicUserCode"] = viewModel.PublicUserSecurityCode;
                    return RedirectToAction("Index", "Search");
                }
                errorMessage = "The security code entered is invalid!";

            }
            MessageInfo _msg = new MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = errorMessage };
            ViewBag.TitleMessage = _msg;
            return View(viewModel);
        }

        public JsonResult AuditCacLink()
        {
            RequestBase request = new RequestBase();
            request.SecurityUser = SecurityUser;
            this.HttpRequestToServiceRequest(request);
            ResponseBase response = SS.AuditCacLink(request);
            string message = "";
            if (response.MessageInfo.MessageType == MessageType.Success)
                message = "Audit successful";
            var result = new
            {
                message = message
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchCAC()
        {
            CACSearchrequest request = new CACSearchrequest();

            var searchResults = new CACSearch();

            //request.apiUrl = Constants.cacurl;
            //System.Configuration.ConfigurationManager.AppSettings["cacurl"];

            //request.uniqueId = Session["uniqueId"] != null ? Session["uniqueId"].ToString() : null;
            CACSearchresponse response = SS.GetCACSearchResults(request);

            searchResults = JsonConvert.DeserializeObject<CACSearch>(response.CACResults);
            return View(searchResults);
        }


        public ActionResult DummyView()
        {
            return View();
        }

        public ActionResult AnotherDummyView()
        {
            return View();
        }
    }
}
