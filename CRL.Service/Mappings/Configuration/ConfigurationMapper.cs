using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.Configuration;
using CRL.Service.Views.Configuration;

namespace CRL.Service.Mappings.Configuration
{
   

    public static class ConfigurationMapper
    {

        public static ConfigurationWorkflow ConvertToConfigurationWorkflow(this ConfigurationWorkflowView ConfigurationWorkflowView)
        {
            ConfigurationWorkflow ConfigurationWorkflow = Mapper.Map<ConfigurationWorkflowView, ConfigurationWorkflow>(ConfigurationWorkflowView);
            return ConfigurationWorkflow;
        }
        public static void ConvertToConfigurationWorkflow(this ConfigurationWorkflowView ConfigurationWorkflowView, ConfigurationWorkflow ConfigurationWorkflow)
        {
            Mapper.Map<ConfigurationWorkflowView, ConfigurationWorkflow>(ConfigurationWorkflowView, ConfigurationWorkflow);
        }

        public static ConfigurationWorkflowView ConvertToConfigurationWorkflowView(this ConfigurationWorkflow ConfigurationWorkflow)
        {
            ConfigurationWorkflowView ConfigurationWorkflowView = Mapper.Map<ConfigurationWorkflow, ConfigurationWorkflowView>(ConfigurationWorkflow);
            return ConfigurationWorkflowView;
        }


        
      
      

       

    }

}
