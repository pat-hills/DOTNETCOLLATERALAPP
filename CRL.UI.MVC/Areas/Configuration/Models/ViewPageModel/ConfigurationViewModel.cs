using CRL.Service.Messaging.Configuration.Request;
using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel
{
    public class ConfigurationViewModel
    {
        public _GeneralConfigurationViewModel _GeneralConfigurationViewModel { get; set; }
        public _WorkFlowConfigurationViewModel _WorkFlowConfigurationViewModel { get; set; }
        public _FeeConfigurationViewModel _FeeConfigurationViewModel { get; set; }
        public ConfigurationPage ConfigurationPage { get; set; }
    }
}