using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Infrastructure.Helpers
{
    public static class AuditHelper
    {
        public static string Resolve(string resource)
        {
            return string.Format("{0}://{1}{2}{3}",
            HttpContext.Current.Request.Url.Scheme,
            HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
            (HttpContext.Current.Request.ApplicationPath.Equals("/")) ?
            string.Empty : HttpContext.Current.Request.ApplicationPath,
            resource);
        }
        public static Uri UrlOriginal(this HttpRequestBase request)
        {
            string hostHeader = request.Headers["host"];

            return new Uri(string.Format("{0}://{1}{2}",
               request.Url.Scheme,
               hostHeader,
               request.RawUrl));
        }

        public static string GetUserIP(this HttpRequestBase request)
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }
    }
}
