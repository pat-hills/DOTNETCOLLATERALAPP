using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL.UI.MVC.Models.Report;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.Mvc;

namespace CRL.UI.MVC.Views.Shared
{
    public partial class ReportViewer : ViewUserControl<ReportBaseViewModel>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportBaseViewModel model = (ReportBaseViewModel)Model;
            //Now let's call the view model again
            Assembly assembly = Assembly.Load("CRL.UI.MVC");
            Type ViewModelHelperType = assembly.GetType(model.ViewModelHelper);
            var viewModelHelper = Activator.CreateInstance(ViewModelHelperType);

            MethodInfo methodInfo = ViewModelHelperType.GetMethod("ProcessReport", BindingFlags.Instance | BindingFlags.Public);

            object[] parameters = new object[] { model,ReportViewer1   };
            //Call the method to load the object
            ReportBaseViewModel viewModel = (ReportBaseViewModel)methodInfo.Invoke(viewModelHelper, parameters);

        
            
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            
            // Required for report events to be handled properly.
            Context.Handler = Page;
        }
    }
}