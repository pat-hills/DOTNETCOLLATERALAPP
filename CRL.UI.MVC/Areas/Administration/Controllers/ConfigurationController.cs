using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.UI.MVC.Areas.Administration.Models.ModelHelpers;
using CRL.UI.MVC.Areas.Administration.Models.ViewPageModels;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using StructureMap;
using CRL.Model;

namespace CRL.UI.MVC.Areas.Administration.Controllers
{
    [MasterEventAuthorizationAttribute]
    public class ConfigurationController : Controller
    {
        SecurityUser SecurityUser
        {
            get
            {
                return (SecurityUser)this.HttpContext.User;

            }
        }
        //
        // GET: /Administration/Configuration/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Administration/Configuration/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Administration/Configuration/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Administration/Configuration/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (collection["WorkflowMode"] == "0")
                    Session.Add("WorkflowMode", false);
                else
                    Session.Add("WorkflowMode", true);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Administration/Configuration/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Administration/Configuration/Edit/5

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
        // GET: /Administration/Configuration/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Administration/Configuration/Delete/5

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






        public ActionResult GlobalMessageIndex()
        {
            GlobalMessageIndexViewModel viewModel = new GlobalMessageIndexViewModel();
            return View(viewModel);
        }

        public PartialViewResult SubmitSearchFilterForJqGrid(_GlobalMessageJqGridViewModel _GlobalMessageJqGridViewModel)
        {
            return PartialView("~/Areas/Administration/Views/Shared/_GlobalMessageJqGrid.cshtml", _GlobalMessageJqGridViewModel);
        }

        public ActionResult ViewGlobalMessageJsonData(GridSettings grid, _GlobalMessageJqGridViewModel viewModel)
        {
            //ViewGlobalMessagesRequest request = new ViewGlobalMessagesRequest();
            ViewGlobalMessagesModelRequest request = new ViewGlobalMessagesModelRequest();
            GlobalMessageViewModelHelper.MapViewModelToRequest(viewModel,request,grid);
            request.SecurityUser = SecurityUser;


            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            ViewGlobalMessagesResponse response = ES.GetAllGlobalMessages(request);

            GlobalMessageView[] ArrayRows = response.GlobalMessageViews.ToArray();

            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GlobalMessageDetails(int mode, int Id = 0)
        {
            GlobalMessageDetailsViewModel viewModel = new GlobalMessageDetailsViewModel();
            
            switch (mode)
            {
                case 3:
                    viewModel.ChangeFormMode(EditMode.Edit);
                    viewModel.InUpdateMode = true;
                    break;
                case 2:
                    viewModel.ChangeFormMode(EditMode.Create);
                    break;
                default:
                    viewModel.ChangeFormMode(EditMode.View);
                    break;
            }

            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            CreateGMRequest request = new CreateGMRequest();
            request.SecurityUser = SecurityUser;
            request.Mode = mode;
            if (viewModel.FormMode != EditMode.Create)
                request.IsEditOrView = true;
            request.Id = Id;

            CreateGMResponse response = ES.GetCreateDetails(request);

            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new {Area = ""});
            }
           
            AdminViewModelHelper.BuildViewModelForEditView(viewModel, response,request.IsEditOrView);
            viewModel.UniqueGuidForm = Guid.NewGuid().ToString("N") + SecurityUser.Id.ToString("0000");
            return View("GlobalMessageDetails", viewModel);
        }

        [ValidateInput(false)] //[AllowHtml] on property 
        [HttpPost]
        public ActionResult CreateSubmitGlobalMessage(GlobalMessageDetailsViewModel model)
        {
            MessageInfo _msg = null;
            if (ModelState.IsValid)
            {
                IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
                CreateSubmitGlobalMessageRequest request = new CreateSubmitGlobalMessageRequest();

                AdminViewModelHelper.BuildForCreateEdit(model, request);
                request.SecurityUser = SecurityUser;
                CreateSubmitGlobalMessageResponse response = null;

                response = model.InUpdateMode ? ES.UpdateSubmitGm(request) : ES.CreateSubmitGm(request);

                if (response.MessageInfo.MessageType == MessageType.BusinessValidationError ||
                 response.MessageInfo.MessageType == MessageType.Error)
                {
                    _msg = new MessageInfo
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
                else
                {
                    TempData["TitleMessage"] = response.MessageInfo;
                    return RedirectToAction("GlobalMessageIndex");
                }
                //return success page with message
            }
            else
            {

                //If we had business errors before we called the service then we need to formulate the error message but we do not need to
                //remap to the Model State since that has been taken care of already
                _msg = new MessageInfo
                {
                    MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError,
                    Message = "Busness Validation Errors were detected"
                };
                ViewBag.TitleMessage = _msg;

            }

            return View("GlobalMessageDetails", model);
        }

        public ActionResult GetDeleteGlobalMessage(int Id)
        {
            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            CreateGMRequest request = new CreateGMRequest();
            request.SecurityUser = SecurityUser;
            request.IsEditOrView = true;
            request.Id = Id;

            CreateGMResponse response = ES.GetCreateDetails(request);
            
            var result = new
                             {
                                 id = response.GlobalMessageDetailsView.Id,
                                 title = response.GlobalMessageDetailsView.Title
                             };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGlobalMessage(int Id)
        {

            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            DeleteGMRequest request = new DeleteGMRequest();
            request.SecurityUser = SecurityUser;
            request.Id = Id;
            DeleteGMResponse response = ES.DeletSubmitGm(request);
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
                    _msg.Message = "Busness Validation Errors were detected";
                }
                ViewBag.TitleMessage = _msg;
            }
            else if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Unauthorized)
            {
                return RedirectToAction("AuthorizeError", "Error", new { Area = "" });
            }
            else
            {
                TempData["TitleMessage"] = response.MessageInfo;
                return RedirectToAction("GlobalMessageIndex");
            }
            //return success page with message
            TempData["TitleMessage"] = response.MessageInfo;
            return RedirectToAction("GlobalMessageIndex");
        }

    }
}
