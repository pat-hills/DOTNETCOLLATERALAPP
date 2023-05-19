using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Configuration.Models.Shared
{
    public class _WorkFlowConfigurationViewModel
    {
        public ConfigurationWorkflowView GlobalConfigurationWorkflowView { get; set; }
        public ConfigurationWorkflowView MemberConfigurationWorkflowView { get; set; }

    }
}