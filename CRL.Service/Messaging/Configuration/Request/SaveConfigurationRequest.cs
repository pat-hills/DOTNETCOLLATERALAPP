using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Configuration;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Configuration.Request
{
    public class SaveConfigurationRequest:RequestBase
    {
        public ConfigurationPage ConfigurationPage { get; set; }
        public ConfigurationWorkflowView GlobalConfigurationWorkflowView { get; set; }
        public ConfigurationWorkflowView MemberConfigurationWorkflowView { get; set; }
       
    }
}

public class SaveFeeConfigurationRequest : RequestBase
{

    public ConfigurationFeeView ConfigurationFeesView { get; set; }
    public bool CreateNewConfiguration { get; set; }

    public int LenderTypeId { get; set; }
    public decimal PubilcSearch { get; set; }
    public decimal PublicGenerateSearchResult { get; set; }
    public IList<ConfigurationTransactionFeesView> ConfigurationTransactionFeesViews { get; set; }
}
