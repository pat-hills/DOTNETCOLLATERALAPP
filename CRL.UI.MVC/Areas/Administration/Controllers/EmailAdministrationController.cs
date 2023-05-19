using CRL.Infrastructure.Authentication;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.EmailAdministration.Request;
using CRL.Service.Messaging.EmailAdministration.Response;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Service.Views.Administration;
using CRL.Service.Views.Memberships;
using CRL.UI.MVC.Areas.Administration.Models.ViewPageModels;
using CRL.UI.MVC.Areas.Membership.Models.ViewPageModels.Client;
using CRL.UI.MVC.Common;
using MvcJqGrid;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Areas.Administration.Controllers
{
      [MasterEventAuthorizationAttribute]
    public class EmailAdministrationController : Controller
    {

          SecurityUser SecurityUser
          {
              get
              {
                  return (SecurityUser)this.HttpContext.User;

              }
          }
        //
        // GET: /Administration/EmailAdministration/

        public ActionResult Index()
        {
            EmailsIndexViewModel viewModel = new EmailsIndexViewModel();
            viewModel._EmailJqGridViewModel = new _EmailJqGridViewModel();
            return View(viewModel);
        }

        public PartialViewResult SubmitSearchFilterForJqGrid(_EmailJqGridViewModel _EmailJqGridViewModel)
        {
            //viewModel.FormMode = EditMode.View;
            //Calls the partial view
            // viewModel.InstitutionId = institutionId;
            return PartialView("~/Areas/Administration/Views/Shared/_EmailsJqGrid.cshtml", _EmailJqGridViewModel);

        }



        public ActionResult ViewEmailJsonData(GridSettings grid, _EmailJqGridViewModel viewModel)
        {
          
            EmailView email1 = new EmailView(){Id=1,EmailBody="This is the first content",EmailFrom="bsystems.collateral@gmail.com",IsActive=true,IsSent=false,EmailTo="primekad@gmail.com;primekad@yahoo.com",EmailSubject="Testing mail function"};
            EmailView email2 = new EmailView() { Id = 2, EmailBody = "This is the second content", EmailFrom = "bsystems.collatera@gmail.com", IsActive = true, IsSent = false, EmailTo = "primekad@gmail.com", EmailSubject = "Testing mail function" };
            EmailView email3 = new EmailView() { Id = 3, EmailBody = "This is the third content", EmailFrom = "bsystems.collatera@gmail.com", IsActive = true, IsSent = false, EmailTo = "primekad@gmail.com", EmailSubject = "Testing mail function" };
            EmailView email4 = new EmailView() { Id = 4, EmailBody = "This is the fourth content", EmailFrom = "bsystems.collatera@gmail.com", IsActive = true, IsSent = false, EmailTo = "primekad@gmail.com", EmailSubject = "Testing mail function" };
            EmailView email5 = new EmailView() { Id = 5, EmailBody = "This is the fifth content", EmailFrom = "bsystems.collatera@gmail.com", IsActive = true, IsSent = false, EmailTo = "primekad@gmail.com", EmailSubject = "Testing mail function" };
            EmailView email6 = new EmailView() { Id = 6, EmailBody = "This is the sixth content", EmailFrom = "bsystems.collatera@gmail.com", IsActive = true, IsSent = false, EmailTo = "primekad@gmail.com", EmailSubject = "Testing mail function" };


            ViewEmailRequest request = new ViewEmailRequest();
            request.AllOrUnsent = viewModel.AllOrUnsent;
            request.SentDate = viewModel.GenerateDateRange();
            request.SecurityUser = SecurityUser;
            request.SortColumn = grid.SortColumn;
            request.SortOrder = grid.SortOrder;

            request.PageSize = grid.PageSize;
            request.PageIndex = grid.PageIndex;
            request.SecurityUser = SecurityUser;

            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            ViewEmailResponse response = ES.GetAllEmails(request);

            EmailView[] ArrayRows = response.EmailView.ToArray();

            var result = new
            {
                total = (int)Math.Ceiling((double)response.NumRecords / grid.PageSize),
                page = grid.PageIndex,
                records = response.NumRecords,
                rows = ArrayRows
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewEmailDetails(int id)
        {
            ViewClientEmailRequest request = new ViewClientEmailRequest();
            request.AssignmentId = id;
            request.SecurityUser = SecurityUser;
            request.IsAdminMode = true;
            IEmailService ES = ObjectFactory.GetInstance<IEmailService>();
            ViewClientEmailResponse response = ES.ViewEmailDetails(request);

            EmailDetailViewModel viewModel = new EmailDetailViewModel() { ClientEmailView = new ClientEmailView() };
            viewModel.ClientEmailView = response.ClientEmailView;
            viewModel.DaysAgo = "(" + Math.Round((DateTime.Now - response.ClientEmailView.CreatedOn).TotalDays).ToString() +
                                " days ago)";
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult ResendMail(int[] selectedIds, EmailView[] selectedRowData)
        {
              string result;
            try
            {
                foreach (var mail in selectedRowData)
                {
                    sendEmail(mail.EmailTo, mail.EmailSubject, mail.EmailBody, mail.EmailFrom);
                }
                 result = "All mails were successfully sent";
            }
            catch (Exception ex)
            {

                result = ex.Message;
            }
           
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public void sendEmail(string email, string Subject, string body,string from)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential("bsystems.collateral@gmail.com", "@bsystems01");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(email));
        

            mail.Body = body;

            mail.Subject = Subject;

            smtpClient.Send(mail);
        }
    }
}
