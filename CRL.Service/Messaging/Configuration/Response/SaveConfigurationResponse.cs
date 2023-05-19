using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Configuration.Response
{
    public class SaveConfigurationResponse:ResponseBase
    {
        
        public ConfigurationWorkflowView GlobalConfigurationWorkflowView { get; set; }
        public ConfigurationWorkflowView MemberConfigurationWorkflowView { get; set; }
       
    }


    public class SaveFeeConfigurationResponse : ResponseBase
    {

        public ICollection<LookUpView> ServiceFeeType { get; set; }
      

    }
}
