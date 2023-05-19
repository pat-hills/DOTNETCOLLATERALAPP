using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using CRL.Infrastructure.Authentication;
using CRL.Service;
using CRL.Service.Interfaces;
using CRL.Service.Views.Configuration;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Memberships;
using CRL.UI.MVC.Areas.Configuration.Models.ModelBinders;
using CRL.UI.MVC.Areas.FinancialStatement.Models.FinancialStatementActivity.ViewPageModels.Shared;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ModelBinders;
using CRL.UI.MVC.Areas.Membership.Models.ModelBinders;
using CRL.UI.MVC.Common;
using CRL.UI.MVC.Models.Report;
using CRL.UI.MVC.Models.Report.ModelBinders;
using StructureMap;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.UI.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {


            AreaRegistration.RegisterAllAreas();
            ControllerBuilder.Current.SetControllerFactory(new ControllerFactoryBase());
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperBootStrapper.ConfigureAutoMapper();
            BootStrapper.ConfigureDependencies();
            ModelBinders.Binders.Add(typeof(ParticipantView), new ParticipantViewModelBinder());
            ModelBinders.Binders.Add(typeof(SubordinatingPartyView), new SubordinatingPartyViewModelBinder());
            ModelBinders.Binders.Add(typeof(MembershipRegistrationView), new MembershipRegistrationModelBinder());
            ModelBinders.Binders.Add(typeof(AmendViewModelBase), new ActivityViewModelBaseBinder());
            ModelBinders.Binders.Add(typeof(ConfigurationFeeView), new ConfigurationFeeModelBinder());
            ModelBinders.Binders.Add(typeof(ReportBaseViewModel), new ReportViewModelBinder());
            
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            HttpContext.Current.Response.AddHeader("x-frame-options", "SAMEORIGIN");
        }
        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {

            HttpContext context = HttpContext.Current;
            if (!(context.User == null))
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity.GetType() == typeof(FormsIdentity))
                    {
                        FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;
                        //CustomIdentity ci = new CustomIdentity(fi.Ticket);
                        FormsAuthenticationTicket fat = fi.Ticket;
                        if (context.Session == null)
                        {
                            IAuthenticationService service = ObjectFactory.GetInstance<IAuthenticationService>();
                            HttpContext.Current.User = service.GetUserById(Convert.ToInt32(fat.UserData));

                        }
                        else
                        {
                            if (Session["SecurityUser"] == null)
                            {
                                IAuthenticationService service = ObjectFactory.GetInstance<IAuthenticationService>();
                                HttpContext.Current.User = service.GetUserById(Convert.ToInt32(fat.UserData));
                            }
                            else
                            {

                                HttpContext.Current.User = (SecurityUser)Session["SecurityUser"];
                            }
                        }
                        //HttpContext.Current.User = CollateralRegistry.Security.User.GetUserByID(Convert.ToInt32(fat.UserData));
                        //Okay let's resue the same

                    }
                }
            }

        }

        void Application_Error(object sender, EventArgs e)
        {
            //MailMessage message = new MailMessage("ajoy.mazumdar@gmail.com", "ajoy.mazumdar@gmail.com");
            //message.Body = "Test Mail Body";
            //message.Subject = "Test Subject";
            //SmtpClient client = new SmtpClient();
            //client.EnableSsl = true;
            //client.Send(message);
            //// Code that runs when an unhandled error occurs
            Exception ex = Server.GetLastError();
            if (!(ex.InnerException == null))
            {
                //if (ex.InnerException is BoGCollateralCustomErrors.BaseCustomException)
                //{
                Application["TheException"] = ex.InnerException; //store the error for later                
                //Server.ClearError(); //clear the error so we can continue onwards
                //Response.Redirect("~/ErrorPages/BoGCollaterlErrorPage.aspx"); //direct user to error page

                //}
            }


        }
        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            if (Context.Items["AjaxPermissionDenied"] is bool)
            {
                Context.Response.StatusCode = 401;
                Context.Response.End();
            }                     
        }
        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{
         
        //    HttpContext context = HttpContext.Current;
        //    if (!(context.User == null))
        //    {
        //        if (HttpContext.Current.User.Identity.IsAuthenticated)
        //        {
        //            if (HttpContext.Current.User.Identity.GetType() == typeof(FormsIdentity))
        //            {
        //                FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;
        //                //CustomIdentity ci = new CustomIdentity(fi.Ticket);
        //                FormsAuthenticationTicket fat = fi.Ticket;
        //                if (context.Session == null)
        //                {
        //                    IAuthenticationService service = ObjectFactory.GetInstance<IAuthenticationService>();
        //                    HttpContext.Current.User = service.GetUserById(Convert.ToInt32(fat.UserData));

        //                }
        //                else
        //                {
        //                    if (Session["SecurityUser"] == null)
        //                    {
        //                        IAuthenticationService service = ObjectFactory.GetInstance<IAuthenticationService>();
        //                        HttpContext.Current.User = service.GetUserById(Convert.ToInt32(fat.UserData));
        //                    }
        //                    else
        //                    {

        //                        ;
        //                    }
        //                }
        //                //HttpContext.Current.User = CollateralRegistry.Security.User.GetUserByID(Convert.ToInt32(fat.UserData));
        //                //Okay let's resue the same

        //            }
        //        }
        //    }

        //}
      
        
    }
}