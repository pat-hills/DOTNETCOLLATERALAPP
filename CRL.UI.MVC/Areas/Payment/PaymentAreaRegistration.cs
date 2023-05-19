using System.Web.Mvc;

namespace CRL.UI.MVC.Areas.Payment
{
    public class PaymentAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Payment";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Payment_default",
                "Payment/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
