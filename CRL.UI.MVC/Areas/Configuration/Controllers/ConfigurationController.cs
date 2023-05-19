using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Messaging;
using CRL.Model.Configuration.IRepository;
using CRL.Service.BusinessServices;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.Service.Views.Configuration;
using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel;
using CRL.UI.MVC.Common.CustomActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using StructureMap;
using CRL.Infrastructure.Enums;

using CRL.Infrastructure.Helpers;
using System.IO;
using CsvHelper;

using CRL.UI.MVC.Areas.Configuration.Models.ModelHelpers;

namespace CRL.UI.MVC.Areas.Configuration.Controllers
{
     [MasterEventAuthorizationAttribute]
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService CS;
        //
        // GET: /Configuration/Configuration/
        private readonly ILKServiceFeeCategoriesRepository _serviceFeeCategoryRepository;

        public ConfigurationController( IConfigurationService _cs)
        {
            CS = _cs ;
        }




        SecurityUser SecurityUser
        {
            get
            {
                return (SecurityUser)this.HttpContext.User;

            }
        }



        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create(ConfigurationPage ConfigurationPage = ConfigurationPage.Workflow)
        {
            var tempMessage = TempData["TitleMessage"];
            if (tempMessage != null)
                ViewBag.TitleMessage = tempMessage;
            
            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.ConfigurationPage = ConfigurationPage;

            GetWorkflowConfigurationResponse response = CS.GetDataForConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage, SecurityUser = SecurityUser });

            if (ConfigurationPage == ConfigurationPage.Workflow)
            {
                viewModel._WorkFlowConfigurationViewModel = new _WorkFlowConfigurationViewModel();
                viewModel._WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView = response.GlobalConfigurationWorkflowView;
                viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView = response.MemberConfigurationWorkflowView;
            }

            if (ConfigurationPage == ConfigurationPage.General)
            {

                viewModel._GeneralConfigurationViewModel = new _GeneralConfigurationViewModel();
                // more code here                      
             }
           

            //if (ConfigurationPage == ConfigurationPage.Fee)
            //{
            //    viewModel._FeeConfigurationViewModel = new _FeeConfigurationViewModel();
            //    viewModel._FeeConfigurationViewModel.ConfigurationFeeView = response.c;         
            //}

        
            return View("Configuration", viewModel);
        }

        public ActionResult GeneralConfiguration()
        {
            var tempMessage = TempData["TitleMessage"];
            if (tempMessage != null)
                ViewBag.TitleMessage = tempMessage;

            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.ConfigurationPage = ConfigurationPage.Workflow;

            //GetWorkflowConfigurationResponse response = CS.GetDataForConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Workflow, SecurityUser = SecurityUser });

            //viewModel._WorkFlowConfigurationViewModel = new _WorkFlowConfigurationViewModel();
            //viewModel._WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView = response.GlobalConfigurationWorkflowView;
            //viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView = response.MemberConfigurationWorkflowView;

            return View("Configuration", viewModel);
        }


        public ActionResult WorkflowConfiguration()
        {
            var tempMessage = TempData["TitleMessage"];
            if (tempMessage != null)
                TempData["TitleMessage"] = null;
                ViewBag.TitleMessage = tempMessage;

            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.ConfigurationPage = ConfigurationPage.Workflow;

            GetWorkflowConfigurationResponse response = CS.GetDataForConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Workflow, SecurityUser = SecurityUser });

            viewModel._WorkFlowConfigurationViewModel = new _WorkFlowConfigurationViewModel();
            viewModel._WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView = response.GlobalConfigurationWorkflowView;
            viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView = response.MemberConfigurationWorkflowView;

            return View("Configuration",viewModel);
        }


        [HttpPost]
        [AuthorizeAllAdministratorOnly]
        public ActionResult WorkflowConfiguration(_WorkFlowConfigurationViewModel _WorkFlowConfigurationViewModel)
        {

            if (ModelState.IsValid)
            {

                SaveConfigurationRequest request = new SaveConfigurationRequest();
                request.ConfigurationPage = ConfigurationPage.Workflow;
                request.SecurityUser = SecurityUser;
                if (!SecurityUser.IsOwnerUser)
                {

                    if (!_WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView.UseGlobalSettings)
                    {

                        request.MemberConfigurationWorkflowView = _WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView;
                    }
                    else
                    {
                        request.GlobalConfigurationWorkflowView = _WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView;
                    }

                }

                else
                {

                    request.GlobalConfigurationWorkflowView = _WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView;

                }


                SaveConfigurationResponse response = CS.SaveConfiguration(request);
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("WorkflowConfiguration");
                }
                else
                {
                    //Get the response message into the ViewBag Title message
                    ViewBag.TitleMessage = response.MessageInfo;
                    if (response.MessageInfo.MessageType == MessageType.BusinessValidationError)
                    {
                        //For business errors we need to remap them t the Model State class


                    }
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
            return View();
         }
        public ActionResult FeeConfiguration()
        {
            var tempMessage = TempData["TitleMessage"];
            if (tempMessage != null)
                ViewBag.TitleMessage = tempMessage;

            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.ConfigurationPage = ConfigurationPage.Fee;

            GetFeeConfigurationResponse response = CS.GetFeeConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Fee, SecurityUser = SecurityUser });

            viewModel._FeeConfigurationViewModel = new _FeeConfigurationViewModel();
            foreach (var config in response.ConfigurationFeesSetupView.ConfiguationFeesView)
            {
                _ConfigurationFeeViewModel _temp = new _ConfigurationFeeViewModel()
               { ConfigurationFeeView = config,
                   ServiceFeeType = response.ServiceFeeType
               };
                 viewModel._FeeConfigurationViewModel._ConfigurationFeeViewModel.Add(_temp); 
            }
                                 
            
            
        
          



            //viewModel._FeeConfigurationViewModel = new _FeeConfigurationViewModel();
            //_ConfigurationFeeViewModel viewModel2 = new _ConfigurationFeeViewModel();
            //foreach (var config in response.ConfigurationFee.ToList())
            //{
            //    viewModel2.ConfigurationFee = config;
            //    viewModel2.ServiceFeeType = LookUpServiceHelper.ServiceFeeCategories(_serviceFeeCategoryRepository);
            //    viewModel2.ServiceFeeCategories =config.ServiceFeeType.Select(s =>(int)s.Id).ToArray();
            //    foreach (var servicefee in config.ServiceFeeType)
            //    { 
                  
                
            //    }

            //}


           // viewModel._FeeConfigurationViewModel._ConfigurationFeeViewModel = (_FeeConfigurationViewModel)response.ConfigurationFee.ToList(); 


            
                           
            //viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView = response.MemberConfigurationWorkflowView;

            return View("Configuration", viewModel);
        }

        public ActionResult BVCUpload()
        {

            return View("UploadBankVerification");


        }

        [HttpPost]
        public ActionResult BVCUpload(BVCUploadViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                if (viewModel.CSVfile.ContentType == "application/vnd.ms-excel" || viewModel.CSVfile.ContentType == "text/csv")
                {
                    if (ModelState.IsValid)
                    {
                        CreateNewBVCDataRequest request = new CreateNewBVCDataRequest();
                        try
                        {
                            ICsvParser csvParser = new CsvParser(new StreamReader(viewModel.CSVfile.InputStream));


                            var csv = new CsvReader(csvParser);
                            var BVCList = csv.GetRecords<BankVerificationCodeView>().ToList();

                            request.BankVerificationCodesView = BVCList;
                        }

                        catch (Exception ex)
                        {

                            MessageInfo _msg = new MessageInfo
                            {
                                MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                                Message = "The was an error processing the csv file uploaded. Please check the file and try again"
                            };
                            ViewBag.TitleMessage = _msg;

                            return View("UploadBankVerification");
                        }




                        request.SecurityUser = SecurityUser;
                        ResponseBase response = CS.CreateBVCData(request);

                        if (response.MessageInfo.MessageType == MessageType.Success)
                        {
                            TempData["TitleMessage"] = response.MessageInfo;
                            return View("UploadBankVerification");
                        }

                    }

                    else
                    {
                        MessageInfo _msg = new MessageInfo
                        {
                            MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                            Message = "Busness Validation Errors were detected"
                        };
                        ViewBag.TitleMessage = _msg;

                    }

                }

                else
                {
                    MessageInfo _msg = new MessageInfo
                    {
                        MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                        Message = "Invalid file type! Please upload a valid csv file"
                    };
                    ViewBag.TitleMessage = _msg;


                }

            }
            return View("UploadBankVerification");
        }

        public PartialViewResult BVCJqGrid(BVCUploadViewModel viewModel)
        {
            //viewModel.FormMode = EditMode.View;
            //Calls the partial view
            // viewModel.InstitutionId = institutionId;
            viewModel._BvcJqgridViewModel = new _BvcJqgridViewModel();
            viewModel._BvcJqgridViewModel.Levels = new Dictionary<string, string>() { { "National", "National" }, { "State", "State" }, { "Unit", "Unit" }};
            viewModel._BvcJqgridViewModel.BVCTypes = new Dictionary<string, string>() { { "Microfinance", "Microfinance" }, { "Bank", "Bank" }};
            return PartialView("~/Areas/Configuration/Views/Shared/_BVCcodeGrid.cshtml", viewModel._BvcJqgridViewModel);

        }

        public ActionResult BVCJsonData(GridSettings grid, _BvcJqgridViewModel _BvcJqgridViewModel)
        {

            GetBvcDataRequest request = new GetBvcDataRequest();

            //Map view model to the request object
            //Section Requires a model helper


            _BVCJqGridViewModelHelper.MapViewModelToViewFSRequest(_BvcJqgridViewModel, request, grid);

            //show paypoint users only
            // request.IsPaypointUser = true;
            request.SecurityUser = SecurityUser;

            //Service call
            //ViewUsersResponse response = us.ViewUsers(request);


            //We need to know how to return errors from the response object.  The grid must know how to return errors. Weill check this later on  

            //We are not going to return a view model but a special object and so how do we do this, let's jsut do it here
            GetBvcDataResponse response = CS.GetBVCData(request);
            BankVerificationCodeView[] ArrayRows = response.BvcCodes.ToArray();
            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };



            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public ActionResult FeeConfiguration(_FeeConfigurationViewModel viewModel)
        //{


        //    if (ModelState.IsValid)
        //    {

        //        SaveFeeConfigurationRequest request = new SaveFeeConfigurationRequest();

         

        //        foreach (var config in viewModel._ConfigurationFeeViewModel)
        //    {
        //        ConfigurationFeeView _temp = new ConfigurationFeeView();
        //       _temp = config.ConfigurationFee;
        //         request.ConfigurationFeesView.ConfiguationFeesView.Add(_temp);
        //    }


        //        SaveFeeConfigurationResponse response = CS.SaveFeeConfiguration(request);
            
        //           if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
        //        {
        //            TempData["TitleMessage"] = response.MessageInfo;
        //            return RedirectToAction("FeeConfiguration");
        //        }
        //        else
        //        {
        //            //Get the response message into the ViewBag Title message
        //            ViewBag.TitleMessage = response.MessageInfo;
        //            if (response.MessageInfo.MessageType == MessageType.BusinessValidationError)
        //            {
        //                //For business errors we need to remap them t the Model State class
                        
           
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //If we had business errors before we called the service then we need to formulate the error message but we do not need to
        //        //remap to the Model State since that has been taken care of already
        //        MessageInfo _msg = new MessageInfo
        //        {
        //            MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
        //            Message = "Busness Validation Errors were detected"
        //        };
        //        ViewBag.TitleMessage = _msg;
        //    }


        //    return View();
        //    }




        public ActionResult IndexOfFeeConfigurations()
        {



            IndexOfFeeConfigurationViewModel viewModel = new IndexOfFeeConfigurationViewModel();
            return View("IndexOfFeeConfigurations",viewModel);
        }



        public PartialViewResult SubmitSearchFeeConfigurations()
        {

            //Calls the partial view
            return PartialView("~/Areas/Configuration/Views/Shared/_FeeConfigurationsJqGrid.cshtml");
        }


        public ActionResult ViewFeeConfigurationJsonData(GridSettings grid, _FeeConfigurationsJqgrid viewModel)
        {
            //Code here for validating view model using the View Model helper


            //Create the request 
            GetDataForConfigurationRequest request = new GetDataForConfigurationRequest();
            request.SecurityUser = SecurityUser;
            request.ConfigurationPage = ConfigurationPage.Fee;

            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.SecurityUser = SecurityUser;

            //Service call

            IConfigurationService CS = ObjectFactory.GetInstance<IConfigurationService>();
            GetFeeConfigurationResponse response = CS.GetFeeConfiguration(request);

            ConfigurationFeeView[] ArrayRows = response.ConfigurationFeesSetupView.ConfiguationFeesView.ToArray();

            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult FeeConfigurationDetails(int id=0,EditMode FormMode = EditMode.View)
        {

            var tempMessage = TempData["TitleMessage"];
            if (tempMessage != null)
                ViewBag.TitleMessage = tempMessage;

            FeeConfigurationDetailViewModel viewModel = new FeeConfigurationDetailViewModel();
            viewModel._ConfigurationFeeViewModel = new _ConfigurationFeeViewModel();
            GetFeeConfigurationResponse response = CS.GetFeeConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Fee, SecurityUser = SecurityUser });
            viewModel.ChangeFormMode(FormMode);
            viewModel._ConfigurationFeeViewModel.ChangeFormMode(FormMode);
           
            if (FormMode == EditMode.Edit || FormMode == EditMode.View)
            {
                viewModel._ConfigurationFeeViewModel.ConfigurationFeeView = response.ConfigurationFeesSetupView.ConfiguationFeesView.Where(s => s.Id == id).SingleOrDefault();
            }
            if (FormMode == EditMode.Create)
            {

                viewModel._ConfigurationFeeViewModel.ConfigurationFeeView.PerTransactionOrReoccurence = 1;
            }

            viewModel._ConfigurationFeeViewModel.LenderTransactionFeeConfigurationViews = response.LenderTransactionFeeConfigurationViews;
            viewModel._ConfigurationFeeViewModel.LenderTypes = response.LenderType;
            viewModel._ConfigurationFeeViewModel.ConfigurationTransactionFeesViews = response.ConfigurationTransactionFeesViews;
            viewModel._ConfigurationFeeViewModel.PublicSearchId = response.PublicSearchId;
            viewModel._ConfigurationFeeViewModel.PublicGenerateCertificateId = response.PublicGenerateCertificateId;

            viewModel._ConfigurationFeeViewModel.ServiceFeeType = response.ServiceFeeType;
            return View("FeeConfigurationDetail",viewModel);
        }


         [HttpPost]
        public ActionResult FeeConfigurationDetails(FeeConfigurationDetailViewModel viewModel)
        {

            if (ModelState.IsValid)
            {

                SaveFeeConfigurationRequest request = new SaveFeeConfigurationRequest();

                request.ConfigurationFeesView = viewModel._ConfigurationFeeViewModel.ConfigurationFeeView;
                request.ConfigurationTransactionFeesViews = viewModel._ConfigurationFeeViewModel.ConfigurationTransactionFeesViews;
                request.LenderTypeId = viewModel._ConfigurationFeeViewModel.LenderTypeId;
                request.PubilcSearch = viewModel._ConfigurationFeeViewModel.PublicSearch;
                request.PublicGenerateSearchResult = viewModel._ConfigurationFeeViewModel.PublicGenerateCertificate;
                request.SecurityUser = SecurityUser;
                if (viewModel.FormMode == EditMode.Create)
                {
                    request.CreateNewConfiguration = true;               
                }

                SaveFeeConfigurationResponse response = CS.SaveFeeConfiguration(request);

                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("FeeConfigurationDetails");
                }
                else
                {
                    //Get the response message into the ViewBag Title message
                    ViewBag.TitleMessage = response.MessageInfo;
                    if (response.MessageInfo.MessageType == MessageType.BusinessValidationError)
                    {
                        //For business errors we need to remap them t the Model State class


                    }
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
            GetFeeConfigurationResponse response2 = CS.GetFeeConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Fee, SecurityUser = SecurityUser });
            viewModel._ConfigurationFeeViewModel.ServiceFeeType = response2.ServiceFeeType;

            viewModel._ConfigurationFeeViewModel.LenderTransactionFeeConfigurationViews = response2.LenderTransactionFeeConfigurationViews;
            viewModel._ConfigurationFeeViewModel.LenderTypes = response2.LenderType;
            viewModel._ConfigurationFeeViewModel.ConfigurationTransactionFeesViews = response2.ConfigurationTransactionFeesViews;

            return View("FeeConfigurationDetail", viewModel);
             
        }
            //viewModel._FeeConfigurationViewModel = new _FeeConfigurationViewModel();
            //_ConfigurationFeeViewModel viewModel2 = new _ConfigurationFeeViewModel();
            //foreach (var config in response.ConfigurationFee.ToList())
            //{
            //    viewModel2.ConfigurationFee = config;
            //    viewModel2.ServiceFeeType = LookUpServiceHelper.ServiceFeeCategories(_serviceFeeCategoryRepository);
            //    viewModel2.ServiceFeeCategories =config.ServiceFeeType.Select(s =>(int)s.Id).ToArray();
            //    foreach (var servicefee in config.ServiceFeeType)
            //    { 


            //    }

            //}


            // viewModel._FeeConfigurationViewModel._ConfigurationFeeViewModel = (_FeeConfigurationViewModel)response.ConfigurationFee.ToList(); 




            //viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView = response.MemberConfigurationWorkflowView;



         public ActionResult ToggleEnableDisableFeeConfiguration(int id)
         {

             RequestBase request = new RequestBase();
             request.Id = id;
             request.SecurityUser = SecurityUser;
             request.UserIP = AuditHelper.GetUserIP(this.Request);

             ResponseBase response = new ResponseBase();
             response = CS.ToggleEnableDisableConfiguration(request);
             if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
             {
                 string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "The Configuration fee was successfully disabled" : response.MessageInfo.Message;
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
             else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
             {
                 string result = "You are not authorized to perform this action";
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
             else
             {

                 string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "Request Failed" : response.MessageInfo.Message;
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
         
         
         }

         public ActionResult DeleteFeeConfiguration(int id)
         {

             RequestBase request = new RequestBase();
             request.Id = id;
             request.SecurityUser = SecurityUser;
             request.UserIP = AuditHelper.GetUserIP(this.Request);

             ResponseBase response = new ResponseBase();
             response = CS.DeleteFeeConfiguration(request);
             if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
             {
                 string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "The Configuration fee was successfully deleted" : response.MessageInfo.Message;
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
             else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
             {
                 string result = "You are not authorized to perform this action";
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
             else
             {

                 string result = String.IsNullOrWhiteSpace(response.MessageInfo.Message) ? "Request Failed" : response.MessageInfo.Message;
                 return Json(result, JsonRequestBehavior.AllowGet);
             }


         }



        [HttpPost]
        public ActionResult Create(ConfigurationViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                //Create the request
                SaveConfigurationRequest request = new SaveConfigurationRequest();
                //Update the request with the view model using the View Model Helper

                request.ConfigurationPage = viewModel.ConfigurationPage;


                if (viewModel.ConfigurationPage == ConfigurationPage.Workflow)
                {
                    request.SecurityUser = this.SecurityUser;
                    request.MemberConfigurationWorkflowView= new ConfigurationWorkflowView();
                    request.GlobalConfigurationWorkflowView = new ConfigurationWorkflowView();
                    request.MemberConfigurationWorkflowView = viewModel._WorkFlowConfigurationViewModel.MemberConfigurationWorkflowView;
                    request.GlobalConfigurationWorkflowView = viewModel._WorkFlowConfigurationViewModel.GlobalConfigurationWorkflowView;
                  
                }
                



                //Call the application service function
                SaveConfigurationResponse response = CS.SaveConfiguration(request);

                //If we are okay then let us redirect to the View action.
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("Create", new { ConfigurationPage = viewModel.ConfigurationPage });
                }
                else
                {
                    //Get the response message into the ViewBag Title message
                    ViewBag.TitleMessage = response.MessageInfo;
                    if (response.MessageInfo.MessageType == MessageType.BusinessValidationError)
                    {
                        //For business errors we need to remap them t the Model State class
                        
           
                    }
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

            //Need to get all data again especially for lookup values

            return View();
        }


        public ViewResult BlankFeeLoanSetup(int IsLSDorUSD)
        {
            this.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            // string s = this.Request.ToString();


            FeeLoanSetupView viewModel = new FeeLoanSetupView();
            viewModel.IsLSDorUSD = IsLSDorUSD;
            return View("~/Areas/Configuration/Views/Shared/_FeeLoanSetupConfiguration.cshtml", viewModel);
        }


        public ViewResult BlankPerTransactionFeeConfiguration()
        {
            this.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            // string s = this.Request.ToString();

              GetFeeConfigurationResponse response = CS.GetFeeConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Fee, SecurityUser = SecurityUser });
            _ConfigurationFeeViewModel viewModel = new _ConfigurationFeeViewModel();

              
                    viewModel.ServiceFeeType = response.ServiceFeeType;
       
       


            return View("~/Areas/Configuration/Views/Shared/_PerTransactionFeeConfiguration.cshtml", viewModel);
        }



        public ViewResult BlankPeriodicFeeConfiguration()
        {
            this.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            // string s = this.Request.ToString();

            GetFeeConfigurationResponse response = CS.GetFeeConfiguration(new GetDataForConfigurationRequest { ConfigurationPage = ConfigurationPage.Fee, SecurityUser = SecurityUser });
            _ConfigurationFeeViewModel viewModel = new _ConfigurationFeeViewModel();


            viewModel.ServiceFeeType = response.ServiceFeeType;
            return View("~/Areas/Configuration/Views/Shared/_PeriodicFeeConfiguration.cshtml", viewModel);
        }

         public ActionResult TransactionsFeeConfiguration()
         {
             var viewModel = new FeeConfigurationViewModel();
             GetDataForConfigurationTransactionFeesResponse response =
                 CS.GetDataForConfigurationTransactionFees(new GetDataForConfigurationTransactionFeesRequest()
                                                               {SecurityUser = SecurityUser});
             viewModel.ConfigurationTransactionFeesView = response.ConfigurationTransactionFeesViews.ToList();
             return View(viewModel);
         }

         [HttpPost]
         public ActionResult TransactionsFeeConfiguration(FeeConfigurationViewModel viewModel)
         {
             if (ModelState.IsValid)
             {
                 SaveFeesConfigurationResponse response =
                     CS.SaveFeesConfiguration(new SaveFeesConfigurationRequest
                                                  {
                                                      SecurityUser = SecurityUser,
                                                      ConfigurationTransactionFees =
                                                          viewModel.ConfigurationTransactionFeesView
                                                  });
                 if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Success)
                 {
                     TempData["TitleMessage"] = response.MessageInfo;
                 }
                 else
                 {
                     //Get the response message into the ViewBag Title message
                     ViewBag.TitleMessage = response.MessageInfo;
                     if (response.MessageInfo.MessageType == MessageType.BusinessValidationError)
                     {
                         //For business errors we need to remap them t the Model State class


                     }
                 }

                 viewModel.ConfigurationTransactionFeesView = response.ConfigurationTransactionFees.ToList();
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
    }
}
