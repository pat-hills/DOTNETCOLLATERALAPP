using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error(string Error)
        {
            if (!String.IsNullOrWhiteSpace(Error))
            {
                ViewBag.ErrorMessage = Error;
            }
            return View();
        }

        public ActionResult PageNotFound()
        {
            ViewBag.PageNotFound = true;
            return View("Error");
        }
       
        public ActionResult Timeout()
        {
            ViewBag.Timeout = true;
            return View("Error");
        }

        public ActionResult AuthorizeError()
        {
           
            return View();
        }

    }
}
