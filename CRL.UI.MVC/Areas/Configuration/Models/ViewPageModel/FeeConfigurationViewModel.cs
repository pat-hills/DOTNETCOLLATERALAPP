using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Service.Views.Configuration;

namespace CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel
{
    public class FeeConfigurationViewModel
    {
        public FeeConfigurationViewModel()
        {
            ConfigurationTransactionFeesView = new List<ConfigurationTransactionFeesView>();
        }
        public IList<ConfigurationTransactionFeesView> ConfigurationTransactionFeesView { get; set; }
    }
}