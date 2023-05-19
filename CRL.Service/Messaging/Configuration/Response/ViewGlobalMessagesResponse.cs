using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;
using CRL.Model.ModelViews;

namespace CRL.Service.Messaging.Configuration.Response
{
    public class ViewGlobalMessagesResponse : ResponseBase
    {
        public ICollection<GlobalMessageView> GlobalMessageViews { get; set; }
        public int NumRecords { get; set; }
    }
}
