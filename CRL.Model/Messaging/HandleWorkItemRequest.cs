using CRL.Infrastructure.Messaging;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Messaging
{
    public class HandleWorkItemRequest : RequestBase
    {

        public int WorkItemId { get; set; }
        public int CaseId { get; set; }
        public string Comment { get; set; }
        public int TransitionId { get; set; }
        public int WorkflowId { get; set; }
        public WFTaskType wfTaskType { get; set; }

        public string Misc { get; set; }

        public void CloneTo(HandleWorkItemRequest request)
        {
            request.WorkItemId = this.WorkItemId;
            request.CaseId = this.CaseId;
            request.Comment = this.Comment;
            request.TransitionId = this.TransitionId;
            request.wfTaskType = this.wfTaskType;
            request.WorkflowId = this.WorkflowId;
            base.CloneTo(request);
        }

    }

}
