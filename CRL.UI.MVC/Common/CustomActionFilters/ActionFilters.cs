using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Authentication;
using System.Diagnostics.CodeAnalysis;
using System.Web.Routing;

namespace CRL.UI.MVC.Common.CustomActionFilters
{
    public class AuthorizeClientOnlyAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
            if (!securityUser.IsOwnerUser )
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // not yet implemented
        }
    }
   
        public class AuthorizeOwnerOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                 SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser .IsOwnerUser )
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }

        public class AuthorizeAdministratorOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser.IsAdministrator ())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }

        public class AuthorizeAllAdministratorOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser.IsAdministrator() && !securityUser.IsUnitAdministrator ())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }

        public class AuthorizeAllAdministratorRegistrySupportOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser.IsOwnerAdminRegistrySupport () && !securityUser.IsClientAdministrator() && !securityUser.IsUnitAdministrator() )
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }


        public class AuthorizeOwnerAdministratorOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser .IsOwnerAdministrator())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }
        public class AuthorizeOAdminRegistrarSupportOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser.IsOwnerAdminRegistrySupport ())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }
        public class AuthorizeOAdminRegistrarSupportCAdminOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser.IsOwnerAdminRegistrySupport() && !securityUser .IsClientAdministrator ())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }
        public class AuthorizeClientAdministratorOnlyAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SecurityUser securityUser = (SecurityUser)filterContext.HttpContext.User;
                if (!securityUser .IsClientAdministrator ())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                // not yet implemented
            }
        }


        [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
          Justification = "This attribute is AllowMultiple = true and users might want to override behavior.")]
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        public class LoginAntiforgeryHandleErrorAttribute : FilterAttribute, IExceptionFilter
        {
            #region Implemented Interfaces

            #region IExceptionFilter

            /// <summary>
            /// </summary>
            /// <param name="filterContext">
            /// The filter context.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// </exception>
            public virtual void OnException(ExceptionContext filterContext)
            {
                if (filterContext == null)
                {
                    throw new ArgumentNullException("filterContext");
                }

                if (filterContext.IsChildAction)
                {
                    return;
                }

                // If custom errors are disabled, we need to let the normal ASP.NET exception handler
                // execute so that the user can see useful debugging information.
                if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    return;
                }

                Exception exception = filterContext.Exception;

                // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
                // ignore it.
                if (new HttpException(null, exception).GetHttpCode() != 500)
                {
                    return;
                }

                // check if antiforgery
                if (!(exception is HttpAntiForgeryException))
                {
                    return;
                }

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                {
                    { "action", "Index" }, 
                    { "controller", "Home" }
                });

                filterContext.ExceptionHandled = true;
            }

            #endregion

            #endregion
        }


       
    
}