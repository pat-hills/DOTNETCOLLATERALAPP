using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Views.Payments;
using CRL.UI.MVC.Areas.Payment.Models.ModelHelpers;
using CRL.UI.MVC.Areas.Payment.Models.ViewPageModels;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using System.Configuration;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using StructureMap;
using CRL.Model.Payments;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Areas.Payment.Controllers
{
    public class InterSwitchController : CRLController
    {
        private IPaymentService _ps;


        public InterSwitchController(IPaymentService ps)
        {
            _ps = ps;
        }

        //
        // GET: /Payment/InterSwitch/

        public ActionResult PaymentDetails(string refno = null)
        {
            var viewModel = new InterSwitchUserViewModel();
            IConfigurationService _cs = ObjectFactory.GetInstance<IConfigurationService>();
            RequestBase request1 = new RequestBase ();

            GetFeeResponse response1 = _cs.GetFeeForPublicSearch(request1);
            //    GetFeeConfigurationResponse  response1 = _cs.GetFeeConfiguration (request1);

        ////    fee = _configurationFeeRepository.GetDbSetComplete(feeTytpe, LenderType).SingleOrDefault();         


            if (!String.IsNullOrEmpty(refno))
            {
                GetInterSwitchDetailsRequest request = new GetInterSwitchDetailsRequest();
                request.TransactionRefNo = InterSwitchHelper.Decrypt(refno);
                GetInterSwitchDetailsResponse response = _ps.GetInterSwitchDetails(request);
                if ((bool)response.InterSwitchTransactionView.IsProcessed)
                {
                    MessageInfo _msg = new MessageInfo
                    {
                        MessageType = Infrastructure.Messaging.MessageType.Error,
                        Message = "Transaction unvailable!"
                    };

                    ViewBag.TitleMessage = _msg;
                    TempData["TitleMessage"] = _msg;
                    return RedirectToAction("PaymentDetails", "InterSwitch");
                }

                InterSwitchViewModelHelper.BuildViewModelForView(viewModel, response);
            }
            viewModel.InterSwitchUserView.TopUpCode = null;
            if (response1.fee != null)
                viewModel.InterSwitchUserView.PricePerSearch = response1.fee.Fee;// onfigurationTransactionFeesViews.SingleOrDefault(s => s.ServiceTypeId == ServiceFees.PublicSearch && s.LenderTypeId == 360).Fee;
            else
                viewModel.InterSwitchUserView.PricePerSearch = 0;
            return View(viewModel);
        }

        [HttpPost, CaptchaMvc.Attributes.CaptchaVerify("The symbols you entered were incorrect")]
        public ActionResult PaymentDetails(InterSwitchUserViewModel viewModel)
        {
            if (!viewModel.InterSwitchUserView.IsTopUpPayment)
            {
               
                    IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("InterSwitchUserView.TopUpCode"));
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


            if (ModelState.IsValid)
            {
                SubmitInterSwitchDetailsRequest request = new SubmitInterSwitchDetailsRequest();

                string port = "";
                if (ConfigurationManager.AppSettings["ApplicationPort"] != null)
                {
                    port = !String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ApplicationPort"].ToString()) ? ":" + ConfigurationManager.AppSettings["ApplicationPort"] : "";
                }

                string Root = System.Web.HttpContext.Current.Request.ApplicationPath;

                if (Root.Trim() != "/")
                    Root += "/";
                string urlSuffix = System.Web.HttpContext.Current.Request.Url.Authority + port + Root;
                Root = System.Web.HttpContext.Current.Request.Url.Scheme + @"://" + urlSuffix;

                if (ConfigurationManager.AppSettings["RootFolder"] != null)
                {
                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RootFolder"].ToString()))
                        Root += ConfigurationManager.AppSettings["RootFolder"].ToString() + "/";
                }
                viewModel.InterSwitchUserView.site_redirect_url = Root + InterSwitchConfig.RedirectUrl;

                InterSwitchViewModelHelper.BuildForCreateEdit(viewModel, request);

                SubmitInterSwitchDetailsResponse response = _ps.CreateEditInterSwitchUserDetails(request);

                if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
                  response.MessageInfo.MessageType == MessageType.Error)
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
                else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    //TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("VerifyPaymentDetails", new { refno = InterSwitchHelper.Encrypt(response.TransactionRefNo) });
                }
                else
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                }

            }
            else 
            {
                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Business Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;
            }
            return View(viewModel);
        }

        public ActionResult VerifyPaymentDetails(string refno)
        {
            var viewModel = new InterSwitchTransactionDetailsViewModel();
            GetInterSwitchDetailsRequest request = new GetInterSwitchDetailsRequest();
            request.TransactionRefNo = InterSwitchHelper.Decrypt(refno);
            GetInterSwitchDetailsResponse response = _ps.GetInterSwitchDetails(request);

            if ((bool)response.InterSwitchTransactionView.IsProcessed)
            {
                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.Error,
                    Message = "Transaction unvailable!"
                };

                ViewBag.TitleMessage = _msg;
                TempData["TitleMessage"] = _msg;
                return RedirectToAction("PaymentDetails", "InterSwitch");
            }
            
            IConfigurationService _cs = ObjectFactory.GetInstance<IConfigurationService>();
            //GetDataForConfigurationTransactionFeesRequest request1 = new GetDataForConfigurationTransactionFeesRequest();
            RequestBase request1 = new RequestBase();

            GetFeeResponse response1 = _cs.GetFeeForPublicSearch(request1);
            //GetDataForConfigurationTransactionFeesResponse response1 = _cs.GetDataForConfigurationTransactionFees(request1);
            response.InterSwitchTransactionView.PricePerSearch = response1.fee.Fee;//response1.ConfigurationTransactionFeesViews.SingleOrDefault(s => s.ServiceTypeId == ServiceFees.Search).Fee;
            InterSwitchViewModelHelper.BuildVerifyViewModelForView(viewModel, response);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Return(InterSwitchResponseViewModel viewModel)
        {
            string transactionRefNo = null;
            try
            {
                if (!String.IsNullOrEmpty(viewModel.txnref))
                {
                    InterSwitchApiQueryRequest request = new InterSwitchApiQueryRequest();
                    request.TransactionRefNo = viewModel.txnref;
                    transactionRefNo = viewModel.txnref;
                    request.UrlLink = Url.Action("Search", "Search", new { Area = "Search" }, Request.Url.Scheme);
                    InterSwitchApiQueryResponse response = _ps.GetInterSwitchApiQueryResult(request);

                    TempData["TxnRefNo"] = InterSwitchHelper.Encrypt(transactionRefNo);
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ViewPayment");
        }

        public ActionResult ViewPayment(string txnRef = null, int type = 0)
        {
            GetInterSwitchViewPaymentDetailsRequest request = new GetInterSwitchViewPaymentDetailsRequest();
            request.TransactionRefNo = txnRef;
            
            if (type == 0) 
            {
                if (TempData["TxnRefNo"] != null)
                {
                    request.TransactionRefNo = InterSwitchHelper.Decrypt(TempData["TxnRefNo"].ToString());
                }
                
            }
            
            GetInterSwitchViewPaymentDetailsResponse response = _ps.ViewSummaryInterSwitchTransaction(request);
            var viewModel = new InterSwitchPaymentSummaryViewModel();
            viewModel._TransactionDetailsPartialViewModel.TransactionResponseView = response.TransactionResponseView;
            viewModel._TransactionDetailsPartialViewModel.TransactionRefNo = response.TransactionRefNo;
            viewModel._TransactionDetailsPartialViewModel.Message = response.Message;
            viewModel._TransactionDetailsPartialViewModel.PaymentId = response.PaymentId;
            viewModel._TransactionDetailsPartialViewModel.SecurityCode = response.SecurityCode;
            viewModel._TransactionDetailsPartialViewModel.HasEmail = response.HasEmail;
            return View(viewModel);
        }

        public ActionResult TransactionDetails(string refno = null)
        {
            var viewModel = new TransactionDetailsViewModel();
            if (refno != null)
            {
                viewModel.TransRefNo = refno;
            }
            viewModel._TransactionDetailsPartialViewModel = new _TransactionDetailsPartialViewModel();
            viewModel._TransactionDetailsPartialViewModel.IsPageLoad = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult TransactionDetails(TransactionDetailsViewModel viewModel)
        {
            viewModel.IsTempResponse = false;
            if (viewModel.QueryTransaction != null)
            {
                InterSwitchApiQueryRequest request1 = new InterSwitchApiQueryRequest();
                request1.TransactionRefNo = viewModel.TransRefNo;
                request1.UrlLink = Url.Action("Search", "Search", new { Area = "Search" }, Request.Url.Scheme);
                request1.IsReQuery = true;
                InterSwitchApiQueryResponse response1 = _ps.GetInterSwitchApiQueryResult(request1);
                if (!response1.IsConnectivityError)
                {
                    if (response1.ResponseCode == InterSwitchConfig.UnknownTxnRefNo)
                    {
                        viewModel.IsTempResponse = true;
                    }
                }
            }
            GetInterSwitchViewPaymentDetailsRequest request = new GetInterSwitchViewPaymentDetailsRequest();
            request.TransactionRefNo = viewModel.TransRefNo;
            GetInterSwitchViewPaymentDetailsResponse response = _ps.ViewSummaryInterSwitchTransaction(request);
            viewModel._TransactionDetailsPartialViewModel.TransactionResponseView = response.TransactionResponseView;
            viewModel._TransactionDetailsPartialViewModel.TransactionRefNo = response.TransactionRefNo;
            viewModel._TransactionDetailsPartialViewModel.Message = response.Message;
            viewModel._TransactionDetailsPartialViewModel.PaymentId = response.PaymentId;
            viewModel._TransactionDetailsPartialViewModel.HasEmail = response.HasEmail;
            viewModel._TransactionDetailsPartialViewModel.SecurityCode = response.SecurityCode;

            viewModel.TransRefNo = response.TransactionRefNo;
            viewModel._TransactionDetailsPartialViewModel.IsPageLoad = false;
            viewModel.IsProcessed = response.IsAuthorized;


            return View(viewModel);
        }

        public ActionResult GetTransactionResult(_TransactionDetailsPartialViewModel model)
        {
            return PartialView("_TransactionDetails", model);
        }



        [MasterEventAuthorizationAttribute]
        public ActionResult InterSwitchTransactionsIndex()
        {
            InterSwitchTransactionIndexViewModel viewModel = new InterSwitchTransactionIndexViewModel();
            InterSwitchViewModelHelper.LoadWebPayTransactionTypes(viewModel._InterSwitchTransactionsJqGridViewModel.TransactionLogTypes);
            return View("InterSwitchTransactionsIndex", viewModel);
        }

        public PartialViewResult SubmbitSearchFilterForJqGrid(_InterSwitchTransactionsJqGridViewModel _InterSwitchTransactionsJqGridViewModel)
        {
            return PartialView("~/Areas/Payment/Views/InterSwitch/_InterSwitchTransactionJqGrid.cshtml", _InterSwitchTransactionsJqGridViewModel);
        }

        public ActionResult ViewInterSwitchJsonData(GridSettings grid, _InterSwitchTransactionsJqGridViewModel viewModel)
        {
            GetAllInterSwitchTransactionsRequest request = new GetAllInterSwitchTransactionsRequest();
            request.SecurityUser = SecurityUser;
            InterSwitchViewModelHelper.MapViewModelToViewInterSwitchTransactions(viewModel, request, grid);
            GetAllInterSwitchTransactionsResponse response = _ps.GetInterSwitchTransactions(request);
            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
            {
                InterSwitchTransactionView[] ArrayRows = response.InterSwitchTransactionViews.ToArray();
                var result = new { total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize), page = grid.PageIndex, records = response.NumRecords, rows = ArrayRows };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "failure" : response.MessageInfo.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [MasterEventAuthorizationAttribute]
        public ActionResult DirectPayIndex()
        {
            DirectPayTransactionIndexViewModel viewModel = new DirectPayTransactionIndexViewModel();
            InterSwitchViewModelHelper.LoadDirectPayTransactionTypes(viewModel._DirectPayTransactionsJqGridViewModel.TransactionLogTypes);
            return View(viewModel);
        }

        public PartialViewResult SubmbitDirectPaySearchFilterForJqGrid(_DirectPayTransactionsJqGridViewModel _DirectPayTransactionsJqGridViewModel)
        {
            return PartialView("~/Areas/Payment/Views/InterSwitch/_DirectPayTransactionsJqGrid.cshtml", _DirectPayTransactionsJqGridViewModel);
        }

        public ActionResult ViewDirectPayJsonData(GridSettings grid, _DirectPayTransactionsJqGridViewModel viewModel)
        {
            GetAllDirectPayTransactionsRequest request = new GetAllDirectPayTransactionsRequest();
            request.SecurityUser = SecurityUser;
            InterSwitchViewModelHelper.MapViewModelToViewDirectPayTransactions(viewModel, request, grid);
            GetAllDirectPayTransactionsResponse response = _ps.GetDirectPayTransactions(request);
            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
            {
                InterSwitchDetailsView[] ArrayRows = response.InterSwitchDetailViews.ToArray();
                var result = new { total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize), page = grid.PageIndex, records = response.NumRecords, rows = ArrayRows };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "failure" : response.MessageInfo.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DirectPayDetails(int? id)
        {
            GetDirectPayTransactionDetailsRequest request = new GetDirectPayTransactionDetailsRequest();
            request.SecurityUser = SecurityUser;
            request.Id = (int)id;

            GetDirectPayTransactionDetailsResponse response = _ps.GetDirectPayTransactionDetails(request);
         
            DirectPayTransactionDetailsViewModel viewModel = new DirectPayTransactionDetailsViewModel();
            if (response.MessageInfo.MessageType != Infrastructure.Messaging.MessageType.Success)
            {
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
                {
                    return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
                }

                return RedirectToAction("DirectPayIndex", "InterSwitch", new { Area = "Payment" });
            }

            viewModel.InterSwitchDetailsView = response.InterSwitchDetailsView;
            return View("ViewDirectPayDetails", viewModel);
        }

        public ActionResult Download(int Id)
        {
            GetReceiptRequest request = new GetReceiptRequest { Id = Id, SecurityUser = SecurityUser };

            GetReceiptResponse response = PY.GetReceiptRequest(request);

            //System.Net.Mime.MediaTypeNames.Application.Octet
            return File(
                response.AttachedFile, response.AttachedFileType, response.AttachedFileName + ".pdf");
        }

        public ActionResult CheckBalance()
        {
            CheckBalanceViewModel viewModel = new CheckBalanceViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CheckBalance(CheckBalanceViewModel viewModel)
        {
            string errorMessage = "";

            if (String.IsNullOrWhiteSpace(viewModel.PublicUserSecurityCode))
            {
                errorMessage = "Please provide a valid PIN Code";
            }

            if (ModelState.IsValid)
            {
                ValidateSecurityCodeRequest request = new ValidateSecurityCodeRequest();
                request.SecurityCode = viewModel.PublicUserSecurityCode;
                ValidateSecurityCodeResponse response = SS.ValidateSecurityCode(request);
                if (response.IsValid)
                {                    
                    viewModel.Balance = response.Balance;
                    viewModel.HasResponse = true;
                    return View(viewModel);
                }
                errorMessage = "The PIN Code entered is invalid!";

            }

            MessageInfo _msg = new MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = errorMessage };
            ViewBag.TitleMessage = _msg;
            return View(viewModel);
        }

        public ActionResult GenerateVoucherCode()
        {
            var viewModel = new InterSwitchUserViewModel();
            IConfigurationService _cs = ObjectFactory.GetInstance<IConfigurationService>();
            GetDataForConfigurationTransactionFeesRequest request1 = new GetDataForConfigurationTransactionFeesRequest();
            GetDataForConfigurationTransactionFeesResponse response1 = _cs.GetDataForConfigurationTransactionFees(request1);
            viewModel.InterSwitchUserView.PricePerSearch = response1.ConfigurationTransactionFeesViews.SingleOrDefault(s => s.ServiceTypeId == ServiceFees.PublicSearch).Fee;
            return View(viewModel);
        }

        [HttpPost, CaptchaMvc.Attributes.CaptchaVerify("The symbols you entered were incorrect")]
        public ActionResult GenerateVoucherCode(InterSwitchUserViewModel viewModel)
        {
            IEnumerable<string> mykeys = ModelState.Keys.Where(s => s.Contains("InterSwitchUserView.TopUpCode") 
                || s.Contains("InterSwitchUserView.Amount")
                || s.Contains("InterSwitchUserView.Quantity"));
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
                CreateDirectPayDetailsRequest request = new CreateDirectPayDetailsRequest();
                request.InterSwitchUserView = new InterSwitchUserView();
                request.InterSwitchUserView = viewModel.InterSwitchUserView;
                CreateDirectPayDetailsResponse response = _ps.CreateDirectPayDetails(request);
                if (response.MessageInfo.MessageType == MessageType.Success)
                {
                    TempData["Details"] = new string[]{response.PublicVoucherCode,request.InterSwitchUserView.Name, request.InterSwitchUserView.Phone};
                    return RedirectToAction("GenerateVoucherCodeSuccess");
                }
            }
            return View(viewModel);
        }

        public ActionResult GenerateVoucherCodeSuccess()
        {
            if (TempData["Details"] != null)
            {
                var details = (string[])TempData["Details"];
                ViewBag.Voucher = details[0];
                ViewBag.Name = details[1];
                ViewBag.Phone = details[2];
            }
            else
            {
                return RedirectToAction("GenerateVoucherCode");
            }
            return View();
        }

        public ActionResult DownloadPaymentVoucher(string voucherCode)
        {
            GetPaymentVoucherRequest request = new GetPaymentVoucherRequest { PaymentVoucherCode = voucherCode, SecurityUser = SecurityUser };

            GetPaymentVoucherResponse response = PY.GetPaymentVoucher(request);

            //System.Net.Mime.MediaTypeNames.Application.Octet
            return File(
                response.AttachedFile, response.AttachedFileType, response.AttachedFileName);
        }
    }
}
