using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Configuration;

using CRL.Service.Views.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Configuration.Response
{
    
    public class GetWorkflowConfigurationResponse:ResponseBase
    {
        public ConfigurationWorkflowView GlobalConfigurationWorkflowView { get; set; }
        public ConfigurationWorkflowView MemberConfigurationWorkflowView { get; set; }
      
    }

    public class GetFeeResponse : ResponseBase
    {
        public TransactionPaymentSetup fee { get; set; }
    }

    public class GetFeeConfigurationResponse : ResponseBase
    {
        public GetFeeConfigurationResponse()
        {
            ConfigurationFeesSetupView = new ConfigurationFeesSetupView();
        }
       
        public ConfigurationFeesSetupView ConfigurationFeesSetupView { get; set; }
        public ICollection<LookUpView> ServiceFeeType { get; set; }
        public ICollection<LookUpView> LenderType { get; set; }
        public IList<ConfigurationTransactionFeesView> ConfigurationTransactionFeesViews { get; set; }
        public IList<LenderTransactionFeeConfigurationView> LenderTransactionFeeConfigurationViews { get; set; }
        public int PublicSearchId { get; set; }
        public int PublicGenerateCertificateId { get; set; }
        public int NumRecords { get; set; }
       
    }
}
