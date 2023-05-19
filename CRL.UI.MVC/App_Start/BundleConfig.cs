using System.Web;
using System.Web.Optimization;

namespace CRL.UI.MVC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery-migrate-{version}.js",
                      "~/Scripts/i18n/grid.locale-en.js",
                          "~/Scripts/jquery.jqGrid.src.js",                          
                          "~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/necessary").Include(
                        "~/Scripts/jquery.easyListSplitter.js",
                         "~/Scripts/bootstrap.js"
                        //"~/Scripts/superfish.js",                        
                        //   "~/Scripts/autoNumeric.js"
                   ));

          

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryloadmask").Include(
                        "~/Scripts/jquery.loadmask.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include( 
                "~/Content/fontawesome/css/font-awesome.min.css",
                "~/Content/site.css", "~/Content/messages.css", "~/Content/jumbotron.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryloadmask").Include(
               "~/Content/jquery.loadmask.css"));


           

        }
    }
}