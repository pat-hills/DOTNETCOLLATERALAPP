using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Configuration;

namespace CRL.Service.Messaging.Configuration.Response
{
    public class GetDataForConfigurationTransactionFeesResponse:ResponseBase
    {
        public ICollection<ConfigurationTransactionFeesView> ConfigurationTransactionFeesViews { get; set; }
    }
}
