using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Authentication.Request;
using CRL.Service.Messaging.Authentication.Response;
using CRL.UI.MVC.Common;
using CRL.UI.MVC.Models;
using CRL.Infrastructure.Messaging;
using CRL.UI.MVC.Common.CustomActionFilters;
using System.Web.SessionState;

namespace CRL.UI.MVC.Controllers
{
     [MasterEventAuthorizationAttribute]
    public class AccountController : CRLController
    {
         IAuthenticationService _as;
         public AccountController(IAuthenticationService _As)
         {
             _as = _As;
         }
        //
        // GET: /Account/Login
       
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
         
            //Generate a unique token for this request and add it to the view model
            LoginModel viewModel = new LoginModel();
            viewModel.ReturnUrl = returnUrl;
            viewModel.UniqueGuidForm = Guid.NewGuid().ToString("N") + "0001";
            return View(viewModel);
        }


        //
        // POST: /Account/Login
         //[RequireHttps]
        [HttpPost]
        [AllowAnonymous]
        
      
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (!String.IsNullOrWhiteSpace(returnUrl) && (returnUrl.Contains("+") || returnUrl.Contains("%") 
                || returnUrl.Contains("-") || returnUrl.Contains("'")))
            {
                MessageInfo msg = new MessageInfo();
                msg.MessageType = MessageType.Error;
                msg.Message = "Invalid url passed";
                ViewBag.TitleMessage = msg;
                return View(model);
            }
           
            string Message = "";
            //We will first validate using the membership authentication class
            //If User is validated then let's log them in
            if (ModelState.IsValid)
            {
                //Setup the service request
                LoginRequest request = new LoginRequest();
                request.Username = model.UserName;
                request.Password = model.Password;
                request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
                request.UserIP = AuditHelper.GetUserIP(this.Request);
                request.UniqueGuidForm = model.UniqueGuidForm;
                //Call the service
                LoginResponse response = null;

                int numRetriesAfterConcurrencyError = 0;
                do
                {
                    numRetriesAfterConcurrencyError++;
                    response = AS.LoginUser(request);

                    if (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries)
                    {
                        this.NewAuthenticationService(AS);
                    }
                    numRetriesAfterConcurrencyError++;

                } while (response.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict && numRetriesAfterConcurrencyError < Constants.MaxDatabaseConcurrencyConflictRetries);

                HandleServiceResponse handleServiceResponse = this.HandleServiceResponse(response);


                if (handleServiceResponse.ActionResult != null)
                {
                    return handleServiceResponse.ActionResult;
                }
                else if (handleServiceResponse.NoErrorsEncountered)
                {
                    if (response.PasswordChangeRequired)
                    {
                        if (String.IsNullOrWhiteSpace(returnUrl))
                        {
                            //returnUrl = "/Home/Dashboard"; 
                            return RedirectToAction("Index", "Home");
                        }


                        return RedirectToAction("MandatoryPasswordChange", "User", new { Area = "Membership", Id = response.User.Id, Code = response.ResetPasswordNextLoginCode, ReturnUrl = returnUrl });
                    }

                    if (User.Identity.IsAuthenticated == false)
                    {

                        System.Web.Helpers.AntiForgery.Validate();
                        FormsAuthentication.Initialize();
                        FormsAuthenticationTicket fat = new FormsAuthenticationTicket(1, response.User.Name, DateTime.Now, DateTime.Now.AddMinutes(Constants.Timeout), false, response.User.Id.ToString(), FormsAuthentication.FormsCookiePath);
                        HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(fat)));
                        SessionIDManager manager = new SessionIDManager();
                        string newSessionId = manager.CreateSessionID(System.Web.HttpContext.Current);
                        Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", newSessionId));
                        HttpContext.User = response.User;
                        Session["SecurityUser"] = response.User;
                    }

                    if (response.PasswordWarnOfChangeRequired)
                    {
                        //**Set a session which will be used to warn the user of the need to change their password
                        TempData["PasswordWarnOfChangeRequired"] = true;
                        TempData["NumPasswordChangeDaysRemaining"] = 100;
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(returnUrl))
                        {
                            //return RedirectToLocal("Home/Dashboard");
                            return RedirectToAction("Dashboard", "Home");
                        }
                        else

                            return RedirectToLocal(returnUrl);
                    }
                }

                Message = response.MessageInfo.Message;
            }


            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", Message);
            return View(model);
        }

        //
        // POST: /Account/LogOff
          [AllowAnonymous]
        public ActionResult LogOff()
        {
            LogoffRequest request = new LogoffRequest() { SecurityUser = SecurityUser ,
                RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath,
                UserIP = AuditHelper.GetUserIP(this.Request)};
            LogoffResponse response = _as.LogoffUser(request);

            FormsAuthentication.SignOut();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return RedirectToAction("Index", "Home");                               
            
              
        }

          public string ExtendSession(string timestamp)
         {
             return "Session extended";
         }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            //if (Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return RedirectToAction("Dashboard", "Home");
        }
        #endregion Helpers


    }
}
