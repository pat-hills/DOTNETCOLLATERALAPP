using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Views;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class ResubmitFSRequest : HandleWorkItemRequest
    {
        public FSView FSView { get; set; }
    }
}
