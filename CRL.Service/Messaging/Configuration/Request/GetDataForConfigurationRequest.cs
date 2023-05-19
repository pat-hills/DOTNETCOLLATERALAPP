using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Configuration.Request
{
    public enum ConfigurationPage { General = 1, Workflow = 3, Fee=4, Currency=5 }
    public class GetDataForConfigurationRequest : PaginatedRequest
    {
        public ConfigurationPage ConfigurationPage { get; set; }
    }
}
