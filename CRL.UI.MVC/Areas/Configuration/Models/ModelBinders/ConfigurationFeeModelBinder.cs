using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Areas.Configuration.Models.ModelBinders
{
 

    public class ConfigurationFeeModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {

          //  var typeValue = (int)bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".PerTransactionOrReoccurence").ConvertTo(typeof(int));

            ConfigurationTransactionFeesView model;

            model = (ConfigurationTransactionFeesView)Activator.CreateInstance(typeof(ConfigurationTransactionFeesView));
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(ConfigurationTransactionFeesView));
            
            return model;
        }

    }
}