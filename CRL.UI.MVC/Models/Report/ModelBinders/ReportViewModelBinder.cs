using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using CRL.Service.Views.FinancialStatement;

namespace CRL.UI.MVC.Models.Report.ModelBinders
{
    public class ReportViewModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var typeValue = (string)bindingContext.ValueProvider.GetValue("ViewModelType").ConvertTo(typeof(string));
            Assembly assembly = Assembly.Load("CRL.UI.MVC");
            Type ViewModelType = assembly.GetType(typeValue);
            var viewModel = Activator.CreateInstance(ViewModelType);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => viewModel, ViewModelType);
            return viewModel;
        }


    }
}