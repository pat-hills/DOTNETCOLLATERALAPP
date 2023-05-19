using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Configuration;

namespace CRL.Service.Messaging.Configuration.Response
{
    public class SaveFeesConfigurationResponse:ResponseBase
    {
        public ICollection<ConfigurationTransactionFeesView> ConfigurationTransactionFees { get; set; }
    }
}
