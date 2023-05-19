using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Views.Workflow;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Workflow.Request
{
    public class GetNumberOfTaskRequest:RequestBase
    {
    }

    public class ViewMyTasksRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public int? CaseType { get; set; }
        public string SubmittedBy { get; set; }
    }

   
}
