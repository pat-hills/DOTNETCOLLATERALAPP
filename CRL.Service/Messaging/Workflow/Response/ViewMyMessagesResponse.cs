using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;
using CRL.Service.Views.Workflow;

namespace CRL.Service.Messaging.Workflow.Response
{
    public class ViewMyMessagesResponse : ResponseBase
    {
        public IEnumerable<MessagesView> MessagesViews { get; set; }
        public int NumRecords { get; set; }
    }
}
