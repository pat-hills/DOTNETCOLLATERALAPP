using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Workflow.Request
{

    public class GetTaskHandleRequest : RequestBase
    {
        public int CaseId { get; set; }
      
    }
}
