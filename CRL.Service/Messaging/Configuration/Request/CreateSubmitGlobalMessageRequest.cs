using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;

namespace CRL.Service.Messaging.Configuration.Request
{
    public class CreateSubmitGlobalMessageRequest : RequestBase
    {
        public GlobalMessageDetailsView GlobalMessageDetailsView { get; set; }
        public int[] MessageRoles { get; set; }
    }
}
