using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using StructureMap;
using CRL.Service.Interfaces.FinancialStatement;
using CRL.Service.IOC;
using CRL.Service.Interfaces;


namespace CRL.UI.MVC.Common
{
    public class HandleServiceResponse : ResponseBase
    {
                
        public bool MapValidationsToModelState { get; set; }
        public bool NoErrorsEncountered { get; set; }
        public ActionResult ActionResult { get; set; } 
    }
    public class HandleServiceResponseJSON : ResponseBase
    {
        public bool NoErrorsEncountered { get; set; }
        public JsonResult ActionResult { get; set; }
    }
    
    public class ControllerFactoryBase : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {

            if (controllerType == null)
                return null;

            try
            {
                return ObjectFactory.GetInstance(controllerType) as IController;
            }
            catch
            {
                // Debug.WriteLine(ObjectFactory.WhatDoIHave());
                throw;
            }
        }
      

      
    }

    public abstract class CRLController : Controller
    {
         private  IFinancingStatementService _fs;
         private ISearchService _ss;
         private IMembershipRegistrationService _ms;
         private IUserService _us;
         private IInstitutionService _is;
         private IInstitutionUnitService _ius;
         private IClientService _cs;
         private IPaymentService _ps;
         private IAuthenticationService _as;



         protected void NewAuthenticationService(IAuthenticationService SS)
         {
             _as = null;

         }

         protected IAuthenticationService AS
         {
             get { if (_as != null) return _as; else { _as = ServiceFactory.AuthenticatonService (); return _as; } }

         }

         protected void NewPaymentService(IPaymentService PY)
         {
             _ps = null;

         }


        protected IPaymentService PY
        {
            get { if (_ps != null) return _ps; else { _ps = ServiceFactory.PaymentService(); return _ps; } }

    }


         protected IClientService CS
         {
             get { if (_cs != null) return _cs; else { _cs = ServiceFactory.ClientService(); return _cs; } }
         }

         

       

         protected IMembershipRegistrationService  MS
         {
             get { if (_ms != null) return _ms; else { _ms = ServiceFactory.MembershipRegistrationService (); return _ms; } }

         }

         protected void NewMembershipReistrationService(IMembershipRegistrationService MS)
         {
             _ms = null;

         }
         protected IUserService US
         {
             get { if (_us != null) return _us; else { _us = ServiceFactory.UserService (); return _us; } }

         }

         protected void NewUserService(IUserService  US)
         {
             _us = null;

         }
         protected IInstitutionService  IS
         {
             get { if (_is != null) return _is; else { _is = ServiceFactory.InstitutionService (); return _is; } }

         }

         protected void NewInstitutionService(IInstitutionService  IS)
         {
             _is = null;

         }

         protected IInstitutionUnitService IUS
         {
             get { if (_ius != null) return _ius; else { _ius = ServiceFactory.InstitutionUnitService(); return _ius; } }

         }

         protected void NewInstitutionUnitService(IInstitutionUnitService IUS)
         {
             _ius = null;

         }
        protected IFinancingStatementService FS
        {
            get { if (_fs != null) return _fs; else { _fs = ServiceFactory.FinancialStatementService(); return _fs; } }
          
        }

        protected void NewFinanceService(IFinancingStatementService FS)
        {
            _fs = null;

        }

        protected ISearchService SS
        {
            get { if (_ss != null) return _ss; else { _ss = ServiceFactory.SearchService(); return _ss; } }

        }

        protected void NewSearchService(ISearchService SS)
        {
            _ss = null;

        }

    
        /// <summary>
        /// Secutiry User. If there is no current user then should it not return a security null user
        /// </summary>
        protected SecurityUser SecurityUser
        {
            get
            {
                if (this.HttpContext.User.Identity.IsAuthenticated)
                    return (SecurityUser)this.HttpContext.User;
                else
                    return null;

            }
        }

        protected void HttpRequestToServiceRequest(RequestBase request)
        {

            request.SecurityUser = this.SecurityUser; //We must first check if this request is authenticated
            request.RequestUrl = AuditHelper.UrlOriginal(this.Request).AbsolutePath;
            request.UserIP = AuditHelper.GetUserIP(this.Request);
        }


        protected HandleServiceResponseJSON HandleServiceResponseJSON(ResponseBase ServiceResponse)
        {
            HandleServiceResponseJSON response = new HandleServiceResponseJSON();
            string result;
            if (ServiceResponse.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
            {
                result = String.IsNullOrWhiteSpace(ServiceResponse.MessageInfo.Message) ? "Action was successful" : ServiceResponse.MessageInfo.Message;
                
            }
            else if (ServiceResponse.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                result = String.IsNullOrWhiteSpace(ServiceResponse.MessageInfo.Message) ? "You are not authorized to perform this action" : ServiceResponse.MessageInfo.Message;
               
            }
            else
            {

                result = String.IsNullOrWhiteSpace(ServiceResponse.MessageInfo.Message) ? "Could not perform this action" : ServiceResponse.MessageInfo.Message;
                
            }

            response.ActionResult = Json(result, JsonRequestBehavior.AllowGet);
            return response;
        }
        /// <summary>
        /// Checks the response from a service and generates the necessary action 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected HandleServiceResponse HandleServiceResponse(ResponseBase ServiceResponse, bool referBusinessErrorsToGeneralPage = false)
        {
            HandleServiceResponse response = new HandleServiceResponse();
           

            if (ServiceResponse.MessageInfo.MessageType == MessageType.DatabaseConcurrencyConflict)
            {
                ServiceResponse.MessageInfo.MessageType = MessageType.BusinessValidationError;
                ServiceResponse.MessageInfo.Message = "An error occured whiles trying to submit your request.  Please try again!";
                
            }

            else if (ServiceResponse.MessageInfo.MessageType == MessageType.DatabaseUniqueKeyConflict )
            {
                ServiceResponse.MessageInfo.MessageType = MessageType.BusinessValidationError;
                ServiceResponse.MessageInfo.Message = "An error occured whiles trying to submit your request.  Please try again!";

            }

      
            //If we are okay then let us redirect to the View action.
            else if (ServiceResponse.MessageInfo.MessageType == MessageType.BusinessValidationError ||
         ServiceResponse.MessageInfo.MessageType == MessageType.Error )
            {
                if (ServiceResponse.ValidationErrors!= null && ServiceResponse.ValidationErrors.Count > 0)
                {
                    response.MapValidationsToModelState = true;
                    
                    
                }
                MessageInfo _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = ServiceResponse.MessageInfo.Message
                };
                if (String.IsNullOrWhiteSpace(_msg.Message))
                {
                    _msg.Message = "Business Validation Errors were detected";
                }
                ViewBag.TitleMessage = _msg;

                if (referBusinessErrorsToGeneralPage == true)
                {

                    response.ActionResult = RedirectToAction("Error", "Error", new { Area = "", Error = ServiceResponse.MessageInfo.Message });
                }
            }
            else if (ServiceResponse.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                response.ActionResult = RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            else
            {
                response.NoErrorsEncountered = true;
            }

            return response;
            
        }



    } 
}